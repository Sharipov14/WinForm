using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace WinForm
{
    public partial class FillOrCanel : Form
    {

        private int parsedOrderID;

        /// <summary>
        /// Проверяет наличие идентификатора заказа и наличие допустимых символов.
        /// </summary>
        /// <returns></returns>
        private bool IsOrderIDValid()
        {
            // Проверяем ввод в текстовое поле Order ID.
            if (txtOrderID.Text == "")
            {
                MessageBox.Show("Please specify the Order ID.");
                return false;
            }
            // Проверяем наличие символов, отличных от целых.
            else if (Regex.IsMatch(txtOrderID.Text, @"^\D*S"))
            {
                // Показать сообщение и очистить ввод.
                MessageBox.Show("Customer ID must contain only numbers.");
                txtOrderID.Clear();
                return false;
            }
            else
            {
                // Преобразование текста в текстовом поле в целое число для отправки в базу данных.
                parsedOrderID = Int32.Parse(txtOrderID.Text);
                return true;
            }
        }
        public FillOrCanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Выполняет инструкцию t-SQL SELECT для получения данных заказа для указанного
        /// идентификатор заказа, затем отображает его в DataGridView в форме.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFindByOrderID_Click(object sender, EventArgs e)
        {
            if (IsOrderIDValid())
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.ConnString))
                {
                    // Определить строку запроса t-SQL, которая имеет параметр для идентификатора заказа.
                    const string sql = "SELECT * FROM Sales.Orders WHERE orderID = @orderID";

                    // Создаем объект SqlCommand.
                    using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                    {
                        // Определение параметра @orderID и установка его значения.
                        sqlCommand.Parameters.Add(new SqlParameter("@orderID", SqlDbType.Int));
                        sqlCommand.Parameters["@orderID"].Value = parsedOrderID;

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
                                this.dgvCustomerOrders.DataSource = dataTable;
                                
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
        }

        /// <summary>
        /// Отменяет заказ, вызывая Sales.uspCancelOrder.
        /// хранимая процедура в базе данных.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            if (IsOrderIDValid())
            {
                // Создаем соединение.
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.ConnString))
                {
                    // Создаем объект SqlCommand и идентифицируем его как хранимую процедуру.
                    using (SqlCommand sqlCommand = new SqlCommand("Sales.uspCancelOrder", connection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        // Добавляем входной параметр идентификатора заказа для хранимой процедуры.
                        sqlCommand.Parameters.Add(new SqlParameter("@orderID", SqlDbType.Int));
                        sqlCommand.Parameters["@orderID"].Value = parsedOrderID;

                        try
                        {
                            // Открываем соединение.
                            connection.Open();

                            // Запускаем команду для выполнения хранимой процедуры.
                            sqlCommand.ExecuteNonQuery();
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("The cancel operation was not comleted");
                        }
                        finally
                        {
                            // Закрываем соединение.
                            connection.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Заполняет заказ, вызывая сохраненный Sales.uspFillOrder
        /// процедура в базе данных.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFillOeder_Click(object sender, EventArgs e)
        {
            if (IsOrderIDValid())
            {
                // Создаем соединение.
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.ConnString))
                {
                    // Создаем команду и идентифицируем ее как хранимую процедуру.
                    using (SqlCommand sqlCommand = new SqlCommand("Sales.uspFillOrder", connection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add(new SqlParameter("@orderID", SqlDbType.Int));
                        sqlCommand.Parameters["@orderID"].Value = parsedOrderID;

                        // Добавляем входной параметр идентификатора заказа для хранимой процедуры.
                        sqlCommand.Parameters.Add(new SqlParameter("@FilledDate", SqlDbType.DateTime, 8));
                        sqlCommand.Parameters["@FilledDate"].Value = dtpFillDate.Value;

                        try
                        {
                            connection.Open();

                            // Выполняем хранимую процедуру.
                            sqlCommand.ExecuteNonQuery();
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("The fill operation was not completed.");
                        }
                        finally
                        {
                            // Закрываем соединение.
                            connection.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Закрывает форму.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFinishUpdates_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
