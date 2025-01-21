using System;
using System.Collections.Generic;
using System.Drawing;

public class Schets
{
    private Bitmap bitmap;
    private SchetsWin schetswin;
    public Bitmap Bitmap
    {
        get { return bitmap; }
    }

    public Schets(SchetsWin schetswin)
    {
        bitmap = new Bitmap(1, 1);
        this.schetswin = schetswin;
    }
    public Graphics BitmapGraphics
    {
        get { return Graphics.FromImage(bitmap); }
    }
    public void VeranderAfmeting(Size sz)   // past de size van de window aan
    {
        if (sz.Width > 0 || sz.Height > 0)
        {
            Bitmap nieuw = new Bitmap(Math.Max(sz.Width, bitmap.Size.Width)
                                     , Math.Max(sz.Height, bitmap.Size.Height)
                                     );
            Graphics gr = Graphics.FromImage(nieuw);
            gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
            gr.DrawImage(bitmap, 0, 0);
            bitmap = nieuw;
        }
    }
    public void Teken(Graphics gr)
    {
        gr.DrawImage(bitmap, 0, 0);
    }
    public void Schoon()  // make een nieuwe bitmap
    {
        Graphics gr = Graphics.FromImage(bitmap);
        gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
        schetswin.unsavedChanges = false;
    }
    public void Roteer()  // flip de bitmap
    {
        bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
    }

}