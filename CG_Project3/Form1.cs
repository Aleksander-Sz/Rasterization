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
            comboBox1.SelectedIndex = 0;
            label1.Text = "Select the first point.";
            currentColor = Color.White;
            panel1.BackColor = currentColor;
            Shapes = new List<IShape>();
            Shapes.Add(new Line(new Point(400, 200), new Point(100, 50), Color.FromArgb(255, 0, 255, 0)));
            Shapes.Add(new Line(new Point(600, 300), new Point(400, 50), Color.FromArgb(255, 255, 0, 0)));
            Shapes.Add(new ThickLine(new Point(0, 0), new Point(400, 50), 5, Color.FromArgb(255, 0, 0, 100)));
            Shapes.Add(new ThickLine(new Point(100, 100), new Point(100, 100), 15, Color.FromArgb(255, 0, 0, 100)));
            Shapes.Add(new Circle(new Point(500, 250), 30, Color.FromArgb(255, 100, 100, 100)));
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

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
            int dx, dy;
            //check chosen mode
            if (prevPoints.Count == 0)
            {
                prevPoints.Add(new Point(x, y));
                label1.Text = "Select the second point.";
            }
            else
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 0: // thin line
                        Shapes.Add(new Line((Point)prevPoints[0], new Point(x, y), currentColor));
                        prevPoints.Clear();
                        label1.Text = "Select the first point.";
                        break;
                    case 1: // thick line
                        Shapes.Add(new ThickLine((Point)prevPoints[0], new Point(x, y), (int)numericLineWidth.Value, currentColor));
                        prevPoints.Clear();
                        label1.Text = "Select the first point.";
                        break;
                    case 2: // circle
                        Point center = (Point)prevPoints[0];
                        dx = x - center.X;
                        dy = y - center.Y;
                        int radius = (int)Math.Sqrt((double)(dx * dx + dy * dy));
                        Shapes.Add(new Circle((Point)prevPoints[0], radius, currentColor));
                        prevPoints.Clear();
                        break;
                    case 3: // polygon
                        prevPoints.Add(new Point(x, y));
                        if(prevPoints.Count == 2)
                        {
                            Shapes.Add(new Polygon(currentColor, prevPoints, (int)numericLineWidth.Value));
                        }
                        dx = x - prevPoints[0].X;
                        dy = y - prevPoints[0].Y;
                        if ((dx * dx) + (dy * dy) < 100)
                        {

                            ((Polygon)Shapes[Shapes.Count - 1]).Closed = true;
                        }
                        else
                            return;
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
    }
}
