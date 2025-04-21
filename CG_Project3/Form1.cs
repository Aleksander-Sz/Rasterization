using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace CG_Project3
{
    public partial class Form1 : Form
    {
        Bitmap? Image;
        List<IShape> Shapes;
        public Form1()
        {
            InitializeComponent();
            Image = new Bitmap(pictureBox.Width, pictureBox.Height);
            pictureBox.Image = Image;
            Shapes = new List<IShape>();
            Shapes.Add(new Line(new Point(400, 200), new Point(100, 50), Color.FromArgb(255, 255, 255, 255)));
            DrawShapes();
        }

        public void DrawShapes()
        {
            int stride;
            byte[] ImageBytes = Program.ImageToByteArray(Image, out stride);
            foreach (IShape shape in Shapes)
            {
                shape.Draw(ImageBytes, stride);
            }
            int width = Image.Width;
            int height = Image.Height;
            Image.Dispose();
            Image = Program.ByteArrayToImage(ImageBytes, width, height, stride);
            pictureBox.Image = Image;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void loadFileToolStripMenuItem_Click(object sender, EventArgs e)
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
                            case 'C':
                                Shapes.Add(new Circle(elements[1]));
                                break;
                        }
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Error loading the vector data: " + exception.Message);
                }
            }
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
    }
}
