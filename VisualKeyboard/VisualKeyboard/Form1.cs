using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VisualKeyboard
{
    public partial class Form1 : Form
    {
        const int PositionLeft = 30;
        const int PositionRight = 160;
        const int KeySizeSqu = 70;
 
        const int MaxLenthWord = 1024;
        static int index_times = 0;
        bool CatchStart = false;
        string Word = "";
        string buffer = "";
        private string key_old;
        BoardKey BoardKey1;
        private static Point vectori = new Point(0,0);
        private static Point vectoriplus1 = new Point(0, 0);
        private const int Max_SquMinDistance = (int)(KeySizeSqu * KeySizeSqu / 2);
        private const int SquMinDistance = Max_SquMinDistance - 2350;//max SquMinDistance = 35.35 * 35.35 which 35 comes from 25* squ_root(2) / 1250
        ColorCord[] WordPosStack = new  ColorCord[MaxLenthWord];
        int[] CurvePointIndex = new int[MaxLenthWord];
        private string[] input_buffer= new string[MaxLenthWord];
        private int top_buffer = -1;
        private int top = -1;
        //Words handler
        FindWords Org_Word = new FindWords();

        public Form1()
        {
            InitializeComponent();
            Graphics dc = this.CreateGraphics();  // create Graphics object
            FontInfo KeyFontFormat1 = new FontInfo( "Arial", 12f, FontStyle.Bold);
            BoardKey1 = new BoardKey(PositionLeft, PositionRight, KeySizeSqu, dc, KeyFontFormat1);
            this.Show();  // show control box to user
            top = -1;
            top_buffer = -1;
            string s_val = System.String.Empty;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && ((e.X >= PositionLeft) && (e.X <= PositionLeft + 10 * KeySizeSqu) && (e.Y <= PositionRight + 4 * KeySizeSqu) && (e.Y >= PositionRight)))
            {//first time button click
                CatchStart = true;//change state to start painting 
                top = -1;
                ColorCord tmp = new ColorCord("", -1, -1, -1, -1, false);
                Word = "";
                key_old = "";
                tmp = BoardKey1.ConvCoord2String(PositionLeft, PositionRight, e.X, e.Y, KeySizeSqu, Max_SquMinDistance);
                if (tmp.Flag)
                {
                    WordPosStack[++top] = tmp;
                    //CurvePointIndex[top] = 1;
                    Word += tmp.key;
                    key_old = tmp.key;
                    index_times = 1 ;
                    FontInfo KeyFontFormat1 = new FontInfo("Arial", 12f, FontStyle.Bold);
                    BoardKey1.KeyIndexPlus(WordPosStack[top], KeyFontFormat1, KeySizeSqu, index_times);
                }
            }
            if ( e.Button == MouseButtons.Right && CatchStart)
            {//right button which cause double
                ColorCord tmp = new ColorCord("", -1, -1, -1, -1, false);
                tmp = BoardKey1.ConvCoord2String(PositionLeft, PositionRight, e.X, e.Y, KeySizeSqu, Max_SquMinDistance);
                if (tmp.Flag && index_times<2)
                {//Comment: a restriction added: if you click the same key twice, then we only record the key for once 
                 //cause there is no English word that has trible same letters
                    WordPosStack[++top] = tmp;
                    Word += tmp.key;
                    key_old = tmp.key;
                    ++index_times;
                    //curve point cal:double letters
                    if (top > 1)
                    {
                        vectoriplus1.X = WordPosStack[top].RX - WordPosStack[top - 1].RX;
                        vectoriplus1.Y = WordPosStack[top].RY - WordPosStack[top - 1].RY;
                        CurvePointIndex[top - 1] = 1;
                        vectori = vectoriplus1;
                    }
                    else if (top == 1)
                    {
                        vectori.X = WordPosStack[top].RX - WordPosStack[top - 1].RX;
                        vectori.Y = WordPosStack[top].RY - WordPosStack[top - 1].RY;
                        CurvePointIndex[top - 1] = 1;
                    }
                    else
                    {//top=0, theorical, no possible this will happen

                    }
                }
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (CatchStart)
            {
                Graphics dc = this.CreateGraphics();
                dc.FillEllipse(new SolidBrush(Color.BlueViolet), e.X, e.Y, 10, 12);
                ColorCord tmp = BoardKey1.ConvCoord2String(PositionLeft, PositionRight, e.X, e.Y, KeySizeSqu, SquMinDistance);
                if ((tmp.key != key_old) && tmp.Flag)
                {
                    Word += tmp.key;
                    key_old = tmp.key;
                    WordPosStack[++top] = tmp;
                    index_times = 1;
                    FontInfo KeyFontFormat1 = new FontInfo("Arial", 12f, FontStyle.Bold);
                    BoardKey1.KeyIndexPlus(WordPosStack[top], KeyFontFormat1, KeySizeSqu, index_times);
                    //Curve point cal
                    if (top > 1)
                    {
                        vectoriplus1.X = WordPosStack[top].RX - WordPosStack[top - 1].RX;
                        vectoriplus1.Y = WordPosStack[top].RY - WordPosStack[top - 1].RY;
                        if ((vectoriplus1.X - vectori.X == 0) && (vectoriplus1.Y - vectori.Y == 0))
                        {
                            CurvePointIndex[top - 1] = 0;
                        }
                        else
                        {
                            CurvePointIndex[top - 1] = 1;
                        }
                        vectori = vectoriplus1;
                    }
                    else if (top == 1)
                    {
                        vectori.X = WordPosStack[top].RX - WordPosStack[top - 1].RX;
                        vectori.Y = WordPosStack[top].RY - WordPosStack[top - 1].RY;
                        CurvePointIndex[top - 1] = 1;
                    }
                    else
                    {//top=0, there is only one point, then do nothing

                    }
                }
                if (tmp.key == " ")
                {
                    CatchStart = false;
                }
                dc.Dispose();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && top>-1)
            {
                CatchStart = false;
                CurvePointIndex[0] = 1;
                CurvePointIndex[top] = 1;
                if(WordPosStack[top].key == " " && top>0)
                {
                    CurvePointIndex[--top] = 1;
                }
                
                if (top == 0 && WordPosStack[top].key == " ")
                {
                    this.textBox1.Paste(" ");
                }
                else
                {
                    bool Flag = true;
                    //Word find
                WordFind:
                    List<string> Result = new List<string>();
                    Result.Clear();
                    Result = Org_Word.Find(Word.ToLower(), CurvePointIndex);
                    this.listBox1.Items.Clear();
                    string tmp = "";
                    int min_lenth = Word.Length;
                    if (Result.Count != 0)
                    {    
                        foreach (string ResultWord in Result)
                        {
                            this.listBox1.Items.Add(ResultWord.ToUpper());
                            if (Word.Length - ResultWord.Length < min_lenth)
                                tmp = ResultWord.ToUpper();
                        }
                    }
                    else
                    {
                        if (Flag)
                        {
                            CurvePointIndex = new int[MaxLenthWord];
                            CurvePointIndex[0] = CurvePointIndex[top] = 1;
                            Flag = false;
                            goto WordFind;
                        }
                        this.listBox1.Items.Add(Word);
                        tmp = Word;
                    }
                    input_buffer[++top_buffer] = tmp;

                    if (tmp.Length > 1)
                    {
                        buffer += tmp + " ";
                        this.textBox1.Paste(tmp + " ");
                    }
                    else if(tmp.Length == 1)
                    {
                        buffer += tmp;
                        this.textBox1.Paste(tmp);
                    }
                    else
                    { }
                }
                //Create a new word string
                Graphics hc = this.CreateGraphics();
                hc.Clear(this.BackColor);// if need to save path, then comment this line
                FontInfo KeyFontFormat1 = new FontInfo("Arial", 12f, FontStyle.Bold);
                BoardKey1 = new BoardKey(PositionLeft, PositionRight, KeySizeSqu,hc, KeyFontFormat1);
                Word = "";
                top = -1;
            }
            if(e.Button == MouseButtons.Right && CatchStart && top>-1)
            {
                FontInfo KeyFontFormat1 = new FontInfo("Arial", 12f, FontStyle.Bold);
                BoardKey1.KeyIndexPlus(WordPosStack[top], KeyFontFormat1, KeySizeSqu, index_times);
            }
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 2)
            {
                if( (!this.listBox1.SelectedIndex.Equals(-1)) && top_buffer!=-1)
                {
                    string value = this.listBox1.SelectedItem.ToString();

                    if (value != input_buffer[top_buffer])
                    {
                        input_buffer[top_buffer] = value;
                        buffer = "";
                        for (int i = 0; i < top_buffer+1; ++i)
                            buffer += input_buffer[i] + " ";
                        this.textBox1.Clear();
                        this.textBox1.Paste(buffer);
                        this.listBox1.Items.Clear();  
                    }
                    else
                    {
                        this.listBox1.Items.Clear(); 
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox1.Clear();
            this.listBox1.Items.Clear();
            top_buffer = -1;
            buffer = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(top_buffer!=-1)
            {
                --top_buffer;
                buffer = "";
                for (int i = 0; i < top_buffer+1; ++i)
                    buffer += input_buffer[i] + " ";
                this.textBox1.Clear();
                this.textBox1.Paste(buffer);
            }
            this.listBox1.Items.Clear(); 
        }

        private void button3_Click(object sender, EventArgs e)
        {
            L_Distance p = new L_Distance();
            string s = this.textBox2.Text.ToUpper();
            string t = this.textBox1.Text.ToUpper();
            this.textBox3.Text = p.LevenshteinDistance(s, t).ToString();
        }


    }
}
