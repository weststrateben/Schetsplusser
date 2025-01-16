using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class SchetsControl : UserControl
{
    private Schets schets;
    private Color penkleur;
    private Elementen tekeningen;
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
    public SchetsControl(SchetsWin parent)
    {
        this.BorderStyle = BorderStyle.Fixed3D;
        this.schets = new Schets(parent);
        this.Paint += this.teken;
        this.Resize += this.veranderAfmeting;
        this.veranderAfmeting(null, null);
        this.DoubleBuffered = true;  // heb dit van chatgpt was de enige manier om de flikker makkelijk te fixen
    }
    public void Elementen(Elementen tekeningen)
    {
        this.tekeningen = tekeningen;
    }
    protected override void OnPaintBackground(PaintEventArgs e)
    {
    }
    private void teken(object o, PaintEventArgs pea)
    {
        schets.Teken(pea.Graphics);
        foreach (Elementen.Tekening tekening in tekeningen.elementen )
        {
            switch (tekening.Tool)
            {
                case "kader":
                    pea.Graphics.DrawRectangle(tekening.pen,
                        TweepuntTool.Punten2Rechthoek(tekening.start_punt, tekening.eind_punt));
                    break;

                case "vlak":
                    pea.Graphics.FillRectangle(tekening.pen.Brush,
                        TweepuntTool.Punten2Rechthoek(tekening.start_punt, tekening.eind_punt));
                    break;

                case "cirkel":
                    pea.Graphics.DrawEllipse(tekening.pen,
                        TweepuntTool.Punten2Rechthoek(tekening.start_punt, tekening.eind_punt));
                    break;

                case "bol":
                    pea.Graphics.FillEllipse(tekening.pen.Brush,
                        TweepuntTool.Punten2Rechthoek(tekening.start_punt, tekening.eind_punt));
                    break;

            }    
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
        if (kleurNaam == "Ander...")
        {
            Kleur kleur = new Kleur(this);
            kleur.ShowDialog();
        }
        else
        {
            penkleur = Color.FromName(kleurNaam);
        }
    }

    public void VeranderKleurViaMenu(object obj, EventArgs ea)
    {
        string kleurNaam = ((ToolStripMenuItem)obj).Text;
        if (kleurNaam == "Ander...")
        {
            Kleur kleur = new Kleur(this);
            kleur.ShowDialog();
        }
        else
        {
            penkleur = Color.FromName(kleurNaam);
        }
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