using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace CG_Project3
{
    public partial class Form1 : Form
    {
        Bitmap? Image;
        List<IShape> Shapes;
        List<Point> prevPoints;
        Color currentColor;
        const int CLICK_DISTANCE = 1000;
        Vertex activeVertex;
        DateTime prevClick;
        Point prevMousePosition;
        public Form1()
        {
            InitializeComponent();
            Image = new Bitmap(pictureBox.Width, pictureBox.Height);
            pictureBox.Image = Image;
            prevPoints = new List<Point>();
            comboBox1.Items.Add("Line");
            comboBox1.Items.Add("Thick Line");
            comboBox1.Items.Add("Circle");
            comboBox1.Items.Add("Polygon");
            comboBox1.Items.Add("Anti-Alliased Line");
            comboBox1.Items.Add("Move Shape");
            comboBox1.Items.Add("Change Color");
            comboBox1.Items.Add("Change Thickness");
            comboBox1.Items.Add("Delete Shape");
            comboBox1.Items.Add("PacMan");
            int mode = Settings1.Default.Mode;
            if (mode < 0 || mode > 9)
                mode = 0;
            currentColor = Settings1.Default.Color;
            int width = Settings1.Default.Width;
            if (width < 1 || width > 100)
                width = 5;
            numericLineWidth.Value = width;
            comboBox1.SelectedIndex = mode;
            label1.Text = "Select the first point.";
            panel1.BackColor = currentColor;
            Shapes = new List<IShape>();
            /*Shapes.Add(new Line(new Point(400, 200), new Point(100, 50), Color.FromArgb(255, 0, 255, 0)));
            Shapes.Add(new Line(new Point(600, 300), new Point(400, 50), Color.FromArgb(255, 255, 0, 0)));
            Shapes.Add(new ThickLine(new Point(0, 0), new Point(400, 50), 5, Color.FromArgb(255, 0, 0, 100)));
            Shapes.Add(new ThickLine(new Point(100, 100), new Point(100, 100), 15, Color.FromArgb(255, 0, 0, 100)));
            Shapes.Add(new Circle(new Point(500, 250), 30, Color.FromArgb(255, 100, 100, 100)));
            Shapes.Add(new AALine(new Point(100, 300), new Point(600,400),10,Color.FromArgb(255,255,0,255)));*/
            DrawShapes();
        }

        public void DrawShapes()
        {
            int stride;
            Image.Dispose();
            Image = new Bitmap(pictureBox.Width, pictureBox.Height); ;
            byte[] ImageBytes = Program.ImageToByteArray(Image, out stride);
            foreach (IShape shape in Shapes)
            {
                shape.Draw(ImageBytes, stride);
            }
            // diagnostics code, for displaying vertices as red pixels
            /*foreach (IShape shape in Shapes)
            {
                foreach (Vertex vertex in shape.GetVertices())
                {
                    int i = vertex.Point.Y * stride + vertex.Point.X * 3;
                    ImageBytes[i] = (byte)0;
                    ImageBytes[i + 1] = (byte)0;
                    ImageBytes[i + 2] = (byte)255;
                }
            }*/
            int width = Image.Width;
            int height = Image.Height;
            Image = Program.ByteArrayToImage(ImageBytes, width, height, stride);
            pictureBox.Image = Image;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) // save
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Txt File|*.txt|Vector Image|*.vec|Any format|*.*";
            saveFileDialog.Title = "Save Image As";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (!File.Exists(saveFileDialog.FileName))
                    {
                        using (StreamWriter outputFile = new StreamWriter(saveFileDialog.FileName))
                        {
                            foreach (IShape shape in Shapes)
                                outputFile.WriteLine(shape.ToString());
                        }
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Error saving the vector data: " + exception.Message);
                }
            }
        }

        private void loadFileToolStripMenuItem_Click(object sender, EventArgs e) // load
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Txt File|*.txt|Vector Image|*.vec|Any format|*.*";
            openFileDialog.Title = "Select an Image File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Shapes.Clear();
                    foreach (string line in File.ReadLines(openFileDialog.FileName))
                    {
                        string[] elements = line.Split(';');
                        switch (elements[0][0])
                        {
                            case 'L':
                                Shapes.Add(new Line(elements[1]));
                                break;
                            case 'T':
                                Shapes.Add(new ThickLine(elements[1]));
                                break;
                            case 'C':
                                Shapes.Add(new Circle(elements[1]));
                                break;
                            case 'P':
                                Shapes.Add(new Polygon(elements[1]));
                                break;
                            case 'A':
                                Shapes.Add(new AALine(elements[1]));
                                Shapes.Last().AA = AACheckBox.Checked;
                                break;
                        }
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Error loading the vector data: " + exception.Message);
                    return;
                }
            }
            DrawShapes();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shapes.Clear();
            Image = new Bitmap(pictureBox.Width, pictureBox.Height);
            pictureBox.Image = Image;
            prevPoints.Clear();
            DrawShapes();
        }

        private void exportImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
            saveFileDialog.Title = "Save Image As";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Image.Save(saveFileDialog.FileName);
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Error exporting the image: " + exception.Message);
                }
            }
        }

        private void pictureBoxSingleClick(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
            int dx, dy;
            if (prevPoints.Count == 0 && comboBox1.SelectedIndex < 5)
            {
                prevPoints.Add(new Point(x, y));
                label1.Text = "Select the second point.";
            }
            else
            {
                //check chosen mode
                switch (comboBox1.SelectedIndex)
                {
                    case 0: // thin line
                        AddShape(new Line((Point)prevPoints[0], new Point(x, y), currentColor));
                        prevPoints.Clear();
                        label1.Text = "Select the first point.";
                        break;
                    case 1: // thick line
                        AddShape(new ThickLine((Point)prevPoints[0], new Point(x, y), (int)numericLineWidth.Value, currentColor));
                        prevPoints.Clear();
                        label1.Text = "Select the first point.";
                        break;
                    case 2: // circle
                        Point center = (Point)prevPoints[0];
                        dx = x - center.X;
                        dy = y - center.Y;
                        int radius = (int)Math.Sqrt((double)(dx * dx + dy * dy));
                        AddShape(new Circle((Point)prevPoints[0], radius, currentColor));
                        prevPoints.Clear();
                        label1.Text = "Select the first point.";
                        break;
                    case 3: // polygon
                        prevPoints.Add(new Point(x, y));
                        if (prevPoints.Count == 2)
                        {
                            AddShape(new Polygon(currentColor, new List<Point>(prevPoints), (int)numericLineWidth.Value, false, AACheckBox.Checked));
                            break;
                        }
                        dx = x - prevPoints[0].X;
                        dy = y - prevPoints[0].Y;
                        if ((dx * dx) + (dy * dy) < CLICK_DISTANCE)
                        {

                            ((Polygon)Shapes.Last()).Closed = true;
                            prevPoints.Clear();
                            label1.Text = "Select the first point.";
                            break;
                        }
                        else
                        {
                            ((Polygon)Shapes.Last()).Add(new Point(x, y));
                        }
                        break;
                    case 4:
                        AddShape(new AALine((Point)prevPoints[0], new Point(x, y), (int)numericLineWidth.Value, currentColor, AACheckBox.Checked));
                        prevPoints.Clear();
                        label1.Text = "Select the first point.";
                        break;
                    case 6: // change color
                        foreach (IShape shape in Shapes)
                        {
                            foreach (Vertex vertex in shape.GetVertices())
                            {
                                dx = vertex.Point.X - x;
                                dy = vertex.Point.Y - y;
                                if (dx * dx + dy * dy < CLICK_DISTANCE)
                                {
                                    vertex.Owner.color = currentColor;
                                    break;
                                }
                            }
                        }
                        break;
                    case 7: // change width
                        foreach (IShape shape in Shapes)
                        {
                            foreach (Vertex vertex in shape.GetVertices())
                            {
                                dx = vertex.Point.X - x;
                                dy = vertex.Point.Y - y;
                                if (dx * dx + dy * dy < CLICK_DISTANCE)
                                {
                                    vertex.Owner.width = (int)numericLineWidth.Value;
                                    break;
                                }
                            }
                        }
                        break;
                    case 8: // delete
                        foreach (IShape shape in Shapes)
                        {
                            foreach (Vertex vertex in shape.GetVertices())
                            {
                                dx = vertex.Point.X - x;
                                dy = vertex.Point.Y - y;
                                if (dx * dx + dy * dy < CLICK_DISTANCE)
                                {
                                    Shapes.Remove(vertex.Owner);
                                    break;
                                }
                            }
                        }
                        break;
                    case 9:
                        if (prevPoints.Count == 2)
                        {
                            Shapes.Add(new PacMan(prevPoints[0], prevPoints[1], new Point(x, y), currentColor));
                            prevPoints.Clear();
                            label1.Text = "Select the first point.";
                            break;
                        }
                        if (prevPoints.Count == 0)
                            label1.Text = "Select the second point.";
                        if (prevPoints.Count == 1)
                            label1.Text = "Select the third point.";
                        prevPoints.Add(new Point(x, y));
                        break;
                }
                DrawShapes();
            }
        }

        private void pickColorButton_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                currentColor = colorDialog1.Color;
                panel1.BackColor = currentColor;
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            pickColorButton_Click(sender, e);
        }
        private void AddShape(IShape shape)
        {
            Shapes.Add(shape);
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            prevClick = DateTime.Now;
            int x = e.X;
            int y = e.Y;
            prevMousePosition = new Point(x, y);
            int dx, dy;
            foreach (IShape shape in Shapes)
            {
                foreach (Vertex vertex in shape.GetVertices())
                {
                    dx = vertex.Point.X - x;
                    dy = vertex.Point.Y - y;
                    if (dx * dx + dy * dy < CLICK_DISTANCE)
                    {
                        activeVertex = vertex;
                        break;
                    }
                }
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - prevClick;
            if (elapsed.TotalMilliseconds < 250)
            {
                pictureBoxSingleClick(sender, e);
            }
            activeVertex = null;
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - prevClick;
            if (activeVertex == null || elapsed.TotalMilliseconds < 250)
                return;
            int x = e.X;
            int y = e.Y;
            int radius, dx, dy;
            if (comboBox1.SelectedIndex == 5)
            {
                dx = x - activeVertex.Point.X;
                dy = y - activeVertex.Point.Y;
                activeVertex.Owner.Move(dx, dy);
            }
            else
            {
                switch (activeVertex.Type)
                {
                    case Vertex.VertexType.Normal:
                        activeVertex.Point.X = x;
                        activeVertex.Point.Y = y;
                        break;
                    case Vertex.VertexType.Circumference:
                        dx = ((Circle)activeVertex.Owner).center.X - x;
                        dy = ((Circle)activeVertex.Owner).center.Y - y;
                        radius = (int)Math.Sqrt((double)(dx * dx + dy * dy));
                        ((Circle)activeVertex.Owner).radius = radius;
                        List<Vertex> toDelete = new List<Vertex>();
                        break;
                    case Vertex.VertexType.Center:
                        dx = x - activeVertex.Point.X;
                        dy = y - activeVertex.Point.Y;
                        activeVertex.Owner.Move(dx, dy);
                        break;
                }
            }
            prevMousePosition = new Point(x, y);
            DrawShapes();
        }

        private void AACheckBox_CheckedChanged(object sender, EventArgs e)
        {
            foreach (IShape shape in Shapes)
            {
                shape.AA = AACheckBox.Checked;
            }
            DrawShapes();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings1.Default.Mode = comboBox1.SelectedIndex;
            Settings1.Default.Color = currentColor;
            Settings1.Default.Width = (int)numericLineWidth.Value;
            Settings1.Default.Save();
        }
    }
}
