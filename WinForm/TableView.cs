using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WinForm
{
    public partial class TableView : Form
    {
        public TableView()
        {
            InitializeComponent();
            showDataGridView();
        }

        /// <summary>
        /// Выводит данные из таблицы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showDataGridView()
        {
            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.ConnString))
            {
                // Определить строку запроса t-SQL, которая имеет параметр для идентификатора заказа.
                const string sql = @"SELECT OrderID,
                                            OrderDate,
                                            FilledDate,
                                            Status,
                                            Amount,
	                                        CustomerName
                                        FROM Sales.Orders INNER JOIN Sales.Customer 
                                        ON Sales.Orders.CustomerID = Sales.Customer.CustomerID";

                // Создаем объект SqlCommand.
                using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                {
                    try
                    {
                        connection.Open();

                        // Запускаем запрос, вызывая ExecuteReader ().
                        using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                        {
                            // Создаем таблицу данных для хранения извлеченных данных.
                            DataTable dataTable = new DataTable();

                            // Загружаем данные из SqlDataReader в таблицу данных.
                            dataTable.Load(dataReader);

                            // Отображение данных из таблицы данных в виде сетки данных.
                            this.dataGridView.DataSource = dataTable;

                            // Закройте SqlDataReader.
                            dataReader.Close();
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("The requested order could not be loaded into the form.");
                    }
                    finally
                    {
                        // Закрываем соединение.
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Закрывает текущее окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
