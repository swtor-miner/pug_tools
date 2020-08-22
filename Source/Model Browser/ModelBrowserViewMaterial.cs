using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileFormats;
using TorLib;
using System.Xml;
using ColorCode;
using DevIL;

namespace tor_tools
{
    public partial class ModelBrowserViewMaterial : Form
    {
        private readonly GR2_Material material;
        private readonly XmlDocument xmlDoc = new XmlDocument();
        private readonly Dictionary<string, NodeAsset> matDict = new Dictionary<string, NodeAsset>();

        public ModelBrowserViewMaterial(GR2_Material material)
        {
            InitializeComponent();
            this.material = material;
            this.Refresh();
            backgroundWorker1.RunWorkerAsync();
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Assets currentAssets = AssetHandler.Instance.GetCurrentAssets();
            var materialFile = currentAssets.FindFile("/resources/art/shaders/materials/" + this.material.materialName + ".mat");
            if (materialFile != null)
            {
                var materialStream = materialFile.OpenCopyInMemory();
                this.xmlDoc.Load(materialStream);
            }
            matDict.Add("/", new NodeAsset("/", "", "Root", null));
            matDict.Add("/xml", new NodeAsset("/xml", "/", "XML", this.xmlDoc));

            if (this.material.parsed == false)
                this.material.ParseMAT(null, null);

            DevIL.ImageImporter imp = new ImageImporter();
            DevIL.ImageExporter exp = new ImageExporter();
            DevIL.Image dds;

            if (this.material.diffuseDDS != null)
            {
                var diffuseFile = currentAssets.FindFile(this.material.diffuseDDS);
                if (diffuseFile != null)
                {
                    var memStream = new MemoryStream();
                    var diffuseStream = diffuseFile.OpenCopyInMemory();
                    dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, diffuseStream);
                    exp.SaveImageToStream(dds, ImageType.Png, memStream);
                    Bitmap bm = new Bitmap(memStream);
                    matDict.Add("/diffuse", new NodeAsset("/diffuse", "/", "Diffuse", bm));

                    int width = bm.Width;
                    int height = bm.Height;

                    Bitmap red = new Bitmap(bm);
                    Bitmap green = new Bitmap(bm);
                    Bitmap blue = new Bitmap(bm);
                    Bitmap alpha = new Bitmap(bm);

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            System.Drawing.Color p = bm.GetPixel(x, y);
                            int a = p.A;
                            int r = p.R;
                            int g = p.G;
                            int b = p.B;

                            alpha.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, 0, 0));
                            red.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, 0, 0));
                            green.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, g, 0));
                            blue.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, 0, b));
                        }
                    }

                    matDict.Add("/diffuse/alpha", new NodeAsset("/diffuse/alpha", "/diffuse", "Diffuse - Alpha", alpha));
                    matDict.Add("/diffuse/red", new NodeAsset("/diffuse/red", "/diffuse", "Diffuse - Red", red));
                    matDict.Add("/diffuse/green", new NodeAsset("/diffuse/green", "/diffuse", "Diffuse - Green", green));
                    matDict.Add("/diffuse/blue", new NodeAsset("/diffuse/blue", "/diffuse", "Diffuse - Blue", blue));
                }
            }

            if (this.material.rotationDDS != null)
            {
                var rotationFile = currentAssets.FindFile(this.material.rotationDDS);
                if (rotationFile != null)
                {
                    var memStream = new MemoryStream();
                    var rotationStream = rotationFile.OpenCopyInMemory();
                    dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, rotationStream);
                    exp.SaveImageToStream(dds, ImageType.Png, memStream);
                    Bitmap bm = new Bitmap(memStream);
                    matDict.Add("/rotation", new NodeAsset("/rotation", "/", "Rotation", bm));
                    int width = bm.Width;
                    int height = bm.Height;

                    Bitmap red = new Bitmap(bm);
                    Bitmap green = new Bitmap(bm);
                    Bitmap blue = new Bitmap(bm);
                    Bitmap alpha = new Bitmap(bm);

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            System.Drawing.Color p = bm.GetPixel(x, y);
                            int a = p.A;
                            int r = p.R;
                            int g = p.G;
                            int b = p.B;

                            alpha.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, 0, 0));
                            red.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, 0, 0));
                            green.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, g, 0));
                            blue.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, 0, b));
                        }
                    }

                    matDict.Add("/rotation/alpha", new NodeAsset("/rotation/alpha", "/rotation", "Rotation - Alpha", alpha));
                    matDict.Add("/rotation/red", new NodeAsset("/rotation/red", "/rotation", "Rotation - Red", red));
                    matDict.Add("/rotation/green", new NodeAsset("/rotation/green", "/rotation", "Rotation - Green", green));
                    matDict.Add("/rotation/blue", new NodeAsset("/rotation/blue", "/rotation", "Rotation - Blue", blue));
                }
            }

            if (this.material.glossDDS != null)
            {

                var glossFile = currentAssets.FindFile(this.material.glossDDS);
                if (glossFile != null)
                {
                    var memStream = new MemoryStream();
                    var glossStream = glossFile.OpenCopyInMemory();
                    dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, glossStream);
                    exp.SaveImageToStream(dds, ImageType.Png, memStream);
                    Bitmap bm = new Bitmap(memStream);
                    matDict.Add("/gloss", new NodeAsset("/gloss", "/", "Gloss", bm));

                    int width = bm.Width;
                    int height = bm.Height;

                    Bitmap red = new Bitmap(bm);
                    Bitmap green = new Bitmap(bm);
                    Bitmap blue = new Bitmap(bm);
                    Bitmap alpha = new Bitmap(bm);

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            System.Drawing.Color p = bm.GetPixel(x, y);
                            int a = p.A;
                            int r = p.R;
                            int g = p.G;
                            int b = p.B;

                            alpha.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, 0, 0));
                            red.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, 0, 0));
                            green.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, g, 0));
                            blue.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, 0, b));
                        }
                    }

                    matDict.Add("/gloss/alpha", new NodeAsset("/gloss/alpha", "/gloss", "Gloss - Alpha", alpha));
                    matDict.Add("/gloss/red", new NodeAsset("/gloss/red", "/gloss", "Gloss - Red", red));
                    matDict.Add("/gloss/green", new NodeAsset("/gloss/green", "/gloss", "Gloss - Green", green));
                    matDict.Add("/gloss/blue", new NodeAsset("/gloss/blue", "/gloss", "Gloss - Blue", blue));
                }
            }

            if (this.material.paletteDDS != null)
            {
                var paletteMaskFile = currentAssets.FindFile(this.material.paletteDDS);
                if (paletteMaskFile != null)
                {
                    var memStream = new MemoryStream();
                    var paletteMaskStream = paletteMaskFile.OpenCopyInMemory();
                    dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, paletteMaskStream);
                    exp.SaveImageToStream(dds, ImageType.Png, memStream);
                    Bitmap bm = new Bitmap(memStream);
                    matDict.Add("/palettemask", new NodeAsset("/palettemask", "/", "Palette Mask", bm));

                    int width = bm.Width;
                    int height = bm.Height;

                    Bitmap red = new Bitmap(bm);
                    Bitmap green = new Bitmap(bm);
                    Bitmap blue = new Bitmap(bm);
                    Bitmap alpha = new Bitmap(bm);

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            System.Drawing.Color p = bm.GetPixel(x, y);
                            int a = p.A;
                            int r = p.R;
                            int g = p.G;
                            int b = p.B;

                            alpha.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, 0, 0));
                            red.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, 0, 0));
                            green.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, g, 0));
                            blue.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, 0, b));
                        }
                    }

                    matDict.Add("/palettemask/alpha", new NodeAsset("/palettemask/alpha", "/palettemask", "Palette Mask - Alpha", alpha));
                    matDict.Add("/palettemask/red", new NodeAsset("/palettemask/red", "/palettemask", "Palette Mask - Red", red));
                    matDict.Add("/palettemask/green", new NodeAsset("/palettemask/green", "/palettemask", "Palette Mask - Green", green));
                    matDict.Add("/palettemask/blue", new NodeAsset("/palettemask/blue", "/palettemask", "Palette Mask - Blue", blue));
                }
            }

            if (this.material.paletteMaskDDS != null)
            {
                var paletteMaskMapFile = currentAssets.FindFile(this.material.paletteMaskDDS);
                if (paletteMaskMapFile != null)
                {
                    var memStream = new MemoryStream();
                    var paletteMaskMapStream = paletteMaskMapFile.OpenCopyInMemory();
                    dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, paletteMaskMapStream);
                    exp.SaveImageToStream(dds, ImageType.Png, memStream);
                    Bitmap bm = new Bitmap(memStream);
                    matDict.Add("/palettemaskmap", new NodeAsset("/palettemaskmap", "/", "Palette Mask Map", bm));

                    int width = bm.Width;
                    int height = bm.Height;

                    Bitmap red = new Bitmap(bm);
                    Bitmap green = new Bitmap(bm);
                    Bitmap blue = new Bitmap(bm);
                    Bitmap alpha = new Bitmap(bm);

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            System.Drawing.Color p = bm.GetPixel(x, y);
                            int a = p.A;
                            int r = p.R;
                            int g = p.G;
                            int b = p.B;

                            alpha.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, 0, 0));
                            red.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, 0, 0));
                            green.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, g, 0));
                            blue.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, 0, b));
                        }
                    }

                    matDict.Add("/palettemaskmap/alpha", new NodeAsset("/palettemaskmap/alpha", "/palettemaskmap", "Palette Mask Map - Alpha", alpha));
                    matDict.Add("/palettemaskmap/red", new NodeAsset("/palettemaskmap/red", "/palettemaskmap", "Palette Mask Map - Red", red));
                    matDict.Add("/palettemaskmap/green", new NodeAsset("/palettemaskmap/green", "/palettemaskmap", "Palette Mask Map - Green", green));
                    matDict.Add("/palettemaskmap/blue", new NodeAsset("/palettemaskmap/blue", "/palettemaskmap", "Palette Mask Map - Blue", blue));
                }
            }

            if (this.material.complexionDDS != null)
            {

                var complexionFile = currentAssets.FindFile(this.material.complexionDDS);
                if (complexionFile != null)
                {
                    var memStream = new MemoryStream();
                    var complexionStream = complexionFile.OpenCopyInMemory();
                    dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, complexionStream);
                    exp.SaveImageToStream(dds, ImageType.Png, memStream);
                    Bitmap bm = new Bitmap(memStream);
                    matDict.Add("/complexion", new NodeAsset("/complexion", "/", "Complexion Map", bm));

                    int width = bm.Width;
                    int height = bm.Height;

                    Bitmap red = new Bitmap(bm);
                    Bitmap green = new Bitmap(bm);
                    Bitmap blue = new Bitmap(bm);
                    Bitmap alpha = new Bitmap(bm);

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            System.Drawing.Color p = bm.GetPixel(x, y);
                            int a = p.A;
                            int r = p.R;
                            int g = p.G;
                            int b = p.B;

                            alpha.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, 0, 0));
                            red.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, 0, 0));
                            green.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, g, 0));
                            blue.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, 0, b));
                        }
                    }

                    matDict.Add("/complexion/alpha", new NodeAsset("/complexion/alpha", "/complexion", "Complexion Map - Alpha", alpha));
                    matDict.Add("/complexion/red", new NodeAsset("/complexion/red", "/complexion", "Complexion Map - Red", red));
                    matDict.Add("/complexion/green", new NodeAsset("/complexion/green", "/complexion", "Complexion Map - Green", green));
                    matDict.Add("/complexion/blue", new NodeAsset("/complexion/blue", "/complexion", "Complexion Map - Blue", blue));
                }
            }

            if (this.material.facepaintDDS != null)
            {

                var facepaintFile = currentAssets.FindFile(this.material.facepaintDDS);
                if (facepaintFile != null)
                {
                    var memStream = new MemoryStream();
                    var facepaintStream = facepaintFile.OpenCopyInMemory();
                    dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, facepaintStream);
                    exp.SaveImageToStream(dds, ImageType.Png, memStream);
                    Bitmap bm = new Bitmap(memStream);
                    matDict.Add("/facepaint", new NodeAsset("/facepaint", "/", "Facepaint Map", bm));

                    int width = bm.Width;
                    int height = bm.Height;

                    Bitmap red = new Bitmap(bm);
                    Bitmap green = new Bitmap(bm);
                    Bitmap blue = new Bitmap(bm);
                    Bitmap alpha = new Bitmap(bm);

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            System.Drawing.Color p = bm.GetPixel(x, y);
                            int a = p.A;
                            int r = p.R;
                            int g = p.G;
                            int b = p.B;

                            alpha.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, 0, 0));
                            red.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, 0, 0));
                            green.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, g, 0));
                            blue.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, 0, b));
                        }
                    }

                    matDict.Add("/facepaint/alpha", new NodeAsset("/facepaint/alpha", "/facepaint", "Facepaint Map - Alpha", alpha));
                    matDict.Add("/facepaint/red", new NodeAsset("/facepaint/red", "/facepaint", "Facepaint Map - Red", red));
                    matDict.Add("/facepaint/green", new NodeAsset("/facepaint/green", "/facepaint", "Facepaint Map - Green", green));
                    matDict.Add("/facepaint/blue", new NodeAsset("/facepaint/blue", "/facepaint", "Facepaint Map - Blue", blue));
                }
            }

            if (this.material.ageDDS != null)
            {

                var ageFile = currentAssets.FindFile(this.material.ageDDS);
                if (ageFile != null)
                {
                    var memStream = new MemoryStream();
                    var ageStream = ageFile.OpenCopyInMemory();
                    dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, ageStream);
                    exp.SaveImageToStream(dds, ImageType.Png, memStream);
                    Bitmap bm = new Bitmap(memStream);
                    matDict.Add("/age", new NodeAsset("/age", "/", "Age Map", bm));

                    int width = bm.Width;
                    int height = bm.Height;

                    Bitmap red = new Bitmap(bm);
                    Bitmap green = new Bitmap(bm);
                    Bitmap blue = new Bitmap(bm);
                    Bitmap alpha = new Bitmap(bm);

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            System.Drawing.Color p = bm.GetPixel(x, y);
                            int a = p.A;
                            int r = p.R;
                            int g = p.G;
                            int b = p.B;

                            alpha.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, 0, 0));
                            red.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, 0, 0));
                            green.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, g, 0));
                            blue.SetPixel(x, y, System.Drawing.Color.FromArgb(a, 0, 0, b));
                        }
                    }

                    matDict.Add("/age/alpha", new NodeAsset("/age/alpha", "/age", "Age Map - Alpha", alpha));
                    matDict.Add("/age/red", new NodeAsset("/age/red", "/age", "Age Map - Red", red));
                    matDict.Add("/age/green", new NodeAsset("/age/green", "/age", "Age Map - Green", green));
                    matDict.Add("/age/blue", new NodeAsset("/age/blue", "/age", "Age Map - Blue", blue));
                }
            }
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            webBrowser1.DocumentText = new CodeColorizer().Colorize(AssetBrowser.Beautify(this.xmlDoc), Languages.Xml);
            webBrowser1.Visible = true;
            Func<NodeAsset, string> getId = (x => x.Id);
            Func<NodeAsset, string> getParentId = (x => x.parentId);
            Func<NodeAsset, string> getDisplayName = (x => x.displayName);
            treeViewFast1.BeginUpdate();
            treeViewFast1.LoadItems<NodeAsset>(matDict, getId, getParentId, getDisplayName);
            treeViewFast1.EndUpdate();
            treeViewFast1.Nodes[0].Expand();
            toolStripProgressBar1.Visible = false;
            toolStripStatusLabel1.Text = "";
            pictureBox2.Visible = false;
        }

        private void TreeViewFast1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = treeViewFast1.SelectedNode;
            NodeAsset asset = (NodeAsset)node.Tag;
            if (asset.dynObject != null)
            {
                if (asset.dynObject is Bitmap)
                {
                    webBrowser1.Visible = false;
                    pictureBox1.Image = (System.Drawing.Bitmap)asset.dynObject;
                    pictureBox1.Visible = true;
                }
            }
            else if (asset.Id == "/xml")
            {
                pictureBox1.Visible = false;
                webBrowser1.Visible = true;
            }

        }

    }
}
