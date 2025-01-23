using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

public class SchetsEditor : Form
{
    private MenuStrip menuStrip;

    public SchetsEditor()
    {
        this.ClientSize = new Size(1000, 800);  // het grijze achtergrond scherm
        menuStrip = new MenuStrip();    
        this.Controls.Add(menuStrip);
        this.maakFileMenu();
        this.maakHelpMenu();
        this.Text = "Schets editor";
        this.IsMdiContainer = true;
        this.MainMenuStrip = menuStrip;
    }
    private void maakFileMenu()
    {
        ToolStripDropDownItem menu = new ToolStripMenuItem("File");   // je maakt de File knop
        menu.DropDownItems.Add("Nieuw", null, this.nieuw);  // roept functie nieuw aan
        //menu.DropDownItems.Add("Open", null, this.open);
        menu.DropDownItems.Add("Exit", null, this.afsluiten);   // roept functie afsluiten aan
        menuStrip.Items.Add(menu);
    }
    private void maakHelpMenu()
    {
        ToolStripDropDownItem menu = new ToolStripMenuItem("Help");   // je maakt de Help knop
        menu.DropDownItems.Add("Over \"Schets\"", null, this.about);   // roept functie about aan
        menuStrip.Items.Add(menu);
    }
    private void about(object o, EventArgs ea)    // de tabs onder de Help knop
    {
        MessageBox.Show("Schets versie 2.0\n(c) UU Informatica 2022"
                        , "Over \"Schets\""
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Information
                        );
    }

    private void nieuw(object sender, EventArgs e)  // de Nieuw knop onder File
    {
        SchetsWin s = new SchetsWin();
        s.MdiParent = this;
        s.Show();
    }


    private void afsluiten(object sender, EventArgs e)  // de Exit knop onder File
    {
        this.Close();
    }


}