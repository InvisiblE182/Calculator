using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;

namespace Calculator
{
    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            tb.Text += b.Content.ToString();
            tb.Focus();
            tb.SelectionStart = tb.Text.Length; // asdfas
            tb.SelectionLength = 0;
        }

        private void Result_click(object sender, RoutedEventArgs e)
        {
            tb.Text = tb.Text.Replace(",", ".");
            Calculate(tb.Text);
            tb.Focus();
            tb.SelectionStart = tb.Text.Length;
            tb.SelectionLength = 0;
        }

        private void Button_Click_Del(object sender, RoutedEventArgs e)
        {
            tb.Text = "";
            tb.Focus();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            this.PreviewKeyDown += new KeyEventHandler(MainWindow_KeyDown);
            tb.Focus();
        }
        

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.Key == Key.Enter)
            {
                Calculate(tb.Text);
                tb.Focus();
            }

            if (e.Key == Key.Delete)
            {
                tb.Text = "";
                tb.Focus();
            }

        }

        public double Calculate(string input)
        {
            try { return double.Parse(GetExpression(input)); }
            catch (Exception) { return Counting(GetExpression(input)); }
        }

        static private string GetExpression(string input)
        {
            string output = string.Empty;
            string fun = string.Empty;
            Stack<char> operStack = new Stack<char>();
            char k = ' '; string p = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (IsOperator(input[i]) || Char.IsDigit(input[i]))
                {
                    if (k == ' ')
                        k = input[i];
                    else
                        if (input[i] == '-' && !Char.IsDigit(k))
                        p += " 0 ";
                    k = input[i];
                }
                p += input[i];
            }
            input = p;
            for (int i = 0; i < input.Length; i++)
            {
                if (IsDelimeter(input[i]))
                    continue;
                if (Char.IsDigit(input[i]))
                {
                    while (!IsDelimeter(input[i]) && !IsOperator(input[i]))
                    {
                        output += input[i];
                        i++;
                        if (i == input.Length) break;
                    }
                    output += " ";
                    i--;
                }
                else
                    if (IsOperator(input[i]))
                {
                    if (input[i] == '(')
                        operStack.Push(input[i]);
                    else if (input[i] == ')')
                    {
                        char s = operStack.Pop();
                        while (s != '(')
                        {
                            output += s.ToString() + ' ';
                            s = operStack.Pop();
                        }
                    }
                    else
                    {
                        if (operStack.Count > 0)
                            if (GetPriority(input[i]) <= GetPriority(operStack.Peek()))
                                output += operStack.Pop().ToString() + " ";

                        operStack.Push(char.Parse(input[i].ToString()));

                    }
                }
                else if (input[i] == '\u03C0')
                    output += " \u03C0 ";
                else if (input[i] == 'e')
                    output += " e ";
                else
                {
                    fun = String.Empty;
                    while (input[i] != '(')
                    {
                        fun += input[i];
                        i++;
                        if (i == input.Length) break;
                    }
                    i++;
                    if (IsFunction(fun))
                    {
                        String param = String.Empty;
                        while (input[i] != ')')
                        {
                            param += input[i];
                            i++;
                            if (i == input.Length) break;
                        }
                        double d;
                        try { d = double.Parse(param); }
                        catch (Exception) { d = 1; }
                    }
                }
            }
            while (operStack.Count > 0)
                output += operStack.Pop() + " ";
            return output; 
        }

        private double Counting(string input)
        {
            double result = 0;
            double b = 0;
            Stack<double> temp = new Stack<double>();
            try { return double.Parse(input); }
            catch (Exception)
            {
                for (int i = 0; i < input.Length; i++)
                {
                    if (Char.IsDigit(input[i]))
                    {
                        string a = string.Empty;

                        while (!IsDelimeter(input[i]) && !IsOperator(input[i]))
                        {
                            a += input[i];
                            i++;
                            if (i == input.Length) break;
                        }
                        temp.Push(double.Parse(a));
                        i--;
                    }
                    else if (input[i] == '\u03C0')
                        temp.Push(Math.PI);
                    else if (input[i] == 'e')
                        temp.Push(Math.E);
                    else if (IsOperator(input[i]))
                    {
                        double a = temp.Pop();
                        try
                        { b = temp.Pop(); }
                        catch (Exception) { b = 0; }

                        switch (input[i])
                        {
                            case '!': result = factorial((int)a); break;
                            case 'P': result = factorial((int)b) / factorial((int)(b - a)); break;
                            case 'C': result = factorial((int)b) / (factorial((int)a) * factorial((int)(b - a))); break;
                            case '^': result = Math.Pow(b, a); break;
                            case '%': result = b % a; break;
                            case '+': result = b + a; break;
                            case '-': result = b - a; break;
                            case '*': result = b * a; break;
                            case '/': result = b / a; break;
                        }
                        temp.Push(result);
                    }
                }
                tb.Text = result.ToString();
                tb.SelectionStart = tb.Text.Length;
                tb.SelectionLength = 0;
                return result; 
            }
        }

        static private bool IsDelimeter(char c)
        {
            if ((" =".IndexOf(c) != -1))
                return true;
            return false;
        }
         
        static private bool IsOperator(char с)
        {
            if (("+-/*^()PC!%".IndexOf(с) != -1))
                return true;
            return false;
        }

        static private bool IsFunction(String s)
        {
            String[] func = { "sin", "cos", "tg", "asin", "acos", "atg", "sqrt", "ln", "lg" };
            if (Array.Exists(func, e => e == s))
                return true;
            return false;
        }


        static private byte GetPriority(char s)
        {
            switch (s)
            {
                case '(': return 0;
                case ')': return 1;
                case '+': return 2;
                case '-': return 3;
                case '!': return 4;
                case '%': return 4;
                case '*': return 4;
                case '/': return 4;
                case '^': return 5;
                default: return 4;
            }
        }

        private static int factorial(int x)
        {
            int i = 1;
            for (int s = 1; s <= x; s++)
                i = i * s;
            return i;
        }
        
        private void Button_Click_Back(object sender, RoutedEventArgs e)
        {
            if (tb.Text.Length != 0)
            {
                tb.Text = tb.Text.Substring(0, tb.Text.Length - 1);
            }
        }
         

        private void Button_Click_Pow(object sender, RoutedEventArgs e)
        {
            tb.Text += "^";
            tb.Focus();
        }

        private void Button_Click_pi(object sender, RoutedEventArgs e)
        {
            tb.Text += "π";
            tb.Focus();
        }

        private void Button_Click_e(object sender, RoutedEventArgs e)
        {
            tb.Text += "e";
            tb.Focus();
        }
    }
}
