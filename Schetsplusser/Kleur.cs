using System.Windows.Forms;
using System.Drawing;
using System.Security.Policy;
using System;
using System.Drawing.Drawing2D;
public class Kleur : Form
{
    private SchetsControl schetscontrol;
    private Bitmap bitmap;
    private Point hier;
    public Color selected_kleur;
    public Kleur(SchetsControl parent)
    {
        this.schetscontrol = parent;
        this.ClientSize = new Size(285, 300);

        bitmap = new Bitmap(259, 259);
        Graphics gr = Graphics.FromImage(bitmap);
        Point mid = new Point(127, 127);
        for (int i = 1; i < 255; i++)
        {
            for (int j = 1; j < 255; j++)
            {
                Point p = new Point(i, j);
                if(cirkle_afstand(mid, p) <= 127)
                bitmap.SetPixel(i, j, ColorFromHSV(cirkle_graden(mid, p), van_0_tot_1(cirkle_afstand(mid, p))));
            }
        }
        gr.DrawImage(bitmap, 0, 0);

        this.Paint += Kleur_teken;
    }
    private void Kleur_teken(object sender, PaintEventArgs e)
    {
        e.Graphics.DrawImage(bitmap, 15, 15);
    }
    private void Klik(object sender, MouseEventArgs mea)
    {
        hier = mea.Location;
    }
    private Color ColorFromHSV(double hue, double saturation)  // te ingewikkeld gewoon van internet geplukt
    {
        int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        double f = hue / 60 - Math.Floor(hue / 60);

        int value = 255;
        int v = Convert.ToInt32(value);
        int p = Convert.ToInt32(value * (1 - saturation));
        int q = Convert.ToInt32(value * (1 - f * saturation));
        int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

        if (hi == 0)
            return Color.FromArgb(255, v, t, p);
        else if (hi == 1)
            return Color.FromArgb(255, q, v, p);
        else if (hi == 2)
            return Color.FromArgb(255, p, v, t);
        else if (hi == 3)
            return Color.FromArgb(255, p, q, v);
        else if (hi == 4)
            return Color.FromArgb(255, t, p, v);
        else
            return Color.FromArgb(255, v, p, q);
    }
    private double cirkle_graden(Point mid, Point p)
    {
        double radian = Math.Atan2(p.Y - mid.Y, p.X - mid.X);
        double graden = radian * (180 / Math.PI);

        if (graden < 0)
        {
            graden += 360;
        }
        return graden;
    }
    private double cirkle_afstand(Point mid, Point p)
    {
        double dx = Math.Abs(mid.X - p.X);
        double dy = Math.Abs(mid.Y - p.Y);
        return Math.Sqrt(dx * dx + dy * dy);
    }
    private double van_0_tot_1(double afstand)
    {
        return afstand / 127;
    }
}

