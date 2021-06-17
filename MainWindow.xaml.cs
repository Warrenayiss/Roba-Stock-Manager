using Roba_Stock_Manager.Classes;
using System;
using System.Collections.Generic;
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
		List<Product> productsToOrder = new List<Product>();
		DataTable InventoryTb = new DataTable();


		public MainWindow()
		{
			InitializeComponent();

			string connectionString = ConfigurationManager.ConnectionStrings["Roba_Stock_Manager.Properties." +
				"Settings.RobaDbConnectionString"].ConnectionString;

			sqlConnection = new SqlConnection(connectionString);

			
			ShowInventory();
			ShowProductCb();
			ShowProvider();
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
					//TODO: Showing two column not working
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

		private void AddProduct_Click(object sender, RoutedEventArgs e)
		{
			string productName = cbProduct.SelectedItem.ToString();
			string productQuantity = tbQuantity.Text;
			int quantity = Int32.Parse(productQuantity);
			productsToOrder.Add(
				new Product(productName, quantity)
			);
			tbQuantity.Text = "";

		}
	}
}
