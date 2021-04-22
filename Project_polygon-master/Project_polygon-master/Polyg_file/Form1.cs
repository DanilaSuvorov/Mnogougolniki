using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Polygons
{
    public partial class Form1 : Form
    {
        bool go_1 = false;
        bool go_2 = false;
        Random rand = new Random();
        int random_max;
        int figure;
        bool Draw;
        List<My_Shape> shapes = new List<My_Shape>();
        bool open;
        string fileNamePath;
        bool ChangeSaved = false;
        string currentpath = Directory.GetCurrentDirectory();

        public Form1()
        {
            InitializeComponent();
            Draw = false;
            DoubleBuffered = true;
            random_max = 5;
            open = false;
            My_Shape.r = 40;
            My_Shape.colo = Color.Red;
        }

        private bool CheckMoving(int x_m, int y_m, List<My_Shape> shapes)
        {
            if (shapes.Count() > 2)
            {
                double delta_test;
                bool Up_test = false;
                bool Down_test = false;
                List<My_Shape> Test_polygon = new List<My_Shape>();
                My_Shape Test_point = new Circle(x_m, y_m);
                shapes.Add(Test_point);
                Test_polygon.Clear();
                for (int i = 0; i < shapes.Count() - 1; i++)
                {
                    for (int j = i + 1; j < shapes.Count(); j++)
                    {
                        for (int k = 0; k < shapes.Count(); k++)
                        {
                            if (k != j && k != i)
                            {
                                delta_test = ((shapes[k].SetY - shapes[j].SetY) * (shapes[i].SetX - shapes[j].SetX)) -
                                         ((shapes[i].SetY - shapes[j].SetY) * (shapes[k].SetX - shapes[j].SetX));

                                if (delta_test >= 0) Up_test = true;
                                else Down_test = true;
                            }
                        }
                        if ((Up_test && Down_test) == false)
                        {
                            Test_polygon.Add(shapes[j]);
                            Test_polygon.Add(shapes[i]);
                        }
                        Up_test = false;
                        Down_test = false;
                    }
                }
                if (Test_polygon.Contains(Test_point) == false)
                {
                    shapes.Remove(Test_point);
                    return true;
                }
                shapes.Remove(Test_point);
            }
            return false;
        }



        private void CircleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            figure = 1;
        }

        private void SquareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            figure = 2;
        }

        private void TriangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            figure = 3;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            bool Up = false;
            bool Down = false;
            double delta;
            if (Draw)
            {
                Graphics g = e.Graphics;
                foreach (My_Shape polyg in shapes)
                {
                    polyg.Inside = false;
                    polyg.Draw(g);
                }
                Pen brush = new Pen(Color.Black);
                if (shapes.Count() > 2)
                {
                    for (int i = 0; i < shapes.Count() - 1; i++)
                    {
                        for (int j = i + 1; j < shapes.Count(); j++)
                        {
                            for (int k = 0; k < shapes.Count(); k++)
                            {
                                if (k != i && k != j)
                                {
                                    delta = ((shapes[k].SetY - shapes[j].SetY) * (shapes[i].SetX - shapes[j].SetX)) -
                                        ((shapes[i].SetY - shapes[j].SetY) * (shapes[k].SetX - shapes[j].SetX));

                                    if (delta >= 0) Up = true;
                                    else Down = true;
                                }
                            }
                            if ((Up && Down) == false)
                            {
                                shapes[i].Inside = true;
                                shapes[j].Inside = true;
                                e.Graphics.DrawLine(brush, shapes[i].SetX, shapes[i].SetY, shapes[j].SetX, shapes[j].SetY);
                            }

                            Up = false;
                            Down = false;
                        }
                    }
                }
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeSaved = true;
            bool check = false;

            if (shapes.Any())
            {
                foreach (My_Shape polyg in shapes)
                {
                    if (Draw && polyg.Check(e.X, e.Y))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            polyg.SetDelX = e.X - polyg.SetX;
                            polyg.SetDelY = e.Y - polyg.SetY;
                            polyg.SetChange = true;
                            check = true;
                        }
                        if (e.Button == MouseButtons.Right)
                        {
                            shapes.Remove(polyg);
                            Refresh();
                            check = true;
                            break;
                        }
                    }
                }
                if (check == false)
                {
                    if (CheckMoving(e.X, e.Y, shapes))
                    {
                        Draw = true;
                        for (int k = 0; k < shapes.Count; k++)
                        {
                            shapes[k].SetDelX = e.X - shapes[k].SetX;
                            shapes[k].SetDelY = e.Y - shapes[k].SetY;
                            shapes[k].SetChange = true;
                        }
                    }
                    else
                    {
                        Draw = true;
                        switch (figure)
                        {
                            case 1: shapes.Add(new Circle(e.X, e.Y)); break;
                            case 2: shapes.Add(new Square(e.X, e.Y)); break;
                            case 3: shapes.Add(new Triangle(e.X, e.Y)); break;
                            default: shapes.Add(new Circle(e.X, e.Y)); break;
                        }
                        Refresh();
                    }
                }

            }
            else
            {
                Draw = true;
                switch (figure)
                {
                    case 1: shapes.Add(new Circle(e.X, e.Y)); break;
                    case 2: shapes.Add(new Square(e.X, e.Y)); break;
                    case 3: shapes.Add(new Triangle(e.X, e.Y)); break;
                    default: shapes.Add(new Circle(e.X, e.Y)); break;
                }
                this.Refresh();
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            foreach (My_Shape polyg in shapes)
            {
                if (polyg.SetChange)
                {
                    polyg.SetX = e.X - polyg.SetDelX;
                    polyg.SetY = e.Y - polyg.SetDelY;
                    this.Invalidate();
                }
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            foreach (My_Shape polyg in shapes)
            {
                if (polyg.SetChange)
                {
                    polyg.SetChange = false;
                    if (polyg.Inside == false)
                    {
                        shapes.Remove(polyg);
                        Refresh();
                        break;
                    }
                    Refresh();
                }
            }
        }

        private void выбратьЦветФигурыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowHelp = true;
            cd.Color = My_Shape.colo;

            if (cd.ShowDialog() == DialogResult.OK)
            {
                My_Shape.colo = cd.Color;
                Invalidate();
            }
            
            ChangeSaved = true;
        }
        
        private void startToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            timer1.Start();
            go_1 = true;
            ChangeSaved = true;
        }

        private void stopToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            ChangeSaved = true;
        }

        private void движениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 frm_2 = new Form2();

            frm_2.Owner = this;
            frm_2.ShowDialog();
            random_max = frm_2.Data;
            open = true;

        }

        private void Figure_sizeToolStripMenuItem_Click(object sender, EventArgs e)
        {          
            Form3 Radius_figure = new Form3();
            Radius_figure.Show();
            Radius_figure.Rch += RadiusDelegate;

            open = true;
        }

        private void RadiusDelegate(object sender, RadiusEventArgs e)
        {
            My_Shape.r = e.Rad;
            Refresh();
            
            ChangeSaved = true;
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            int random_number = rand.Next(0, random_max);
            if (go_1)
            {
                foreach (My_Shape polyg in shapes)
                {
                    polyg.SetX += random_number;
                    polyg.SetY += random_number;
                    go_1 = false;
                    go_2 = true;
                }
            }
            else if (go_2)
            {
                foreach (My_Shape polyg in shapes)
                {
                    polyg.SetX -= random_number;
                    polyg.SetY -= random_number;
                    go_2 = false;
                    go_1 = true;
                }
            }
            Refresh();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveDialog = new SaveFileDialog();
            SaveDialog.InitialDirectory = currentpath;
            SaveDialog.Filter = "bin files (*.bin)|*.bin";
            BinaryFormatter bf = new BinaryFormatter();
            if (SaveDialog.ShowDialog() == DialogResult.OK && SaveDialog.FileName != null)
            {
                FileStream fs = new FileStream(SaveDialog.FileName, FileMode.Create, FileAccess.Write);
                bf.Serialize(fs, shapes);
                bf.Serialize(fs, My_Shape.colo);
                bf.Serialize(fs, My_Shape.r);
                fileNamePath = SaveDialog.FileName;
                ChangeSaved = true;
                fs.Close();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileNamePath != null)
            {
                SaveFileDialog SaveDialog = new SaveFileDialog();
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = new FileStream(fileNamePath, FileMode.Create, FileAccess.Write);
                bf.Serialize(fs, shapes);
                bf.Serialize(fs, My_Shape.colo);
                bf.Serialize(fs, My_Shape.r);
                ChangeSaved = true;
                fs.Close();
            }
            else
            {
                SaveFileDialog SaveDialog = new SaveFileDialog();
                SaveDialog.InitialDirectory = currentpath;
                SaveDialog.Filter = "bin files (*.bin)|*.bin";
                BinaryFormatter bf = new BinaryFormatter();
                if (SaveDialog.ShowDialog() == DialogResult.OK && SaveDialog.FileName != null)
                {
                    FileStream fs = new FileStream(SaveDialog.FileName, FileMode.Create, FileAccess.Write);
                    bf.Serialize(fs, shapes);
                    bf.Serialize(fs, My_Shape.colo);
                    bf.Serialize(fs, My_Shape.r);
                    fileNamePath = SaveDialog.FileName;
                    ChangeSaved = true;
                    fs.Close();
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenDialog = new OpenFileDialog();
            OpenDialog.InitialDirectory = currentpath;
            OpenDialog.Filter = "bin files (*.bin)|*.bin";
            BinaryFormatter bf = new BinaryFormatter();
            if (OpenDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(OpenDialog.FileName, FileMode.Open, FileAccess.Read);
                shapes = (List<My_Shape>)bf.Deserialize(fs);
                My_Shape.colo = (Color)bf.Deserialize(fs);
                My_Shape.r = (int)bf.Deserialize(fs);
                fileNamePath = OpenDialog.FileName;
                fs.Close();
                Refresh();
            }
        }

    }
    [Serializable]
    public abstract class My_Shape
    {
        protected int x_0, y_0;

        public int SetX { set { x_0 = value; } get { return x_0; } }

        public int SetY { set { y_0 = value; } get { return y_0; } }

        public int SetDelX { set; get; }

        public int SetDelY { set; get; }

        public static int r { get; set; }

        public static Color colo { get; set; }

        [NonSerialized] public bool Inside;

        [NonSerialized] public bool SetChange;
        
        public My_Shape(int x, int y)
        {
            x_0 = x;
            y_0 = y;           
        }

        abstract public void Draw(Graphics g);

        abstract public bool Check(int x, int y);
    }

    [Serializable]
    class Circle : My_Shape
    {
        public Circle(int x, int y) : base(x, y) { }
        public override void Draw(Graphics g)
        {
            SolidBrush brush = new SolidBrush(colo);
            g.FillEllipse(brush, x_0 - r, y_0 - r, 2 * r, 2 * r);
        }
        override public bool Check(int x, int y)
        {
            if (Math.Pow(r, 2) >= Math.Pow(x - x_0, 2) + Math.Pow(y - y_0, 2))
                return true;
            else
                return false;
        }
    }

    [Serializable]
    class Square : My_Shape
    {
        public Square(int x, int y) : base(x, y) { }
        public override void Draw(Graphics g)
        {
            SolidBrush brush = new SolidBrush(colo);
            g.FillRectangle(brush, (int)(x_0 - r / Math.Sqrt(2)), (int)(y_0 - r / Math.Sqrt(2)), (int)(2 * r / Math.Sqrt(2)), (int)(2 * r / Math.Sqrt(2)));
        }
        override public bool Check(int x, int y)
        {
            if (Math.Pow(r, 2) >= Math.Pow(x - x_0, 2) + Math.Pow(y - y_0, 2))
                return true;
            else
                return false;
        }
    }

    [Serializable]
    class Triangle : My_Shape
    {
        public Triangle(int x, int y) : base(x, y) { }
        public override void Draw(Graphics g)
        {           
            SolidBrush brush = new SolidBrush(colo);

            Point point1 = new Point(x_0, y_0 - r);
            Point point2 = new Point(x_0 - (int)(r * Math.Sqrt(3) / 2), y_0 + r / 2);
            Point point3 = new Point(x_0 + (int)(r * Math.Sqrt(3) / 2), y_0 + r / 2);
            Point[] points = { point1, point2, point3 };

            g.FillPolygon(brush, points);
        }
        override public bool Check(int x, int y)
        {
            if (Math.Pow(r, 2) >= Math.Pow(x - x_0, 2) + Math.Pow(y - y_0, 2))
                return true;
            else
                return false;
        }
    }
}