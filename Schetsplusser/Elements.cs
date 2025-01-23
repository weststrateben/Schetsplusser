using System;
using System.Collections.Generic;
using System.Drawing;


public class Elementen
{
    internal enum Veranderingen { nieuw, oud, omhoog, omlaag }

    public List<Tekening> elementen;
    private List<(Veranderingen, Tekening)> undo;
    private List<(Veranderingen, Tekening)> redo;
    private SchetsControl schetscontrol;
    public Elementen(SchetsControl control)
    {
        elementen = new List<Tekening>();
        undo = new List<(Veranderingen, Tekening)>();
        redo = new List<(Veranderingen, Tekening)>();
        schetscontrol = control;
    }
    public void Omhoog_Tekening(Tekening tekening)
    {
        if (tekening != null)
        {
            int index = elementen.IndexOf(tekening);
            if (index < elementen.Count - 1)  
            {
                tekening.lijst_positie = index;  
                elementen.RemoveAt(index);
                elementen.Insert(elementen.Count, tekening);
                undo.Add((Veranderingen.omhoog, tekening));
                schetscontrol.Invalidate();
            }
        }
    }

    public void Omlaag_Tekening(Tekening tekening)
    {
        if (tekening != null)
        {
            int index = elementen.IndexOf(tekening);
            if (index > 0) 
            {
                tekening.lijst_positie = index; 
                elementen.RemoveAt(index);
                elementen.Insert(0, tekening);
                undo.Add((Veranderingen.omlaag, tekening));
                schetscontrol.Invalidate();
            }
        }
    }
    public void Verwijder_Tekening(Tekening tekening)
    {
        if (tekening != null)
        {
            tekening.lijst_positie = elementen.IndexOf(tekening);
            undo.Add((Veranderingen.oud, tekening));
            elementen.Remove(tekening);
            schetscontrol.Invalidate(); 
        }
    }
    public void Beweeg_Tekening(Tekening tekening, Point klik, Point beweeg)
    {
        int dx = beweeg.X - klik.X;
        int dy = beweeg.Y - klik.Y;

        tekening.start_punt = new Point(tekening.start_punt.X + dx, tekening.start_punt.Y + dy);
        tekening.eind_punt = new Point(tekening.eind_punt.X + dx, tekening.eind_punt.Y + dy);
    }
    public void Undo(object o, EventArgs ea)
    {
        if (undo.Count != 0)
        {
            Tekening tekening = undo[undo.Count - 1].Item2;
            switch (undo[undo.Count - 1].Item1)
            {
                case Veranderingen.nieuw:
                    elementen.Remove(tekening);
                    break;
                case Veranderingen.oud:
                    elementen.Insert(tekening.lijst_positie, tekening);
                    break;
                case Veranderingen.omhoog:
                    elementen.Remove(tekening);
                    elementen.Insert(tekening.lijst_positie, tekening);
                    break;
                case Veranderingen.omlaag:
                    elementen.Remove(tekening);
                    elementen.Insert(tekening.lijst_positie, tekening);
                    break;
            }
            redo.Add(undo[undo.Count - 1]);
            undo.RemoveAt(undo.Count - 1);
            schetscontrol.Invalidate();
        }
    }
    public void Redo(object o, EventArgs ea)
    {
        if (redo.Count > 0)
        {
            Tekening tekening = redo[redo.Count - 1].Item2;
            switch (redo[redo.Count - 1].Item1)
            {
                case Veranderingen.nieuw:
                    elementen.Remove(tekening);
                    elementen.Insert(tekening.lijst_positie, tekening);
                    break;
                case Veranderingen.oud:
                    elementen.Remove(tekening);
                    break;
                case Veranderingen.omhoog:
                    elementen.Remove(tekening);
                    elementen.Insert(elementen.Count, tekening);
                    break;
                case Veranderingen.omlaag:
                    elementen.Remove(tekening);
                    elementen.Insert(0, tekening);
                    break;
            }
            undo.Add(redo[redo.Count - 1]);
            redo.RemoveAt(redo.Count - 1);
            schetscontrol.Invalidate();
        }
    }
    public Tekening Welke_Tekening(Point klik)
    {
        for (int i = elementen.Count - 1; i >= 0; i--)
        {
            Tekening tekening = elementen[i];
            switch (tekening.Tool)
            {
                case "lijn":
                case "pen":
                    if (lijn(klik, tekening)) return tekening;
                    break;
                case "kader":
                    if (kader(klik, tekening)) return tekening;
                    break;
                case "vlak":
                    if (vlak(klik, tekening)) return tekening;
                    break;
                case "cirkel":
                    if (cirkel(klik, tekening)) return tekening;
                    break;
                case "bol":
                    if (bol(klik, tekening)) return tekening;
                    break;
                case "tekst":
                    if (tekst(klik, tekening)) return tekening;
                    break;    
            }
        }
        return null;
    }
    public bool lijn(Point klik, Tekening tekening)
    {
        double tolerantie = 3 + (tekening.pen.Width / 2);

        double dx = tekening.eind_punt.X - tekening.start_punt.X;
        double dy = tekening.eind_punt.Y - tekening.start_punt.Y;
        double lijn_lengte = dx * dx + dy * dy;

        double t = ((klik.X - tekening.start_punt.X) * dx + (klik.Y - tekening.start_punt.Y) * dy) / lijn_lengte;

        double lijn_puntx = tekening.start_punt.X + t * dx;
        double lijn_punty = tekening.start_punt.Y + t * dy;
        double afstand = Math.Sqrt(Math.Pow(klik.X - lijn_puntx, 2) + Math.Pow(klik.Y - lijn_punty, 2));

        return afstand <= tolerantie;
    }
    public bool kader(Point klik, Tekening tekening)
    {
        int rechts = Math.Max(tekening.start_punt.X, tekening.eind_punt.X);
        int links = Math.Min(tekening.start_punt.X, tekening.eind_punt.X);
        int boven = Math.Max(tekening.start_punt.Y, tekening.eind_punt.Y);
        int onder = Math.Min(tekening.start_punt.Y, tekening.eind_punt.Y);

        double tol = tekening.pen.Width / 2 + 3;

        return Math.Abs(rechts - klik.X) <= tol ||
               Math.Abs(links - klik.X) <= tol || 
               Math.Abs(boven - klik.Y) <= tol || 
               Math.Abs(onder - klik.Y) <= tol;
    }
    public bool vlak(Point klik, Tekening tekening)
    {
        int rechts = Math.Max(tekening.start_punt.X, tekening.eind_punt.X);
        int links = Math.Min(tekening.start_punt.X, tekening.eind_punt.X);
        int boven = Math.Max(tekening.start_punt.Y, tekening.eind_punt.Y);
        int onder = Math.Min(tekening.start_punt.Y, tekening.eind_punt.Y);

        double tol = tekening.pen.Width / 2 + 3;

        return (links - tol < klik.X && rechts + tol > klik.X) && (boven + tol > klik.Y && onder - tol < klik.Y);
    }
    public bool cirkel(Point klik, Tekening tekening)
    {
        int a = Math.Max(tekening.groote.Width / 2, tekening.groote.Height / 2);
        int b = Math.Min(tekening.groote.Width / 2, tekening.groote.Height / 2);
        double c_lengte = Math.Sqrt(Math.Pow(a, 2) - Math.Pow(b, 2));
        Point mid = new Point((tekening.start_punt.X + tekening.eind_punt.X) / 2, (tekening.start_punt.Y + tekening.eind_punt.Y) / 2);

        Point c1 = new Point(mid.X, mid.Y - (int)c_lengte);
        Point c2 = new Point(mid.X, mid.Y + (int)c_lengte);
        if (tekening.groote.Width >= tekening.groote.Height)
        {
            c1 = new Point(mid.X - (int)c_lengte, mid.Y);
            c2 = new Point(mid.X + (int)c_lengte, mid.Y);
        }

        double afstand_c1 = Math.Sqrt(Math.Pow(klik.X - c1.X, 2) + Math.Pow(klik.Y - c1.Y, 2));
        double afstand_c2 = Math.Sqrt(Math.Pow(klik.X - c2.X, 2) + Math.Pow(klik.Y - c2.Y, 2));
        double tol = tekening.pen.Width / 2 + 3;

        return afstand_c1 + afstand_c2 <= 2 * a + tol && afstand_c1 + afstand_c2 >= 2 * a - tol;
    }
    public bool bol(Point klik, Tekening tekening)
    {
        int a = Math.Max(tekening.groote.Width / 2, tekening.groote.Height / 2);
        int b = Math.Min(tekening.groote.Width / 2, tekening.groote.Height / 2);
        double c_lengte = Math.Sqrt(Math.Pow(a, 2) - Math.Pow(b, 2));
        Point mid = new Point((tekening.start_punt.X + tekening.eind_punt.X) / 2, (tekening.start_punt.Y + tekening.eind_punt.Y) / 2);

        Point c1 = new Point(mid.X, mid.Y - (int)c_lengte);
        Point c2 = new Point(mid.X, mid.Y + (int)c_lengte);
        if (tekening.groote.Width >= tekening.groote.Height)
        {
            c1 = new Point(mid.X - (int)c_lengte, mid.Y);
            c2 = new Point(mid.X + (int)c_lengte, mid.Y);
        }

        double afstand_c1 = Math.Sqrt(Math.Pow(klik.X - c1.X, 2) + Math.Pow(klik.Y - c1.Y, 2));
        double afstand_c2 = Math.Sqrt(Math.Pow(klik.X - c2.X, 2) + Math.Pow(klik.Y - c2.Y, 2));
        double tol = tekening.pen.Width / 2 + 3;

        return afstand_c1 + afstand_c2 <= 2 * a + tol;
    }
    public bool tekst(Point klik, Tekening tekening)
    {
        Rectangle tekst_box = new Rectangle(tekening.start_punt, new Size(tekening.eind_punt.X - tekening.start_punt.X, 60));
        return tekst_box.Contains(klik);
    }
    public class Tekening
    {
        public int lijst_positie;
        public Point start_punt { get; set; }
        public Point eind_punt { get; set; }
        public Size groote { get; set; }
        public Pen pen { get; set; }
        public string Tool { get; set; }
        public string Text { get; set; }
        public Font TextFont { get; set; }


        public Tekening(Point p1, Point p2, Pen pens, string tool, Elementen elementen, string text = null, Font font = null)
        {
            start_punt = p1;
            eind_punt = p2;
            groote = new Size(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
            pen = pens;
            Tool = tool;
            Text = text;
            TextFont = font;
            elementen.undo.Add((Veranderingen.nieuw, this));
            this.lijst_positie = elementen.elementen.Count;
            elementen.elementen.Add(this);
        }
    }
}