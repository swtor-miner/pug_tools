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
        public void getnpc()
        {
            Clearlist2();
            
            LoadData();
            var itmList = currentDom.GetObjectsStartingWith("npc."); //.Where(obj => !obj.Name.StartsWith("npc.test."));
            double ttl = itmList.Count();
            bool append = false;
            bool addedChanged = false;
            string changed = "";
            if(chkBuildCompare.Checked)
            {
                addedChanged = true;
                changed = "Changed";
            }
            var filename = changed + "Npcs.xml";

            if(outputTypeName == "Text")
            {
                filename = changed + "Npcs.txt";
                string npcContent = NpcDataFromFqnList(itmList);
                WriteFile(npcContent, filename, append);
            }
            else
            {
                if (addedChanged) ProcessGameObjects("npc.", "Npcs");
                else
                {
                    XDocument xmlContent = new XDocument(NpcDataFromFqnListAsXElement(itmList, addedChanged));
                    WriteFile(xmlContent, filename, append);
                }
            }

            //addtolist("the npc lists has been generated there are " + ttl + " npc's");
            //MessageBox.Show("the npc lists has been generated there are " + ttl + " npc's");
            EnableButtons();
        }

        private string NpcDataFromFqnList(IEnumerable<GomLib.GomObject> itmList)
        {
            double i = 0;
            string n = Environment.NewLine;

            var txtFile = new StringBuilder();
            foreach (var gomItm in itmList)
            {
                GomLib.Models.Npc itm = new GomLib.Models.Npc();
                currentDom.npcLoader.Load(itm, gomItm);

                addtolist2("Name: " + itm.Name);

                txtFile.Append("------------------------------------------------------------" + n);
                txtFile.Append("Name: " + itm.Name + n);
                txtFile.Append("NodeId: " + itm.NodeId + n);
                txtFile.Append("Id: " + itm.Id + n);
                txtFile.Append("  Title: " + itm.Title + n);
                txtFile.Append("  Level (Min/Max): " + itm.MinLevel + "/" + itm.MaxLevel + n);
                txtFile.Append("  Toughness: " + itm.Toughness + n);
                txtFile.Append("  Faction: " + itm.Faction + n);
                txtFile.Append("  Fqn: " + itm.Fqn + n);
                txtFile.Append("  ClassSpec: " + n);
                txtFile.Append("    Name: " + itm.ClassSpec.Name + " (" + n); // + itm.ClassSpec.NameId + ")" + n );
                txtFile.Append("    NodeId: " + itm.ClassSpec.Id + n);
                txtFile.Append("    ID: " + itm.ClassSpec.Id + n);
                txtFile.Append("    Fqn: " + itm.ClassSpec.Fqn + n);
                //txtFile.Append("    Player Class?: " + itm.ClassSpec.IsPlayerClass + n); //Always false
                //txtFile.Append("    Alignment (Dark/Light): " + itm.ClassSpec.AlignmentDark + "/" + itm.ClassSpec.AlignmentLight + n); //Always 0
                txtFile.Append("    Ability Package Id: " + itm.ClassSpec.AbilityPackageId + n);
                txtFile.Append("    Ability Package:" + n);
                txtFile.Append("    --------------------" + n);
                int p = 1;
                if (itm.ClassSpec.AbilityPackage != null)
                {
                    foreach (var ablpack in itm.ClassSpec.AbilityPackage.PackageAbilities)
                    {
                        txtFile.Append("     Package " + p + ":" + n);
                        var abl = ablpack.Ability;
                        txtFile.Append("      Ability Name: " + abl.Name + n);
                        txtFile.Append("      Ability NodeId: " + abl.NodeId + n);
                        //txtFile.Append("      Ability Id: " + abl.Id + n);
                        //txtFile.Append("      Flags (Passive/Hidden): " + abl.IsPassive + "/" + abl.IsHidden + n);
                        txtFile.Append("      Description: " + abl.Description + " (" + abl.DescriptionId + ")" + n);
                        txtFile.Append("      Tokens: " + abl.TalentTokens + n);
                        /*if (abl.ablEffects != null) // May have gone overboard here on the text output. Add check for Verbose flag and radiobutton to set it.
                        {
                            txtFile.Append("      Ability Effects:" + abl.ablEffects.ToArray().ToString() + n);
                        }
                        txtFile.Append("      Fqn: " + abl.Fqn + n);
                        txtFile.Append("      Icon: " + abl.Icon + n);
                        txtFile.Append("      level: " + ablpack.Level + n);
                        if (ablpack.Scales) { txtFile.Append("      levels: " + String.Join(", ", ablpack.Levels.ToArray()) + n); }
                        txtFile.Append("      Range (Min/Max): " + abl.MinRange + "/" + abl.MaxRange + n);
                        if (abl.ChannelingTime != 0) { txtFile.Append("      Channeling Time: " + abl.ChannelingTime + n); }
                        else
                        {
                            if (abl.CastingTime != 0) { txtFile.Append("      Casting Time: " + abl.CastingTime + n); }
                            else { txtFile.Append("      Casting Time: Instant" + n); }
                        }
                        txtFile.Append("      Cooldown Time: " + abl.Cooldown + n);
                        if (abl.GCD != -1) { txtFile.Append("      GCD: " + abl.GCD + n); }
                        if (abl.GcdOverride) { txtFile.Append("      Overrides GCD: " + abl.GcdOverride + n); }
                        txtFile.Append("      LoS Check: " + abl.LineOfSightCheck + n);
                        if (abl.ModalGroup != 0) { txtFile.Append("      Modal Group: " + abl.ModalGroup + n); }
                        if (abl.SharedCooldown != 0) { txtFile.Append("      Shared Cooldown: " + abl.SharedCooldown + n); }
                        if (abl.TargetArc != 0 && abl.TargetArcOffset != 0) { txtFile.Append("      Target Arc/Offset: " + abl.TargetArc + "/" + abl.TargetArcOffset + n); }
                        txtFile.Append("      Target Rule: " + abl.TargetRule + n);*/
                        txtFile.Append("    --------------------" + n);
                        p++;
                    }
                }
                if (itm.Codex != null)
                {
                    txtFile.Append("  Codex: " + itm.Codex.Fqn.ToString() + ": " + itm.Codex.LocalizedName["enMale"] + n);
                }
                /* txtFile.Append("  CompanionOverride: " + itm.CompanionOverride + n); //Always Empty
                txtFile.Append("  Conversation: " + itm.Conversation + n);
                txtFile.Append("  ConversationFqn: " + itm.ConversationFqn + n); */
                txtFile.Append("  DifficultyFlags: " + itm.DifficultyFlags + n);
                if (itm.IsClassTrainer) { txtFile.Append("  ProfessionTrained: " + itm.ProfessionTrained + n); }
                if (itm.IsVendor) { txtFile.Append("  VendorPackages: { " + String.Join(", ", itm.VendorPackages.ToArray<string>()) + " }" + n); }
                txtFile.Append("  LootTableId: " + itm.LootTableId + n);
                txtFile.Append("------------------------------------------------------------" + n);
                txtFile.Append(Environment.NewLine + n);

                string insert_name = itm.Name.Replace("'", "''");
                string insert_title = null;
                if (itm.Title != null)
                {
                    insert_title = itm.Title.Replace("'", "''");
                }

                //sqlexec("INSERT INTO `npc` (`npc_name`,`npc_nodeid`, `npc_id`, `ClassSpec`, `Codex`, `CompanionOverride`, `Conversation`, `ConversationFqn`, `DifficultyFlags`, `Faction`, `Fqn`, `IsClassTrainer`, `IsVendor`, `LootTableId`, `MaxLevel`, `MinLevel`, `ProfessionTrained`, `Title`, `Toughness`, `VendorPackages`) VALUES ('" + insert_name + "', '" + itm.NodeId + "', '" + itm.Id + "', '" + itm.ClassSpec + "', '" + itm.Codex + "', '" + itm.CompanionOverride + "', '" + itm.Conversation + "', '" + itm.ConversationFqn + "', '" + itm.DifficultyFlags + "', '" + itm.Faction + "', '" + itm.Fqn + "', '" + itm.IsClassTrainer + "', '" + itm.IsVendor + "', '" + itm.LootTableId + "', '" + itm.MaxLevel + "', '" + itm.MinLevel + "', '" + itm.ProfessionTrained + "', '" + insert_title + "', '" + itm.Toughness + "', '" + itm.VendorPackages + "');");

                i++;
            }
            addtolist("The Npc list has been generated there are " + i + " Npcs");
            return txtFile.ToString();
        }

        private XElement NpcDataFromFqnListAsXElement(IEnumerable<GomLib.GomObject> itmList, bool addedChangedOnly)
        {
            double i = 0;
            XElement npcs = new XElement("Npcs");
            foreach (var gomItm in itmList)
            {
                GomLib.Models.Npc itm = new GomLib.Models.Npc();
                currentDom.npcLoader.Load(itm, gomItm);

                addtolist2("NPC Name: " + itm.Name+ " - " + itm.Fqn);

                XElement npcNode = NpcToXElement(itm);
                npcNode.Add(ReferencesToXElement(gomItm.References));
                npcs.Add(npcNode); //add NPC to NPCs

                //Add_NPCs_SQL_DB(itm); //create this method then uncomment
                i++;
            }

            if (addedChangedOnly)
            {
                //addtolist("Comparing the Current NPCs to the loaded Patch");
                XElement addedItems = FindChangedEntries(npcs, "Npcs", "Npc");
                addtolist("The Npc list has been generated there are " + addedItems.Elements("Npc").Count() + " new/changed Npcs.");
                addedItems = SortNpcs(addedItems);
                npcs = null;
                return addedItems;
            }

            npcs = SortNpcs(npcs);
            addtolist("The Npc list has been generated there are " + i + " Npcs");
            return npcs;

        }

        public XElement NpcToXElement(GomObject gomItm)
        {
            return NpcToXElement(gomItm, false);
        }

        public XElement NpcToXElement(GomObject gomItm, bool overrideVerbose)
        {
            if (gomItm != null)
            {
                GomLib.Models.Npc itm = new GomLib.Models.Npc();
                currentDom.npcLoader.Load(itm, gomItm);
                return NpcToXElement(itm, overrideVerbose);
            }
            return null;
        }

        public XElement NpcToXElement(GomLib.Models.Npc itm)
        {
            return NpcToXElement(itm, false);
        }

        public XElement NpcToXElement(GomLib.Models.Npc itm, bool overrideVerbose)
        {
            if (itm.NodeId == 0) return null;

            /* Missing Element Fixers */
            string codexFQN = "none";
            string codexTitle = "none";
            if (itm.Codex != null)
            {
                codexFQN = itm.Codex.Fqn;
                codexTitle = itm.Codex.LocalizedName["enMale"];
            }
            string conversation = null;
            string conversationFQN = null;
            if (itm.Conversation != null)
            {
                conversation = itm.Conversation.Id.ToString();
                conversationFQN = itm.cnvConversationName;
            }

            XElement npcNode = new XElement("Npc",
                new XElement("Fqn", itm.Fqn,
                    new XAttribute("NodeId", itm.NodeId)),
                new XAttribute("Id", itm.NodeId), //this is a hack otherwise the id would always be "376543"
                //new XAttribute("Hash", itm.GetHashCode()),
                new XElement("Name", itm.Name),
                new XElement("MinLevel", itm.MinLevel),
                new XElement("MaxLevel", itm.MaxLevel),
                new XElement("Toughness", itm.Toughness));
            if (verbose && !overrideVerbose)
            {
                if (itm.VisualDataList != null)
                {
                    for(var x = 0; x < itm.VisualDataList.Count; x++)
                    {
                        npcNode.Add(NpcVisualDataToXElement(itm.VisualDataList[x], x));
                    }
                }
                npcNode.Add(new XElement("Faction", itm.Faction),
                new XElement("Codex", new XElement("Title", codexTitle), //add codex section here
                    new XAttribute("Id", codexFQN)),
                new XElement("CompanionOverride", NpcToXElement(itm.CompanionOverride, true)),
                    /*new XElement("Conversation", new XElement("String", conversation), //add code here to cycle through nodes
                        new XElement("Fqn", conversationFQN)), */
                new XElement("DifficultyFlags", itm.DifficultyFlags),
                new XElement("ProfessionTrained", itm.ProfessionTrained),
                new XElement("LootTableId", itm.LootTableId));

                XElement vendorPackages = new XElement("VendorPackages");
                foreach (var venPack in itm.VendorPackages)
                {
                    vendorPackages.Add(new XElement("VendorPackage", venPack));
                }
                npcNode.Add(vendorPackages); //add VendorPackages to NPC

                XElement classSpec = new XElement("ClassSpec",
                    new XElement("Name", itm.ClassSpec.Name,
                        new XAttribute("Id", itm.ClassSpec.NameId)),
                    new XAttribute("Id", itm.ClassSpec.Id),
                    //new XAttribute("Hash", itm.ClassSpec.GetHashCode()),
                    new XElement("Fqn", itm.ClassSpec.Fqn,
                        new XAttribute("Id", itm.ClassSpec.Id)));

                classSpec.Add(new XElement("PlayerClass", itm.ClassSpec.IsPlayerClass),
                        new XElement("AlignmentDark", itm.ClassSpec.AlignmentDark),
                        new XElement("AlignmentLight", itm.ClassSpec.AlignmentLight));

                classSpec.Add(AbilityPackageToXElement(itm.ClassSpec.AbilityPackage));

                npcNode.Add(classSpec); //add ClassSpec to NPC
            }

            return npcNode;
        }

        public XElement NpcVisualDataToXElement(GomLib.Models.NpcVisualData nvd, int i)
        {
            XElement visualDataElement = new XElement("VisualData", new XAttribute("Id", i));

            visualDataElement.Add(new XElement("Skeleton", nvd.CharSpec),
                new XElement("ScaleAdjustment", nvd.ScaleAdjustment),
                new XElement("Appearance", nvd.AppearanceFqn, new XAttribute("Id", nvd.AppearanceId)));
            if (nvd.SpeciesScale != 0) visualDataElement.Add(new XElement("SpeciesScale", nvd.SpeciesScale));
            if (nvd.MeleeWepId != 0) visualDataElement.Add(new XElement("MeleeWep", ConvertToXElement(nvd.MeleeWep, true), new XAttribute("Id", nvd.MeleeWepId)));
            if (nvd.MeleeOffWepId != 0) visualDataElement.Add(new XElement("MeleeOffWep", ConvertToXElement(nvd.MeleeOffWep, true), new XAttribute("Id", nvd.MeleeOffWepId)));
            if (nvd.RangedWepId != 0) visualDataElement.Add(new XElement("RangedWep", ConvertToXElement(nvd.RangedWep, true), new XAttribute("Id", nvd.RangedWepId)));
            if (nvd.RangedOffWepId != 0) visualDataElement.Add(new XElement("RangedOffWep", ConvertToXElement(nvd.RangedOffWep, true), new XAttribute("Id", nvd.RangedOffWepId)));
            if (nvd.AppearanceId != 0)
            {
                XElement npp = NpcAppearanceToXElement(nvd.Appearance, nvd.AppearanceId);
                visualDataElement.Add(npp);
                if (ExportNPP)
                {
                    WriteFile(new XDocument(npp), nvd.AppearanceFqn.Replace(".", "\\") + ".npp", false);
                }
            }

            return visualDataElement;
        }

        public XElement NpcAppearanceToXElement(GomLib.Models.NpcAppearance npp, ulong i)
        {
            XElement npcAppearance = new XElement("NpcAppearance", new XAttribute("Id", i));

            if (npp == null) return npcAppearance;

            npcAppearance.Add(new XElement("Type", npp.NppType),
                new XElement("BodyType", npp.BodyType),
                new XElement("Fqn", npp.Fqn));
            if (npp.AppearanceSlotMap.Count > 0)
            {
                XElement slotMap = new XElement("AppearanceSlotMap");
                foreach (var appSlot in npp.AppearanceSlotMap)
                {
                    XElement slot = new XElement(appSlot.Key);
                    foreach (var randomSlot in appSlot.Value)
                    {
                        slot.Add(AppearanceSlotToXElement(randomSlot));
                    }
                    slotMap.Add(slot);
                }
                npcAppearance.Add(slotMap);
            }

            return npcAppearance;
        }

        public XElement AppearanceSlotToXElement(GomLib.Models.AppSlot app)
        {
            XElement appearanceSlot = new XElement("AppearanceSlot", new XAttribute("Id", app.ModelID));

            appearanceSlot.Add(new XElement("Type", app.Type));
            if (app.ModelID != 0) appearanceSlot.Add(new XElement("Model", app.Model, new XAttribute("Id", app.ModelID)));
            if (app.MaterialIndex != 0) appearanceSlot.Add(new XElement("Material", app.Material0, new XAttribute("Id", app.MaterialIndex)),
                new XElement("MaterialMirror", app.Material0, new XAttribute("Id", app.MaterialIndex)));
            if (app.Attachments.Count > 0)
            {
                XElement attachments = new XElement("Attachments");
                for (int i = 0; i < app.Attachments.Count; i++)
                {
                    attachments.Add(new XElement("Attachment", app.AttachedModels[i], new XAttribute("Id", app.Attachments[i])));
                }
                appearanceSlot.Add(attachments);
            }
            if (app.PrimaryHueId != 0) appearanceSlot.Add(new XElement("PrimaryHue", app.PrimaryHue, new XAttribute("Id", app.PrimaryHueId)));
            if (app.SecondaryHueId != 0) appearanceSlot.Add(new XElement("SecondaryHue", app.SecondaryHue, new XAttribute("Id", app.SecondaryHueId)));

            appearanceSlot.Add(new XElement("RandomWeight", app.RandomWeight));
            return appearanceSlot;
        }
        
        private XElement SortNpcs(XElement npcs)
        {
            //addtolist("Sorting Npcs");
            npcs.ReplaceNodes(npcs.Elements("Npc")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Element("Fqn"))
                .ThenBy(x => (string)x.Element("Name"))
                .ThenBy(x => (string)x.Attribute("Id")));
            return npcs;
        }
    }
}
