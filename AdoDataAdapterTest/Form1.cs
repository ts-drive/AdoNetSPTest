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
        public Form1()
        {
            InitializeComponent();

            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sqlexpression = "Select * from users";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sqlexpression, connStr);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
            }
        }
    }
}
