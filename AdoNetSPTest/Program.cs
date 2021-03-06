﻿/*Основные приемы работы с ADO.NET взятые с https://metanit.com/sharp/adonet/1.1.php
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
 *  1.1 HardCoded - Нежелательный метод
 *   --------------------------------------------
 *   static void Main(string[] args){
 *          string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=usersdb;Integrated Security=True";
 *     }
 *     ------------------------------------------
 *    1.2 Через App.config (web.config)
 * 
 *       using System.Configuration и подключаем System.Configuration в References
 *        В App.config, в <cofiguration> добавляем секцю <connectionStrings>
 *   ------------------------------------------
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
 *4. sample 3 - DataAdapter at console:
 *                                             ______________
 *     |DB|<--->|Connection|<--->|Command|<--->|DATA ADAPTER|<--->{Command builder}
 *                                             |------------|
 *                                                |      ^
 *                                                V      |
 *                                              fill  update
 *                                             |------------|
 *                                             | DatTable/  |
 *                                             | DataSet    |
 *                                             |------------|
 *                                             
 *5. Sample 4 - Обновление из DataSet вручную
 * Хотя в предыдущей теме объект SqlCommandBuilder позволял нам автоматически
 * создать все нужные выражения для обновления данных в БД из DataSet,
 * но все же этот способ имеет свои недостатки. Например, взглянем на следующий кусочек кода,
 * который использовался в прошлой теме:
 *      SqlCommandBuilder commandBuilder = new SqlCommandBuilder(adapter);
 *       adapter.Update(ds);
 *       ds.Clear();
 *       adapter.Fill(ds);
 * После обновления происходит очистка DataSet и перезагрузка данных,
 * что снижает производительность приложения. Хотя в DataTable
 * у нас уже добавлена строка с новыми данными, и в ней не хватает только id - значения,
 * которое генерируется самой базой данных при добавлении
 */

using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace AdoNetSPTest
{
    class Program
    {
        static string connStr = ConfigurationManager.ConnectionStrings["Default connection"].ConnectionString;
        static string name;
        static int age;
        static int sampleNum=0;

        static void Main(string[] args)
        {
            Console.Write(">Введите номер примера Sample[0..4]: ");

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
                        age = Int32.Parse(Console.ReadLine());
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
                    //Sample 3
                    case 3:
                        Console.Write(">Введите имя пользователя: ");
                        name = Console.ReadLine();
                        Console.Write(">Введите возраст: ");
                        age = Int32.Parse(Console.ReadLine());

                        sample3(name,age);
                        break;
                    //Sample 4
                    case 4:
                        Console.Write(">Введите имя пользователя: ");
                        name = Console.ReadLine();
                        Console.Write(">Введите возраст: ");
                        age = Int32.Parse(Console.ReadLine());

                        sample3(name, age);
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

        private static void sample3(string name,int age)
        {
            string sqlexpression = "select * from users";

            using (SqlConnection conn=new SqlConnection(connStr))
            {
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlexpression, conn);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                DataTable dt = ds.Tables[0];
                DataRow newRow = dt.NewRow();
                newRow["name"] = name;
                newRow["Age"] = age;
                dt.Rows.Add(newRow);

                //Создаем объект SqlCommandBuilder!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                // Для применения этого класса достаточно вызвать его конструктор,
                //в который передается нужный адаптер
                //Причем больше нигде в коде вы этот объект не вызываем.
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(adapter);
                adapter.Update(ds);
                //Альтернатива - обновление только одной таблицы - adapter.Update(dt);

                //Заново получаем данные из БД, зачищаем DataSet
                //Чтобы получить id - значения, которое генерируется самой базой данных при добавлении
                //Альтернатива - хранимая процедура возвращающая @id=SCOPE_IDENTITY()
                ds.Clear();
                //перезагружаем данные
                adapter.Fill(ds);

                foreach (DataColumn column in dt.Columns)
                    Console.Write("\t{0}",column.ColumnName);
                Console.WriteLine();

                //Перебор всех строк таблицы
                foreach (DataRow dr in dt.Rows)
                {
                    //получаем все ячейки строки
                    var cells = dr.ItemArray;
                    foreach (object cell in cells)
                        Console.Write("\t{0}", cell);
                    Console.WriteLine();
                }
            }
            Console.Read();
        }

        private static void sample4(string name,int age)
        {
            string sqlExpression = "select * from users";

            using (SqlConnection conn=new SqlConnection(connStr))
            {
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlExpression, conn);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

                adapter.InsertCommand = new SqlCommand("sp_CreateUser", conn);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@name",SqlDbType.NVarChar,50,"Name"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@age", SqlDbType.Int, 0, "Age"));
                SqlParameter parameter=adapter.InsertCommand.Parameters.Add(new SqlParameter("@id", SqlDbType.Int, 0, "id"));
                parameter.Direction = ParameterDirection.Output;

                DataSet ds = new DataSet();
                adapter.Fill(ds);

                DataTable dt = ds.Tables[0];
                DataRow newRow = dt.NewRow();
                newRow["name"] = name;
                newRow["age"] = age;
                dt.Rows.Add(newRow);

                adapter.Update(ds);
                ds.AcceptChanges();

                foreach (DataColumn column in dt.Columns)
                    Console.Write("\t{0}",column.ColumnName);
                Console.WriteLine();

                foreach(DataRow dr in dt.Rows)
                {
                    var cells = dr.ItemArray;
                    foreach (object cell in cells)
                        Console.Write("\t{0}", cell);
                    Console.WriteLine();
                }
            }

            Console.ReadLine();
        }
    }
}
