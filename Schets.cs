using System;
using System.Collections.Generic;
using System.Drawing;

public class Schets
{
    public Bitmap bitmap;
    public List<GetekendObject> getekendelijst;
        
    public Schets()
    {
        bitmap = new Bitmap(1, 1);
        getekendelijst = new List<GetekendObject>();
    }
    public Graphics BitmapGraphics
    {
        get { return Graphics.FromImage(bitmap); }
    }
    public List<GetekendObject> Getekendelijst
    {
        get { return getekendelijst; }
    }
    public void VeranderAfmeting(Size sz)
    {
        if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
        {
            Bitmap nieuw = new Bitmap( Math.Max(sz.Width,  bitmap.Size.Width)
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
    public void TekenUitLijst(Graphics gr)
    {
        this.Schoon();
        for (int i = 0; i < getekendelijst.Count; i++)
        {
            switch (getekendelijst[i].soort)
            {
                case ("kader"):
                    gr.DrawRectangle(new Pen(getekendelijst[i].kleur), TweepuntTool.Punten2Rechthoek(getekendelijst[i].beginpunt, getekendelijst[i].eindpunt));
                    break;
                case ("cirkel"):
                    gr.DrawEllipse(new Pen(getekendelijst[i].kleur), TweepuntTool.Punten2Rechthoek(getekendelijst[i].beginpunt, getekendelijst[i].eindpunt));
                    break;
                case ("vlak"):
                    gr.FillRectangle(new SolidBrush(getekendelijst[i].kleur), TweepuntTool.Punten2Rechthoek(getekendelijst[i].beginpunt, getekendelijst[i].eindpunt));
                    break;
                case ("volcirkel"):
                    gr.FillEllipse(new SolidBrush(getekendelijst[i].kleur), TweepuntTool.Punten2Rechthoek(getekendelijst[i].beginpunt, getekendelijst[i].eindpunt));
                    break;
                case ("lijn"):
                    gr.DrawLine(new Pen(getekendelijst[i].kleur), getekendelijst[i].beginpunt, getekendelijst[i].eindpunt);
                    break;
                case ("pen"):
                    gr.DrawLine(new Pen(getekendelijst[i].kleur), getekendelijst[i].beginpunt, getekendelijst[i].eindpunt);
                    break;
            }
        }
    }
    public void Schoon()
    {
        Graphics gr = Graphics.FromImage(bitmap);
        gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
    }
    public void Roteer()
    {
        bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
    }
}