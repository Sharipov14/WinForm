using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WinForm
{
    public partial class NewCustomer : Form
    {
        // Хранит значения, возвращаемых из БД
        private int parsedCustomerID;
        private int orderID;

        /// <summary>
        /// Проверяет, не пусто ли текстовое поле имени клиента.
        /// </summary>
        /// <returns></returns>
        private bool IsCustomerNameValid()
        {
            if (txtCustomerName.Text == "")
            {
                MessageBox.Show("Please enter a name.");
                return false;
            }
            else return true;
        }

        /// <summary>
        /// Проверяет, что были предоставлены идентификатор клиента и сумма заказа.
        /// </summary>
        /// <returns></returns>
        private bool IsOrderDataValid()
        {
            // Проверяем наличие CustomerID.
            if (txtCustomerID.Text == "")
            {
                MessageBox.Show("Please create customer account before placing order.");
                return false;
            }
            // Проверяем, что Сумма не равна 0.
            else if (numOrderAmount.Value < 1)
            {
                MessageBox.Show("Please specify an order amount.");
                return false;
            }
            // Заказ можно отправить.
            else return true;
        }
        
        // Очищаем данные
        private void ClearForm()
        {
            txtCustomerName.Clear();
            txtCustomerID.Clear();
            dtpOrderDate.Value = DateTime.Now;
            numOrderAmount.Value = 0;
            this.parsedCustomerID = 0;
        }


        public NewCustomer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Создает нового клиента, вызывая хранимую процедуру Sales.uspNewCustomer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            if (IsCustomerNameValid())
            {
                // Создаем соединение.
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.ConnString))
                {
                    // Создаем SqlCommand и идентифицируем его как хранимую процедуру.
                    using (SqlCommand command = new SqlCommand("Sales.uspNewCustomer", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Добавить входной параметр для хранимой процедуры и указать, что использовать в качестве его значения.
                        command.Parameters.Add(new SqlParameter("@CustomerName", SqlDbType.NVarChar, 40));
                        command.Parameters["@CustomerName"].Value = txtCustomerName.Text;

                        // Добавляем выходной параметр.
                        command.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int));
                        command.Parameters["@CustomerID"].Direction = ParameterDirection.Output;

                        try
                        {
                            connection.Open();

                            // Запуск хранимой процедуры.
                            command.ExecuteNonQuery();

                            // Идентификатор клиента - это значение IDENTITY из базы данных.
                            this.parsedCustomerID = (int)command.Parameters["@CustomerID"].Value;

                            // Поместите значение идентификатора клиента в текстовое поле только для чтения.
                            this.txtCustomerID.Text = Convert.ToString(parsedCustomerID);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Customer ID was not returned. Account could not be created.");
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Вызывает хранимую процедуру Sales.uspPlaceNewOrder для размещения заказа.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            // Убедитесь, что требуемый ввод присутствует.
            if (IsOrderDataValid())
            {
                // Создаем соединение.
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.ConnString))
                {
                    // Создаем SqlCommand и идентифицируем его как хранимую процедуру.
                    using (SqlCommand sqlCommand = new SqlCommand("Sales.uspPlaceNewOrder", connection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                    }
                }
            }
        }
    }
}
