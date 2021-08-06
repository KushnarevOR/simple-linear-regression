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

namespace Regression
{
    public partial class Form1 : Form
    {
        List<float> X_var;
        List<float> Y_var;
        List<float> Eq;
        List<float> F_tab;
        int obs_num;
        float X_dev;
        float Y_dev;
        float error;
        float F;
        float R2;
        float uh;//коэффициент эластичности
        float a;
        float b;
        float t_a;
        float t_b;
   
        public Form1()
        {
            InitializeComponent();
            X_var = new List<float>();
            Y_var = new List<float>();
            Eq = new List<float>();
            F_tab = new List<float>() { 161.45f, 199.50f, 215.71f, 224.58f, 230.16f, 233.99f, 236.77f, 238.88f, 240.54f, 241.88f, 245.95f };
        }

        public float calc_t_a(List<float> x_var, List<float> y_var, List<float> eq, float x_dev, float a)
        {
            float S = 0;
            float x2_sum = 0;
            for (int i = 0; i < y_var.Count(); i++)
            {
                S += (float)Math.Pow((y_var[i] - eq[i]), 2);
                x2_sum += (float)Math.Pow(x_var[i], 2);
            }
            S = (float)Math.Sqrt(S / y_var.Count()
                );

            float ma = (S * (float)Math.Sqrt(x2_sum)) / (y_var.Count() * x_dev);

            return a / ma;
        }

        public float calc_t_b(List<float> y_var, List<float> eq, float x_dev, float b)
        {
            float S = 0;
            for (int i = 0; i < y_var.Count(); i++)
            {
                S += (float)Math.Pow((eq[i] - y_var[i]), 2);
            }
            S = (float)Math.Sqrt(S / y_var.Count());

            float mb = S / (x_dev * (float)Math.Sqrt(y_var.Count()));

            return b / mb;
        }

        public List<float> calc_eq(List<float> x_var, float a, float b)
        {
            List<float> eq = new List<float>();
            foreach (var item in x_var)
                eq.Add(a + b * item);

            return eq;
        }

        public float calc_err(List<float> y_var, List<float> eq)
        {
            float sum = 0;
            for(int i = 0; i < y_var.Count(); i++)
            {
                sum += Math.Abs((y_var[i] - eq[i]) / y_var[i]) * 100;
            }

            return sum / y_var.Count();
        }

        public float calc_F(float x_dev, float y_dev, float b)
        {
            float r2 = (float)Math.Pow((b * x_dev / y_dev), 2);

            return (X_var.Count - 2) * r2 / (1 - r2);
        }

        public float calc_R(List<float> y_var, List<float> eq)
        {
            float sum_1 = 0;
            float sum_2 = 0;
            for(int i = 0; i < y_var.Count; i++)
            {
                sum_1 += (float)Math.Pow((eq[i] - y_var.Average()), 2);
                sum_2 += (float)Math.Pow((y_var[i] - y_var.Average()), 2);
            }

            return sum_1 / sum_2;
        }

        public float calc_uh(List<float> x_var, List<float> y_var, float b)
        {
            return b * x_var.Average() / y_var.Average();
        }

        public float calc_dev_X(List<float> x_var)
        {
            float sum = 0;
            for(int i = 0; i < x_var.Count; i++)
            {
                sum += (float)Math.Pow((x_var[i] - x_var.Average()), 2);
            }
            return (float)Math.Sqrt(sum / x_var.Count());
        }

        public float calc_dev_Y(List<float> y_var)
        {
            float sum = 0;
            for (int i = 0; i < y_var.Count; i++)
            {
                sum += (float)Math.Pow((y_var[i] - y_var.Average()), 2);
            }
            return (float)Math.Sqrt(sum / y_var.Count());
        }

        public float calc_b(List<float> x_var, List<float> y_var)
        {
            List<float> yx = new List<float>();
            List<float> xx = new List<float>();
            
            for(int i = 0; i < x_var.Count; i++)
                yx.Add(x_var[i] * y_var[i]);
            foreach (var item in x_var)
                xx.Add((float)Math.Pow(item, 2));

            return (yx.Average() - y_var.Average() * x_var.Average()) / (xx.Average() - (float)Math.Pow(x_var.Average(), 2));
        }

        public float calc_a(List<float> x_var, List<float> y_var, float b)
        {
            return (y_var.Average() - b * x_var.Average());
        }

        private void label1_TextChanged(object sender, EventArgs e)
        {
            if (label1.Text != "Имя текстового файла" && label2.Text != "Имя текстового файла")
                button3.Enabled = true;
        }

        private void label2_TextChanged(object sender, EventArgs e)
        {
            if (label1.Text != "Имя текстового файла" && label2.Text != "Имя текстового файла")
                button3.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string num;
            OpenFileDialog file = new OpenFileDialog();
            file.InitialDirectory = @"C:\Users\Olga\Desktop\Многомерный статистический анализ\лаб4";
            file.Filter = "txt files (*.txt)|*.txt";
            file.Title = "Factor";
            if(file.ShowDialog() == DialogResult.OK)
            {
                X_var.Clear();
                string filepath = file.FileName;
                label1.Text = filepath;
                using (StreamReader reader = new StreamReader(filepath))
                {
                    try
                    {
                        while ((num = reader.ReadLine()) != null)
                        {
                            X_var.Add(float.Parse(num));
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Something went wrong!");
                        X_var.Clear();
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string num;
            OpenFileDialog file = new OpenFileDialog();
            file.InitialDirectory = @"C:\Users\Olga\Desktop\Многомерный статистический анализ\лаб4";
            file.Filter = "txt files (*.txt)|*.txt";
            file.Title = "Result";
            if (file.ShowDialog() == DialogResult.OK)
            {
                Y_var.Clear();
                string filepath = file.FileName;
                label2.Text = filepath;
                using (StreamReader reader = new StreamReader(filepath))
                {
                    try
                    {
                        while ((num = reader.ReadLine()) != null)
                        {
                            Y_var.Add(float.Parse(num));
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Something went wrong!");
                        Y_var.Clear();
                    }
                }
            }
        }

        private void check_the_model(float R, float error, float F, float a, float b)
        {
            bool check = true;
            if (R >= 0.7 && error <= 10)
                textBox1.Text += "Модель высокого качества\r\n";
            else
            {
                textBox1.Text += "Модель низкого качества\r\n";
                check = false;
            }

            if (obs_num > F_tab.Count())
                obs_num = F_tab.Count();

            if (F >= F_tab[obs_num - 1])
                textBox1.Text += "Оцениваемые характеристики статистически значимы и надежны\r\n";
            else
            {
                textBox1.Text += "Оцениваемые характеристики ненадежны\r\n";
                check = false;
            }

            if (!check)
                textBox1.Text += "\r\nПо данной модели нельзя строить прогнозы!";
            else
            {
                float eq = a + b * (X_var.Average() * 1.1f);
                textBox1.Text += "\r\nПрогноз при увеличении X на 10%: Y = " + eq;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            b = calc_b(X_var, Y_var);
            a = calc_a(X_var, Y_var, b);
            Eq = calc_eq(X_var, a, b);
            error = calc_err(Y_var, Eq);
            X_dev = calc_dev_X(X_var);
            Y_dev = calc_dev_Y(Y_var);
            F = calc_F(X_dev, Y_dev, b);
            R2 = calc_R(Y_var, Eq);
            uh = calc_uh(X_var, Y_var, b);
            obs_num = X_var.Count();
            t_a = calc_t_a(X_var, Y_var, Eq, X_dev, a);
            t_b = calc_t_b(Y_var, Eq, X_dev, b);
            textBox1.Text += "Уравнение регрессии: Y = " + a + " + " + b + "*X" + "\r\n";
            textBox1.Text += "Средняя ошибка аппроксимации: A = " + error + "%" + "\r\n";
            textBox1.Text += "Коэффициент детерминации: R_квадрат = " + R2 + "\r\n";
            textBox1.Text += "Средний коэффициент эластичности Э = " + uh + "\r\n";
            textBox1.Text += "Критерий Фишера F = " + F + "\r\n";
            textBox1.Text += "________________________________________________________________________\r\n";
            check_the_model(R2, error, F, a, b);
        }
    }
}
