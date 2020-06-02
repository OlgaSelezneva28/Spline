using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace Spline3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Form2 f2 = new Form2();
        double m1, m2;
        int nn, NN;
        double a;
        double b;

        //Функции
        public double Function(double x, int z)
        {
            if (z == 1) //Тестовая 
            {
                if (x <= 0.0)
                    return Math.Pow(x, 3.0) + 3 * Math.Pow(x, 2.0);
                if (x > 0)
                    return Math.Pow(-x, 3.0) + 3 * Math.Pow(x, 2.0);
            }
            if (z == 2)//Основная 
            {
                return Math.Log(x + 1.0) / (x + 1.0);
            }
            if (z == 3)//Осциллирующая
            {
                return Math.Log(x + 1.0) / (x + 1.0) + Math.Cos(10 * x);
            }

            return 0;
        }
        //Первые производные функций
        public double Function1(double x, int z)
        {
            if (z == 1)
            {
                if (x < 0)
                    return 3.0 * x * x + 6.0 * x;
                else
                    return -3.0 * x * x + 6.0 * x;
            }
            if (z == 2)
            {
                return (1.0 - Math.Log(x + 1.0)) / (Math.Pow(x + 1.0, 2.0));
            }
            if (z == 3)
            {
                return (1.0 - Math.Log(x + 1.0)) / (Math.Pow(x + 1.0, 2.0)) - 10.0 * Math.Sin(10 * x);
            }

            return 0;
        }
        //Вторые производные функций
        public double Function2(double x, int z)
        {
            if (z == 1)
            {
                if (x < 0)
                    return 6.0 * x + 6.0;
                else
                    return -6.0 * x + 6.0;
            }
            if (z == 2)
            {
                return -(3.0 - 2 * Math.Log(1 + x)) / (Math.Pow(1 + x, 3));
            }
            if (z == 3)
            {
                return -(3.0 - 2 * Math.Log(1 + x)) / (Math.Pow(1 + x, 3)) - 100 * Math.Cos(10 * x);
            }

            return 0;
        }

        //Прогонка и коэффициенты сплайна
        public double[] X;
        public double[] A;
        public double[] B;
        public double[] C;
        public double[] D;

        public void XX(int n, double a, double b)
        {
            X = new double[n + 1];
            double h = (b - a) / n;
            for (int i = 0; i < n + 1; i++)
                X[i] = a + i * h;
        }

        public void AA(int z, int n, double a, double b)
        {
            XX(n, a, b);
            A = new double[n + 1];
            for (int i = 0; i < n + 1; i++)
                A[i] = Function(X[i], z);
        }

        public void CC(int z, int n, double a, double b)
        {
            double[] ai = new double[n + 1];
            double[] bi = new double[n + 1];
            double[] xi = new double[n + 1];
            double h = (b - a) / n;
            AA(z,n,a,b);
            ai[1] = 0;
            bi[1] = m1;

            for (int i = 1; i < n; i++)
            {
                xi[i] = a + i * h;
                ai[i + 1] = -(h) / (ai[i] * h + 4 * h);
                bi[i + 1] = ((-6.0 / h) * (A[i - 1] - 2.0 * A[i] + A[i + 1]) + bi[i] * h) / (-4.0 * h - ai[i] * h);
            }

            C = new double[n + 1];
            C[n] = m2;
            for (int i = n; i >= 1; i--)
            {
                C[i - 1] = ai[i] * C[i] + bi[i];
            }
        }

        public void DD(int z, int n, double a, double b)
        {
            CC(z,n,a,b);
            D = new double[n + 1];
            double h = (b - a) / n;
            for (int i = 1; i < n + 1; i++)
            {
                D[i] = (C[i] - C[i - 1]) / (h);
            }
        }

        public void BB(int z, int n, double a, double b)
        {
            AA(z,n,a,b);
            CC(z, n, a, b);
            B = new double[n + 1];
            double h = (b - a) / n;
            for (int i = 1; i <= n; i++)
            {
                B[i] = (A[i] - A[i - 1]) / h + C[i] * h / 3.0 + C[i - 1] * h / 6.0;
            }
        }
        //
        public void GetCoeff(int z, int n, double a, double b)
        {
            XX(n, a, b);
            CC(z, n, a, b);
            AA(z, n, a, b);
            DD(z, n, a, b);
            BB(z, n, a, b);
        }

        //
        public void InTable1(int z, int n, double a, double b)
        {
            GetCoeff(z, n, a, b);
            double h = (b - a) / n;

            //Cout in Table 
            dataGridView1.Rows.Clear();
            dataGridView1.RowCount = n + 1;
            dataGridView1.ColumnCount = 7;
            dataGridView1.Rows[0].Cells[0].Value = "i";
            dataGridView1.Rows[0].Cells[1].Value = "xi-1";
            dataGridView1.Rows[0].Cells[2].Value = "xi";
            dataGridView1.Rows[0].Cells[3].Value = "ai";
            dataGridView1.Rows[0].Cells[4].Value = "bi";
            dataGridView1.Rows[0].Cells[5].Value = "ci";
            dataGridView1.Rows[0].Cells[6].Value = "di";

            for (int i = 1; i < n + 1; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = (i ).ToString();
                dataGridView1.Rows[i].Cells[1].Value = (X[i - 1]).ToString();
                dataGridView1.Rows[i].Cells[2].Value = (X[i]).ToString();
                dataGridView1.Rows[i].Cells[3].Value = A[i].ToString();
                dataGridView1.Rows[i].Cells[4].Value = B[i].ToString();
                dataGridView1.Rows[i].Cells[5].Value = C[i].ToString();
                dataGridView1.Rows[i].Cells[6].Value = D[i].ToString();
            }
        }


        //Сплайн
        public double Spline(int z, int n, double a, double b, double x)
        {
            //GetCoeff(z, n, a, b);
            for (int i = 1; i < n + 1; i++)
            {
                if ((x >= X[i - 1]) && (x <= X[i]))
                {
                    return (A[i] + B[i] * (x - X[i]) + C[i] / 2.0 * Math.Pow((x - X[i]), 2) + D[i] / 6.0 * Math.Pow((x - X[i]), 3));
                }
            }
            return (0.0);
        }

        public double Spline1(int z, int n, double a, double b, double x)
        {
            //GetCoeff(z, n, a, b);
            for (int i = 1; i < n + 1; i++)
            {
                if ((x >= X[i - 1]) && (x <= X[i]))
                {
                    return (B[i] + C[i] * (x - X[i]) + D[i] / 2.0  * Math.Pow((x - X[i]), 2));
                }
            }
            return (0.0);
        }

        public double Spline2(int z, int n, double a, double b, double x)
        {
            //GetCoeff(z, n, a, b);
            for (int i = 1; i < n + 1; i++)
            {
                if ((x >= X[i - 1]) && (x <= X[i]))
                {
                    return (C[i] + D[i] * (x - X[i]));
                }
            }
            return (0.0);
        }

        //
        double max , max1 , max2 ;
        double x0 , x1 , x2 ;
        public void maxs(int z, int n, double a, double b)
        {
            max = 0;
            max1 = 0;
            max2 = 0;
            x0 = 0;
            x1 = 0;
            x2 = 0;
            double h = (b-a)/n;
            for (double x = a; x <= b; x += h / 4.0)
            {
                if (Math.Abs(Function(x,z) - Spline(z,n,a,b,x)) > max)
                {
                    max = Math.Abs(Function(x, z) - Spline(z, n, a, b, x));
                    x0 = x;
                }

                if (Math.Abs(Function1(x, z) - Spline1(z, n, a, b, x)) > max1)
                {
                    max1 = Math.Abs(Function1(x, z) - Spline1(z, n, a, b, x));
                    x1 = x;
                }

                if (Math.Abs(Function2(x, z) - Spline2(z, n, a, b, x)) > max2)
                {
                    max2 = Math.Abs(Function2(x, z) - Spline2(z, n, a, b, x));
                    x2 = x;
                }
            }

            f2.label1.Text = Convert.ToString(max);
            f2.label2.Text = Convert.ToString(x0);
            f2.label3.Text = Convert.ToString(max1);
            f2.label4.Text = Convert.ToString(x1);
            f2.label5.Text = Convert.ToString(max2);
            f2.label6.Text = Convert.ToString(x2);
        }

        public void InTable2(int z, int n, double a, double b)
        {
            maxs(z, n, a, b);

            double h = (b - a) / n;
            
            int N = 4 * n;
            if (N > 100)
                N = 100;

            //Cout in Table2 
            dataGridView2.Rows.Clear();
            dataGridView2.RowCount = N + 1;
            dataGridView2.ColumnCount = 11;
            
            dataGridView2.Rows[0].Cells[0].Value = "j";
            dataGridView2.Rows[0].Cells[1].Value = "xj";
            dataGridView2.Rows[0].Cells[2].Value = "F(xj)";
            dataGridView2.Rows[0].Cells[3].Value = "S(xj)";
            dataGridView2.Rows[0].Cells[4].Value = "F(xj) - S(xj)";
            dataGridView2.Rows[0].Cells[5].Value = "F`(xj)";
            dataGridView2.Rows[0].Cells[6].Value = "S`(xj)";
            dataGridView2.Rows[0].Cells[7].Value = "F`(xj) - S`(xj)";
            dataGridView2.Rows[0].Cells[8].Value = "F``(xj)";
            dataGridView2.Rows[0].Cells[9].Value = "S``(xj)";
            dataGridView2.Rows[0].Cells[10].Value = "F``(xj) - S``(xj)";

            double yzel = a;

            for (int i = 1; i < N + 1; i++)
            {
                dataGridView2.Rows[i].Cells[0].Value = (i).ToString();
                dataGridView2.Rows[i].Cells[1].Value = yzel.ToString();
                dataGridView2.Rows[i].Cells[2].Value = (Function(yzel, z)).ToString();
                dataGridView2.Rows[i].Cells[3].Value = (Spline(z, n, a, b, yzel)).ToString();
                dataGridView2.Rows[i].Cells[4].Value = (Function(yzel, z) - Spline(z, n, a, b, yzel)).ToString();
                dataGridView2.Rows[i].Cells[5].Value = (Function1(yzel, z)).ToString();
                dataGridView2.Rows[i].Cells[6].Value = (Spline1(z, n, a, b, yzel)).ToString();
                dataGridView2.Rows[i].Cells[7].Value = (Function1(yzel, z) - Spline1(z, n, a, b, yzel)).ToString();
                dataGridView2.Rows[i].Cells[8].Value = (Function2(yzel, z)).ToString();
                dataGridView2.Rows[i].Cells[9].Value = (Spline2(z, n, a, b, yzel)).ToString();
                dataGridView2.Rows[i].Cells[10].Value = (Function2(yzel, z) - Spline2(z, n, a, b, yzel)).ToString();
                
                yzel += h/4.0;
            }
        }


        //----------------------//
        

        //Тестовая задача
        public void Test()
        {
            a = -1.0;
            b = 1.0;

            InTable1(1, nn, a, b);
            InTable2(1,nn,a,b);
        }

        //Основная задача 
        public void Osnov()
        {
            a = 0.2;
            b = 2;

            InTable1(2, nn, a, b);
            InTable2(2, nn, a, b);
        }
        //Осциллирующая задача
        public void Osscil()
        {
            a = 0.2;
            b = 2;

            InTable1(3, nn, a, b);
            InTable2(3, nn, a, b);
        }

        //Запуск


        private void button1_Click_1(object sender, EventArgs e)
        {
            nn = int.Parse(textBox1.Text);
            NN = 4 * nn;

            m1 = System.Convert.ToDouble(textBox2.Text);
            m2 = System.Convert.ToDouble(textBox3.Text);  

            f2.label7.Text = Convert.ToString(nn);
            f2.label8.Text = Convert.ToString(NN);

            if (radioButton1.Checked == true)
            {
                Test();
                DrawGraph(1,NN,a,b);
            }
            if (radioButton2.Checked == true)
            {
                Osnov();
                DrawGraph(2, NN, a, b);
            }
            if (radioButton3.Checked == true)
            {
                Osscil();
                DrawGraph(3, NN, a, b);
            }
 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            f2.Update();
            f2.Show();
        }

        
        public void DrawGraph(int z, int n, double a, double b)
        {
            ZedGraph.PointPairList F = new ZedGraph.PointPairList();
            ZedGraph.PointPairList S = new ZedGraph.PointPairList();
            ZedGraph.PointPairList F1 = new ZedGraph.PointPairList();
            ZedGraph.PointPairList S1 = new ZedGraph.PointPairList();
            ZedGraph.PointPairList F2 = new ZedGraph.PointPairList();
            ZedGraph.PointPairList S2 = new ZedGraph.PointPairList();
            ZedGraph.PointPairList e = new ZedGraph.PointPairList();
            ZedGraph.PointPairList e1 = new ZedGraph.PointPairList();
            ZedGraph.PointPairList e2 = new ZedGraph.PointPairList();

            double h = (b - a)/n;
            
            for (int i = 0; i < n + 1; i++)
            {
                if (i % 4 == 0 && i != 0 && i != n)
                    continue;
                double x0 = a + i * h;
                double FF = Function(x0,z);
                double SS = Spline(z,n,a,b,x0);
                double FF1 = Function1(x0,z);
                double SS1 = Spline1(z, n, a, b, x0);
                double FF2 = Function2(x0, z);
                double SS2 = Spline2(z, n, a, b, x0);
                double E = FF - SS;
                double E1 = FF1 - SS1;
                double E2 = FF2 - SS2;

                F.Add(x0, FF);
                S.Add(x0, SS);
                F1.Add(x0, FF1);
                S1.Add(x0, SS1);
                F2.Add(x0, FF2);
                S2.Add(x0, SS2);
                e.Add(x0, E);
                e1.Add(x0, E1);
                e2.Add(x0, E2);
            }

            ZedGraph.GraphPane.CurveList.Clear();

            if (comboBox1.SelectedIndex == 0)
            {
                LineItem Curve = ZedGraph.GraphPane.AddCurve("F(x)", F, Color.Green, SymbolType.None);
                LineItem SCurve = ZedGraph.GraphPane.AddCurve("S(x)", S, Color.Blue, SymbolType.None);
                LineItem ECurve = ZedGraph.GraphPane.AddCurve("F(x)-S(x)", e, Color.Red, SymbolType.None);
            }
            if (comboBox1.SelectedIndex == 1)
            {
                LineItem Curve = ZedGraph.GraphPane.AddCurve("F'(x)", F1, Color.Green, SymbolType.None);
                LineItem SCurve = ZedGraph.GraphPane.AddCurve("S'(x)", S1, Color.Blue, SymbolType.None);
                LineItem ECurve = ZedGraph.GraphPane.AddCurve("F'(x)-S'(x)", e1, Color.Red, SymbolType.None);
            }
            if (comboBox1.SelectedIndex == 2)
            {
                LineItem Curve = ZedGraph.GraphPane.AddCurve("F''(x)", F2, Color.Green, SymbolType.None);
                LineItem SCurve = ZedGraph.GraphPane.AddCurve("S''(x)", S2, Color.Blue, SymbolType.None);
                LineItem ECurve = ZedGraph.GraphPane.AddCurve("F''(x)-S''(x)", e2, Color.Red, SymbolType.None);
            }
            

            ZedGraph.AxisChange();

            ZedGraph.Invalidate();
        }
        
    }
}

