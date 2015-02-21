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

namespace tor_tools
{
    public partial class Tools
    {
        private XElement SortCodices(XElement codices)
        {
            //addtolist("Sorting Codex Entries");
            codices.ReplaceNodes(codices.Elements("Codex")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Element("Fqn"))
                .ThenBy(x => (string)x.Element("Title"))
                .ThenBy(x => (string)x.Element("ID")));
            return codices;
        }

        public XElement CodexToXElement(GomObject gomItm)
        {
            return CodexToXElement(gomItm, false);
        }

        public XElement CodexToXElement(GomObject gomItm, bool overrideVerbose)
        {
            if (gomItm != null)
            {
                GomLib.Models.Codex itm = new GomLib.Models.Codex();
                currentDom.codexLoader.Load(itm, gomItm);
                gomItm.Unload(); // These GomObjects are staying loaded in memory and cause our massive memory issues.
                return CodexToXElement(itm, overrideVerbose);
            }
            return null;
        }

        private static XElement CodexToXElement(GomLib.Models.Codex itm) 
        {
            return CodexToXElement(itm, false);
        }

        private static XElement CodexToXElement(GomLib.Models.Codex itm, bool overrideVerbose) //split to see if it was necessary to loop through linked codices. Didn't seem like it.
        {
            XElement codex = new XElement("Codex");

            codex.Add(new XElement("Title", itm.Name),
                new XElement("NodeId", itm.NodeId),
                new XAttribute("Id", itm.Id),
                //new XAttribute("Hash", itm.GetHashCode()),
                new XElement("CategoryId", itm.CategoryId));
            if (verbose && !overrideVerbose)
            {
                string reqclasses = null;
                if (itm.Classes != null)
                {
                    foreach (var reqclass in itm.Classes)
                    {
                        reqclasses += reqclass.Name.ToString() + ", ";
                    }
                }
                if (reqclasses != null) { reqclasses = reqclasses.Substring(0, reqclasses.Length - 2); }
                codex.Add(new XElement("Classes", reqclasses));
                codex.Add(new XElement("ClassRestricted", itm.ClassRestricted),
                    new XElement("Faction", itm.Faction),
                    new XElement("Fqn", itm.Fqn),
                    new XElement("HasPlanets", itm.HasPlanets),
                    new XElement("Image", itm.Image),
                    new XElement("IsHidden", itm.IsHidden),
                    new XElement("IsPlanet", itm.IsPlanet),
                    new XElement("Level", itm.Level),
                    new XElement("Text", itm.LocalizedDescription["enMale"]));
                XElement subCodices = new XElement("LinkedCodexEntries");
                if (itm.HasPlanets && itm.Planets != null)
                {
                    foreach (var planet in itm.Planets)
                    {
                        if (planet != null) subCodices.Add(new XElement("Codex", planet.Fqn, new XAttribute("Id", planet.Id))); //change this to call the parent method to iterate through linked codices
                    }
                    codex.Add(subCodices);
                }
                //codex.Add(subCodices);
            }
            return codex;
        }
    }
}
