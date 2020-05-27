using System;
using System.Drawing;
using System.Windows.Forms;

namespace kskr_teplo_stab
{
    public partial class Form2 : Form
    {
        private Graphics g;
        private DataGridView data;
        private Pen pen; // карандаш
        private int width = 0, height = 0;
        private double max_phi = 0, min_phi = 0; // максимальный и минимальный элемент матрицы dataGridView1
        private Color[] array_color;
        private double steps_phi;
        private double lenght_phi = 0;


        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Image bmp = pictureBox1.Image;
            g = Graphics.FromImage(bmp);
            pen = new Pen(Color.Black, 1); // перо
            width = Convert.ToInt32(930 / data.ColumnCount);
            height = Convert.ToInt32(480 / data.RowCount);
            min_phi = FindMin(data);
            max_phi = FindMax(data);
            lenght_phi = max_phi - min_phi;
            steps_phi = lenght_phi / 10;
            array_color = new Color[10];
            FillColors(10);
            DrawSurface();

        }

        private double FindMax(DataGridView data)
        {
            max_phi = Convert.ToDouble(data.Rows[0].Cells[0].Value);
            for (int i = 0; i < data.RowCount; i++)
            {
                for (int j = 0; j < data.ColumnCount; j++)
                {
                    if (max_phi < Convert.ToDouble(data.Rows[i].Cells[j].Value))
                    {
                        max_phi = Convert.ToDouble(data.Rows[i].Cells[j].Value);
                    }
                }
            }
            return max_phi;
        }

        private double FindMin(DataGridView data)
        {
            min_phi = Convert.ToDouble(data.Rows[0].Cells[0].Value);
            for (int i = 0; i < data.RowCount; i++)
            {
                for (int j = 0; j < data.ColumnCount; j++)
                {
                    if (min_phi > Convert.ToDouble(data.Rows[i].Cells[j].Value))
                    {
                        min_phi = Convert.ToDouble(data.Rows[i].Cells[j].Value);
                    }
                }
            }
            return min_phi;
        }

        public void SetData(DataGridView s)
        {
            data = s;
        }

        public void DrawSurface()
        {
            for (int i = 0; i < data.ColumnCount; i++)
            {
                for (int j = 0; j < data.RowCount; j++)
                {
                    double value = Convert.ToDouble(data.Rows[j].Cells[i].Value);
                    int val = Convert.ToInt32((max_phi-value)/steps_phi)-1;
                    if (val < 0)
                    { val = 0; }

                    g.FillRectangle(new SolidBrush(array_color[val]),
                        (i * width), (j * height), width, height);
                }
            }
            pictureBox1.Refresh();
        }
        private void FillColors(int step)
        {
            double iter_step = step / 5;
            double color_step = 255 / iter_step;
            int a = 0, b = 1, c = 1, d = 1, e = 1;
            for (int i = 0; i < step; i++)
            {
                if (i <= iter_step)
                {
                    array_color[i] = Color.FromArgb(255, Convert.ToInt32(a * color_step), 0);
                    a++;
                }
                if (i > iter_step && i <= Convert.ToInt32(iter_step * 2))
                {
                    array_color[i] = Color.FromArgb(255 - Convert.ToInt32(b * color_step), 255, 0);
                    b++;
                }
                if (i > iter_step * 2 && i <= Convert.ToInt32(iter_step * 3))
                {
                    array_color[i] = Color.FromArgb(0, 255, Convert.ToInt32(c * color_step));
                    c++;
                }
                if (i > iter_step * 3 && i <= Convert.ToInt32(iter_step * 4))
                {
                    array_color[i] = Color.FromArgb(0, 255 - Convert.ToInt32(d * color_step), 255);
                    d++;
                }
                if (i > iter_step * 4 && i <= Convert.ToInt32(iter_step * 5))
                {
                    array_color[i] = Color.FromArgb(Convert.ToInt32(e * color_step), 0, 255);
                    e++;
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            steps_phi = lenght_phi / (Convert.ToInt32(comboBox1.SelectedItem));
            array_color = new Color[Convert.ToInt32(comboBox1.SelectedItem)];
            FillColors(Convert.ToInt32(comboBox1.SelectedItem));
            DrawSurface();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveToFile();
        }

        public void SaveToFile()
        {
            Bitmap bitmap_for_save = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.DrawToBitmap(bitmap_for_save, pictureBox1.ClientRectangle);
            bitmap_for_save.Save(@"D:\123.bmp");


        }
    }
}
/*
           //создание диалогового окна "Сохранить как..", для сохранения изображения
           SaveFileDialog dialog_save_bitmap = new SaveFileDialog();
           dialog_save_bitmap.Title = "Сохранить картинку как...";
           //отображать ли предупреждение, если пользователь указывает имя уже существующего файла
           dialog_save_bitmap.OverwritePrompt = true;
           //отображать ли предупреждение, если пользователь указывает несуществующий путь
           dialog_save_bitmap.CheckPathExists = true;
           //список форматов файла, отображаемый в поле "Тип файла"
           dialog_save_bitmap.Filter = "Image Files(*.BMP)|*.BMP";
           //отображается ли кнопка "Справка" в диалоговом окне
           dialog_save_bitmap.ShowHelp = true;
           //
           if (dialog_save_bitmap.ShowDialog() == DialogResult.OK) //если в диалоговом окне нажата кнопка "ОК"
           {
               try
               {
                   //pictureBox1.BackgroundImage.Save(dialog_save_bitmap.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                   //pictureBox1.Image.Save(dialog_save_bitmap.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                   //pictureBox1.BackColor
                   //pictureBox1.BackgroundImage
               }
               catch
               {
                   MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
               }
           }*/
/////// КНОПКА "СОХРАНИТЬ"
//Bitmap savedBit = new Bitmap(pictureBox1.Width, pictureBox1.Height);
//pictureBox1.DrawToBitmap(savedBit, pictureBox1.ClientRectangle);
//savedBit.Save(@"D:\123.bmp");
//Bitmap bitmap_for_save = new Bitmap(pictureBox1.Width, pictureBox1.Height);

//pictureBox1.DrawToBitmap(bitmap_for_save, pictureBox1.ClientRectangle);
//
/*
//создание диалогового окна "Сохранить как..", для сохранения изображения
SaveFileDialog dialog_save_bitmap = new SaveFileDialog();
dialog_save_bitmap.Title = "Сохранить картинку как...";
//отображать ли предупреждение, если пользователь указывает имя уже существующего файла
dialog_save_bitmap.OverwritePrompt = true;
//отображать ли предупреждение, если пользователь указывает несуществующий путь
dialog_save_bitmap.CheckPathExists = true;
//список форматов файла, отображаемый в поле "Тип файла"
dialog_save_bitmap.Filter = "Image Files(*.BMP)|*.BMP";
//отображается ли кнопка "Справка" в диалоговом окне
dialog_save_bitmap.ShowHelp = true;
//
if (dialog_save_bitmap.ShowDialog() == DialogResult.OK) //если в диалоговом окне нажата кнопка "ОК"
{
    try
    {
        //pictureBox1.BackgroundImage.Save(dialog_save_bitmap.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
        //pictureBox1.Image.Save(dialog_save_bitmap.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
        //pictureBox1.BackColor
        //pictureBox1.BackgroundImage
    }
    catch
    {
        MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
        MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}

//bitmap_for_save = new Bitmap(pictureBox1.Width, pictureBox1.Height);
//pictureBox1.Image = bitmap_for_save;
//bmp_image = pictureBox1.Image;
//g = Graphics.FromImage(bmp_image);
//g.DrawLine(new Pen(Brushes.Black, 2f), 0, 20, 100, 20);
*/
