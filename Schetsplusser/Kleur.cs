using System.Windows.Forms;
using System.Drawing;
using System.Security.Policy;
using System;
using System.Drawing.Drawing2D;

public class Kleur : Form
{
    private SchetsControl schetscontrol;
    private Bitmap bitmap;
    private TrackBar slider;
    private Panel colorPreview;
    public Color selected_kleur;
    private bool geklikt = false;

    public Kleur(SchetsControl parent)
    {
        this.schetscontrol = parent;
        this.ClientSize = new Size(285, 350);
        this.BackColor = Color.White;

        colorPreview = new Panel();
        colorPreview.Size = new Size(50, 50);
        colorPreview.Location = new Point(15, 280);
        colorPreview.BorderStyle = BorderStyle.Fixed3D;
        colorPreview.BackColor = Color.White;
        this.Controls.Add(colorPreview);

        slider = new TrackBar();
        slider.Location = new Point(80, 280);
        slider.Size = new Size(180, 45);
        slider.BackColor = Color.Gray;
        slider.Minimum = 1;
        slider.Maximum = 255;
        slider.Value = 255;
        slider.ValueChanged += Slider_ValueChanged;
        this.Controls.Add(slider);

        bitmap = new Bitmap(259, 259);
        Graphics gr = Graphics.FromImage(bitmap);
        Point mid = new Point(127, 127);
        for (int i = 1; i < 255; i++)
        {
            for (int j = 1; j < 255; j++)
            {
                Point p = new Point(i, j);
                if (cirkle_afstand(mid, p) <= 127)
                    bitmap.SetPixel(i, j, ColorFromHSV(cirkle_graden(mid, p), van_0_tot_1(cirkle_afstand(mid, p))));
            }
        }
        gr.DrawEllipse(new Pen(Color.Black, 3), 0, 0, 254, 254);
        gr.DrawImage(bitmap, 0, 0);

        this.Paint += Kleur_teken;
        this.MouseClick += Klik_Kleurwiel;
        this.MouseMove += Beweeg;
    }
    private void Slider_ValueChanged(object sender, EventArgs e)
    {
        this.DoubleBuffered = true;
        if (selected_kleur != Color.Empty)
        {
            int waarden = slider.Value; 
            selected_kleur = Color.FromArgb(selected_kleur.R/waarden, selected_kleur.G/waarden, selected_kleur.B/waarden);// dit veranderen
            colorPreview.BackColor = selected_kleur;
        }
    }

    private void Klik_Kleurwiel(object sender, MouseEventArgs e)
    {
        Point hier = new Point(e.X - 15, e.Y - 15);
        geklikt = true;
        if (hier.X >= 0 && hier.X < bitmap.Width && hier.Y >= 0 && hier.Y < bitmap.Height)
        {
            Color pixelkleur = bitmap.GetPixel(hier.X, hier.Y);
            selected_kleur = Color.FromArgb(slider.Value, pixelkleur.R, pixelkleur.G, pixelkleur.B);
            colorPreview.BackColor = selected_kleur;
        }
    }

    private void Kleur_teken(object sender, PaintEventArgs e) //akkoord
    {
        e.Graphics.DrawImage(bitmap, 15, 15);
    }
    private void Beweeg(object sender, MouseEventArgs mea) //akkoord
    {
        if (!geklikt)
        {
            int pixelX = mea.X - 15;
            int pixelY = mea.Y - 15;
            if (pixelX >= 0 && pixelX < bitmap.Width && pixelY >= 0 && pixelY < bitmap.Height)
            {
                Color pixelkleur = bitmap.GetPixel(pixelX, pixelY);
                colorPreview.BackColor = pixelkleur;
            }
        }
    }
    private Color ColorFromHSV(double hue, double saturation)  // te ingewikkeld gewoon van internet geplukt akkoord
    {
        int hi = (int)(Math.Floor(hue / 60)) % 6;
        double f = hue / 60 - Math.Floor(hue / 60);

        int value = 255;
        int v = (int)(value);
        int p = (int)(value * (1 - saturation));
        int q = (int)(value * (1 - f * saturation));
        int t = (int)(value * (1 - (1 - f) * saturation));

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
    private double cirkle_graden(Point mid, Point p) //akkoord
    {
        double radian = Math.Atan2(p.Y - mid.Y, p.X - mid.X);
        double graden = radian * (180 / Math.PI);

        if (graden < 0)
        {
            graden += 360;
        }
        return graden;
    }
    private double cirkle_afstand(Point mid, Point p) //akkoord
    {
        double dx = Math.Abs(mid.X - p.X);
        double dy = Math.Abs(mid.Y - p.Y);
        return Math.Sqrt(dx * dx + dy * dy);
    }
    private double van_0_tot_1(double afstand) //akkoord
    {
        return afstand / 127;
    }
}

