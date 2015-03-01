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
                {"Items", new SQLInitStore(
                    "INSERT INTO item (current_version, previous_version, Name, NodeId, NameId, Fqn, ItemLevel, RequiredLevel, AppearanceColor, ArmorSpec, Binding, CombinedRating, CombinedRequiredLevel, CombinedStatModifiers, ConsumedOnUse, Conversation, ConversationFqn, DamageType, Description, DescriptionId, DisassembleCategory, Durability, EnhancementCategory, EnhancementSlots, EnhancementSubCategory, EnhancementType, EquipAbilityId, GiftRank, GiftType, Icon, IsModdable, MaxStack, ModifierSpec, MountSpec, Quality, Rating, RequiredAlignmentInverted, RequiredClasses, RequiredGender, RequiredProfession, RequiredProfessionLevel, RequiredSocialTier, RequiredValorRank, RequiresAlignment, RequiresSocial, Schematic, SchematicId, ShieldSpec, Slots, StatModifiers, SubCategory, TreasurePackageId, TreasurePackageSpec, UniqueLimit, UseAbilityId, Value, VendorStackSize, WeaponSpec, TypeBitSet, Hash, StackCount, MaxDurability, WeaponAppSpec, Model, ImperialVOModulation, RepublicVOModulation) VALUES ",
                    @"ON DUPLICATE KEY UPDATE 
previous_version = IF((@update_record := (Hash <> VALUES(Hash))), current_version, previous_version),
current_version = IF(@update_record, VALUES(current_version), current_version),
Name = IF(@update_record, VALUES(Name), Name),
NodeId = IF(@update_record, VALUES(NodeId), NodeId),
NameId = IF(@update_record, VALUES(NameId), NameId),
Fqn = IF(@update_record, VALUES(Fqn), Fqn),
ItemLevel = IF(@update_record, VALUES(ItemLevel), ItemLevel),
RequiredLevel = IF(@update_record, VALUES(RequiredLevel), RequiredLevel),
AppearanceColor = IF(@update_record, VALUES(AppearanceColor), AppearanceColor),
ArmorSpec = IF(@update_record, VALUES(ArmorSpec), ArmorSpec),
Binding = IF(@update_record, VALUES(Binding), Binding),
CombinedRating = IF(@update_record, VALUES(CombinedRating), CombinedRating),
CombinedRequiredLevel = IF(@update_record, VALUES(CombinedRequiredLevel), CombinedRequiredLevel),
CombinedStatModifiers = IF(@update_record, VALUES(CombinedStatModifiers), CombinedStatModifiers),
ConsumedOnUse = IF(@update_record, VALUES(ConsumedOnUse), ConsumedOnUse),
Conversation = IF(@update_record, VALUES(Conversation), Conversation),
ConversationFqn = IF(@update_record, VALUES(ConversationFqn), ConversationFqn),
DamageType = IF(@update_record, VALUES(DamageType), DamageType),
Description = IF(@update_record, VALUES(Description), Description),
DescriptionId = IF(@update_record, VALUES(DescriptionId), DescriptionId),
DisassembleCategory = IF(@update_record, VALUES(DisassembleCategory), DisassembleCategory),
Durability = IF(@update_record, VALUES(Durability), Durability),
EnhancementCategory = IF(@update_record, VALUES(EnhancementCategory), EnhancementCategory),
EnhancementSlots = IF(@update_record, VALUES(EnhancementSlots), EnhancementSlots),
EnhancementSubCategory = IF(@update_record, VALUES(EnhancementSubCategory), EnhancementSubCategory),
EnhancementType = IF(@update_record, VALUES(EnhancementType), EnhancementType),
EquipAbilityId = IF(@update_record, VALUES(EquipAbilityId), EquipAbilityId),
GiftRank = IF(@update_record, VALUES(GiftRank), GiftRank),
GiftType = IF(@update_record, VALUES(GiftType), GiftType),
Icon = IF(@update_record, VALUES(Icon), Icon),
IsModdable = IF(@update_record, VALUES(IsModdable), IsModdable),
MaxStack = IF(@update_record, VALUES(MaxStack), MaxStack),
ModifierSpec = IF(@update_record, VALUES(ModifierSpec), ModifierSpec),
MountSpec = IF(@update_record, VALUES(MountSpec), MountSpec),
Quality = IF(@update_record, VALUES(Quality), Quality),
Rating = IF(@update_record, VALUES(Rating), Rating),
RequiredAlignmentInverted = IF(@update_record, VALUES(RequiredAlignmentInverted), RequiredAlignmentInverted),
RequiredClasses = IF(@update_record, VALUES(RequiredClasses), RequiredClasses),
RequiredGender = IF(@update_record, VALUES(RequiredGender), RequiredGender),
RequiredProfession = IF(@update_record, VALUES(RequiredProfession), RequiredProfession),
RequiredProfessionLevel = IF(@update_record, VALUES(RequiredProfessionLevel), RequiredProfessionLevel),
RequiredSocialTier = IF(@update_record, VALUES(RequiredSocialTier), RequiredSocialTier),
RequiredValorRank = IF(@update_record, VALUES(RequiredValorRank), RequiredValorRank),
RequiresAlignment = IF(@update_record, VALUES(RequiresAlignment), RequiresAlignment),
RequiresSocial = IF(@update_record, VALUES(RequiresSocial), RequiresSocial),
Schematic = IF(@update_record, VALUES(Schematic), Schematic),
SchematicId = IF(@update_record, VALUES(SchematicId), SchematicId),
ShieldSpec = IF(@update_record, VALUES(ShieldSpec), ShieldSpec),
Slots = IF(@update_record, VALUES(Slots), Slots),
StatModifiers = IF(@update_record, VALUES(StatModifiers), StatModifiers),
SubCategory = IF(@update_record, VALUES(SubCategory), SubCategory),
TreasurePackageId = IF(@update_record, VALUES(TreasurePackageId), TreasurePackageId),
TreasurePackageSpec = IF(@update_record, VALUES(TreasurePackageSpec), TreasurePackageSpec),
UniqueLimit = IF(@update_record, VALUES(UniqueLimit), UniqueLimit),
UseAbilityId = IF(@update_record, VALUES(UseAbilityId), UseAbilityId),
Value = IF(@update_record, VALUES(Value), Value),
VendorStackSize = IF(@update_record, VALUES(VendorStackSize), VendorStackSize),
WeaponSpec = IF(@update_record, VALUES(WeaponSpec), WeaponSpec),
TypeBitSet = IF(@update_record, VALUES(TypeBitSet), TypeBitSet),
Hash = IF(@update_record, VALUES(Hash), Hash),
StackCount = IF(@update_record, VALUES(StackCount), StackCount),
MaxDurability = IF(@update_record, VALUES(MaxDurability), MaxDurability),
WeaponAppSpec = IF(@update_record, VALUES(WeaponAppSpec), WeaponAppSpec),
Model = IF(@update_record, VALUES(Model), Model),
ImperialVOModulation = IF(@update_record, VALUES(ImperialVOModulation), ImperialVOModulation),
RepublicVOModulation = IF(@update_record, VALUES(RepublicVOModulation), RepublicVOModulation);")},
                {"Abilities", new SQLInitStore(
                    "INSERT INTO `ability` (`current_version`, `previous_version`, `Name`, `NodeId`, `NameId`, `Description`, `DescriptionId`, `Fqn`, `Level`, `Icon`, `IsHidden`, `IsPassive`, `Cooldown`, `CastingTime`, `ForceCost`, `EnergyCost`, `ApCost`, `ApType`, `MinRange`, `MaxRange`, `Gcd`, `GcdOverride`, `ModalGroup`, `SharedCooldown`, `TalentTokens`, `AbilityTokens`, `TargetArc`, `TargetArcOffset`, `TargetRule`, `LineOfSightCheck`, `Pushback`, `IgnoreAlacrity`, `Hash`) VALUES ",
                    @"ON DUPLICATE KEY UPDATE 
previous_version = IF((@update_record := (Hash <> VALUES(Hash))), current_version, previous_version),
current_version = IF(@update_record, VALUES(current_version), current_version),
Name = IF(@update_record, VALUES(Name), Name),
NodeId = IF(@update_record, VALUES(NodeId), NodeId),
NameId = IF(@update_record, VALUES(NameId), NameId),
Description = IF(@update_record, VALUES(Description), Description),
DescriptionId = IF(@update_record, VALUES(DescriptionId), DescriptionId),
Fqn = IF(@update_record, VALUES(Fqn), Fqn),
Level = IF(@update_record, VALUES(Level), Level),
Icon = IF(@update_record, VALUES(Icon), Icon),
IsHidden = IF(@update_record, VALUES(IsHidden), IsHidden),
IsPassive = IF(@update_record, VALUES(IsPassive), IsPassive),
Cooldown = IF(@update_record, VALUES(Cooldown), Cooldown),
CastingTime = IF(@update_record, VALUES(CastingTime), CastingTime),
ForceCost = IF(@update_record, VALUES(ForceCost), ForceCost),
EnergyCost = IF(@update_record, VALUES(EnergyCost), EnergyCost),
ApCost = IF(@update_record, VALUES(ApCost), ApCost),
ApType = IF(@update_record, VALUES(ApType), ApType),
MinRange = IF(@update_record, VALUES(MinRange), MinRange),
MaxRange = IF(@update_record, VALUES(MaxRange), MaxRange),
Gcd = IF(@update_record, VALUES(Gcd), Gcd),
GcdOverride = IF(@update_record, VALUES(GcdOverride), GcdOverride),
ModalGroup = IF(@update_record, VALUES(ModalGroup), ModalGroup),
SharedCooldown = IF(@update_record, VALUES(SharedCooldown), SharedCooldown),
TalentTokens = IF(@update_record, VALUES(TalentTokens), TalentTokens),
AbilityTokens = IF(@update_record, VALUES(AbilityTokens), AbilityTokens),
TargetArc = IF(@update_record, VALUES(TargetArc), TargetArc),
TargetArcOffset = IF(@update_record, VALUES(TargetArcOffset), TargetArcOffset),
TargetRule = IF(@update_record, VALUES(TargetRule), TargetRule),
LineOfSightCheck = IF(@update_record, VALUES(LineOfSightCheck), LineOfSightCheck),
Pushback = IF(@update_record, VALUES(Pushback), Pushback),
IgnoreAlacrity = IF(@update_record, VALUES(IgnoreAlacrity), IgnoreAlacrity),
Hash = IF(@update_record, VALUES(Hash), Hash);")}
                    
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
            string create = File.ReadAllText("SQL Files\\item_create.sql");

            sqlexec(create);

            //Npcs

            //Abilities
            create = File.ReadAllText("SQL Files\\abilities_create.sql");

            sqlexec(create);
            //Quests

            addtolist("Database Tables created.");
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

        public string InitBegin = "";
        public string InitEnd = "";
    }
}
