using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Fractals.Sources;

namespace  Fractals
{public partial class FractalMenu : Form
    {
        private readonly Color _backGround = Color.FromArgb(64, 64, 64);

        private readonly FractalStrategy _fs = FractalStrategy.GetInstance();

        private int _maxWidth;

        private Pen _pen;

        private FormData Data;

        private readonly Dictionary<string, GroupBox> _groups = new Dictionary<string, GroupBox>();


        public FractalMenu()
        {
            InitializeComponent();
            InitComponents();

            treeGroup.Hide();

            MinimumSize = new Size(1000, MaximumSize.Height);
            pictureBox1.MinimumSize = new Size(600, 980);
            InvokeEvents();
        }

        private int CurrentWidth => splitContainer1.Panel2.Width;

        /// <summary>
        /// Inner initiations of all data needed for the form.
        /// </summary>
        private void InitComponents()
        {
            _pen = new Pen(Color.White, 4);
            FormClosing += Close;

            Height = Screen.PrimaryScreen.Bounds.Height;
            Width = Screen.PrimaryScreen.Bounds.Width;
            MinimumSize = new Size(Height / 2, 1000);

            fractalType.Items.AddRange(new Fractal[]
            {
                new TreeFractal("Фрактальное дерево"),
                new KohFractal("Кривая Коха"),
                new SierpinskiCarpetFractal("Ковёр Серпинского"),
                new SierpinskiTriangleFractal("Треугольник Серпинского"),
                new CantorSetFactorial("Множество Кантора")
            });

            Data = new FormData();

            _groups.Add("Фрактальное дерево", treeGroup);
            _groups.Add("Кривая Коха", kohGroup);
            _groups.Add("Ковёр Серпинского", carpSerpGroup);
            _groups.Add("Треугольник Серпинского", triangleGroup);
            _groups.Add("Множество Кантора", cantorGroup);

            _fs.ChangeGroup(_groups);

            firstColorDialog.Color = Color.White;
            secondColorDialog.Color = Color.White;

            _maxWidth = splitContainer1.Panel2.Width;
            _fs.SetPos(fractalType.Location);
            splitContainer1.SplitterDistance = (int)(this.Width * 0.2);
        }

        /// <summary>
        /// Method adds listeners to all needed events.
        /// </summary>
        private void InvokeEvents()
        {
            // TrackBar updates.
            leftAngleBar.ValueChanged += DataUpdate;
            rightAngleBar.ValueChanged += DataUpdate;
            decreaseBar.ValueChanged += DataUpdate;
            lengthBar.ValueChanged += DataUpdate;
            curveBar.ValueChanged += DataUpdate;
            gradBar.ValueChanged += DataUpdate;
            triangleBar.ValueChanged += DataUpdate;
            cantorWidthBar.ValueChanged += DataUpdate;
            cantorMaxBar.ValueChanged += DataUpdate;
            cantorSpaceBar.ValueChanged += DataUpdate;
            // Resize update.
            ResizeEnd += DataUpdate;

            // Color Dialog updates.
            firstColorButton.Click += ChooseFirstColor;
            firstColorButton.Click += DataUpdate;
            secondColorButton.Click += ChooseSecondColor;
            secondColorButton.Click += DataUpdate;

            // Redraw updates.
            gradBar.ValueChanged += Draw;
            triangleBar.ValueChanged += Draw;
            curveBar.ValueChanged += Draw;
            decreaseBar.ValueChanged += Draw;
            cantorMaxBar.ValueChanged += Draw;
            ResizeEnd += Draw;
        }

        /// <summary>
        /// Method updates data in data struct.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataUpdate(object sender, EventArgs e)
        {
            Data.resise = (double) CurrentWidth / _maxWidth;
            Data.decrease = decreaseBar.Value;
            Data.angleLeft = leftAngleBar.Value;
            Data.angleRight = rightAngleBar.Value;
            Data.length = lengthBar.Value;
            Data.startColor = firstColorDialog.Color;
            Data.finishColor = secondColorDialog.Color;
            Data.curveMax = curveBar.Value;
            Data.gradLevel = gradBar.Value;
            Data.cantorSpace = cantorSpaceBar.Value;
            Data.triaLevel = triangleBar.Value;
            Data.cantorWidth = cantorWidthBar.Value;
            Data.cantorMax = cantorMaxBar.Value;
            _fs.UpdateFractal(ref Data);
        }

        private void Close(object sender, FormClosingEventArgs e)
        {
            Owner.Show();
        }

        /// <summary>
        /// Method changes current fractal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cmb = (ComboBox) sender;
            _fs.SetFractal((Fractal) cmb.SelectedItem);
            DataUpdate(sender, e);

            _fs.ChangeGroup(_groups);
        }

        
        /// <summary>
        /// Method draws current fractal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Draw(object sender, EventArgs e)
        {
            var bit = new Bitmap(splitContainer1.Panel2.Size.Width, splitContainer1.Panel2.Size.Height);
            pictureBox1.Image = bit;
            var g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(_backGround);
            
            try
            {
                _fs.DrawFractal(g, _pen, new Point(splitContainer1.Panel2.Size.Width / 2,
                    splitContainer1.Panel2.Size.Height - 100));

                // Zoom
                bit = new Bitmap(pictureBox1.Image, Convert.ToInt32(pictureBox1.Image.Width * zoomBar.Value),
                    Convert.ToInt32(pictureBox1.Image.Height * zoomBar.Value));
                g = Graphics.FromImage(bit);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                pictureBox1.Image = bit;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Ошибка", MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// Method calls first color dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseFirstColor(object sender, EventArgs e)
        {
            firstColorDialog.ShowDialog();
            if (firstColorDialog.Color != Color.Empty)
                firstColorButton.BackColor = firstColorDialog.Color;
        }

        /// <summary>
        /// Method calls second color dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseSecondColor(object sender, EventArgs e)
        {
            secondColorDialog.ShowDialog();
            if (secondColorDialog.Color != Color.Empty)
                secondColorButton.BackColor = secondColorDialog.Color;
        }

        #region USELESS_I_DONT_KNOW_HOW_TO_REMOVE

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
        }
        #endregion

        /// <summary>
        /// Method saves image to jpg file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (_fs.CurrentFractal != null && pictureBox1.Image != null)
            {
                saveImageDialog.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp";
                saveImageDialog.Title = "Save an Image File";
                saveImageDialog.ShowDialog();
                if (saveImageDialog.FileName != string.Empty)
                    try
                    {
                        pictureBox1.Image.Save(saveImageDialog.FileName);
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);
                    }
            }
            else
            {
                MessageBox.Show("Изображение пустое. Выберите фрактал и нажмите нарисовать");
            }
        }

        /// <summary>
        /// Method closes the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Struct saves all needed data.
        /// </summary>
        public struct FormData
        {
            public int decrease;
            public int angleLeft;
            public int angleRight;
            public int length;
            public int curveMax;
            public int gradLevel;
            public int cantorWidth;
            public double resise;
            public int triaLevel;
            public int cantorMax;
            public int cantorSpace;
            public Color startColor;
            public Color finishColor;

        }

        private void firstColorButton_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }
    }
}