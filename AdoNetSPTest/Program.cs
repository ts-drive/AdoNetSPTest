/*Основные приемы работы с ADO.NET взятые с https://metanit.com/sharp/adonet/1.1.php
 * 
 * System.Data.*
 * System.Data.Common
 * System.Data.(Odbc,OleDb, SqlClient,Mysql....)
 * 
 *                     __________________ADO.NET_________________ 
 *   |Приложение|----->|Провайдер данных    |                   |
 *                     |--------------------|                   |
 *                     |подключенный уровень|отключенный уровень|
 *                     |--------------------|-------------------|
 *                     |Connection          |DataSet            |
 *                     |Command             |DataTable          |-------------->|База данных|
 *                     |DataReader          |DataRow            |
 *                     |DataAdapter         |DataColumn         |
 *                     |____________________|___________________| 
 * 
 * 1. Sample 0 - ConnectionStrings & Commands:
     * 1.1 HardCoded - Нежелательный метод
     * --------------------------------------------
     *   static void Main(string[] args){
     *       string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=usersdb;Integrated Security=True";
     *  }
     *  ------------------------------------------
     * 1.2 Через App.config (web.config)
     * 
     *     using System.Configuration и подключаем System.Configuration в References
     *     В App.config, в <cofiguration> добавляем секцю <connectionStrings>
     * ------------------------------------------
     *  <connectionStrings>
     *   <add name="Default connection" connectionString="Data source=.\SQLSERV2014; Initial catalog=usersdb; Integrated security=True"
     *        providerName="System.Data.SqlClient"/>
     *   </connectionStrings>
     *   
     *   в коде получем строк так:
     *   
     *     static string connStr = ConfigurationManager.ConnectionStrings["Default connection"].ConnectionString;
     *  ------------------------------------------
 *2. Sample 1 - commands with !StoredProcedures!:
 *3. Sample 2 - stored procedures with output parameters: 
 */

using System;
using System.Data.SqlClient;
using System.Configuration;

namespace AdoNetSPTest
{
    class Program
    {
        static string connStr = ConfigurationManager.ConnectionStrings["Default connection"].ConnectionString;
        static string name;
        static int sampleNum=0;

        static void Main(string[] args)
        {
            Console.Write(">Введите номер примера Sample[0..2]: ");

            Int32.TryParse(Console.ReadLine(),out sampleNum);

            switch (sampleNum)
                {
                    //sample 0
                    case 0:
                        sample0();
                    break;
                    // Sample 1 
                    case 1:
                        Console.Write(">Введите имя пользователя: ");
                        name = Console.ReadLine();
                        Console.Write(">Введите возраст: ");
                        int age = Int32.Parse(Console.ReadLine());
                        addUser(name, age);
                        Console.WriteLine();
                        getUsers();
                    break;
                    //Sample 2
                    case 2:
                        Console.Write(">Введите имя пользователя: ");
                        name = Console.ReadLine();
                        getAgeRange(name);
                        Console.Read();
                    break;
                default:
                    sample0();
                break;
            }

        }

        private static void sample0()
        {
            /* 
             * SqlCommand имеет 3 метода:
             *  1) ExecuteNonCuery() - (insert, update, delete) - возвращает кол-во задетых записей
             *  2) ExecuteReader() - (select) - возвращает строки из таблицы - в тип SqlDataReader 
             *  3) ExecuteScalar() - (select+: min, max, sum, count)
             */

            //Использование соединения через try/catch/finally
            //------------------------------------------------
            //Console.WriteLine(connectionString);
            ////Console.Read();
            //SqlConnection connection = new SqlConnection(connectionString);
            //try
            //{
            //    connection.Open();
            //    Console.WriteLine("Соединение открыто");
            //}
            //catch(SqlException ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            //finally
            //{
            //    connection.Close();
            //    Console.WriteLine("Соединение закрыто...");
            //}
            //Console.Read();
            //------------------------------------------------------------------------

            //Исползуя using соединение закрывается автоматически, без вызова connection.Close()
            using (SqlConnection connection = new SqlConnection(connStr))
            {

                connection.Open();
                Console.WriteLine(connection.ClientConnectionId);
                connection.Close();

                connection.Open();
                Console.WriteLine(connection.ClientConnectionId);
                connection.Close();
                Console.WriteLine("---------------------------");

                connection.Open();
                Console.WriteLine("Подключение открыто");

                // Вывод информации о подключении
                Console.WriteLine("Свойства подключения:");
                Console.WriteLine("\tСтрока подключения: {0}", connection.ConnectionString);
                Console.WriteLine("\tБаза данных: {0}", connection.Database);
                Console.WriteLine("\tСервер: {0}", connection.DataSource);
                Console.WriteLine("\tВерсия сервера: {0}", connection.ServerVersion);
                Console.WriteLine("\tСостояние: {0}", connection.State);
                Console.WriteLine("\tWorkstationld: {0}", connection.WorkstationId);
                Console.WriteLine("\tРазмер пакета: {0}", connection.PacketSize);

                Console.WriteLine("---------------------------");

                string sqlExpression = "SELECT * FROM USERS";
                SqlCommand command = new SqlCommand(sqlExpression, connection);

                //SqlDataReader не наследуемый однонаправленный, создается без - NEW!
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    Console.WriteLine("{0}\t{1}\t{2}", reader.GetName(0), reader.GetName(1), reader.GetName(2));

                    while (reader.Read())
                    {
                        //Нетипизированно извлекаем из SqlDataReader
                        object id = reader.GetValue(0);
                        object name = reader.GetValue(1);
                        object age = reader.GetValue(2);

                        Console.WriteLine("{0} \t{1} \t{2}", id, name, age);
                    }
                }
                else
                    Console.WriteLine("Записией не найдено");
                //Обязательно закрываем!
                reader.Close();
            }

            Console.WriteLine("Подключение закрыто...");
            Console.Read();
        }

        private static void addUser(string name, int age)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connStr;

            string sqlExpression = "sp_InsertUser";

            try
            {
                conn.Open();

                SqlCommand command = new SqlCommand(sqlExpression, conn);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                //Объявление параметра через инициализатор, как вариант возможно через конструктор
                SqlParameter paramName = new SqlParameter
                {
                    ParameterName="@name",
                    Value=name
                };
                command.Parameters.Add(paramName);

                //Объявление параметра через конструктор, как вариант возможно через инициализатор
                SqlParameter paramAge = new SqlParameter("@age", age);
                command.Parameters.Add(paramAge);

                //если нам не надо возвращать id
                //var result=command.ExecuteNonQuery();
                var result = command.ExecuteScalar();
                Console.WriteLine("Id параметров добавлено, {0}", result);

            }
            catch(SqlException sqlEx)
            {
                Console.WriteLine(sqlEx.Message);
            }
            finally
            {
                conn.Close();
                Console.WriteLine(">Соединение закрыто...");
            }
        }

        private static void getUsers()
        {
            using (SqlConnection conn=new SqlConnection(connStr))
            {
                conn.Open();
                string sqlExpression = "sp_getUsers";
                SqlCommand command = new SqlCommand(sqlExpression,conn);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    Console.WriteLine("{0}\t{1}\t{2}",reader.GetName(0), reader.GetName(1), reader.GetName(2));
                    while (reader.Read())
                    {
                        //Типизированно извлекаем из SqlDataReader
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        int age = reader.GetInt32(2);

                        Console.WriteLine("{0}\t{1}\t{2}", id,name,age);
                    }
                }
                else
                    Console.WriteLine("Нет записей");
                //Обязательно закрываем!
                reader.Close();
            }

        }

        private static void getAgeRange(string name)
        {
            string sqlExpression = "sp_GetAgeRange";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(sqlExpression, conn);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                if (!String.IsNullOrEmpty(name))
                {
                    //Объявление параметра через конструктор, как вариант возможно через инициализатор
                    SqlParameter nameParam = new SqlParameter("@name", name);
                    command.Parameters.Add(nameParam);
                }

                //Объявление параметра через инициализатор, так как он приходит в программу необходимо
                //указать тип SqlDataType
                SqlParameter minAgeParam = new SqlParameter
                {
                    ParameterName = "@minAge",
                    SqlDbType=System.Data.SqlDbType.Int
                };
                //Указываем направление, по умолч. input
                minAgeParam.Direction = System.Data.ParameterDirection.Output;
                command.Parameters.Add(minAgeParam);

                //Объявление параметра через инициализатор, так как он приходит в программу необходимо
                //указать тип SqlDataType
                SqlParameter maxAgeParam = new SqlParameter
                {
                    ParameterName = "@maxAge",
                    SqlDbType = System.Data.SqlDbType.Int
                };
                //Указываем направление, по умолч. input
                maxAgeParam.Direction = System.Data.ParameterDirection.Output;
                command.Parameters.Add(maxAgeParam);

                command.ExecuteNonQuery();

                Console.WriteLine("Минимальный возраст: {0} ", command.Parameters["@minAge"].Value);
                Console.WriteLine("Максимальный возраст: {0} ", command.Parameters["@maxAge"].Value);
            }
        }
    }
}
