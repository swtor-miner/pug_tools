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
        delegate void SetTextCallback(string text);
        delegate void SetText2Callback(string text);
        delegate void ClearlistCallback();
        delegate void ProgressCallback(int progress, int count);
        delegate void ClearProgressCallback();
        delegate void ContinuousProgressCallback();

        //Should probably replicate this for the model and node viewers.
        Form AssetBrowser = null;
        Form ModelBrowser = null;

        #region UI Element Event Receivers
        #region Folder Find Buttons
        private void buttonFindAssets_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = textBoxAssetsFolder.Text;
            DialogResult result = fbd.ShowDialog();
            if (fbd.SelectedPath.EndsWith("\\"))
            {
                textBoxAssetsFolder.Text = fbd.SelectedPath;
            }
            else
            {
                textBoxAssetsFolder.Text = fbd.SelectedPath + "\\";
            }
        }

        private void buttonSelectExtractFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = textBoxExtractFolder.Text;
            DialogResult result = fbd.ShowDialog();
            if (fbd.SelectedPath.EndsWith("\\"))
            {
                textBoxExtractFolder.Text = fbd.SelectedPath;
            }
            else
            {
                textBoxExtractFolder.Text = fbd.SelectedPath + "\\";
            }
        }

        private void buttonFindPrevAssets_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = textBoxPrevAssetsFolder.Text;
            DialogResult result = fbd.ShowDialog();
            if (fbd.SelectedPath.EndsWith("\\"))
            {
                textBoxPrevAssetsFolder.Text = fbd.SelectedPath;
            }
            else
            {
                textBoxPrevAssetsFolder.Text = fbd.SelectedPath + "\\";
            }
        }
        #endregion
        private void button3_Click(object sender, EventArgs e)
        {
            if (sql)
            {
                listBox1.Items.Add("Mysql is now OFF");
                buttonToggleDatabase.Text = "Mysql OFF";
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                textBox5.Enabled = false;
                sql = false;
            }
            else
            {
                listBox1.Items.Add("Mysql is now ON");
                buttonToggleDatabase.Text = "Mysql ON";
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                textBox5.Enabled = true;
                sql = true;
            }
        }

        private void extractButton_Click(object sender, EventArgs e)
        {
            DisableButtons();
            try
            {
                string selected = comboBoxExtractTypes.SelectedItem.ToString();
                if (cbxExtractFormat.SelectedItem.ToString() == "SQL" && versionTexBox.Text == "")
                {
                    System.Windows.Forms.MessageBox.Show("A patch version number is required for SQL Output.");
                    EnableButtons();
                    return;
                }
                if (!System.IO.Directory.Exists(Config.ExtractPath + prefix)) { System.IO.Directory.CreateDirectory(Config.ExtractPath + prefix); }
                /*ThreadStart t = new ThreadStart(getAll);
                Thread oGetItems = new Thread(t);
                oGetItems.Start();*/
                ThreadStart t = null;
                switch (selected)
                {
                    case "(Everything)": t = new ThreadStart(getAll);
                        break;
                    case "Abilities": t = () => getObjects("abl.", "Abilities");
                        break;
                    case "Achievements": t = () => getObjects("ach.", "Achievements");
                        break;
                    case "Areas": t = () => getPrototypeObjects("Areas", "mapAreasDataProto", "mapAreasDataObjectList"); //new ThreadStart(getAreas);
                        break;
                    case "Cartel Market": t = () => getPrototypeObjects("MtxStoreFronts", "mtxStorefrontInfoPrototype", "mtxStorefrontData"); //new ThreadStart(getMtx);
                        break;
                    case "Codex": t = () => getObjects("cdx.", "CodexEntries");
                        break;
                    case "Collections": t = () => getPrototypeObjects("Collections", "colCollectionItemsPrototype", "colCollectionItemsData"); //new ThreadStart(getCollect);
                        break;
                    case "Companions": t = () => getPrototypeObjects("Companions", "chrCompanionInfo_Prototype", "chrCompanionInfoData"); //new ThreadStart(getCompanions);
                        break;
                    case "Conversations": t = () => getObjects("cnv.", "Conversations");
                        break;
                    //case "Filenames": t = new ThreadStart(getFilenames);
                        //break;
                    case "Icons": t = new ThreadStart(getIcons);
                        break;
                    case "Items": t = () => getObjects("itm.", "Items");
                        break;
                    case "Item Appearances": t = () => getObjects("ipp.", "ItemAppearances"); //t = new ThreadStart(getItemApps);
                        break;
                    case "Npcs": t = () => getObjects("npc.", "Npcs");
                        break;
                    case "Quests": t = () => getObjects("qst.", "Quests");
                        break;
                    case "Raw GOM": t = new ThreadStart(getRaw);
                        break;
                    case "String Tables": t = new ThreadStart(getStrings);
                        break;
                    case "Starfighter Ships": t = () => getPrototypeObjects("Ships", "scFFShipsDataPrototype", "scFFShipsData"); //new ThreadStart(getSpaceShip);
                        break;
                    case "Talents": t = () => getObjects("tal.", "Talents");
                        break;
                    case "Schematics": t = () => getObjects("schem.", "Schematics");
                        break;
                    case "Decorations": t = () => getObjects("dec.", "Decorations");
                        break;
                    case "Ability Effects": t = () => getObjects("eff.", "Effects");
                        break;
                    case "Strongholds": t = () => getObjects("apt.", "Strongholds");
                        break;
                    case "Conquests": t = () => getPrototypeObjects("Conquests", "wevConquestInfosPrototype", "wevConquestTable");
                        break;
                    case "Advanced Classes": t = () => getObjects("class.pc.advanced", "AdvancedClasses");
                        break;
                    case "FindNewMtxImages": t = new ThreadStart(FindNewMtxImages);
                        break;
                    case "AchCategories": t = () => getPrototypeObjects("AchCategories", "achCategoriesTable_Prototype", "achCategoriesData");
                        break;
                    case "testHashes": t = new ThreadStart(testHashes);
                        break;
                    case "Tooltips": t = new ThreadStart(getTooltips);
                        break;
                    case "Set Bonuses": t = () => getPrototypeObjects("SetBonuses", "itmSetBonusesPrototype", "itmSetBonuses");
                        break;
                    case "Codex Category Totals": t = () => getPrototypeObjects("CodexCategoryTotals", "cdxCategoryTotalsPrototype", "cdxFactionToClassToPlanetToTotalLookupList");
                        break;
                    case "Schematic Variations": t = () => getPrototypeObjects("SchematicVariations", "prfSchematicVariationsPrototype", "prfSchematicVariationMasterList");
						break;
                    case "BuildBnkIdDict": t = new ThreadStart(BuildBnkIdDict);
                        break;
                        
                }
                if (t != null)
                {
                    Thread oGetItems = new Thread(t);
                    oGetItems.Start();
                }
                else
                {
                    EnableButtons();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            DisableButtons();
            listBox1.Items.Add("Geting info for fqn " + textBox6.Text);
            listBox1.Items.Add("Will output raw GOM for model building to:");
            listBox1.Items.Add(Config.ExtractPath + "GOM\\ in xml format");
            Thread oGetItems = new Thread(delegate() { getFqn(textBox6.Text); });
            oGetItems.Start();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            verbose = (this.chkVerbose.CheckState == CheckState.Checked);
            //if (verbose) prefix = "Verbose";
            //else prefix = "";
        }

        private void removeElementsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            removeUnchangedElements = (this.chkRemoveElements.CheckState == CheckState.Checked);
        }

        private void cbxExtractFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            outputTypeName = cbxExtractFormat.SelectedItem.ToString();
        }

        private async void btnUnloadAllData_Click(object sender, EventArgs e)
        {
            DisableButtons();
            Clearlist();
            addtolist("All Assets & DOM - Clearing");
            await Task.Run(() => unloadAll());
            addtolist("All Assets & DOM - Cleared");
            EnableButtons();
        }

        private async void usePTSAssets_CheckedChanged(object sender, EventArgs e)
        {
            string path = textBoxAssetsFolder.Text;
            if(usePTSAssets.Checked)
            {
                if(PathContainsPTSAssets(path))
                {
                    buttonFindAssets.Image = global::PugTools.Properties.Resources.tick_shield;
                }
                else
                {
                    buttonFindAssets.Image = global::PugTools.Properties.Resources.cross_shield;
                }
            }
            else
            {
                if (PathContainsLiveAssets(path))
                {
                    buttonFindAssets.Image = global::PugTools.Properties.Resources.tick_shield;
                }
                else
                {
                    buttonFindAssets.Image = global::PugTools.Properties.Resources.cross_shield;
                }
            }

            if (Loaded)
            {
                DisableButtons();
                Clearlist();
                addtolist("Current Assets & DOM - Clearing");
                await Task.Run(() => unloadCurrent());
                addtolist("Current Assets & DOM - Cleared");
                EnableButtons();
                GC.Collect();
            }
        }

        private async void prevUsePTSAssets_CheckedChanged(object sender, EventArgs e)
        {
            string path = textBoxPrevAssetsFolder.Text;
            if (prevUsePTSAssets.Checked)
            {
                if (PathContainsPTSAssets(path))
                {
                    buttonFindPrevAssets.Image = global::PugTools.Properties.Resources.tick_shield;
                }
                else
                {
                    buttonFindPrevAssets.Image = global::PugTools.Properties.Resources.cross_shield;
                }
            }
            else
            {
                if (PathContainsLiveAssets(path))
                {
                    buttonFindPrevAssets.Image = global::PugTools.Properties.Resources.tick_shield;
                }
                else
                {
                    buttonFindPrevAssets.Image = global::PugTools.Properties.Resources.cross_shield;
                }
            }

            if (Loaded)
            {
                DisableButtons();
                Clearlist();
                addtolist("Previous Assets & DOM - Clearing");
                await Task.Run(() => unloadPrevious());
                addtolist("Previous Assets & DOM - Cleared");
                EnableButtons();
                GC.Collect();
            }
        }

        private void btnAssetBrowser_Click(object sender, EventArgs e)
        {
            if (AssetBrowser == null || AssetBrowser.IsDisposed)
            {
                bool usePTS = this.usePTSAssets.Checked;
                AssetBrowser = new AssetBrowser(this.textBoxAssetsFolder.Text, usePTS);
                AssetBrowser.FormClosed += onAssetBrowserClosed;
                AssetBrowser.Show();
                AssetBrowser.Focus();
            }
            else
            {
                AssetBrowser.Focus();
            }
        }

        public void onAssetBrowserClosed(object sender, FormClosedEventArgs e)
        {
            AssetBrowser = null;
            System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }

        private void btnNodeBrowser_Click(object sender, EventArgs e)
        {
            bool usePTS = this.usePTSAssets.Checked;
            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.SustainedLowLatency;
            Form NodeBrowser = new NodeBrowser(this.textBoxAssetsFolder.Text, usePTS, this.textBoxExtractFolder.Text);
            NodeBrowser.Show();
        }

        private void btnModelBrowser_Click(object sender, EventArgs e)
        {
            bool usePTS = this.usePTSAssets.Checked;
            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.SustainedLowLatency;
            ModelBrowser = new ModelBrowser(this.textBoxAssetsFolder.Text, usePTS, this.textBoxPrevAssetsFolder.Text, prevUsePTSAssets.Checked);
            ModelBrowser.FormClosed += onModelBrowserClosed;
            ModelBrowser.Show();
        }

        private void onModelBrowserClosed(object sender, FormClosedEventArgs e)
        {
            ModelBrowser = null;
            System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }

        private void btnWorldBrowser_Click(object sender, EventArgs e)
        {
            bool usePTS = this.usePTSAssets.Checked;
            Form WorldBrowser = new WorldBrowser(this.textBoxAssetsFolder.Text, usePTS);
            WorldBrowser.Show();
        }

        #region Saved Config Events
        private void CrossLinkDomCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Config.CrossLinkDOM = CrossLinkDomCheckBox.Checked;
            Config.Save();
        }

        private async void textBox1_TextChanged(object sender, EventArgs e)
        {
            string path = textBoxAssetsFolder.Text;
            if (path.Length > 0 && !path.EndsWith("\\"))
            {
                path += "\\";
            }
            Config.AssetsPath = path;
            Config.Save();

            bool hasLive = PathContainsLiveAssets(path);
            bool hasPTS = PathContainsPTSAssets(path);
            if(hasLive && !hasPTS)
            {
                buttonFindAssets.Image = global::PugTools.Properties.Resources.tick_shield;
                usePTSAssets.Checked = false;
            } else if(!hasLive && hasPTS) {
                buttonFindAssets.Image = global::PugTools.Properties.Resources.tick_shield;
                usePTSAssets.Checked = true;
            } else if(!hasLive && !hasPTS) {
                buttonFindAssets.Image = global::PugTools.Properties.Resources.cross_shield;
            } else {
                //Has both live and pts assets.
                buttonFindAssets.Image = global::PugTools.Properties.Resources.tick_shield;
            }

            if (Loaded)
            {
                DisableButtons();
                Clearlist();
                addtolist("Current Assets & DOM - Clearing");
                await Task.Run(() => unloadCurrent());
                addtolist("Current Assets & DOM - Cleared");
                EnableButtons();
                //Unload();
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            string path = textBoxExtractFolder.Text;
            if(path.Length > 0 && !path.EndsWith("\\"))
            {
                path += "\\";
            }
            Config.ExtractPath = path;
            Config.Save();
        }

        private async void textBoxPrevAssetsFolder_TextChanged(object sender, EventArgs e)
        {
            string path = textBoxPrevAssetsFolder.Text;
            if (path.Length > 0 && !path.EndsWith("\\"))
            {
                path += "\\";
            }

            bool hasLive = PathContainsLiveAssets(path);
            bool hasPTS = PathContainsPTSAssets(path);
            if (hasLive && !hasPTS)
            {
                buttonFindPrevAssets.Image = global::PugTools.Properties.Resources.tick_shield;
                prevUsePTSAssets.Checked = false;
            }
            else if (!hasLive && hasPTS)
            {
                buttonFindPrevAssets.Image = global::PugTools.Properties.Resources.tick_shield;
                prevUsePTSAssets.Checked = true;
            }
            else if (!hasLive && !hasPTS)
            {
                buttonFindPrevAssets.Image = global::PugTools.Properties.Resources.cross_shield;
            }
            else
            {
                //Has both live and pts assets.
                buttonFindPrevAssets.Image = global::PugTools.Properties.Resources.tick_shield;
            }

            Config.PrevAssetsPath = path;
            Config.Save();
            if (Loaded)
            {
                DisableButtons();
                Clearlist();
                addtolist("Previous Assets & DOM - Clearing");
                await Task.Run(() => unloadPrevious());
                addtolist("Previous Assets & DOM - Cleared");
                EnableButtons();
                GC.Collect();
                //Unload();
            }
        }
        
        private void chkBuildCompare_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBuildCompare.Checked && Loaded && previousDom == null)
            {
                DisableButtons();
                Clearlist();
                addtolist("All Assets & DOM - Clearing");
                unloadAll();
                addtolist("All Assets & DOM - Cleared");
                EnableButtons();
            }
        }
        #endregion
        #endregion
        #region Invokers
        private void DisableButtons()
        {
            if (this.textBoxAssetsFolder.InvokeRequired)
            {
                this.Invoke((Action)DisableButtons);
            }
            else
            {
                textBoxAssetsFolder.Enabled = false;
                textBoxExtractFolder.Enabled = false;
                textBoxPrevAssetsFolder.Enabled = false;
                buttonSelectExtractFolder.Enabled = false;
                buttonFindAssets.Enabled = false;
                buttonFindPrevAssets.Enabled = false;
                gbxFormat.Enabled = false;
                gbxDB.Enabled = false;
                gbxExtract.Enabled = false;
                gbxTools.Enabled = false;
                gbxFQN.Enabled = false;
                gbxLogs.Enabled = false;

                ContinuousProgress();
            }
        }

        private void EnableButtons()
        {
            if (this.textBoxAssetsFolder.InvokeRequired)
            {
                this.Invoke((Action)EnableButtons);
            }
            else
            {
                textBoxAssetsFolder.Enabled = true;
                textBoxExtractFolder.Enabled = true;
                textBoxPrevAssetsFolder.Enabled = true;
                buttonSelectExtractFolder.Enabled = true;
                buttonFindAssets.Enabled = true;
                buttonFindPrevAssets.Enabled = true;
                gbxFormat.Enabled = true;
                gbxDB.Enabled = true;
                gbxExtract.Enabled = true;
                gbxTools.Enabled = true;
                gbxFQN.Enabled = true;
                gbxLogs.Enabled = true;

                ClearProgress();
            }
        }

        private void Clearlist()
        {
            if (this.listBox1.InvokeRequired)
            {
                ClearlistCallback d = new ClearlistCallback(Clearlist);
                this.Invoke(d, new object[] { });
            }
            else
            {
                this.listBox1.Items.Clear();
            }
        }

        private void Clearlist2()
        {
            if (this.listBox1.InvokeRequired)
            {
                ClearlistCallback d = new ClearlistCallback(Clearlist2);
                this.Invoke(d, new object[] { });
            }
            else
            {
                this.listBox2.Items.Clear();
            }
        }

        private void addtolist(string text)
        {
            if (this.listBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(addtolist);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.listBox1.Items.Add(text);
                //this.listBox1.SelectedIndex = listBox1.Items.Count - 1;
                //this.listBox1.SelectedIndex = -1;
                this.listBox1.TopIndex = listBox1.Items.Count - 1;

            }
        }

        private void addtolist2(string text)
        {
            if (this.listBox2.InvokeRequired)
            {
                SetText2Callback d = new SetText2Callback(addtolist2);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.listBox2.Items.Add(text);
                //this.listBox2.SelectedIndex++; //this causes a huge amount of extra processing for no real payoff
            }
        }

        private void progressUpdate(int progress, int count)
        {
            if (this.progressBar1.InvokeRequired)
            {
                ProgressCallback d = new ProgressCallback(progressUpdate);
                this.Invoke(d, new object[] { progress, count });
            }
            else
            {
                int value = 0;
                if (count != 0)
                    value = (int)((progress * 100) / count);
                this.progressBar1.Value = value;
                this.progressBar1.Update();
            }
        }

        private void ClearProgress()
        {
            if (this.progressBar1.InvokeRequired)
            {
                ClearProgressCallback d = new ClearProgressCallback(ClearProgress);
                this.Invoke(d, new object[] { });
            }
            else
            {
                this.progressBar1.Style = ProgressBarStyle.Blocks;
                this.progressBar1.Value = 0;
            }
        }

        private void ContinuousProgress()
        {
            if (this.progressBar1.InvokeRequired)
            {
                ContinuousProgressCallback d = new ContinuousProgressCallback(ContinuousProgress);
                this.Invoke(d, new object[] { });
            }
            else
            {
                this.progressBar1.Style = ProgressBarStyle.Marquee;
                this.progressBar1.MarqueeAnimationSpeed = 25;
            }
        }

        #endregion
    }
}
