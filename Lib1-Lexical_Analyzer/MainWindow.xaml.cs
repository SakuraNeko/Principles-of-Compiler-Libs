using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

namespace Lib1_Lexical_Analyzer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string[] 保留字 = { "bool", "break", "case", "char", "do", "double", "else", "false", "float", "for", "if", "int", "long", "main", "new", "null", "private", "protected", "public", "static", "true", "try", "typeof", "unit", "void", "while", "return", "short", "sizeof", "switch" };
        private static string[] 运算符 = { "*", "/", "%", "+", "-", "<<", ">>", "<", ">", "<=", ">=", "==", "!=", "&", "^", "|", "&&", "||", "?:", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "<<=", ">>=", "=" };
        private static string[] 界符 = { ",", ";", ".", "'", "(", ")", ":", "[", "]", "{", "}", "/" };
        private static string[] 标识符;
        private static string[] 常数;
        private static string[] 未知符号;
        private ArrayList temp = new ArrayList();


        public MainWindow()
        {
            InitializeComponent();
        }
        public int Search_Location(string str, string[] a) //查找字符编码         
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (str == a[i])
                    return i + 1;
            }
            return -1;
        }
        public int InsertId(string strr, ref string[] a)//标识符插入标识符表返回位置
        {
            temp.Clear(); if (a == null)
                temp.Add(strr);
            else
            {
                for (int i = 0; i < a.Length; i++)
                    if (strr == a[i])
                        return i;
                for (int j = 0; j < a.Length; j++)
                {
                    temp.Add(a[j]);
                }
                temp.Add(strr);
            }
            a = (string[])temp.ToArray(typeof(string));
            return a.Length - 1;
        }
        private string Pretreatment()//预处理器
        {
            string s1 = textBox1.Text;
            string str1 = null;
            //去除换行和"//"注释
            string[] str = new string[textBox1.MaxLines];
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i].Contains("//"))
                {
                    string[] strr = str[i].Split('/');
                    str[i] = strr[0];
                }
            }
            for (int i = 0; i < str.Length; i++)
            {
                str1 += str[i] + " ";
            }
            if (str1.Contains("/v"))
                str1 = str1.Replace("/v", " ");
            if (str1.Contains("/r"))
                str1 = str1.Replace("/r", " ");
            if (str1.Contains("/n"))
                str1 = str1.Replace("/n", " ");
            if (str1.Contains("/t"))
                str1 = str1.Replace("/t", " ");
            /*去除注释*/
            while (str1.Contains("/*") && str1.Contains("*/"))
            {
                str1 = str1.Remove(str1.IndexOf("/*"), str1.IndexOf("*/") - str1.IndexOf("/*") + 2);
            }
            //去除多余空格
            while (str1.Contains("  "))
            {
                str1 = str1.Replace("  ", " ");
            }
            return str1;
        }
        private bool IsOperator(string str)
        {
            for (int i = 0; i < 运算符.Length; i++)
                if (str == 运算符[i])
                    return true;
            return false;
        }
        private bool IsCommunity_character(string str)
        {
            for (int i = 0; i < 界符.Length; i++)
                if (str == 界符[i])
                    return true;
            return false;
        }
        void analyze(string string1, ref string string2)//扫描器
        {

            string[] str = string1.Split(new char[] { ' ' });
            for (int i = 0; i < str.Length - 1; i++)
                switch (type_judger(str[i]))
                {
                    case 0:
                        string2 += "( " + str[i] + "," + "保留字," + Search_Location(str[i], 保留字) + " )" + '\r' + '\n';
                        break;
                    case 1:
                        string2 += "( " + str[i] + "," + "标识符," + InsertId(str[i], ref 标识符) + " )" + '\r' + '\n';
                        break;
                    case 2:
                        string2 += "( " + str[i] + "," + "常数," + InsertId(str[i], ref 常数) + " )" + '\r' + '\n';
                        break;
                    case 3:
                        string2 += "( " + str[i] + "," + "运算符," + Search_Location(str[i], 运算符) + " )" + '\r' + '\n';
                        break;
                    case 4:
                        string2 += "( " + str[i] + "," + "界符," + Search_Location(str[i], 界符) + " )" + '\r' + '\n';
                        break;
                    case 5:
                        string2 += "( " + str[i] + "," + "未知符号," + InsertId(str[i], ref 未知符号) + " )" + '\r' + '\n';
                        break;
                }
        }

        private int type_judger(string str)
        {
            if (str == null)
                return -1;
            char a = str[0];
            if (char.IsLetter(a))  //首个字符为字母，则可能为保留字、标识符，也可能是未知符号
            {
                int i = 0;
                for (int k = 0; k < 保留字.Length; k++)
                    if (str == 保留字[k])
                        return 0;//保留字 
                for (i = 0; i < str.Length; i++)
                    if (char.IsLetterOrDigit(str[i]) == false)
                        return 5;//未知符号 
                if (i >= str.Length)
                    return 1;//标识符                 
            }
            else if (char.IsDigit(a))//首个字符为数字，则可能为数字也可能为未知字符 
            {
                int j = 0;
                for (j = 0; j < str.Length; j++)
                    if (char.IsDigit(str[j]) == false)
                        return 5;//未知符号 
                if (j >= str.Length)
                    return 2; //常数 
            }
            else//首个字符不是字母/数字，则可能是运算符/界符或未知字符 
            {
                if (IsOperator(str))
                    return 3;//运算符 
                if (IsCommunity_character(str))
                    return 4;//界符                   
            }
            return 5;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = "int i = 10 ; /*注释部分 */ ?";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string textbox1text = textBox1.Text;
            if (textbox1text == "")
            {
                MessageBox.Show("您没有输入任何字符！");
                return;
            }
            textbox1text = Pretreatment();
            string textbox2text = null;
            analyze(textbox1text, ref textbox2text);
            textBox2.Text = textbox2text;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择数据源文件";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "txt";
            if (openFileDialog.ShowDialog() == false)
            {
                MessageBox.Show("操作失败");
                return;
            }
            string dir = openFileDialog.FileName;
            string text = null;
            StreamReader sr;
            try
            {
                sr = new StreamReader(dir, System.Text.Encoding.Default);

            }
            catch
            {


                return;
            }
            while (sr.Peek() != -1)
            {
                text += sr.ReadLine();
            }
        }
    }
}
