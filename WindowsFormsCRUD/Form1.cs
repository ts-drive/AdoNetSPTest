using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Common;

namespace WindowsFormsCRUD
{
    public partial class Form1 : Form
    {

        DataSet ds;
        SqlDataAdapter adapter;
        SqlCommandBuilder commandBuilder;
        string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        string sqlExpression = "select * from users";

        public Form1()
        {
            InitializeComponent();

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;

            using (SqlConnection connection = new SqlConnection(conStr))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sqlExpression, connection);

                ds = new DataSet();
                adapter.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                // делаем недоступным столбец id для изменения
                dataGridView1.Columns["Id"].ReadOnly = true;
            }

        }

        private void addButton_Click(object sender, EventArgs e)
        {
            DataRow row = ds.Tables[0].NewRow(); // добавляем новую строку в DataTable
            ds.Tables[0].Rows.Add(row);
        }

        private void daleteButton_Click(object sender, EventArgs e)
        {
            // удаляем выделенные строки из dataGridView1
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                dataGridView1.Rows.Remove(row);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sqlExpression, connection);
                commandBuilder = new SqlCommandBuilder(adapter);

                //Переопределям InserCommand билдера -----------------------------
                adapter.InsertCommand = new SqlCommand("sp_CreateUser", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;

                adapter.InsertCommand.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar, 50, "Name"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@age", SqlDbType.Int, 0, "Age"));

                SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
                parameter.Direction = ParameterDirection.Output;
                //----------------------------------

                adapter.Update(ds);
            }
        }
    }
}
