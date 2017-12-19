using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace OPG
{
    public partial class Form1 : Form
    {
        private int[,] relation;
        private string left;
        private string right;
        //char pchar;
        private string[] l;
        private string[,] r;
        private char[,] first;                   //文法非终结符FIRSTVT集
        private char[,] last;                    //文法非终结符LASTVT集
        private int[] _first;
        private int[] _last;

        bool[,] f;
        bool[,] ff;
        Stack<T> st1;
        Stack<T> st2;
        private int grammer_num;
        private int max_column;
        private int terminal_num;
        private int nonterminal_num;
        private char[] atom;
        private int atom_num;

        private char[] nonterminal;
        private char[] terminal;

        public Form1()
        {
            InitializeComponent();
            //divide();
            //copy(3,2);
            /*
            parent = new char[106,20];
            parent['F', 0] = 'F';
            parent['F', 1] = 'T';
            parent['F', 2] = 'E';
            parent['T', 0] = 'T';
            parent['T', 1] = 'E';
            parent['E', 0] = 'E';*/

        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            int counter = 1;
            string str = textBox1.Text;
            left = "#";
            right = str + "#";
            while (right.Length != 0)
            {
                if (!checkRelation(lastVt(left), fisrtVt(right)))
                {
                    MessageBox.Show("Semantic Wrong at " + (str.Length - right.Length + 2));
                    dataGridView1.Rows.Clear();
                    break;
                    //do something!!
                }
                if (relation[lastVt(left), fisrtVt(right)] == -1)  //进栈
                {
                    dataGridView1.Rows.Add(counter++, left, lastVt(left) + " < " + fisrtVt(right), "移入 " + fisrtVt(right), right.Substring(1));
                    left += right[0];
                    right = right.Substring(1);
                }
                else if (relation[lastVt(left), fisrtVt(right)] == 2)
                {
                    dataGridView1.Rows.Add(counter++, left, lastVt(left) + " = " + fisrtVt(right), "移入 " + fisrtVt(right), right.Substring(1));
                    left += right[0];
                    right = right.Substring(1);
                }
                else if (relation[lastVt(left), fisrtVt(right)] == 1 || relation[lastVt(left), fisrtVt(right)] == 111)
                {
                    if (left == "#" + l[0] && right == "#")
                    {
                        dataGridView1.Rows.Add(counter++, left, lastVt(left) + " > " + fisrtVt(right), "移入 " + fisrtVt(right), right);
                        left += "#";
                        right = right.Substring(1);
                        dataGridView1.Rows.Add(counter++, left, " ", "完成", right);
                        break;
                    }
                    int reduction_pos = analysis(left);
                    string reduction = left.Substring(reduction_pos);
                    // specifical!!!!!
                    if (isAtom(reduction[0]) && reduction.Length==1)  // FUCTION TO HANDLE IT!!!!!!!!!!!!!!
                    {
                        dataGridView1.Rows.Add(counter++, left, lastVt(left) + " > " + fisrtVt(right), "归约 " + lastVt(left), right);
                        left = left.Substring(0, left.Length - 1);
                        left += l[0];
                    }
                    else
                    {
                        int i, j;
                        if (isMatch(reduction, grammer_num, max_column, out i, out j))
                        {
                            dataGridView1.Rows.Add(counter++, left, lastVt(left) + " > " + fisrtVt(right), "归约 " + reduction, right);
                            left = left.Substring(0, reduction_pos);
                            left += l[i];
                        }
                        else
                        {
                            MessageBox.Show("Semantic Wrong at " + reduction_pos.ToString());
                            dataGridView1.Rows.Clear();
                            break;
                        }
                    }
                }
            }
        }
        /*private void getVn(string s,out int[] index,out int vn_num)
        {
            index = new int[s.Length];
            int k = 0;
            for(int i=0;i<s.Length;i++)
            {
                for(int j=0;j<3;j++)
                {
                    if (s[i] == nonterminal[j])
                        index[k++] = i;
                }
            }
            vn_num = k - 1;
        }*/
        private void findChar()
        {
            atom = new char[10];
            atom_num = 0;
            for(int i=0;i<grammer_num;i++)
            {
                for(int j=0;j<max_column;j++)
                {
                    if(r[i,j]!=null)
                    {
                        if(r[i,j].Length==1)
                        {
                            atom[atom_num++] = r[i, j][0];
                        }
                    }
                }
            }
        }
        private bool isAtom(char s)
        {
            for(int i=0;i<atom_num;i++)
            {
                if (s == atom[i])
                    return true;
            }
            return false;
        }      
        private char fisrtVt(string s)
        {
            char l = '$';
            bool flag = false;
            for (int i = 0; i < s.Length; i++)
            {
                for (int j = 0; j < terminal_num; j++)
                {
                    if (s[i] == terminal[j])
                        flag = true;
                }
                if (flag)
                {
                    l = s[i];
                    break;
                }
            }
            return l;
        }
        private char lastVt(string s)
        {
            char l = '$';
            bool flag = false;
            for (int i = s.Length - 1; i >= 0; i--)
            {
                for (int j = 0; j < terminal_num; j++)
                {
                    if (s[i] == terminal[j])
                        flag = true;
                }
                if (flag)
                {
                    l = s[i];
                    break;
                }
            }
            return l;
        }
        private bool checkRelation(char a, char b)
        {
            if (a == '$' || b == '$')
                return false;
            if (relation[a, b] != -1 && relation[a, b] != 2 && relation[a, b] != 1 && relation[a, b] != 111)
                return false;
            else
                return true;
        }
        private int analysis(string s)
        {
            char[] a = new char[s.Length];
            int[] index = new int[s.Length];
            int k = 0;
            bool flag = false;
            for (int i = 0; i < s.Length; i++)
            {
                for (int j = 0; j < terminal_num; j++)
                {
                    if (s[i] == terminal[j])
                    {
                        index[k] = i;
                        a[k++] = s[i];
                    }
                }
            }
            k--;
            int p;
            for (p = k; p > 0; p--)
            {
                if (relation[a[p - 1], a[p]] == -1)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
                return -1;
            else
                return index[p - 1] + 1;
        }
        private bool isMatch(string s, int a, int b, out int i, out int j)
        {
            for (i = 0; i < a; i++)
            {
                for (j = 0; j < b; j++)
                {
                    if (r[i, j] == s && r[i,j]!=null)
                        return true;
                }
            }
            i = -1; j = -1;
            return false;
        }
        private void standardlization(int a, int b, int c)//c is the num of nonterminal
        {
            for (int i = 0; i < a; i++)
            {
                for (int j = 0; j < b; j++)
                {
                    if(r[i,j]!=null)
                    {
                        for (int k = 0; k < r[i, j].Length; k++)
                        {
                            for (int h = 0; h < c; h++)
                            {
                                if (r[i, j][k] == nonterminal[h] && r[i, j][k] != l[0][0])
                                {
                                    r[i, j] = r[i, j].Replace(r[i, j][k], l[0][0]);
                                }
                            }
                        }
                    }
                }
                for (int j = 0; j < l[i].Length; j++)
                {
                    for (int h = 0; h < c; h++)
                    {
                        if (l[i][j] == nonterminal[h] && l[i][j] != l[0][0])
                        {
                            l[i] = l[i].Replace(l[i][j], l[0][0]);
                        }
                    }
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.Text = "E::=E+T|T\r\nT::=T*F|F\r\nF::=(E)|i";
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string grammer = textBox2.Text;
            getVn_and_Vt(grammer, out grammer_num, out max_column, out terminal_num, out nonterminal_num);
            if(isOG())
            {
                findChar();
                //fisrtVt and LastVt
                //relation matrix
                f = new bool[512, 512];
                ff = new bool[512, 512];
                for (int i = 0; i < 512; i++)
                {
                    for (int j = 0; j < 512; j++)
                    {
                        f[i, j] = false;
                        ff[i, j] = false;
                    }
                }
                first = new char[512, terminal_num];
                _first = new int[terminal_num];
                last = new char[512, terminal_num];
                _last = new int[nonterminal_num];
                getFirstVt();
                getLastVt();
                if (getRelationship() == -1)
                    MessageBox.Show("这不是算符优先文法.");
                else
                    standardlization(grammer_num, max_column, nonterminal_num);
            }
            else
            {
                MessageBox.Show("这不是算符文法.");
            }
        }
        private void getVn_and_Vt(string s, out int gram, out int column, out int tn, out int nn)
        {
            string[] grammer = s.Split('\n');
            //grammer.Length-1;// lenth!!
            //every row has a \n in the end.
            l = new string[grammer.Length];
            string[] grammer_row = new string[grammer.Length];
            gram = grammer_row.Length;
            for (int i = 0; i < grammer.Length; i++)
            {
                l[i] = Regex.Split(grammer[i], "::=", RegexOptions.IgnoreCase)[0];
                grammer_row[i] = Regex.Split(grammer[i], "::=", RegexOptions.IgnoreCase)[1];
            }
            int max = 0;
            int counter;
            for (int i = 0; i < grammer.Length; i++)
            {
                counter = 0;
                for (int j = 0; j < grammer_row[i].Length; j++)
                {
                    if (grammer_row[i][j] == '|')
                        counter++;
                }
                if (counter > max)
                    max = counter;
            }
            r = new string[grammer.Length, ++max];
            column = max;
            
            string[] temp;
            for (int i = 0; i < grammer.Length; i++)
            {
                temp = grammer_row[i].Split('|');
                for (int j = 0; j < temp.Length; j++)
                    r[i, j] = temp[j];
                if(r[i, temp.Length - 1][r[i, temp.Length - 1].Length-1]=='\r')
                    r[i, temp.Length - 1] = r[i, temp.Length - 1].Substring(0, r[i, temp.Length - 1].Length - 1);
            }
            char[] tem = new char[512];
            for (int i = 0; i < grammer.Length; i++)
                tem[l[i][0]] = l[i][0];
            nonterminal = new char[grammer.Length];
            int c = 0;
            for (int i = 0; i < 512; i++)
            {
                if (tem[i] != '\0')
                    nonterminal[c++] = tem[i];
            }
            // c is the number of nonterminal
            nn = c;
            char[] tem2 = new char[500];
            int co = 0;
            for (int i = 0; i < grammer.Length; i++)
            {
                for (int j = 0; j < max; j++)
                {
                    if(r[i,j]!=null)
                    {
                        for (int k = 0; k < r[i, j].Length; k++)
                        {
                            bool flag = false;
                            for (int h = 0; h < c; h++)
                            {
                                if (r[i, j][k] == nonterminal[h])
                                    flag = true;
                            }
                            if (!flag)
                                tem2[co++] = r[i, j][k];
                        }
                    }
                }
            }
            int cc = 0;
            terminal = new char[512];
            char[] tem1 = new char[512];
            for (int i = 0; i < co; i++)
                tem1[tem2[i]] = tem2[i];
            for (int i = 0; i < 512; i++)
            {
                if (tem1[i] != '\0')
                    terminal[cc++] = tem1[i];
            }
            terminal[cc++] = '#';
            tn = cc;
            //cc is the number of terminal
        }
        private void getFirstVt()
        {
            textBox3.Text = "";
            st1 = new Stack<T>();
            for(int i=0;i<grammer_num;i++)
            {
                for(int j=0;j<max_column;j++)
                {
                    if(r[i,j]!=null)
                    {
                        if (isVt(r[i, j][0]))
                        {
                            insert(l[i][0], r[i, j][0]);
                        }
                        else if (r[i, j].Length > 1)
                        {
                            if (isVn(r[i, j][0]) && isVt(r[i, j][1]))
                            {
                                insert(l[i][0], r[i, j][1]);
                            }
                        }
                    }
                }
            }
            while(st1.Count!=0)
            {
                T tem = new T(st1.Pop());
                for (int i = 0; i < grammer_num; i++)
                {
                    for (int j = 0; j < max_column; j++)
                    {
                        if (r[i, j] != null)
                        {
                            if (r[i, j][0] == tem.getA())
                                insert(l[i][0], tem.getB());
                        }
                    }
                }
            }
            
            for (int i=0;i<nonterminal_num;i++)
            {
                int counter = 0;
                for (int j=0;j<terminal_num;j++)
                {
                    if (f[nonterminal[i], terminal[j]])
                        first[nonterminal[i], counter++] = terminal[j];
                }
                _first[i] = counter;
            }
            for (int i = 0; i < nonterminal_num; i++)
            {
                textBox3.Text += (nonterminal[i] + ": ");
                for (int j = 0; j < _first[i]; j++)
                    textBox3.Text += (first[nonterminal[i], j]+" ");
                textBox3.Text += ("\r\n");
            }
        }
        private void getLastVt()
        {
            textBox4.Text ="";
            st2 = new Stack<T>();
            for (int i = 0; i < grammer_num; i++)
            {
                for (int j = 0; j < max_column; j++)
                {
                    if (r[i, j] != null)
                    {
                        if (isVt(r[i, j][r[i, j].Length - 1]))
                        {
                            insert1(l[i][0], r[i, j][r[i, j].Length - 1]);
                        }
                        else if (r[i, j].Length > 1)
                        {
                            if (isVn(r[i, j][r[i, j].Length - 1]) && isVt(r[i, j][r[i, j].Length - 2]))
                                insert1(l[i][0], r[i, j][r[i, j].Length - 2]);
                        }
                    }
                }
            }
            while (st2.Count != 0)
            {
                T tem = new T(st2.Pop());
                for (int i = 0; i < grammer_num; i++)
                {
                    for (int j = 0; j < max_column; j++)
                    {
                        if (r[i, j] != null)
                        {
                            if (r[i, j][r[i, j].Length - 1] == tem.getA())
                                insert1(l[i][0], tem.getB());
                        }
                    }
                }
            }

            for (int i = 0; i < nonterminal_num; i++)
            {
                int counter = 0;
                for (int j = 0; j < terminal_num; j++)
                {
                    if (ff[nonterminal[i], terminal[j]])
                        last[nonterminal[i], counter++] = terminal[j];
                }
                _last[i] = counter;
            }
            for (int i = 0; i < nonterminal_num; i++)
            {
                textBox4.Text += (nonterminal[i] + ": ");
                for (int j = 0; j < _last[i]; j++)
                    textBox4.Text += (last[nonterminal[i], j] + " ");
                textBox4.Text += ("\r\n");
            }
        }
        private void insert(char a,char b)
        {
            if(!f[a,b])
            {
                f[a, b] = true;
                T t = new OPG.T(a,b);
                st1.Push(t);
            }
        }
        private void insert1(char a, char b)
        {
            if (!ff[a, b])
            {
                ff[a, b] = true;
                T t = new OPG.T(a, b);
                st2.Push(t);
            }
        }
        private bool isVn(char a)
        {
            for(int i=0;i<nonterminal_num;i++)
            {
                if (nonterminal[i] == a)
                    return true;
            }
            return false;
        }
        private bool isVt(char a)
        {
            for (int i = 0; i < terminal_num; i++)
            {
                if (terminal[i] == a)
                    return true;
            }
            return false;
        }
        private int getRelationship()
        {
            textBox5.Text = "";
            relation = new int[256, 256];
            for(int i=0;i<grammer_num;i++)
            {
                for(int j=0;j<max_column;j++)
                {
                    //l[i]::=r[i,j]
                    if(r[i,j]!=null)
                    {
                        for (int k = 0; k <= r[i, j].Length - 2; k++)
                        {
                            if (isVt(r[i, j][k]) && isVt(r[i, j][k + 1]))
                                relation[r[i, j][k], r[i, j][k + 1]] = 2;
                            if (k <= r[i, j].Length - 3 && isVt(r[i, j][k]) && isVt(r[i, j][k + 2]) && isVn(r[i, j][k + 1]))
                                relation[r[i, j][k], r[i, j][k + 2]] = 2;
                            if (isVt(r[i, j][k]) && isVn(r[i, j][k + 1]))
                            {
                                int h;
                                for (h = 0; h < nonterminal_num; h++)
                                {
                                    if (nonterminal[h] == r[i, j][k + 1])
                                        break;
                                }
                                for (int hh = 0; hh < _first[h]; hh++)
                                {
                                    if (relation[r[i, j][k], first[r[i, j][k + 1], hh]] == 1)
                                        return -1;
                                    relation[r[i, j][k], first[r[i, j][k + 1], hh]] = -1;
                                }
                            }
                            if (isVn(r[i, j][k]) && isVt(r[i, j][k + 1]))
                            {
                                int h;
                                for (h = 0; h < nonterminal_num; h++)
                                {
                                    if (nonterminal[h] == r[i, j][k])
                                        break;
                                }
                                for (int hh = 0; hh < _last[h]; hh++)
                                {
                                    if (relation[last[r[i, j][k], hh], r[i, j][k + 1]] == -1)
                                        return -1;
                                    relation[last[r[i, j][k], hh], r[i, j][k + 1]] = 1;
                                }
                            }
                        }
                    }
                }
            }
            for(int i=0;i<terminal_num;i++)
            {
                if(terminal[i]!='#')
                {
                    relation[terminal[i], '#'] = 1;
                    relation['#', terminal[i]] = -1;
                }
                relation['#', '#'] = 111;
            }
            textBox5.Text += ("\t");
            for (int i = 0; i < terminal_num; i++)
            {
                textBox5.Text += (terminal[i] + "\t");
            }
            textBox5.Text += ("\r\n");
            for (int i = 0; i < terminal_num; i++)
            {
                textBox5.Text += (terminal[i] + "\t");
                for (int j=0;j<terminal_num;j++)
                {
                    switch(relation[terminal[i], terminal[j]])
                    {
                        case 0:
                            textBox5.Text += ("X"+"\t");
                            break;
                        case -1:
                            textBox5.Text += ("<" + "\t");
                            break;
                        case 1:
                            textBox5.Text += (">" + "\t");
                            break;
                        case 2:
                            textBox5.Text += ("=" + "\t");
                            break;
                        default:
                            textBox5.Text += (" " + "\t");
                            break;
                    }
                }
                textBox5.Text += ("\r\n");
            }
            return 0;
        }
        private bool isOG()
        {
            for(int i=0;i<grammer_num;i++)
            {
                for(int j=0;j<max_column;j++)
                {
                    if(r[i,j]!=null)
                    {
                        for(int k=0;k<r[i,j].Length-1;k++)
                        {
                            if(isVn(r[i,j][k]) && isVn(r[i, j][k+1]))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
    }

    public class T
    {
        private char a;
        private char b;

        public T(char c,char c1)
        {
            a = c;
            b = c1;
        }
        public T()
        { }
        public T(T t)
        {
            a = t.getA();
            b = t.getB();
        }

        public char getA()
        {
            return a;
        }

        public char getB()
        {
            return b;
        }
    }
}