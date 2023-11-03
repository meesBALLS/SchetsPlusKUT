using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

public interface ISchetsTool
{
    void MuisVast(SchetsControl s, Point p);
    void MuisDrag(SchetsControl s, Point p);
    void MuisLos(SchetsControl s, Point p);
    void Letter(SchetsControl s, char c);
}

public abstract class StartpuntTool : ISchetsTool
{
    protected Point startpunt;
    protected Brush kwast;

    public virtual void MuisVast(SchetsControl s, Point p)
    {   startpunt = p;
    }
    public virtual void MuisLos(SchetsControl s, Point p)
    {   kwast = new SolidBrush(s.PenKleur);
    }
    public abstract void MuisDrag(SchetsControl s, Point p);
    public abstract void Letter(SchetsControl s, char c);
}

public struct GetekendObject
{
    public string soort;
    public Point beginpunt;
    public Point eindpunt;
    public Color kleur;
    public string tekst;
    public GetekendObject(string soort, Point beginpunt, Point eindpunt, Color kleur, string tekst=null)
    {
        this.soort = soort;
        this.beginpunt = beginpunt;
        this.eindpunt = eindpunt;
        this.kleur = kleur;
        this.tekst = tekst;
    }

    public override string ToString() {return this.soort + ", " + this.beginpunt + ", " + this.eindpunt + ", " + this.kleur + ", " + this.tekst;}
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
            Font font = new Font("Tahoma", 40);
            string tekst = c.ToString();
            SizeF sz = 
            gr.MeasureString(tekst, font, this.startpunt, StringFormat.GenericTypographic);
            gr.DrawString   (tekst, font, kwast, 
                                            this.startpunt, StringFormat.GenericTypographic);
            s.Schets.getekendelijst.Add(new GetekendObject(this.ToString(), this.startpunt, Point.Add(this.startpunt, sz.ToSize()), s.PenKleur, tekst));
            //for (int i = 0; i < s.Schets.getekendelijst.Count; i++)
            //{
            //    Console.WriteLine(s.Schets.getekendelijst[i]);
            //}
            //Console.WriteLine("\n");
            //gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width, sz.Height);
            startpunt.X += (int)sz.Width;
            s.Invalidate();
        }
    }
}

public abstract class TweepuntTool : StartpuntTool
{
    public static Rectangle Punten2Rechthoek(Point p1, Point p2)
    {   return new Rectangle( new Point(Math.Min(p1.X,p2.X), Math.Min(p1.Y,p2.Y))
                            , new Size (Math.Abs(p1.X-p2.X), Math.Abs(p1.Y-p2.Y))
                            );
    }
    public static Pen MaakPen(Brush b, int dikte)
    {   Pen pen = new Pen(b, dikte);
        pen.StartCap = LineCap.Round;
        pen.EndCap = LineCap.Round;
        return pen;
    }
    public override void MuisVast(SchetsControl s, Point p)
    {   base.MuisVast(s, p);
        kwast = Brushes.Gray;
    }
    public override void MuisDrag(SchetsControl s, Point p)
    {   s.Refresh();
        this.Bezig(s.CreateGraphics(), this.startpunt, p);
    }
    public override void MuisLos(SchetsControl s, Point p)
    {   base.MuisLos(s, p);
        s.Schets.getekendelijst.Add(new GetekendObject(this.ToString(), this.startpunt, p, s.PenKleur));
        
        //for (int i = 0; i < s.Schets.getekendelijst.Count; i++)
        //{
        //    Console.WriteLine(s.Schets.getekendelijst[i]);
        //}
        //Console.WriteLine("\n");
        this.Compleet(s.MaakBitmapGraphics(), this.startpunt, p);
        s.Invalidate();
    }
    public override void Letter(SchetsControl s, char c)
    {
    }
    public abstract void Bezig(Graphics g, Point p1, Point p2);
        
    public virtual void Compleet(Graphics g, Point p1, Point p2)
    {   this.Bezig(g, p1, p2);
    }
}

public class RechthoekTool : TweepuntTool
{
    public override string ToString() { return "kader"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {   g.DrawRectangle(MaakPen(kwast,3), TweepuntTool.Punten2Rechthoek(p1, p2));
    }
}

public class CirkelTool : TweepuntTool
{
    public override string ToString() { return "cirkel"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {   g.DrawEllipse(MaakPen(kwast,3), TweepuntTool.Punten2Rechthoek(p1, p2));
    }
}
    
public class VolRechthoekTool : RechthoekTool
{
    public override string ToString() { return "vlak"; }

    public override void Compleet(Graphics g, Point p1, Point p2)
    {   g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
    }
}

public class VolCirkelTool : CirkelTool
{
    public override string ToString() { return "volcirkel"; }

    public override void Compleet(Graphics g, Point p1, Point p2)
    {   g.FillEllipse(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
    }
}

public class LijnTool : TweepuntTool
{
    public override string ToString() { return "lijn"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {   g.DrawLine(MaakPen(this.kwast,3), p1, p2);
    }
}

public class PenTool : LijnTool
{
    public override string ToString() { return "pen"; }

    public override void MuisDrag(SchetsControl s, Point p)
    {   this.MuisLos(s, p);
        this.MuisVast(s, p);
    }
}
    
public class GumTool : PenTool
{
    public override string ToString() { return "gum"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {    }

    public int PuntLijnAfstand(Point p1, Point p2, Point p)
    {
        int x1 = p1.X;
        int y1 = p1.Y;
        int x2 = p2.X;
        int y2 = p2.Y;
        int x = p.X;
        int y = p.Y;

        int boven = Math.Abs((x2-x1)*(y1-y)-(x1-x)*(y2-y1));
        int onder = (int)Math.Sqrt(Math.Pow(y2 - y1, 2) + Math.Pow(x2 - x1, 2));
        if (onder == 0)
        {
            onder = 1;
        }
        return boven / onder;
    }

    public bool PuntOpEllips(Point p, double middenx, double middeny, double radiusx, double radiusy, double delta)
    {
        double resultaat = (Math.Pow(p.X-middenx,2)/ Math.Pow(radiusx, 2))+ (Math.Pow(p.Y - middeny, 2) / Math.Pow(radiusy, 2));
        return (1 - delta <= resultaat && resultaat <= 1 + delta);
    }

    public bool Raak(string soort, Point begin, Point eind, Point p)
    {
        double afstand = PuntLijnAfstand(begin, eind, p);
        double radiusx = Math.Abs((double)eind.X - (double)begin.X) / 2.0;
        double radiusy = Math.Abs((double)eind.Y - (double)begin.Y) / 2.0;
        double midpointx = ((double)begin.X + (double)eind.X) / 2.0;
        double midpointy = ((double)begin.Y + (double)eind.Y) / 2.0;
        switch (soort)
        {
            case ("kader"):
                int breedte = Math.Abs(begin.X - eind.X);
                int hoogte = Math.Abs(begin.Y - eind.Y);
                Tuple<Point, Point> lijn1 = new Tuple<Point, Point>(begin, new Point(begin.X, begin.Y + hoogte));
                Tuple<Point, Point> lijn2 = new Tuple<Point, Point>(new Point(begin.X, begin.Y + hoogte), new Point(begin.X + breedte, begin.Y + hoogte));
                Tuple<Point, Point> lijn3 = new Tuple<Point, Point>(new Point(begin.X + breedte, begin.Y + hoogte), new Point(begin.X + breedte, begin.Y));
                Tuple<Point, Point> lijn4 = new Tuple<Point, Point>(new Point(begin.X + breedte, begin.Y), begin);
                if (PuntLijnAfstand(lijn1.Item1, lijn1.Item2, p) <= 8)
                {
                    return true;
                }
                else if (PuntLijnAfstand(lijn2.Item1, lijn2.Item2, p) <= 8)
                {
                    return true;
                }
                else if (PuntLijnAfstand(lijn3.Item1, lijn3.Item2, p) <= 8)
                {
                    return true;
                }
                else if (PuntLijnAfstand(lijn4.Item1, lijn4.Item2, p) <= 8)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case ("cirkel"):
                return (PuntOpEllips(p, midpointx, midpointy, radiusx, radiusy, 24/radiusx));
            case ("vlak"):
            case ("tekst"):
                return (p.X > begin.X && p.X < eind.X && p.Y > begin.Y && p.Y < eind.Y);
            case ("volcirkel"):
                return ((Math.Pow((p.X - midpointx) / radiusx, 2) + Math.Pow((p.Y - midpointy) / radiusy, 2)) <= 1.0);
            case ("lijn"):
            case ("pen"):
                Console.WriteLine(afstand);
                return (afstand <= 8);
            default:
                return false;
        }
    }

    public override void MuisLos(SchetsControl s, Point p)
    {
        for (int i = s.Schets.Getekendelijst.Count-1; i > -1; i--)
        {
            string soort = s.Schets.getekendelijst[i].soort;
            Point beginpunt = s.Schets.Getekendelijst[i].beginpunt;
            Point eindpunt = s.Schets.Getekendelijst[i].eindpunt;

            if (Raak(soort, beginpunt, eindpunt, p))
            {
                s.Schets.Getekendelijst.RemoveAt(i);
                s.tekenuitlijst(s.MaakBitmapGraphics());
                s.Refresh();
                break;
            }
        }
        
    }
}