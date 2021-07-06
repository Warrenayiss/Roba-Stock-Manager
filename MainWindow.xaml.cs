using Roba_Stock_Manager.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Roba_Stock_Manager
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>

	public partial class MainWindow : Window
	{
		SqlConnection sqlConnection;
		DataTable InventoryTb = new DataTable();
		ObservableCollection<Product> productsToOrder { get; } = new ObservableCollection<Product>();
		string productName;
		int orderPrice;
		int orderNumber;
		Int32 orderId = 0;


		public MainWindow()
		{
			InitializeComponent();

			string connectionString = ConfigurationManager.ConnectionStrings["Roba_Stock_Manager.Properties." +
				"Settings.RobaDbConnectionString"].ConnectionString;

			sqlConnection = new SqlConnection(connectionString);

			lvProductToOrder.ItemsSource = productsToOrder;

			dpOrder.SelectedDate = DateTime.Now;

			ShowInventory();
			ShowProductCb();
			ShowProvider();
			ShowOrderNumber();
		}

		private void ShowInventory()
		{
			try
			{
				string query = "select Name,Quantity  from Product";
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

				using (sqlDataAdapter)
				{

					sqlDataAdapter.Fill(InventoryTb);
				}
				lvInventory.ItemsSource = InventoryTb.DefaultView;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
		}

		private void ShowOrderNumber()
		{
			string query = "select Val from Gbvariables where \"Key\" = 'OrderNumber' ";
			SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
			SqlDataReader sqlDataReader = null;

			try
			{
				sqlConnection.Open();
				sqlDataReader = sqlCommand.ExecuteReader();
				while (sqlDataReader.Read())
				{
					orderNumber = (int)sqlDataReader["Val"];
				}
			}
			finally
			{
				if (sqlDataReader != null)
				{
					sqlDataReader.Close();
				}
				if (sqlConnection != null)
				{
					sqlConnection.Close();
				}
			}
			tbOrderNumber.Text = orderNumber.ToString();
		}

		private void ShowProductCb()
		{
			try
			{
				string query = "select Name,Quantity  from Product";
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

				using (sqlDataAdapter)
				{
					DataTable productTb = new DataTable();

					sqlDataAdapter.Fill(productTb);
					cbProduct.DisplayMemberPath = "Name";
					cbProduct.SelectedValuePath = "Id";
					cbProvider.FontSize = 20;
					cbProduct.ItemsSource = productTb.DefaultView;
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
		}

		private void ShowProvider()
		{
			try
			{
				string query = "select *  from Provider";
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

				using (sqlDataAdapter)
				{
					DataTable providerTb = new DataTable();

					sqlDataAdapter.Fill(providerTb);

					cbProvider.DisplayMemberPath = "Name";
					cbProvider.SelectedValuePath = "Id";
					cbProvider.ItemsSource = providerTb.DefaultView;
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
		}

		//TODO: Add price to the productToOrder
		private void AddProduct_Click(object sender, RoutedEventArgs e)
		{
			DataRowView drv = (DataRowView)cbProduct.SelectedItem;
			productName = drv["Name"].ToString();
			string productQuantity = tbQuantity.Text;
			int quantity = Int32.Parse(productQuantity);

			productsToOrder.Add(
				new Product(productName, quantity)
				);

			tbQuantity.Text = "";
			cbProduct.SelectedIndex = -1;
			ShowPrice();

		}

		private void ShowPrice()
		{
			orderPrice = 0;
			foreach (Product product in productsToOrder)
			{

				string query = "select OderPrice, Id from Product where Name = @productName;";
				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

				SqlDataReader sqlDataReader = null;

				try
				{
					sqlConnection.Open();
					sqlCommand.Parameters.AddWithValue("@productName", product.Name);
					sqlDataReader = sqlCommand.ExecuteReader();
					while (sqlDataReader.Read())
					{
						product.Id = (int)sqlDataReader["Id"];
						product.UnitPrice = (int)sqlDataReader["OderPrice"];
						product.TotalPrice = product.UnitPrice * product.Quantity;
						orderPrice += product.TotalPrice;
					}
				}
				finally
				{
					if(sqlDataReader != null)
					{
						sqlDataReader.Close();
					}
					if(sqlConnection != null)
					{
						sqlConnection.Close();
					}
				}
			}
			tbPriceOfOrder.Text = orderPrice.ToString() + " cfa";
		}

		private void confirmOrderbtn_Click(object sender, RoutedEventArgs e)
		{
			int numberOfProduct = productsToOrder.Count();
			DataRowView drv = (DataRowView)cbProvider.SelectedItem;
			
			string provider = drv["Name"].ToString();
			DateTime dateOrder = dpOrder.DisplayDate.Date;
			//Getting providerId
			int providerId = 0;

			string query = "select Id from Provider where Name = @providerName";
			SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

			SqlDataReader sqlDataReader = null;

			try
			{
				sqlConnection.Open();
				sqlCommand.Parameters.AddWithValue("@providerName", provider);
				sqlDataReader = sqlCommand.ExecuteReader();
				while (sqlDataReader.Read())
				{
					providerId = (int)sqlDataReader["Id"];
				}

			}
			finally
			{
				if (sqlDataReader != null)
				{
					sqlDataReader.Close();
				}
				if (sqlConnection != null)
				{
					sqlConnection.Close();
				}
			}
			//Put the Order Data in Database
			try
			{
				string queryAddOrder = "insert into \"Order\"(Date, NumberOfProduct, ProviderId)  OUTPUT INSERTED.ID values (@date, @nbOfProduct, @providerId)";
				SqlCommand sqlCommandOrder = new SqlCommand(queryAddOrder, sqlConnection);
				sqlConnection.Open();
				sqlCommandOrder.Parameters.AddWithValue("@date", dateOrder);
				sqlCommandOrder.Parameters.AddWithValue("@nbOfProduct", numberOfProduct);
				sqlCommandOrder.Parameters.AddWithValue("@providerId", providerId);
				orderId = (int)sqlCommandOrder.ExecuteScalar();
			}
			catch (Exception err)
			{
				MessageBox.Show(err.ToString());
			}
			finally
			{
				sqlConnection.Close();
			}

			foreach (Product product in productsToOrder)
			{
				try
				{
					string queryAddProductToOrder = "insert into OrderProductList(OrderId, ProductId, Quantity) values (@orderId, @productId, @quantity)";
					SqlCommand sqlCommandProductToOrder = new SqlCommand(queryAddProductToOrder, sqlConnection);
					sqlConnection.Open();
					sqlCommandProductToOrder.Parameters.AddWithValue("@orderId", orderId);
					sqlCommandProductToOrder.Parameters.AddWithValue("@productId", product.Id);
					sqlCommandProductToOrder.Parameters.AddWithValue("@quantity", product.Quantity);
					sqlCommandProductToOrder.ExecuteScalar();
				}
				catch (Exception err)
				{
					MessageBox.Show(err.ToString());
				}
				finally
				{
					sqlConnection.Close();
				}
			}
			productsToOrder.Clear();
			UpdateOrderNumber();
			ShowOrderNumber();
		}

		private void UpdateOrderNumber()
		{
			orderNumber++;
			try
			{
				string query = "update Gbvariables set Val = @newOrderNumber where \"Key\" ='OrderNumber'";
				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
				sqlConnection.Open();
				sqlCommand.Parameters.AddWithValue("@newOrderNumber", orderNumber);
				sqlCommand.ExecuteScalar();
			}
			catch (Exception err)
			{
				MessageBox.Show(err.ToString());
			}
			finally
			{
				sqlConnection.Close();
			}
		}

		//TODO: Implement Confirm Order button

		//TODO: Make the Order Number textblock dynamic

		//TODO: Bind Register Delevery

		//TODO: Show new window with detail of the order of selectedItem of lvHistory

	}
}
