using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CyclicGroups
{
    class Program
    {
        static void Main(string[] args)
        {
            PointF[] points;
            int pointCount;
            int radius = 250;            
            string input;
            
            while (true)
            {
                do
                {
                    Console.WriteLine("Enter number of points to draw");

                    input = Console.ReadLine();

                }
                while (!int.TryParse(input, out pointCount) && input != "exit");

                if (input == "exit") break;

                points = CalculatePoints(pointCount, radius);

                Application.Run(new MyForm(points, radius));
            }       
        }

        static PointF[] CalculatePoints( int pointcount, int radius)
        {
            PointF[] points = new PointF[pointcount];
            double segment = (2 * Math.PI) / pointcount;

            for (int i = 0; i < pointcount; i++)
                points[i] = new PointF((float)(radius * Math.Cos(segment * i)), 
                                       (float)(radius * Math.Sin(segment * i)));

            return points;
        }
    }

    class MyForm : Form
    {
        BufferedGraphicsContext context;
        BufferedGraphics buffer;
        Graphics graphics;

        public MyForm(PointF[] points, int radius) : base()
        {
            //Setup Window
            this.Text = "Cyclic Groups - " + points.Length + " points - " + ((int)points.Length / 2) + " groups";
            this.Size = new Size(600, 600);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            //initialize graphics
            context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(this.Width, this.Height);

            if (buffer != null)
            {
                buffer.Dispose();
                buffer = null;
            }

            buffer = context.Allocate(this.CreateGraphics(), new Rectangle(0, 0, this.Width, this.Height));
            graphics = buffer.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            //Draw Groups
            graphics.TranslateTransform(this.Width / 2, this.Height / 2);
            graphics.RotateTransform(-90);

            Pen pen = new Pen(Color.White);
            graphics.DrawEllipse(pen, -radius, -radius, radius * 2, radius * 2);

            double colourBlend = 0.0;
            double colourInterval = 1.0 / (points.Length / 2 - 1);

            for (int i = 0; i < points.Length / 2; i++)
            {
                pen.Color = Color.FromArgb(0, (int)(255 * (1 - colourBlend)), (int)(255 * colourBlend));
                colourBlend += colourInterval;

                for (int j = 0; j < points.Length; j++)
                    graphics.DrawLine(pen, points[j], points[(j + i + 1) % points.Length]);
            }

            buffer.Render(Graphics.FromHwnd(Handle));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            buffer.Render(e.Graphics);
        }
    }
}
