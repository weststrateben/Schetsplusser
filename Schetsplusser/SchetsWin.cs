using System;
using System.Collections.Generic;
using System.Data.Common;
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
    bool vast;
    public bool unsavedChanges = false;

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
        dialoog.Filter = "Bitmap|*.bmp|JPEG|*.jpg|PNG|*.png|All Files|*.*";
        dialoog.Title = "Tekst opslaan als...";
        if (dialoog.ShowDialog() == DialogResult.OK)
        {
            Text = dialoog.FileName;
            schrijfNaarFile(dialoog.FileName);
            unsavedChanges = false;
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
        unsavedChanges = false;
    }
    private void afsluiten(object obj, EventArgs ea) // voor de knop sluiten in menu
    {
            this.Close();  //ik heb deze geupdate, nu krijgen we geen dubbel vraag
    }
    public SchetsWin()
    {
        ISchetsTool[] deTools = { new PenTool()   // hier maken we echt de tools aan
                                , new LijnTool()
                                , new RechthoekTool()
                                , new VolRechthoekTool()
                                , new CirkelTool()
                                , new VolCirkelTool()
                                , new TekstTool()
                                , new GumTool()
                                };
        String[] deKleuren = { "Black", "Red", "Green", "Blue", "Yellow", "Magenta", "Cyan" };  // de mogelijke standaart kleuren
        int[] deDiktes = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };   // de standaart diktes

        this.ClientSize = new Size(700, 500);
        huidigeTool = deTools[0];  // tool in gebruik

        schetscontrol = new SchetsControl();  // we maken een schetscontrol aan
        schetscontrol.Location = new Point(64, 10);
        schetscontrol.MouseDown += (object o, MouseEventArgs mea) =>  // er wordt geklikt
        {
            vast = true;
            huidigeTool.MuisVast(schetscontrol, mea.Location);
            unsavedChanges = true;
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
            unsavedChanges = true;
        };
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
        if (unsavedChanges)
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
        paneel.Size = new Size(600, 24);

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
    }
}