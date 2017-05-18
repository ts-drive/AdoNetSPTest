/*
 * Для получения данных через объект SqlDataAdapter необходимо организовать подключение
 * к БД и выполнить команду SELECT. Есть несколько способов создания SqlDataAdapter: 
 * 
 * SqlDataAdapter adapter = new SqlDataAdapter();
 * SqlDataAdapter adapter = new SqlDataAdapter(command);
 * SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
 * SqlDataAdapter adapter = new SqlDataAdapter(sql, connectionString);
 * 
 */

using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace AdoDataAdapterTest
{
    public partial class Form1 : Form
    {
        int pageSize = 5;
        int pageNumber = 0;
        string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        SqlDataAdapter adapter;
        DataSet ds;

        public Form1()
        {
            InitializeComponent();
            // Первый короткий пример
            /*
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sqlexpression = "Select * from users";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sqlexpression, connStr);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
            }
            */
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                adapter = new SqlDataAdapter(getSql(), conn);
                ds = new DataSet();
                adapter.Fill(ds, "users");
                dataGridView1.DataSource = ds.Tables[0];
                dataGridView1.Columns["id"].ReadOnly = true;

            }
        }

        private void nextButton_Click(object sender, System.EventArgs e)
        {
            if (ds.Tables["users"].Rows.Count<pageSize) return;

            pageNumber++;

            using (SqlConnection conn=new SqlConnection(connStr))
            {
                adapter = new SqlDataAdapter(getSql(), conn);
                ds.Tables["users"].Clear();
                adapter.Fill(ds, "users");
            }
        }

        private void backButton_Click(object sender, System.EventArgs e)
        {
            if (pageNumber == 0) return;

            pageNumber--;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                adapter = new SqlDataAdapter(getSql(), conn);
                ds.Tables["users"].Clear();
                adapter.Fill(ds, "users");
            }
        }

        private string getSql()
        {
            return "SELECT * FROM Users ORDER BY Id OFFSET ((" + pageNumber + ") * " + pageSize + ") " +
                "ROWS FETCH NEXT " + pageSize + "ROWS ONLY";
        }
    }
}
