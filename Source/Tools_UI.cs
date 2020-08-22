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
using nsHashDictionary;

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
        Form NodeBrowser = null;
        Form ModelBrowser = null;

        #region UI Element Event Receivers
        #region Folder Find Buttons
        private void ButtonFindAssets_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                SelectedPath = textBoxAssetsFolder.Text
            };
            _ = fbd.ShowDialog();
            if (fbd.SelectedPath.EndsWith("\\"))
            {
                textBoxAssetsFolder.Text = fbd.SelectedPath;
            }
            else
            {
                textBoxAssetsFolder.Text = fbd.SelectedPath + "\\";
            }
        }

        private void ButtonSelectExtractFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                SelectedPath = textBoxExtractFolder.Text
            };
            _ = fbd.ShowDialog();
            if (fbd.SelectedPath.EndsWith("\\"))
            {
                textBoxExtractFolder.Text = fbd.SelectedPath;
            }
            else
            {
                textBoxExtractFolder.Text = fbd.SelectedPath + "\\";
            }
        }

        private void ButtonFindPrevAssets_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                SelectedPath = textBoxPrevAssetsFolder.Text
            };
            _ = fbd.ShowDialog();
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
        private void Button3_Click(object sender, EventArgs e)
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

        private void ExtractButton_Click(object sender, EventArgs e)
        {
            DisableButtons();
            try
            {
                string selected = comboBoxExtractTypes.SelectedItem.ToString();
                if ((cbxExtractFormat.SelectedItem.ToString() == "SQL" || cbxExtractFormat.SelectedItem.ToString() == "JSON") && versionTexBox.Text == "")
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
                    case "(Everything)":
                        t = new ThreadStart(GetAll);
                        break;
                    case "Abilities":
                        t = () => GetObjects("abl.", "Abilities");
                        break;
                    case "Achievements":
                        t = () => GetObjects("ach.", "Achievements");
                        break;
                    case "Areas":
                        t = () => GetPrototypeObjects("Areas", "mapAreasDataProto", "mapAreasDataObjectList"); //new ThreadStart(getAreas);
                        break;
                    case "Cartel Market":
                        t = () => GetPrototypeObjects("MtxStoreFronts", "mtxStorefrontInfoPrototype", "mtxStorefrontData"); //new ThreadStart(getMtx);
                        break;
                    case "Codex":
                        t = () => GetObjects("cdx.", "CodexEntries");
                        break;
                    case "Collections":
                        t = () => GetPrototypeObjects("Collections", "colCollectionItemsPrototype", "colCollectionItemsData"); //new ThreadStart(getCollect);
                        break;
                    case "Companions":
                        t = () => GetPrototypeObjects("Companions", "chrCompanionInfo_Prototype", "chrCompanionInfoData"); //new ThreadStart(getCompanions);
                        break;
                    case "Conversations":
                        t = () => GetObjects("cnv.", "Conversations");
                        break;
                    //case "Filenames": t = new ThreadStart(getFilenames);
                    //break;
                    case "Icons":
                        t = new ThreadStart(GetIcons);
                        break;
                    case "Items":
                        t = () => GetObjects("itm.", "Items");
                        break;
                    case "Item Appearances":
                        t = () => GetObjects("ipp.", "ItemAppearances"); //t = new ThreadStart(getItemApps);
                        break;
                    case "NPCs":
                        t = () => GetObjects("npc.", "Npcs");
                        break;
                    case "Quests":
                        t = () => GetObjects("qst.", "Quests");
                        break;
                    case "Raw GOM":
                        t = new ThreadStart(GetRaw);
                        break;
                    case "String Tables":
                        t = new ThreadStart(GetStrings);
                        break;
                    case "GSF Ships":
                        t = () => GetPrototypeObjects("Ships", "scFFShipsDataPrototype", "scFFShipsData"); //new ThreadStart(getSpaceShip);
                        break;
                    case "Talents":
                        t = () => GetObjects("tal.", "Talents");
                        break;
                    case "Schematics":
                        t = () => GetObjects("schem.", "Schematics");
                        break;
                    case "Decorations":
                        t = () => GetObjects("dec.", "Decorations");
                        break;
                    case "Ability Effects":
                        t = () => GetObjects("eff.", "Effects");
                        break;
                    case "Strongholds":
                        t = () => GetObjects("apt.", "Strongholds");
                        break;
                    case "Conquests":
                        t = () => GetPrototypeObjects("Conquests", "wevConquestInfosPrototype", "wevConquestTable");
                        break;
                    case "Advanced Classes":
                        t = () => GetObjects("class.pc.advanced", "AdvancedClasses");
                        break;
                    case "Find New MTX Images":
                        t = new ThreadStart(FindNewMtxImages);
                        break;
                    case "Achievement Categories":
                        t = () => GetPrototypeObjects("AchCategories", "achCategoriesTable_Prototype", "achCategoriesData");
                        break;
                    case "Verify Hashes":
                        t = new ThreadStart(VerifyHashes);
                        break;
                    case "Tooltips":
                        t = new ThreadStart(GetTooltips);
                        break;
                    case "Set Bonuses":
                        t = () => GetPrototypeObjects("SetBonuses", "itmSetBonusesPrototype", "itmSetBonuses");
                        break;
                    case "Codex Category Totals":
                        t = () => GetPrototypeObjects("CodexCategoryTotals", "cdxCategoryTotalsPrototype", "cdxFactionToClassToPlanetToTotalLookupList");
                        break;
                    case "Schematic Variations":
                        t = () => GetPrototypeObjects("SchematicVariations", "prfSchematicVariationsPrototype", "prfSchematicVariationMasterList");
                        break;
                    case "Build Bnk ID Dict":
                        t = new ThreadStart(BuildBnkIdDict);
                        break;
                        //case "Dulfy": t = new ThreadStart(getDisciplineCalcData);
                        //    break;
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

        private void SearchButton_Click(object sender, EventArgs e)
        {
            DisableButtons();
            listBox1.Items.Add("Geting info for fqn " + textBox6.Text);
            listBox1.Items.Add("Will output raw GOM for model building to:");
            listBox1.Items.Add(Config.ExtractPath + "GOM\\ in xml format");
            Thread oGetItems = new Thread(delegate () { GetFqn(textBox6.Text); });
            oGetItems.Start();
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            verbose = (this.chkVerbose.CheckState == CheckState.Checked);
            //if (verbose) prefix = "Verbose";
            //else prefix = "";
        }

        private void RemoveElementsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            removeUnchangedElements = (this.chkRemoveElements.CheckState == CheckState.Checked);
        }

        private void CbxExtractFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            outputTypeName = cbxExtractFormat.SelectedItem.ToString();
        }

        private async void BtnUnloadAllData_Click(object sender, EventArgs e)
        {
            DisableButtons();
            Clearlist();
            Addtolist("All Assets & DOM - Clearing");
            await Task.Run(() => UnloadAll());
            Addtolist("All Assets & DOM - Cleared");
            EnableButtons();
        }

        private async void UsePTSAssets_CheckedChanged(object sender, EventArgs e)
        {
            Config.AssetsUsePTS = usePTSAssets.Checked;
            Config.Save();

            string path = textBoxAssetsFolder.Text;

            if (usePTSAssets.Checked)
            {
                if (PathContainsPTSAssets(path))
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
                Addtolist("Current Assets & DOM - Clearing");
                await Task.Run(() => UnloadCurrent());
                Addtolist("Current Assets & DOM - Cleared");
                EnableButtons();
                GC.Collect();
            }
        }

        private async void PrevUsePTSAssets_CheckedChanged(object sender, EventArgs e)
        {
            Config.PrevAssetsUsePTS = prevUsePTSAssets.Checked;
            Config.Save();

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
                Addtolist("Previous Assets & DOM - Clearing");
                await Task.Run(() => UnloadPrevious());
                Addtolist("Previous Assets & DOM - Cleared");
                EnableButtons();
                GC.Collect();
            }
        }

        private void BtnAssetBrowser_Click(object sender, EventArgs e)
        {
            if (AssetBrowser == null || AssetBrowser.IsDisposed)
            {
                bool usePTS = this.usePTSAssets.Checked;
                AssetBrowser = new AssetBrowser(this.textBoxAssetsFolder.Text, usePTS);
                AssetBrowser.FormClosed += OnAssetBrowserClosed;
                AssetBrowser.Show();
                AssetBrowser.Focus();
            }
            else
            {
                AssetBrowser.Focus();
            }
        }

        public void OnAssetBrowserClosed(object sender, FormClosedEventArgs e)
        {
            AssetBrowser = null;
            System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }

        private void BtnNodeBrowser_Click(object sender, EventArgs e)
        {
            if (NodeBrowser == null || NodeBrowser.IsDisposed)
            {
                bool usePTS = this.usePTSAssets.Checked;
                NodeBrowser = new NodeBrowser(this.textBoxAssetsFolder.Text, usePTS, this.textBoxExtractFolder.Text);
                System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.SustainedLowLatency;
                NodeBrowser.Show();
                NodeBrowser.Focus();
            }
            else
            {
                NodeBrowser.Focus();
            }
        }

        private void BtnModelBrowser_Click(object sender, EventArgs e)
        {
            if (ModelBrowser == null || ModelBrowser.IsDisposed)
            {
                bool usePTS = this.usePTSAssets.Checked;
                ModelBrowser = new ModelBrowser(this.textBoxAssetsFolder.Text, usePTS, this.textBoxPrevAssetsFolder.Text, prevUsePTSAssets.Checked, chkBuildCompare.Checked);
                System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.SustainedLowLatency;
                ModelBrowser.FormClosed += OnModelBrowserClosed;
                ModelBrowser.Show();
                ModelBrowser.Focus();
            }
            else
            {
                ModelBrowser.Focus();
            }
        }

        private void OnModelBrowserClosed(object sender, FormClosedEventArgs e)
        {
            ModelBrowser = null;
            System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }

        private void BtnWorldBrowser_Click(object sender, EventArgs e)
        {
            bool usePTS = this.usePTSAssets.Checked;
            Form WorldBrowser = new WorldBrowser(this.textBoxAssetsFolder.Text, usePTS);
            WorldBrowser.Show();
        }

        private void File_button_Click(object sender, EventArgs e)
        {
            LoadData();

            var hashdata = TorLib.HashDictionaryInstance.Instance;
            hashdata.Load();
            HashSet<string> current = GetFilenameHashset(currentAssets);
            HashSet<string> previous = GetFilenameHashset(previousAssets);
            var sorted = current.Except(previous).ToList();
            sorted.Sort();
            WriteFile(String.Join(Environment.NewLine, sorted), "newFiles.txt", false);

            EnableButtons();
        }
        private HashSet<string> GetFilenameHashset(TorLib.Assets assets)
        {
            HashSet<string> ret = new HashSet<string>();
            foreach (var lib in assets.libraries)
            {
                string path = lib.Location;
                if (!lib.Loaded)
                    lib.Load();
                if (lib.archives.Count > 0)
                {
                    foreach (var arch in lib.archives)
                    {
                        foreach (var file in arch.Value.files)
                        {
                            TorLib.HashFileInfo hashInfo = new TorLib.HashFileInfo(file.FileInfo.PrimaryHash, file.FileInfo.SecondaryHash, file);

                            if (hashInfo.IsNamed)
                            {
                                if (hashInfo.FileName == "metadata.bin" || hashInfo.FileName == "ft.sig" || hashInfo.FileName == "groupmanifest.bin")
                                {
                                    continue;
                                }
                                ret.Add(hashInfo.Directory + "/" + hashInfo.FileName);
                            }
                            else
                            {
                                ret.Add(hashInfo.Directory + "/" + hashInfo.Extension + "/" + hashInfo.FileName + "." + hashInfo.Extension);
                            }
                        }
                    }
                }
            }
            return ret;
        }

        #region Saved Config Events
        private void CrossLinkDomCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Config.CrossLinkDOM = CrossLinkDomCheckBox.Checked;
            Config.Save();
        }

        private async void TextBox1_TextChanged(object sender, EventArgs e)
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
            if (hasLive && !hasPTS)
            {
                buttonFindAssets.Image = global::PugTools.Properties.Resources.tick_shield;
                usePTSAssets.Checked = false;
            }
            else if (!hasLive && hasPTS)
            {
                buttonFindAssets.Image = global::PugTools.Properties.Resources.tick_shield;
                usePTSAssets.Checked = true;
            }
            else if (!hasLive && !hasPTS)
            {
                buttonFindAssets.Image = global::PugTools.Properties.Resources.cross_shield;
            }
            else
            {
                //Has both live and pts assets.
                buttonFindAssets.Image = global::PugTools.Properties.Resources.tick_shield;
            }

            if (Loaded)
            {
                DisableButtons();
                Clearlist();
                Addtolist("Current Assets & DOM - Clearing");
                await Task.Run(() => UnloadCurrent());
                Addtolist("Current Assets & DOM - Cleared");
                EnableButtons();
                //Unload();
            }
        }

        private void TextBox7_TextChanged(object sender, EventArgs e)
        {
            string path = textBoxExtractFolder.Text;
            if (path.Length > 0 && !path.EndsWith("\\"))
            {
                path += "\\";
            }
            Config.ExtractPath = path;
            Config.Save();
        }

        private async void TextBoxPrevAssetsFolder_TextChanged(object sender, EventArgs e)
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
                Addtolist("Previous Assets & DOM - Clearing");
                await Task.Run(() => UnloadPrevious());
                Addtolist("Previous Assets & DOM - Cleared");
                EnableButtons();
                GC.Collect();
                //Unload();
            }
        }

        private void ChkBuildCompare_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBuildCompare.Checked && Loaded && previousDom == null)
            {
                DisableButtons();
                Clearlist();
                Addtolist("All Assets & DOM - Clearing");
                UnloadAll();
                Addtolist("All Assets & DOM - Cleared");
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

        private void Addtolist(string text)
        {
            if (this.listBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(Addtolist);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.listBox1.Items.Add(text);
                this.listBox1.TopIndex = listBox1.Items.Count - 1;

            }
        }

        private void Addtolist2(string text)
        {
            if (this.listBox2.InvokeRequired)
            {
                SetText2Callback d = new SetText2Callback(Addtolist2);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.listBox2.Items.Add(text);
                this.listBox2.TopIndex = listBox2.Items.Count - 1;
            }
        }

        private void ProgressUpdate(int progress, int count)
        {
            if (this.progressBar1.InvokeRequired)
            {
                ProgressCallback d = new ProgressCallback(ProgressUpdate);
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
