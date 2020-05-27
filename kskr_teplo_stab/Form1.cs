using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace kskr_teplo_stab
{
    public partial class Form1 : Form
    {
        double cells_x, cells_y;
        double step_x, step_y;
        double[,] array_value;
        double[,] array_index;
        double[] vector_right_side;
        double q, k;
        int size;
        MathParser parser = new MathParser();
        string str_parse1;
        string str_parse2;
        string str_parse3;
        string str_parse4;
        //
        double[] x;
        double[] y;
        //
        private Boolean try_parse = false;
        private Boolean try_write_xy = false;
        private Boolean try_write_step = false;
        private Boolean try_phi = false;
        string path_array_index = @"..\..\..\array_index.txt";
        string path_right_side = @"..\..\..\right_side.txt";
        string path_roots = @"..\..\..\roots.txt";
        string path_excel = Directory.GetCurrentDirectory() + @"\" + "Save_Excel.xlsx";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
        public void FillArrayIndexWithNulls()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    array_index[i, j] = 0;
                }
            }
        }

        public void FillArrayIndexAndRightSide()
        {
            int col = 0, row = 0;
            for (int i = 0; i < size; i++)
            {
                double sum = 0;
                array_index[i, i] = -1 * (-2 / Math.Pow(step_x, 2) - 2 / Math.Pow(step_y, 2));
                try { array_index[i, i + 1] = -1 / Math.Pow(step_x, 2); if (col == dataGridView1.ColumnCount - 3) { sum += Convert.ToDouble(dataGridView1.Rows[row + 1].Cells[col + 2].Value) / Math.Pow(step_x, 2); array_index[i, i + 1] = 0; } } catch { sum += Convert.ToDouble(dataGridView1.Rows[row + 1].Cells[col + 2].Value) / Math.Pow(step_x, 2); try { array_index[i, i + 1] = 0; } catch { } }
                try { array_index[i, i - 1] = -1 / Math.Pow(step_x, 2); if (col == 0) { sum += Convert.ToDouble(dataGridView1.Rows[row + 1].Cells[col].Value) / Math.Pow(step_x, 2); array_index[i, i - 1] = 0; } } catch { sum += Convert.ToDouble(dataGridView1.Rows[row + 1].Cells[col].Value) / Math.Pow(step_x, 2); try { array_index[i, i - 1] = 0; } catch { } }
                try { array_index[i, i + dataGridView1.ColumnCount - 2] = -1 / Math.Pow(step_y, 2); } catch { sum += Convert.ToDouble(dataGridView1.Rows[row + 2].Cells[col + 1].Value) / Math.Pow(step_y, 2); try { array_index[i, i + dataGridView1.ColumnCount] = 0; } catch { } }
                try { array_index[i, i - dataGridView1.ColumnCount + 2] = -1 / Math.Pow(step_y, 2); } catch { sum += Convert.ToDouble(dataGridView1.Rows[row].Cells[col + 1].Value) / Math.Pow(step_y, 2); try { array_index[i, i - dataGridView1.ColumnCount] = 0; } catch { } }
                //
                col++;
                //
                vector_right_side[i] = (sum+q)/k;
                //
                if (col == dataGridView1.ColumnCount - 2)
                {
                    col = 0;
                    row++;
                }
            }
        }
        private void SaveResultsInFile(string path_array_index, string path_right_side, double[,] array_index, double[] vector_right_side)
        {
            // Открываем файл
            FileStream fs_a = new FileStream(path_array_index, FileMode.Create);
            FileStream fs_r = new FileStream(path_right_side, FileMode.Create);
            // Открываем поток для записи
            StreamWriter sw_a = new StreamWriter(fs_a);
            StreamWriter sw_r = new StreamWriter(fs_r);
            // Запись в файл ArrayIndex                        
            try
            {
                sw_a.WriteLine("Матрица Index");
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (j != (size - 1))
                            sw_a.Write(array_index[i, j] + "   ");
                        else
                            sw_a.Write(array_index[i, j] + "\n");
                    }
                }
                // Закрываем потоки
                sw_a.Close();
                //
                MessageBox.Show("Файл \"ArrayIndex\" успешно сохранен!", "Сохранение в файл", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Ошибка при сохранении файла \"ArrayIndex\"!", "Сохранение в файл", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            //
            // Запись в файл ArrayIndex   
            try
            {
                for (int i = 0; i < size; i++)
                {
                    sw_r.WriteLine(vector_right_side[i]);
                }
                // Закрываем потоки
                sw_r.Close();
                //
                MessageBox.Show("Файл \"RightSide\" успешно сохранен!", "Сохранение в файл", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Ошибка при сохранении файла \"RightSide\"!", "Сохранение в файл", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        public double rep(double p, string str)
        {
            return parser.Parse(str.Replace("x", Convert.ToString(p).Replace(".", ",")), true);
        }
        public double f1(double y) // left
        {
            return rep(y, str_parse1);
        }

        public double f2(double y) // right
        {
            return rep(y, str_parse2);
        }

        public double f3(double x) // bottom
        {
            return rep(x, str_parse3);
        }

        public double f4(double x) // top
        {
            return rep(x, str_parse4);
        }
        public void Fill_Left()
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                dataGridView1.Rows[dataGridView1.RowCount-1-i].Cells[0].Value = f1(y[i]);
            }
        }

        public void Fill_Right()
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                dataGridView1.Rows[dataGridView1.RowCount - 1-i].Cells[dataGridView1.ColumnCount - 1].Value = f2(y[i]);
            }
        }

        public void Fill_Bottom()
        {
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[i].Value = f3(x[i]);
            }
        }

        public void Fill_Top()
        {
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                dataGridView1.Rows[0].Cells[i].Value = f4(x[i]);
            }
        }

        public void ShitCode(int size, double[,] matrix, double[] vector)
        {

            double[] xxx = new double[size];    // начальное приближение
            double[] xxnn = new double[size];
            double[] proverka = new double[size];
            double eps = 0.001;
            int count = 0;
            //
            for (int i = 0; i < size; i++) // проверка на диагональное преобладание
            {
                double SumStr = 0;
                for (int j = 0; j < size; j++)
                {
                    SumStr += Math.Abs(matrix[i, j]);
                }
                SumStr -= matrix[i, i];
                proverka[i] = Math.Abs(SumStr);
            }
            do
            {
                int schet = 0;
                //
                for (int q = 0; q < size; q++)
                {
                    double SumPrStroki = 0;
                    for (int w = 0; w < size; w++)
                    {
                        SumPrStroki += (matrix[q, w] * xxx[w]);
                    }
                    SumPrStroki -= (matrix[q, q] * xxx[q]);
                    SumPrStroki = (vector[q] - SumPrStroki) / matrix[q, q];
                    xxnn[q] = SumPrStroki;
                }
                count++;
                for (int ii = 0; ii < size; ii++)
                {
                    if (Math.Abs(xxnn[ii] - xxx[ii]) < eps)
                    { schet++; }
                }
                if (schet == size) { goto LoopEnd; }
                for (int i = 0; i < size; i++)
                {
                    xxx[i] = xxnn[i];
                }
            }
            while (true);
            LoopEnd:
            {
                for (int i = 0; i < size; i++)
                {
                    xxx[i] = xxnn[i];
                }
                int counter = 0;
                for (int i = 0; i < dataGridView1.RowCount - 2; i++)
                {
                    for (int j = 0; j < dataGridView1.ColumnCount - 2; j++)
                    {
                        dataGridView1.Rows[i + 1].Cells[j + 1].Value = xxx[counter];
                        counter++;
                    }
                }
                MessageBox.Show("" + count, "Количество итераций 😁");
                SaveRoots(path_roots, xxx);
            }              
        }

        public void SaveRoots(string path, double[] vector)
        {
            // Открываем файл
            FileStream fs = new FileStream(path_roots, FileMode.Create);
            // Открываем поток для записи
            StreamWriter sw = new StreamWriter(fs);
            // Запись в файл ArrayIndex                        
            try
            {
                sw.WriteLine("Матрица Roots");
                for (int i = 0; i < size; i++)
                {
                    sw.WriteLine(vector[i]);
                }
                // Закрываем потоки
                sw.Close();
                //
                MessageBox.Show("Файл \"Roots\" успешно сохранен!", "Сохранение в файл", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Ошибка при сохранении файла \"Roots\"!", "Сохранение в файл", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Rows.Clear();
                try
                {
                    cells_x = Convert.ToDouble(textBox1.Text);
                    cells_y = Convert.ToDouble(textBox2.Text);
                    try_write_xy = true;
                }
                catch { try_write_xy = false; }
                try
                {
                    step_x = Convert.ToDouble(textBox3.Text);
                    step_y = Convert.ToDouble(textBox4.Text);
                    try_write_step = true;
                }
                catch
                {
                    try_write_step = false;
                }
                str_parse1 = textBox5.Text;
                str_parse2 = textBox6.Text;
                str_parse3 = textBox7.Text;
                str_parse4 = textBox8.Text;
                q = Convert.ToDouble(textBox10.Text);
                k = Convert.ToDouble(textBox9.Text);
                dataGridView1.ColumnCount = Convert.ToInt32(cells_x / step_x) + 1;
                dataGridView1.RowCount = Convert.ToInt32(cells_y / step_y) + 1;
                size = (dataGridView1.ColumnCount - 2) * (dataGridView1.RowCount - 2);
                x = new double[dataGridView1.ColumnCount];
                y = new double[dataGridView1.RowCount];
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    x[i] = (i * step_x);
                }
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    y[i] = (i * step_y);
                }

                string[] prow = new string[] { "первой", "второй", "третьей", "четвёртой" };
                int counter_prow = 0;
                try
                {
                    Fill_Left();
                    counter_prow++;
                    Fill_Right();
                    counter_prow++;
                    Fill_Bottom();
                    counter_prow++;
                    Fill_Top();
                    try_parse = true;
                }
                catch
                {
                    MessageBox.Show("Ошибка в воде " + prow[counter_prow] + " функции");
                    try_parse = false; goto end;
                }
                //
                array_value = new double[dataGridView1.ColumnCount - 2, dataGridView1.RowCount - 2];
                array_index = new double[size, size];
                vector_right_side = new double[size];

                FillArrayIndexWithNulls();
                FillArrayIndexAndRightSide();
                SaveResultsInFile(path_array_index, path_right_side, array_index, vector_right_side);
                end: { }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка! " + ex.Message); }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (try_parse && try_phi && try_write_step && try_write_xy)
            {
                Form2 f = new Form2();
                f.SetData(dataGridView1);
                f.ShowDialog();
            }
            else
            {
                string message_er = "";
                if (!try_parse) { message_er += " парсинг "; }
                if (!try_phi) { message_er += " фи "; }
                if (!try_write_step) { message_er += " шаг "; }
                if (!try_write_xy) { message_er += " ХУ "; }

                MessageBox.Show("Ошибка в: " + message_er);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                ShitCode(size, array_index, vector_right_side);
                try_phi = true;
            }
            catch { MessageBox.Show("Сначала нажмите кнопку 'Вычислить'!"); try_phi = false; }
        }

        public void SaveRootsToExcel(string path, double[] roots)//сохранение ответов в файл
        {
            Excel.Application ObjExcel = new Excel.Application();
            Excel.Workbook ObjWorkBook;
            Excel.Worksheet ObjWorkSheet;            
            ObjWorkBook = ObjExcel.Workbooks.Add(System.Reflection.Missing.Value);  
            ObjWorkBook = ObjExcel.Workbooks.Add(path);
            //Таблица.
            ObjWorkSheet = (Excel.Worksheet)ObjWorkBook.Sheets[1];
            int count = 0;
            for (int i = 0; i < dataGridView1.RowCount - 2; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount - 2; j++)
                {
                    ObjWorkSheet.Cells[i + 1, j + 1] = roots[count];
                    count++;
                }
            }
            ObjExcel.Visible = true;
            ObjExcel.UserControl = true;
            ObjExcel.AlertBeforeOverwriting = false;
            ObjWorkBook.SaveAs(path);
            ObjExcel.Quit();
        }     
    }
}