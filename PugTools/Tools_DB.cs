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

            #region InitQueries
            public static Dictionary<string, SQLInitStore> initTable = new Dictionary<string, SQLInitStore>
            {
                {"Abilities", new SQLInitStore("ability", new GomLib.Models.Ability())},
                {"AchCategories", new SQLInitStore("achcategories", new GomLib.Models.AchievementCategory())},
                {"Achievements", new SQLInitStore("achievement", new GomLib.Models.Achievement())},
                {"Items", new SQLInitStore("item", new GomLib.Models.Item())},
                {"Schematics", new SQLInitStore("schematic", new GomLib.Models.Schematic())},
                {"Tooltip", new SQLInitStore("tooltip", new GomLib.Models.Tooltip())}
            };

            #endregion

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
                if (storedQueryValues.Count > 10000) //This can be enabled if the queries are getting too big.
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
            patchVersion = versionTexBox.Text;
        }

        public void SqlCreate()
        {
            //Items Table
            string create = File.ReadAllText("SQL Files\\item_create.sql");

            sqlexec(create);

            //Npcs

            //Abilities
            create = File.ReadAllText("SQL Files\\abilities_create.sql");

            sqlexec(create);
            //Quests

            addtolist("Database Tables created.");
        }

        private string ToSQL(object obj)
        {
            if (obj is GomLib.Models.GameObject)
            {
                return ((GomLib.Models.GameObject)obj).ToSQL(patchVersion);
            }
            else if (obj is GomLib.Models.PseudoGameObject)
            {
                return ((GomLib.Models.PseudoGameObject)obj).ToSQL(patchVersion);
            }

            return "";
        }
        #endregion 
    }

    public class SQLInitStore
    {
        public SQLInitStore(string b, string e)
        {
            InitBegin = b;
            InitEnd = e;
        }

        public SQLInitStore(string name, object obj) 
        {
            Table = name;
            SqlData = new SQLData(); //this has been  redesigned to elminate most of the grunt work to make new sql outputs.
            if (obj is GomLib.Models.GameObject)
            {
                SqlData = ((GomLib.Models.GameObject)obj).SQLInfo(); //not implemented yet
            }
            else if (obj is GomLib.Models.PseudoGameObject)
            {
                SqlData = ((GomLib.Models.PseudoGameObject)obj).SQLInfo(); //returns the SQLInfo related to the type. It's just a list of SQLProperties right now.
            }
            var names = SqlData.SQLProperties.Select(x => x.Name).ToList(); //Use linq to suck all the sql column names into a list.
            InitBegin = String.Format(@"USE `tor_dump`;
INSERT INTO `{0}` (`current_version`, `previous_version`, `first_seen`, `{1}`, `Hash`) VALUES ", name, String.Join("`, `", names)); //join the name list together and create a basic insert query for the type
            InitEnd = String.Format(@"ON DUPLICATE KEY UPDATE 
`previous_version` = IF((@update_record := (`Hash` <> VALUES(`Hash`))), `current_version`, `previous_version`),
`current_version` = IF(@update_record, VALUES(`current_version`), `current_version`),
{0}
`Hash` = IF(@update_record, VALUES(`Hash`), `Hash`);", String.Join(Environment.NewLine, names.Select(x => String.Format("`{0}` = IF(@update_record, VALUES(`{0}`), `{0}`),", x)))); //same thing, but slightly reversed. Use linq to take the name list and turn it into a formatted string for each row, then join those lines together with with a newline. It's better to use the String.Join/Format options so you're not spawning a billion new strings like when you + them together.
        }
        internal SQLData SqlData;

        public string InitBegin = "";
        public string InitEnd = "";

        public string Table = "";

        public void OutputCreationSQL()
        {
            string defaultQuery = File.ReadAllText("SQL Files\\default_create.sql");

            string columnTypes = String.Join(Environment.NewLine, SqlData.SQLProperties.Select(x => String.Format("  `{0}` {1},", x.Name, x.Type)));
            string priunikey = SqlData.SQLProperties.Where(x => x.IsPrimaryKey).Select(x => x.Name).First();
            string keyString = "  PRIMARY KEY (`{0}`),\r\n  UNIQUE KEY `id_UNIQUE` (`{0}`)";
            string priString = String.Format(keyString, priunikey);
            string oldString = String.Format(keyString, String.Format("{0}`, `version", priunikey));

            List<string> columnNames = SqlData.SQLProperties.Select(x => x.Name).ToList();
            string columns = String.Join("`, `", columnNames);
            string oldColumns = String.Join("`, OLD.`", columnNames);

            /*
             * {0} = table name
             * {1} = primarykey
             * {2} = column names
             * {3} = Old.column names
             */
            string trigger = String.Format(@"((NOT EXISTS(SELECT 1 FROM {0}_old_versions WHERE `{1}` =  OLD.`{1}` AND `Hash` = OLD.`Hash`)) AND (NOT (OLD.`Hash` = NEW.`Hash`))) THEN
	INSERT INTO `{0}_old_versions` (`version`, `{2}`, `Hash`)
	VALUES (OLD.`current_version`, OLD.`{3}`, OLD.`Hash`)", Table, priunikey, columns, oldColumns);

            /*
             * {0} = table name
             * {1} = column types minus version and hash
             * {2} = regular primary unique key statements
             * {3} = trigger if statement
             * {4} = old_versions primary unique key statements
             */
            string creationQuery = String.Format(defaultQuery, Table, columnTypes, priString, trigger, oldString);

            tor_tools.Tools.WriteFile(creationQuery, String.Format("SQL Creation Files\\{0}_create.sql", Table), false);
        }
    }
}
