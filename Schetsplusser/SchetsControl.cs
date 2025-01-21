using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class SchetsControl : UserControl
{
    private Schets schets;
    public Color penkleur;
    private Elementen tekeningen;
    private int diktevanlijn;
    private Size originele_vorm;

    public Color PenKleur
    {
        get { return penkleur; }
        set { penkleur = value; }
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
        this.originele_vorm = this.Size;  
        this.veranderAfmeting(null, null); 
        this.DoubleBuffered = true;
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

                case "lijn":
                    pea.Graphics.DrawLine(tekening.pen, tekening.start_punt, tekening.eind_punt);
                    break;

                case "pen":
                    pea.Graphics.DrawLine(tekening.pen, tekening.start_punt, tekening.eind_punt);
                    break;

                case "tekst":
                    pea.Graphics.DrawString(tekening.Text, tekening.TextFont, tekening.pen.Brush,
                    tekening.start_punt, StringFormat.GenericTypographic);
                    break;

            }    
        }
    }
    private void veranderAfmeting(object o, EventArgs ea)
    {
        schets.VeranderAfmeting(this.ClientSize);
        if (tekeningen != null)
        {
            Size oud = new Size(originele_vorm.Width, originele_vorm.Height);
            Size nieuw = new Size(this.ClientSize.Width, this.ClientSize.Height);
            tekeningen.Verander_afmetingen(oud, nieuw);
            originele_vorm = this.ClientSize;
        }
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
        this.Schoon(null, null); // Verwijdert de bug met de zwarte vlakken bij het roteren
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