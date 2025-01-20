using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

public interface ISchetsTool
{
    void MuisVast(SchetsControl s, Point p);
    void MuisDrag(SchetsControl s, Point p);
    void MuisLos(SchetsControl s, Point p);
    void Letter(SchetsControl s, char c);
    void Elementen(Elementen elementen);
}

public abstract class StartpuntTool : ISchetsTool
{
    protected Point startpunt;
    protected Brush kwast;
    protected int dikte;
    protected Elementen elementen;

    public void Elementen(Elementen elementen)
    {
        this.elementen = elementen;
    }
    public virtual void MuisVast(SchetsControl s, Point p)
    {
        startpunt = p;
    }
    public virtual void MuisLos(SchetsControl s, Point p)
    {
        kwast = new SolidBrush(s.PenKleur);
        dikte = s.DiktevLijn;
    }
    public abstract void MuisDrag(SchetsControl s, Point p);
    public abstract void Letter(SchetsControl s, char c);
}

public class TekstTool : StartpuntTool
{
    public override string ToString() { return "tekst"; }

    public override void MuisDrag(SchetsControl s, Point p) { }

    public override void Letter(SchetsControl s, char c)
    {
        if (c >= 32)
        {
            Graphics gr = s.MaakBitmapGraphics();
            Font font = new Font("Comic Sans MS", 40);
            string tekst = c.ToString();
            SizeF sz = 
            gr.MeasureString(tekst, font, this.startpunt, StringFormat.GenericTypographic);
            new Elementen.Tekening(startpunt, new Point(startpunt.X + (int)sz.Width, startpunt.Y), 
            new Pen(kwast), "tekst", elementen, tekst, font);

            startpunt.X += (int)sz.Width;
            s.Invalidate();
        }
    }
}

public abstract class TweepuntTool : StartpuntTool
{
    public static Rectangle Punten2Rechthoek(Point p1, Point p2)
    {
        return new Rectangle(new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y))
                            , new Size(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y))
                            );
    }
    public static Pen MaakPen(Brush b, int dikte)
    {
        Pen pen = new Pen(b, dikte);
        pen.StartCap = LineCap.Round;
        pen.EndCap = LineCap.Round;
        return pen;
    }
    public override void MuisVast(SchetsControl s, Point p)
    {
        base.MuisVast(s, p);
        kwast = Brushes.Gray;
    }
    public override void MuisDrag(SchetsControl s, Point p)
    {
        s.Refresh();
        this.Bezig(s.CreateGraphics(), this.startpunt, p);
    }
    public override void MuisLos(SchetsControl s, Point p)
    {
        base.MuisLos(s, p);
        this.Compleet(s.MaakBitmapGraphics(), this.startpunt, p);
        s.Invalidate();
    }
    public override void Letter(SchetsControl s, char c)
    {
    }
    public abstract void Bezig(Graphics g, Point p1, Point p2);

    public virtual void Compleet(Graphics g, Point p1, Point p2)
    {
        this.Bezig(g, p1, p2);
    }
}

public class RechthoekTool : TweepuntTool
{
    public override string ToString() { return "kader"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {
        g.DrawRectangle(MaakPen(kwast, dikte), TweepuntTool.Punten2Rechthoek(p1, p2));
    }

    public override void Compleet(Graphics g, Point p1, Point p2)
    {
        new Elementen.Tekening(p1, p2, MaakPen(kwast, dikte), "kader", elementen);
    }
}

public class VolRechthoekTool : RechthoekTool
{
    public override string ToString() { return "vlak"; }

    public override void Compleet(Graphics g, Point p1, Point p2)
    {
        new Elementen.Tekening(p1, p2, MaakPen(kwast, dikte), "vlak", elementen);
    }
}

public class CirkelTool : TweepuntTool
{
    public override string ToString() { return "cirkel"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {
        g.DrawEllipse(MaakPen(kwast, dikte), TweepuntTool.Punten2Rechthoek(p1, p2));
    }

    public override void Compleet(Graphics g, Point p1, Point p2)
    {
        new Elementen.Tekening(p1, p2, MaakPen(kwast, dikte), "cirkel", elementen);
    }
}

public class VolCirkelTool : CirkelTool
{
    public override string ToString() { return "bol"; }

    public override void Compleet(Graphics g, Point p1, Point p2)
    {
        new Elementen.Tekening(p1, p2, MaakPen(kwast, dikte), "bol", elementen);
    }
}

public class LijnTool : TweepuntTool
{
    public override string ToString() { return "lijn"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {
        g.DrawLine(MaakPen(kwast, dikte), p1, p2);
    }
    public override void Compleet(Graphics g, Point p1, Point p2)
    {
        new Elementen.Tekening(p1, p2, MaakPen(kwast, dikte), "lijn", elementen);
    }
}

public class PenTool : LijnTool    // alle andere tools ook implementeren?
{
    public override string ToString() { return "pen"; }
    public override void MuisDrag(SchetsControl s, Point p)
    {
        MuisLos(s, p);
        new Elementen.Tekening(startpunt, p, MaakPen(kwast, dikte), "pen", elementen);
        startpunt = p;
    }
}

public class GumTool : PenTool
{
    public override string ToString() { return "gum"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {
        g.DrawLine(MaakPen(Brushes.White, dikte), p1, p2);
    }

    public override void MuisDrag(SchetsControl s, Point p)
    {
        new Elementen.Tekening(startpunt, p, MaakPen(Brushes.White, dikte), "pen", elementen);
        startpunt = p;
        s.Invalidate();
    }
}