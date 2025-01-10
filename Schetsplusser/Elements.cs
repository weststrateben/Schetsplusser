using System;
using System.Collections.Generic;
using System.Drawing;

public class Elementen
{
    internal enum Veranderingen {nieuw, oud, omhoog, omlaag}

    public List<Tekening> elementen;
    private List<(Veranderingen, Tekening)> undo;
    private List<(Veranderingen, Tekening)> redo;
    public Elementen()
    {
        elementen = new List<Tekening>();
        undo = new List<(Veranderingen, Tekening)>();
        redo = new List<(Veranderingen, Tekening)>();
    }
    public void Omhoog_Tekening(Tekening tekening)
    {
        tekening.lijst_positie = elementen.IndexOf(tekening);
        undo.Add((Veranderingen.omhoog, tekening));
    }
    public void Omlaag_Tekening(Tekening tekening)
    {
        tekening.lijst_positie = elementen.IndexOf(tekening);
        undo.Add((Veranderingen.omlaag, tekening));
    }
    public void Verwijder_Tekening(Tekening tekening)
    {
        tekening.lijst_positie = elementen.IndexOf(tekening);
        undo.Add((Veranderingen.oud, tekening));
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
        }
    }
    public void Redo(object o, EventArgs ea)
    {
        if (redo.Count > 0)
        {
            Tekening tekening = undo[undo.Count - 1].Item2;
            switch (undo[undo.Count - 1].Item1)
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
                    elementen.Add(tekening);
                    break;
                case Veranderingen.omlaag:
                    elementen.Remove(tekening);
                    elementen.Insert(0, tekening);
                    break;
            }
            undo.Add(redo[redo.Count - 1]);
            redo.RemoveAt(redo.Count - 1);
        }
    }
    public class Tekening
    {
        public int lijst_positie;
        public Point start_punt { get; set; }
        public Point eind_punt { get; set; }
        public Color kleur { get; set; }
        public int lijnbreedte { get; set; }
        public string Tool { get; set; }

        public Tekening(Point start, Point eind, Color klr, int lijn_b, string tool, Elementen lijst)
        {
            start_punt = start;
            eind_punt = eind;
            kleur = klr;
            lijnbreedte = lijn_b;
            Tool = tool;
            lijst.undo.Add((Veranderingen.nieuw, this));
            this.lijst_positie = lijst.elementen.Count - 1;
            lijst.elementen.Add(this);
        }
    }
}