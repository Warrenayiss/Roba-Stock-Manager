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

		public MainWindow()
		{
			InitializeComponent();

			string connectionString = ConfigurationManager.ConnectionStrings["Roba_Stock_Manager.Properties." +
				"Settings.RobaDbConnectionString"].ConnectionString;

			sqlConnection = new SqlConnection(connectionString);


			ShowInventory();

		}

		private void ShowInventory()
		{
			try
			{
				string query = "select *  from Product";
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

				using (sqlDataAdapter)
				{
					DataTable InventoryTb = new DataTable();

					sqlDataAdapter.Fill(InventoryTb);

					lvInventory.DisplayMemberPath = "Name";
					lvInventory.SelectedValuePath = "Id";
					lvInventory.ItemsSource = InventoryTb.DefaultView;
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
		}

	}
}
