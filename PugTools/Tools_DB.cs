using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using GomLib;

namespace tor_tools
{
    public partial class Tools
    {
        static protected MySqlConnection conn;
        static protected MySqlDataReader Reader;

        #region SQL Functions

        public MySqlDataReader sqlexec(string info)
        {
            if (sql)
            {
                string mysqlString = "SERVER=" + textBox3.Text + ";" + "DATABASE=" + textBox4.Text + ";" + "UID=" + textBox2.Text + ";" + "PASSWORD=" + textBox5.Text + ";";

                conn = new MySqlConnection(mysqlString);
                conn.Open();

                MySqlCommand command = conn.CreateCommand();
                command.CommandText = info;
                Reader = command.ExecuteReader();

                conn.Close();

                return Reader;
            }

            return null;
        }

        public static List<string> storedQueryValues { get; set; }
        public static string baseStr { get; set; }
        public static string baseQuery { get; set; }
        public static string endQuery { get; set; }

        public void sqlLoadFromFile(string file)
        {
            string mysqlString = "SERVER=" + textBox3.Text + ";" + "DATABASE=" + textBox4.Text + ";" + "UID=" + textBox2.Text + ";" + "PASSWORD=" + textBox5.Text + "; default command timeout=0;";
            conn = new MySqlConnection(mysqlString);
            conn.Open();

            MySqlScript script = new MySqlScript(conn, File.ReadAllText(file)); //Config.ExtractPath + file));
            script.Execute();
            conn.Close();
        }
        /*
         * Call this function with the value you want to add to the batch insert queue.
         * 
         * Example: sqlAddTransactionValue("(100, 'Name 1', 'Value 1', 'Other 1')");
         */
        public void sqlAddTransactionValue(string value)
        {
            if (sql)
            {
                storedQueryValues.Add(value);
                if (storedQueryValues.Count > 20000) //This can be enabled if the queries are getting too big.
                {
                    addtolist("Query limit reached. Stopping to insert values into to mysql table");
                    sqlExecTransaction();
                    addtolist("Done.");
                    /*if(chkBuildCompare.Checked)
                    {
                        sqlExecTransaction(); //On the fly inserts are too slow with this method. Dumping to file instead and we'll do a import from file.
                    }
                    else
                    {
                        WriteFile(String.Join(",", storedQueryValues) + ",", baseStr + ".sql", true);
                        storedQueryValues = new List<string>();
                    }*/
                }
            }
        }

        // Call this function with your base query to initialize the batch insert queue.
        // Example: sqlTransactionsInitialize("INSERT INTO example (example_id, name, value, other_value) VALUES");
        public void sqlTransactionsInitialize(string bStr, string endStr)
        {
            if (sql)
            {
                storedQueryValues = new List<string>();
                //if(chkBuildCompare.Checked)
                //{
                baseQuery = bStr;
                endQuery = endStr;
                /*}
                else
                {
                    baseStr = bStr;
                    string fullQuery = File.ReadAllText(baseStr + ".sql");
                    fullQuery = fullQuery.Replace("`tor_dump`", "`" + textBox4.Text + "`");
                    int index = fullQuery.IndexOf("INSERT INTO `" + baseStr + "` VALUES ") + 22 + baseStr.Length;
                    baseQuery = fullQuery.Substring(0, index);
                    endQuery = fullQuery.Substring(index);
                    WriteFile(baseQuery, baseStr + ".sql", false);
                }*/
            }
        }

        // Call this function to flush out any remaining stored values in the multiple insert queue, and clear the baseQuery.
        public void sqlTransactionsFlush()
        {
            if (sql)
            {
                if (storedQueryValues.Count > 0)
                {
                    //if(chkBuildCompare.Checked)
                    //{
                    //sqlExecTransaction(); //On the fly inserts are too slow with this method. Dumping to file instead and we'll do a import from file.
                    /*}
                    else
                    {
                        WriteFile(String.Join(",", storedQueryValues), baseStr + ".sql", true);
                        storedQueryValues = new List<string>();
                    }*/
                }
                //WriteFile(";" + endQuery, baseStr + ".sql", true);
                addtolist("Finalizing inserts/updates to mysql table");
                sqlExecTransaction();
                addtolist("Done.");
                baseStr = "";
                baseQuery = "";
                endQuery = "";

                /*if (MessageBox.Show("Mysql", "Rebuild the table completely? This can take a very long time.", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    Thread oSqlLoadFromFile = new Thread(new ParameterizedThreadStart(sqlLoadFromFile));
                    oSqlLoadFromFile.Start(baseStr + ".sql");
                }*/
            }
        }

        /*  This function is for executing batch inserts into a Mysql DB - This is all wrong now
         *
         *  The baseQuery will be something like:
         *       "INSERT INTO example (example_id, name, value, other_value) VALUES"
         *  values will be in this format:
         *  { "(100, 'Name 1', 'Value 1', 'Other 1')",
         *     (101, 'Name 2', 'Value 2', 'Other 2')",
         *     (102, 'Name 3', 'Value 3', 'Other 3')",
         *     (103, 'Name 4', 'Value 4', 'Other 4')"}
         */

        public void sqlExecTransaction()
        {
            if (sql)
            {
                string mysqlString = "SERVER=" + textBox3.Text + ";" + "DATABASE=" + textBox4.Text + ";" + "UID=" + textBox2.Text + ";" + "PASSWORD=" + textBox5.Text + "; default command timeout=0; Allow User Variables=True";

                conn = new MySqlConnection(mysqlString);
                conn.Open();

                MySqlCommand command = conn.CreateCommand();
                MySqlTransaction trans;
                trans = conn.BeginTransaction();
                command.Connection = conn;
                command.Transaction = trans;

                try
                {
                    command.CommandText = baseQuery + String.Join(",", storedQueryValues) + endQuery;
                    //command.Parameters.AddWithValue("@update_record", false);
                    command.ExecuteNonQuery();
                    trans.Commit();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                    try
                    {
                        trans.Rollback();
                    }
                    catch (MySqlException mse)
                    {
                        Console.WriteLine(mse);
                    }
                }

                conn.Close();
                conn.Dispose();

                storedQueryValues = new List<string>();
            }
        }
        static async System.Threading.Tasks.Task<int> NonQueryAsync(MySqlConnection conn, MySqlCommand cmd)
        {
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }

        public string sqlSani(string str)
        {
            if (str == null) return "";
            return MySqlHelper.EscapeString(str);
        }

        public static string patchVersion = "";

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            patchVersion = textBox1.Text;
        }

        public void SqlCreate()
        {
            //Items Table
            string create = File.ReadAllText("item.sql");

            sqlexec(create);

            //Npcs

            //Abilities

            //Quests

            addtolist("Database Tables created.");
        }
        #endregion 
    }
}
