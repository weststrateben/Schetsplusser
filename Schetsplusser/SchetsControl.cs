using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class SchetsControl : UserControl
{
    private Schets schets;
    private Elementen tekeningen;
    private Color penkleur;
    private int diktevanlijn;

    public Color PenKleur
    {
        get { return penkleur; }
    }

    public int DiktevLijn
    {
        get { return diktevanlijn; }
    }

    public Schets Schets
    {
        get { return schets; }
    }
    public Elementen Elementen
    {
        get { return tekeningen; }
        set { tekeningen = value; }
    }
    public SchetsControl()
    {
        this.BorderStyle = BorderStyle.Fixed3D;
        this.schets = new Schets();
        this.Paint += this.teken;
        this.Resize += this.veranderAfmeting;
        this.veranderAfmeting(null, null);
    }
    protected override void OnPaintBackground(PaintEventArgs e)
    {
    }
    private void teken(object o, PaintEventArgs pea)
    {
        schets.Teken(pea.Graphics);
        foreach (Elementen.Tekening tekening in tekeningen.elementen )
        {
            
        }
    }
    private void veranderAfmeting(object o, EventArgs ea)
    {
        schets.VeranderAfmeting(this.ClientSize);
        this.Invalidate();
    }
    public Graphics MaakBitmapGraphics()
    {
        Graphics g = schets.BitmapGraphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        return g;
    }
    public void Schoon(object o, EventArgs ea)
    {
        schets.Schoon();
        this.Invalidate();
    }

    public void Roteer(object o, EventArgs ea)
    {
        schets.VeranderAfmeting(new Size(this.ClientSize.Height, this.ClientSize.Width));
        schets.Roteer();
        this.Invalidate();
    }
    public void VeranderKleur(object obj, EventArgs ea)
    {
        string kleurNaam = ((ComboBox)obj).Text;
        penkleur = Color.FromName(kleurNaam);
    }

    public void VeranderKleurViaMenu(object obj, EventArgs ea)
    {
        string kleurNaam = ((ToolStripMenuItem)obj).Text;
        penkleur = Color.FromName(kleurNaam);
    }

    public void VeranderLijnDikte(object obj, EventArgs ea)
    {
        string dikte = ((ComboBox)obj).Text;
        diktevanlijn = int.Parse(dikte);
    }

    public void VeranderLijnDikteViaMenu(object obj, EventArgs ea)
    {
        string dikte = ((ToolStripMenuItem)obj).Text;
        diktevanlijn = int.Parse(dikte);
    }
}