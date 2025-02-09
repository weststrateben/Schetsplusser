﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

public class SchetsControl : UserControl
{
    private Schets schets;
    public Color penkleur;
    private Elementen tekeningen;
    private int diktevanlijn;
 
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
        schets.Schoon();
        foreach (Elementen.Tekening tekening in tekeningen.elementen )
        {
            switch (tekening.Tool)
            {
                case "kader":
                    schets.BitmapGraphics.DrawRectangle(tekening.pen,
                        TweepuntTool.Punten2Rechthoek(tekening.start_punt, tekening.eind_punt));
                    break;

                case "vlak":
                    schets.BitmapGraphics.FillRectangle(tekening.pen.Brush,
                        TweepuntTool.Punten2Rechthoek(tekening.start_punt, tekening.eind_punt));
                    break;

                case "cirkel":
                    schets.BitmapGraphics.DrawEllipse(tekening.pen,
                        TweepuntTool.Punten2Rechthoek(tekening.start_punt, tekening.eind_punt));
                    break;

                case "bol":
                    schets.BitmapGraphics.FillEllipse(tekening.pen.Brush,
                        TweepuntTool.Punten2Rechthoek(tekening.start_punt, tekening.eind_punt));
                    break;

                case "lijn":
                    schets.BitmapGraphics.DrawLine(tekening.pen, tekening.start_punt, tekening.eind_punt);
                    break;

                case "pen":
                    schets.BitmapGraphics.DrawLine(tekening.pen, tekening.start_punt, tekening.eind_punt);
                    break;

                case "tekst":
                    schets.BitmapGraphics.DrawString(tekening.Text, tekening.TextFont, tekening.pen.Brush,
                    tekening.start_punt, StringFormat.GenericTypographic);
                    break;

            }  
        }
        schets.Teken(pea.Graphics);
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
        foreach (var tekening in tekeningen.elementen.ToList())
        {
            tekeningen.Verwijder_Tekening(tekening);
        }
        schets.Schoon();
        this.Invalidate();
    }

    public void Roteer(object o, EventArgs ea)
    {
        int centerX = this.ClientSize.Width / 2;
        int centerY = this.ClientSize.Height / 2;

        foreach (Elementen.Tekening tekening in tekeningen.elementen.ToList())
        {
            int dx = tekening.start_punt.X - centerX;
            int dy = tekening.start_punt.Y - centerY;
            tekening.start_punt = new Point(centerX - dy, centerY + dx);

            dx = tekening.eind_punt.X - centerX;
            dy = tekening.eind_punt.Y - centerY;
            tekening.eind_punt = new Point(centerX - dy, centerY + dx);
        }
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