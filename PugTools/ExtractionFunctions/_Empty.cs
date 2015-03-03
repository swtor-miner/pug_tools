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
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using GomLib;
using Newtonsoft.Json;
using MessageBox = System.Windows.Forms.MessageBox;

namespace tor_tools
{
    public partial class Tools
    {
        public void getEmpty()
        {
            Clearlist2();

            LoadData();

            var data = currentDom.GetObject("...").Data.Get<List<object>>("...");

            bool addedChanged = false;
            string changed = "";
            if (chkBuildCompare.Checked)
            {
                addedChanged = true;
                changed = "Changed";
            }
            var filename = changed + "....xml";
            if (outputTypeName == "Text")
            {
                filename = changed + "....txt";
                EmptyFromPrototypeAsTxt(data);
            }
            else if (outputTypeName == "JSON")
            {
                EmptyFromPrototypeAsJSON(ref data);
            }
            else
            {
                EmptyFromPrototypeAsXElement(data, addedChanged);
            }
            UnloadEmpty();
            EnableButtons();
        }


        #region Txt

        private void EmptyFromPrototypeAsTxt(List<object> repRankData)
        {
            //double i = 0;
            //double e = 0;
            //string n = Environment.NewLine;
            //var txtFile = new StringBuilder();

            //foreach (var rankEntry in repRankData)
            //{
            //    GomLib.Models.Empty rank = new GomLib.Models.Empty();
            //    currentDom.reputationRankLoader.Load(rank, (GomObjectData)rankEntry);

            //    addtolist2("Rank Title: " + rank.RankTitle);
            //    string t = "  ";
            //    txtFile.Append("Rank Title: " + rank.RankTitle + n);
            //    txtFile.Append(t + "Rank Id: " + rank.RankId + n);
            //    txtFile.Append(t + "Rank Title Id: " + rank.RankTitleId + n);
            //    txtFile.Append(t + "Rank Points: " + rank.RankPoints + n);
            //    i++;

            //}
            //WriteFile(txtFile.ToString(), "Reputation_Ranks.txt", false);
            //addtolist("the Reputation Rank list has been generated there are " + i + " Reputation Ranks");
        }
        #endregion
        #region XML

        private XElement EmptyFromPrototypeAsXElement(List<object> rankProto, bool addedChangedOnly)
        {
            XElement ranks = new XElement("Ranks");
            //...

            addtolist("The Empty list has been generated there are " + 0 + " Empty");
            return ranks;
        }
                
        #endregion
        # region JSON

        private void EmptyFromPrototypeAsJSON(ref List<object> proto)
        {
            //double i = 0;
            //string n = Environment.NewLine;

            //StringBuilder txtFile = new StringBuilder();
            //StringWriter txtWriter = new StringWriter(txtFile);
            //JsonWriter jsonWriter = new JsonTextWriter(txtWriter);
            //jsonWriter.Formatting = Newtonsoft.Json.Formatting.Indented;

            //foreach (var entry in proto)
            //{
            //    GomLib.Models.ReputationRank model = new GomLib.Models.ReputationRank();
            //    currentDom.reputationRankLoader.Load(model, (GomObjectData)entry);
            //    addtolist2("Rank Id: " + model.RankId);

            //    jsonWriter.WriteStartObject();

            //    jsonWriter.WritePropertyName("Rank Id");
            //    jsonWriter.WriteValue(model.RankId);

            //    jsonWriter.WritePropertyName("Rank Title Id");
            //    jsonWriter.WriteValue(model.RankTitleId);

            //    jsonWriter.WritePropertyName("Rank Title");
            //    jsonWriter.WriteValue(model.RankTitle);
            //    jsonWriter.WritePropertyName("Rank Points");
            //    jsonWriter.WriteValue(model.RankPoints);

            //    jsonWriter.WriteEndObject();

            //    i++;
            //}
            //string path = "\\JSON\\";
            //if (!System.IO.Directory.Exists(Config.ExtractPath + prefix + path)) { System.IO.Directory.CreateDirectory(Config.ExtractPath + prefix + path); }
            //WriteFile(txtFile.ToString(), path + "Empty.json", false);
            //addtolist("the Empty list has been generated there are " + i + " Ranks");
        }
        #endregion

        private void UnloadEmpty()
        {
            //currentDom.reputationRankLoader.Flush();
        }
    }
}
