using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
public class SchetsWin : Form
{
    MenuStrip menuStrip;
    SchetsControl schetscontrol;
    ISchetsTool huidigeTool;
    Panel paneel;
    public Elementen tekeningen;
    bool vast;
    public bool niet_opgeslagen = false;

    private void veranderAfmeting(object o, EventArgs ea)
    {
        schetscontrol.Size = new Size(this.ClientSize.Width - 70
                                      , this.ClientSize.Height - 50);
        paneel.Location = new Point(64, this.ClientSize.Height - 30);
    }

    private void klikToolMenu(object obj, EventArgs ea)
    {
        this.huidigeTool = (ISchetsTool)((ToolStripMenuItem)obj).Tag;
    }

    private void klikToolButton(object obj, EventArgs ea)
    {
        this.huidigeTool = (ISchetsTool)((RadioButton)obj).Tag;
    }

    private void opslaanAls(object o, EventArgs ea)
    {
        SaveFileDialog dialoog = new SaveFileDialog();
        dialoog.Filter = "Bitmap|*.bmp|JPEG|*.jpg|PNG|*.png|Text files|*.txt|All Files|*.*";
        dialoog.Title = "Tekst opslaan als...";
        if (dialoog.ShowDialog() == DialogResult.OK)
        {
            Text = dialoog.FileName;
            if (dialoog.FileName.EndsWith(".txt"))
            {
                using (StreamWriter writer = new StreamWriter(dialoog.FileName))
                {
                    foreach (Elementen.Tekening tekening in tekeningen.elementen)
                    {
                        writer.Write($"{tekening.Tool}|");
                        writer.Write($"{tekening.start_punt.X},{tekening.start_punt.Y}|");
                        writer.Write($"{tekening.eind_punt.X},{tekening.eind_punt.Y}|");
                        writer.Write($"{tekening.pen.Color.ToArgb()}|");
                        writer.Write($"{tekening.pen.Width}");

                        if (tekening.Tool == "tekst")
                        {
                            writer.Write($"|{tekening.Text}|");
                            writer.Write($"{tekening.TextFont.Name},{tekening.TextFont.Size},{(int)tekening.TextFont.Style}");
                        }
                        writer.WriteLine();
                    }
                }
            }
            else
            {
                schrijfNaarFile(dialoog.FileName);
            }
        }
    }

    public void openVanTxt(object o, EventArgs ea)
    {
        OpenFileDialog dialoog = new OpenFileDialog();
        dialoog.Filter = "Text files|*.txt";
        dialoog.Title = "Tekst openen...";

        if (dialoog.ShowDialog() == DialogResult.OK)
        {
            StreamReader reader = new StreamReader(dialoog.FileName);
            string t = reader.ReadToEnd();
            reader.Close();

            string[] regels = t.Split('\n');
            foreach (string regel in regels)
            {
                if (string.IsNullOrEmpty(regel)) continue;

                string[] onderdelen = regel.Split('|');
                string[] startXY = onderdelen[1].Split(',');
                string[] eindXY = onderdelen[2].Split(',');

                Point startPunt = new Point(int.Parse(startXY[0]), int.Parse(startXY[1]));
                Point eindPunt = new Point(int.Parse(eindXY[0]), int.Parse(eindXY[1]));
                Pen pen = new Pen(Color.FromArgb(int.Parse(onderdelen[3])), float.Parse(onderdelen[4]));

                if (onderdelen[0] == "tekst" && onderdelen.Length > 6)
                {
                    string[] fontInfo = onderdelen[6].Split(',');
                    Font font = new Font(fontInfo[0], float.Parse(fontInfo[1]), (FontStyle)int.Parse(fontInfo[2]));
                    new Elementen.Tekening(startPunt, eindPunt, pen, onderdelen[0], tekeningen, onderdelen[5], font);
                }
                else
                    new Elementen.Tekening(startPunt, eindPunt, pen, onderdelen[0], tekeningen);
            }

            schetscontrol.Invalidate();
            niet_opgeslagen = false;
        }
    }

    private void schrijfNaarFile(string filePath)
    {
        FileStream fs = new FileStream(filePath, FileMode.Create);
        Bitmap bitmap = schetscontrol.Schets.Bitmap;
        ImageFormat imageFormat = ImageFormat.Bmp;
        string fileExtension = Path.GetExtension(filePath).ToLower();
        if (fileExtension == ".jpg")
            imageFormat = ImageFormat.Jpeg;
        else if (fileExtension == ".png")
            imageFormat = ImageFormat.Png;
        bitmap.Save(fs, imageFormat);
        niet_opgeslagen = false;
    }
    private void afsluiten(object obj, EventArgs ea) // voor de knop sluiten in menu
    {
            this.Close();  //ik heb deze geupdate, nu krijgen we geen dubbel vraag
    }
    public SchetsWin()
    {
        schetscontrol = new SchetsControl(this);  // Create SchetsControl first
        tekeningen = new Elementen(schetscontrol); // Then create Elementen with SchetsControl
        schetscontrol.Elementen(tekeningen);
        schetscontrol.Location = new Point(64, 10);
        schetscontrol.MouseDown += (object o, MouseEventArgs mea) =>  // er wordt geklikt
        {
            vast = true;
            huidigeTool.MuisVast(schetscontrol, mea.Location);
            niet_opgeslagen = true;
        };
        schetscontrol.MouseMove += (object o, MouseEventArgs mea) =>    // muis vast en beweegt
        {
            if (vast)
                huidigeTool.MuisDrag(schetscontrol, mea.Location);
        };
        schetscontrol.MouseUp += (object o, MouseEventArgs mea) =>  // laat de muis los
        {
            if (vast)
                huidigeTool.MuisLos(schetscontrol, mea.Location);
            vast = false;
        };
        schetscontrol.KeyPress += (object o, KeyPressEventArgs kpea) =>  // je drukt op een toets
        {
            huidigeTool.Letter(schetscontrol, kpea.KeyChar);
            niet_opgeslagen = true;
        };
        ISchetsTool[] deTools = { new PenTool()   // hier maken we echt de tools aan
                                , new LijnTool()
                                , new RechthoekTool()
                                , new VolRechthoekTool()
                                , new CirkelTool()
                                , new VolCirkelTool()
                                , new TekstTool()
                                , new GumTool()
                                , new MoveTool()
                                , new UpTool()
                                , new DownTool()
                                };
        foreach (ISchetsTool tool in deTools)
        {
            tool.Elementen(tekeningen);
        }
        String[] deKleuren = { "Black", "White", "Red", "Green", "Blue", "Yellow", "Magenta", "Cyan", "Ander..."};  // de mogelijke standaart kleuren
        int[] deDiktes = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };   // de standaart diktes
        this.ClientSize = new Size(800,700);
        huidigeTool = deTools[0];  // tool in gebruik

        this.FormClosing += SchetsWin_FormClosing;
        this.Controls.Add(schetscontrol);

        menuStrip = new MenuStrip();
        menuStrip.Visible = false;

        this.Controls.Add(menuStrip);
        this.maakFileMenu();
        this.maakToolMenu(deTools);
        this.maakActieMenu(deKleuren, deDiktes);
        this.maakToolButtons(deTools);
        this.maakActieButtons(deKleuren, deDiktes);
        this.Resize += this.veranderAfmeting;
        this.veranderAfmeting(null, null);
    }
    private void SchetsWin_FormClosing(object sender, FormClosingEventArgs e)  // de functie die aangeroepen word als er iets gesloten wordt
    {
        if (niet_opgeslagen)
        {
            DialogResult result = MessageBox.Show("Er zijn wijzigingen die nog niet zijn opgeslagen. Wil je doorgaan zonder op te slaan?", "Waarschuwing", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
    }

    private void maakFileMenu()  //de file knop toevoegingen
    {
        ToolStripMenuItem menu = new ToolStripMenuItem("File");
        menu.MergeAction = MergeAction.MatchOnly;           //er bestaat al zon toolstrip dus we mergen ze
        menu.DropDownItems.Add("Opslaan als..", null, this.opslaanAls);
        menu.DropDownItems.Add("Sluiten", null, this.afsluiten);
        menuStrip.Items.Add(menu);
    }

    private void maakToolMenu(ICollection<ISchetsTool> tools) // het tools menu in de grijze achtergrond
    {
        ToolStripMenuItem menu = new ToolStripMenuItem("Tool");
        foreach (ISchetsTool tool in tools)
        {
            ToolStripItem item = new ToolStripMenuItem();
            item.Tag = tool;
            item.Text = tool.ToString();
            item.Image = new Bitmap($"../../../Icons/{tool.ToString()}.png");
            item.Click += this.klikToolMenu;
            menu.DropDownItems.Add(item);
        }
        menuStrip.Items.Add(menu);
    }

    private void maakActieMenu(String[] kleuren, int[] diktes)  // het actie menu in de grijze achtergrond
    {
        ToolStripMenuItem menu = new ToolStripMenuItem("Actie");
        menu.DropDownItems.Add("Clear", null, schetscontrol.Schoon);
        menu.DropDownItems.Add("Roteer", null, schetscontrol.Roteer);
        ToolStripMenuItem kleurkeuze = new ToolStripMenuItem("Kies kleur");
        foreach (string k in kleuren)
            kleurkeuze.DropDownItems.Add(k, null, schetscontrol.VeranderKleurViaMenu);
        ToolStripMenuItem diktekeuze = new ToolStripMenuItem("Kies dikte");
        foreach (int d in diktes)
        {
            string dValue = d.ToString();
            diktekeuze.DropDownItems.Add(dValue, null, schetscontrol.VeranderLijnDikteViaMenu);
        }
        menu.DropDownItems.Add(kleurkeuze);
        menu.DropDownItems.Add(diktekeuze);
        ToolStripMenuItem undo = new ToolStripMenuItem("Undo");
        ToolStripMenuItem redo = new ToolStripMenuItem("Redo");
        menu.DropDownItems.Add("Undo", null, tekeningen.Undo);
        menu.DropDownItems.Add("Redo", null, tekeningen.Redo);
        menuStrip.Items.Add(menu);
    }

    private void maakToolButtons(ICollection<ISchetsTool> tools)
    {
        int t = 0;
        foreach (ISchetsTool tool in tools)  //alle tools staan in Tools.cs
        {
            RadioButton b = new RadioButton();
            b.Appearance = Appearance.Button;
            b.Size = new Size(45, 62);
            b.Location = new Point(10, 10 + t * 62);
            b.Tag = tool;
            b.Text = tool.ToString();
            b.Image = new Bitmap($"../../../Icons/{tool.ToString()}.png"); //haal de image uit de map
            b.TextAlign = ContentAlignment.TopCenter;
            b.ImageAlign = ContentAlignment.BottomCenter;
            b.Click += this.klikToolButton;   
            this.Controls.Add(b);
            if (t == 0) b.Select();
            t++;
        }
    }

    private void maakActieButtons(String[] kleuren, int[] diktes)
    {
        paneel = new Panel(); this.Controls.Add(paneel);
        paneel.Size = new Size(900, 24);

        Button clear = new Button(); paneel.Controls.Add(clear);  // maakt een lege control pannel
        clear.Text = "Clear";
        clear.Location = new Point(0, 0);
        clear.Click += schetscontrol.Schoon;

        Button rotate = new Button(); paneel.Controls.Add(rotate);  //flipt de bitmap 90 graden
        rotate.Text = "Rotate";
        rotate.Location = new Point(80, 0);
        rotate.Click += schetscontrol.Roteer;

        Label penkleur = new Label(); paneel.Controls.Add(penkleur); //kleur optie
        penkleur.Text = "Penkleur:";
        penkleur.Location = new Point(180, 3);
        penkleur.AutoSize = true;

        Label pendikte = new Label(); paneel.Controls.Add(pendikte);  //dikte optie
        pendikte.Text = "Pendikte:";
        pendikte.Location = new Point(380, 3);
        pendikte.AutoSize = true;

        ComboBox cbb = new ComboBox(); paneel.Controls.Add(cbb);  //voeg alle kleuren toe onder kleuren
        cbb.Location = new Point(240, 0);
        cbb.DropDownStyle = ComboBoxStyle.DropDownList;
        cbb.SelectedValueChanged += schetscontrol.VeranderKleur;
        foreach (string k in kleuren)
            cbb.Items.Add(k);
        cbb.SelectedIndex = 0;

        ComboBox dik = new ComboBox(); paneel.Controls.Add(dik);  //voeg alle diktes toe onder de dikte opties
        dik.Location = new Point(440, 0);
        dik.DropDownStyle = ComboBoxStyle.DropDownList;
        dik.SelectedValueChanged += schetscontrol.VeranderLijnDikte;
        foreach (int d in diktes)
        {
            object dValue = d;
            dik.Items.Add(dValue.ToString());
        }
        dik.SelectedIndex = 2;

        Button undo = new Button(); paneel.Controls.Add(undo);  //roept undo aan in elementen
        undo.Text = "Undo";
        undo.Location = new Point(570, 0);
        undo.Click += tekeningen.Undo;

        Button redo = new Button(); paneel.Controls.Add(redo);  //roept redo aan in elementen
        redo.Text = "Redo";
        redo.Location = new Point(650, 0);
        redo.Click += tekeningen.Redo;
    }
}