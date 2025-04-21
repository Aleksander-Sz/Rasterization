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
            Image = null;
            Shapes = new List<IShape>();
            Shapes.Add(new Line(new Point(5, 10), new Point(14, 17), Color.FromArgb(255, 255, 255, 255)));
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
                    MessageBox.Show("Error saving the image: " + exception.Message);
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
                                Point a = new Point(Int32.Parse(elements[1]), Int32.Parse(elements[2]));
                                Point b = new Point(Int32.Parse(elements[3]), Int32.Parse(elements[4]));
                                Color color = Color.FromArgb(Convert.ToInt32(elements[5],16));
                                Shapes.Add(new Line(a, b, color));
                                break;
                        }
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Error loading the image: " + exception.Message);
                }
            }
        }
    }
}
