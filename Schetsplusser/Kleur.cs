using System.Windows.Forms;
using System.Drawing;
using System.Security.Policy;
using System;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;

public class Kleur : Form
{
    private SchetsControl schetscontrol;
    private Bitmap bitmap;
    private TrackBar slider;
    private Panel colorPreview;
    public Color selected_kleur;
    private bool geklikt = false;
    private Point hier;
    private Point mid = new Point(127,127);

    public Kleur(SchetsControl parent)
    {
        this.schetscontrol = parent;
        this.ClientSize = new Size(285, 380);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.BackColor = Color.White;

        colorPreview = new Panel();
        colorPreview.Size = new Size(50, 50);
        colorPreview.Location = new Point(15, 280);
        colorPreview.BorderStyle = BorderStyle.Fixed3D;
        colorPreview.BackColor = Color.White;
        this.Controls.Add(colorPreview);

        Button oke = new Button();
        oke.Location = new Point(10, 350);
        oke.Text = "OK";
        this.Controls.Add((oke));

        Button cancel = new Button();
        cancel.Location = new Point(100, 350);
        cancel.Text = "Cancel";
        this.Controls.Add(cancel);

        slider = new TrackBar();
        slider.Location = new Point(80, 280);
        slider.Size = new Size(180, 45);
        slider.BackColor = Color.Gray;
        slider.Minimum = 0;
        slider.Maximum = 255;
        slider.Value = 255;
        slider.ValueChanged += V_Slider;
        this.Controls.Add(slider);

        bitmap = new Bitmap(259, 259);
        Graphics gr = Graphics.FromImage(bitmap);
        
        Kleurwiel();
        gr.DrawEllipse(new Pen(Color.Black, 3), 0, 0, 254, 254);
        gr.DrawImage(bitmap, 0, 0);
        this.DoubleBuffered = true;

        this.Paint += Kleur_teken;
        this.MouseClick += Klik_Kleurwiel;
        this.MouseMove += Beweeg;
        oke.MouseClick += knop_oke;
        cancel.MouseClick += knop_cancel;
    }
    private void V_Slider(object sender, EventArgs e)
    {
        Kleurwiel();
        if (selected_kleur != Color.Empty)
        {
            selected_kleur = ColorFromHSV(cirkle_graden(mid, hier), cirkle_afstand(mid, hier)/127, slider.Value);
            colorPreview.BackColor = selected_kleur;
        }
    }
    private void Kleurwiel()
    {
        for (int i = 1; i < 255; i++)
        {
            for (int j = 1; j < 255; j++)
            {
                Point p = new Point(i, j);
                if (cirkle_afstand(mid, p) <= 127)
                    bitmap.SetPixel(i, j, ColorFromHSV(cirkle_graden(mid, p),cirkle_afstand(mid, p)/127, slider.Value));
            }
        }
        this.Refresh();
    }

    private void Klik_Kleurwiel(object sender, MouseEventArgs e)
    {
        hier = new Point(e.X - 15, e.Y - 15);
        geklikt = true;
        if (cirkle_afstand(mid, hier) <= 127)
        {
            Color pixelkleur = bitmap.GetPixel(hier.X, hier.Y);
            selected_kleur = Color.FromArgb(pixelkleur.R, pixelkleur.G, pixelkleur.B);
            colorPreview.BackColor = selected_kleur;
        }
    }

    private void Kleur_teken(object sender, PaintEventArgs e) 
    {
        e.Graphics.DrawImage(bitmap, 15, 15);
    }
    private void Beweeg(object sender, MouseEventArgs mea) 
    {
        if (!geklikt)
        {
            int pixelX = mea.X - 15;
            int pixelY = mea.Y - 15;
            if (cirkle_afstand(mid, new Point(pixelX, pixelY)) <= 127)
            {
                Color pixelkleur = bitmap.GetPixel(pixelX, pixelY);
                colorPreview.BackColor = pixelkleur;
            }
        }
    }
    private Color ColorFromHSV(double hue, double saturation, double slider)  // te ingewikkeld gewoon van internet geplukt akkoord
    {
        int hi = (int)(hue / 60) % 6;
        double f = hue / 60 - Math.Floor(hue / 60);

        int v = (int)(slider);
        int p = (int)(slider * (1 - saturation));
        int q = (int)(slider * (1 - f * saturation));
        int t = (int)(slider * (1 - (1 - f) * saturation));

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
    public void knop_oke(object sender, MouseEventArgs mea)
    {
        if (geklikt)
        {
            schetscontrol.PenKleur = selected_kleur;
            this.Close();
        }
    }
    public void knop_cancel(object sender, MouseEventArgs mea)
    {
        this.Close();
    }
}

