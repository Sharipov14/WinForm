using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForm
{
    public partial class Navigation : Form
    {
        public Navigation()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Открывает форму NewCustomer, для добавление учетной записи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGoToAdd_Click(object sender, EventArgs e)
        {
            Form frm = new NewCustomer();
            frm.Show();
        }

        /// <summary>
        /// Открывает форму FillOrCanel в диалоговом окне,
        /// Для заполнения или отмены заказа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGoToFillOrCancel_Click(object sender, EventArgs e)
        {
            Form frm = new FillOrCanel();
            frm.ShowDialog();
        }

        /// <summary>
        /// Закрывет текущее окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
