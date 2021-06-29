﻿using Roba_Stock_Manager.Classes;
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


		public MainWindow()
		{
			InitializeComponent();

			string connectionString = ConfigurationManager.ConnectionStrings["Roba_Stock_Manager.Properties." +
				"Settings.RobaDbConnectionString"].ConnectionString;

			sqlConnection = new SqlConnection(connectionString);

			lvProductToOrder.ItemsSource = productsToOrder;

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

				string query = "select OderPrice from Product where Name = @productName";
				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

				SqlDataReader sqlDataReader = null;

				try
				{
					sqlConnection.Open();
					sqlCommand.Parameters.AddWithValue("@productName", product.Name);
					sqlDataReader = sqlCommand.ExecuteReader();
					while (sqlDataReader.Read())
					{
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

		//TODO: Implement Confirm Order button

		//TODO: Make the Order Number textblock dynamic

		//TODO: Bind Register Delevery

		//TODO: Show new window with detail of the order of selectedItem of lvHistory

	}
}
