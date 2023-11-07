using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.Diagnostics.Eventing.Reader;

namespace EEditor
{
    public partial class Statistics : Form
    {
        private Dictionary<int, int> bcount = new Dictionary<int, int>();
        private Dictionary<int, Dictionary<uint, int>> bcound = new Dictionary<int, Dictionary<uint, int>>();
        private Semaphore wait = new Semaphore(0, 1);
        public Statistics()
        {
            InitializeComponent();
        }

        private void Statistics_Load(object sender, EventArgs e)
        {
            panel1.AutoScroll = true;
            panel1.BackColor = MainForm.themecolors.accent;
            this.BackColor = MainForm.themecolors.background;
            this.ForeColor = MainForm.themecolors.foreground;
            foreach (Control cntrls in this.Controls)
            {
                if (cntrls.GetType() == typeof(RadioButton))
                {
                    ((RadioButton)cntrls).ForeColor = MainForm.themecolors.foreground;
                    ((RadioButton)cntrls).BackColor = MainForm.themecolors.accent;
                }
            }
            sortby(1);
        }

        private void sortby(int id)
        {
            panel1.Controls.Clear();
            bcount.Clear();
            bcound.Clear();
            for (int x = 0; x < MainForm.editArea.CurFrame.Width; x++)
            {
                for (int y = 0; y < MainForm.editArea.CurFrame.Height; y++)
                {
                    if (id >= 0 && id <= 3)
                    {
                        if (bcount.ContainsKey(MainForm.editArea.CurFrame.Foreground[y, x]))
                        {


                            bcount[MainForm.editArea.CurFrame.Foreground[y, x]] += 1;
                        }
                        if (!bcount.ContainsKey(MainForm.editArea.CurFrame.Foreground[y, x]))
                        {

                            bcount.Add(MainForm.editArea.CurFrame.Foreground[y, x], 1);
                        }
                    }
                    if (id == 0 || id == 4)
                    {
                        if (bcount.ContainsKey(MainForm.editArea.CurFrame.Background[y, x]) && MainForm.editArea.CurFrame.Background[y, x] != 0)
                        {
                            bcount[MainForm.editArea.CurFrame.Background[y, x]] += 1;

                        }
                        if (!bcount.ContainsKey(MainForm.editArea.CurFrame.Background[y, x]) && MainForm.editArea.CurFrame.Background[y, x] != 0)
                        {
                            bcount.Add(MainForm.editArea.CurFrame.Background[y, x], 1);
                        }

                        if (MainForm.editArea.CurFrame.BlockData7[y, x].ToString() != null && MainForm.editArea.CurFrame.Background[y, x] >= 631 && MainForm.editArea.CurFrame.Background[y, x] <= 633)

                        {

                            if (bcound.ContainsKey(MainForm.editArea.CurFrame.Background[y, x]))
                            {
                                if (bcound[MainForm.editArea.CurFrame.Background[y, x]].ContainsKey(MainForm.editArea.CurFrame.BlockData7[y, x]))
                                {
                                    bcound[MainForm.editArea.CurFrame.Background[y, x]][MainForm.editArea.CurFrame.BlockData7[y, x]] += 1;
                                }
                                else
                                {
                                    bcound[MainForm.editArea.CurFrame.Background[y, x]].Add(MainForm.editArea.CurFrame.BlockData7[y, x], 1);
                                }
                            }
                            else if (!bcound.ContainsKey(MainForm.editArea.CurFrame.Background[y, x]))
                            {
                                bcound.Add(MainForm.editArea.CurFrame.Background[y, x], new Dictionary<uint, int>() { { MainForm.editArea.CurFrame.BlockData7[y, x], 1 } });
                            }

                        }
                    }
                }
            }
            int position = 0, wposition = 4;
            foreach (var val in bcount)
            {
                PictureBox table = new PictureBox();
                ToolTip tp = new ToolTip();
                tp.SetToolTip(table, val.Key.ToString());
                table.Location = new Point(wposition, position + 4);
                table.Name = $"Table_ID{val.Key}";
                table.Size = new Size(60, 30);
                Bitmap bmp = new Bitmap(table.Width, table.Height);
                Bitmap block = new Bitmap(16, 16);
                if (val.Key < 500 || val.Key >= 1000)
                {
                    if (MainForm.ForegroundBlocks.ContainsKey(val.Key) && (id == 0 || id == 1))
                    {
                        block = MainForm.ForegroundBlocks[val.Key];
                        using (Graphics gr = Graphics.FromImage(bmp))
                        {
                            gr.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(5, 5, 100, 50));
                            gr.DrawRectangle(new Pen(Color.White), new Rectangle(5, 5, 54, 24));
                            gr.DrawImage(block, new Point(8, 8));
                            //gr.DrawString($"{val.Value}", new Font("Arial", 8, FontStyle.Regular), new SolidBrush(Color.Black), new Point(25, 9));
                            gr.DrawString($"{val.Value}", new Font("Arial", 8, FontStyle.Regular), new SolidBrush(Color.White), new Point(24, 8));
                        }
                        table.Image = bmp;
                        wposition += 60;
                        if (wposition == 244) //244
                        {
                            wposition = 4;
                            position += 30;
                        }
                        panel1.Controls.Add(table);
                    }

                    if (MainForm.ActionBlocks.ContainsKey(val.Key) && (id == 0 || id == 2))
                    {

                        block = MainForm.ActionBlocks[val.Key];
                        using (Graphics gr = Graphics.FromImage(bmp))
                        {
                            gr.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(5, 5, 100, 50));
                            gr.DrawRectangle(new Pen(Color.White), new Rectangle(5, 5, 54, 24));
                            gr.DrawImage(block, new Point(8, 8));
                            //gr.DrawString($"{val.Value}", new Font("Arial", 8, FontStyle.Regular), new SolidBrush(Color.Black), new Point(25, 9));
                            gr.DrawString($"{val.Value}", new Font("Arial", 8, FontStyle.Regular), new SolidBrush(Color.White), new Point(24, 8));
                        }
                        table.Image = bmp;
                        wposition += 60;
                        if (wposition == 244) //244
                        {
                            wposition = 4;
                            position += 30;
                        }
                        panel1.Controls.Add(table);
                    }
                    if (MainForm.DecorationBlocks.ContainsKey(val.Key) && (id == 0 || id == 3))
                    {
                        block = MainForm.DecorationBlocks[val.Key];
                        using (Graphics gr = Graphics.FromImage(bmp))
                        {
                            gr.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(5, 5, 100, 50));
                            gr.DrawRectangle(new Pen(Color.White), new Rectangle(5, 5, 54, 24));
                            gr.DrawImage(block, new Point(8, 8));
                            //gr.DrawString($"{val.Value}", new Font("Arial", 8, FontStyle.Regular), new SolidBrush(Color.Black), new Point(25, 9));
                            gr.DrawString($"{val.Value}", new Font("Arial", 8, FontStyle.Regular), new SolidBrush(Color.White), new Point(24, 8));
                        }
                        table.Image = bmp;
                        wposition += 60;
                        if (wposition == 244) //244
                        {
                            wposition = 4;
                            position += 30;
                        }
                        panel1.Controls.Add(table);
                    }

                }
                else
                {
                    if (MainForm.BackgroundBlocks.ContainsKey(val.Key) && val.Key != 0 && (id == 0 || id == 4))
                    {
                        block = MainForm.BackgroundBlocks[val.Key];
                        using (Graphics gr = Graphics.FromImage(bmp))
                        {
                            gr.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(5, 5, 100, 50));
                            gr.DrawRectangle(new Pen(Color.White), new Rectangle(5, 5, 54, 24));
                            gr.DrawImage(block, new Point(8, 8));
                            //gr.DrawString($"{val.Value}", new Font("Arial", 8, FontStyle.Regular), new SolidBrush(Color.Black), new Point(25, 9));
                            gr.DrawString($"{val.Value}", new Font("Arial", 8, FontStyle.Regular), new SolidBrush(Color.White), new Point(24, 8));
                        }
                        table.Image = bmp;
                        wposition += 60;
                        if (wposition == 244) //244
                        {
                            wposition = 4;
                            position += 30;
                        }
                        panel1.Controls.Add(table);
                    }
                    else
                    {
                    }


                }


            }
            if (bcound.Count > 0 && (id == 0 || id == 4))
            {
                if (bcound.ContainsKey(631) || bcound.ContainsKey(632) || bcound.ContainsKey(633))
                {
                    foreach (var val in bcound)
                    {
                        foreach (var kvp in bcound[val.Key])
                        {
                            uint innerKey = kvp.Key;
                            int value = kvp.Value;
                            PictureBox table = new PictureBox();
                            ToolTip tp = new ToolTip();
                            tp.SetToolTip(table, $"{val.Key}");
                            table.Location = new Point(wposition, position + 4);
                            table.Name = $"Table_ID_{val.Key}_{innerKey}";
                            table.Size = new Size(60, 30);
                            Bitmap bmp = new Bitmap(table.Width, table.Height);
                            Bitmap block = new Bitmap(16, 16);
                            block = MainForm.ColoredBGBlocks[val.Key];
                            using (Graphics gr = Graphics.FromImage(bmp))
                            {
                                gr.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(5, 5, 100, 50));
                                gr.DrawRectangle(new Pen(Color.White), new Rectangle(5, 5, 54, 24));
                                gr.FillRectangle(new SolidBrush(bdata.UIntToColor(innerKey)), new Rectangle(8, 8, 16, 16));
                                gr.DrawImage(block, new Point(8, 8));
                                //gr.DrawString($"{val.Value}", new Font("Arial", 8, FontStyle.Regular), new SolidBrush(Color.Black), new Point(25, 9));
                                gr.DrawString($"{value}", new Font("Arial", 8, FontStyle.Regular), new SolidBrush(Color.White), new Point(24, 8));
                            }
                            table.Image = bmp;
                            wposition += 60;
                            if (wposition == 244) //244
                            {
                                wposition = 4;
                                position += 30;
                            }
                            panel1.Controls.Add(table);
                        }
                    }
                }
            }
            //Dictionary<uint, int> dic = bcount[633];
            /*PictureBox table = new PictureBox();
            ToolTip tp = new ToolTip();
            tp.SetToolTip(table, val.Key.ToString());
            table.Location = new Point(wposition, position + 4);
            table.Name = $"Table_ID{val.Key}";
            table.Size = new Size(60, 30);
            Bitmap bmp = new Bitmap(table.Width, table.Height);
            Bitmap block = new Bitmap(16, 16);
            Console.WriteLine(val.Key);
            block = MainForm.ColoredBlocks[val.Key];
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                gr.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(5, 5, 100, 50));
                gr.DrawRectangle(new Pen(Color.White), new Rectangle(5, 5, 54, 24));
                gr.DrawImage(block, new Point(8, 8));
                //gr.DrawString($"{val.Value}", new Font("Arial", 8, FontStyle.Regular), new SolidBrush(Color.Black), new Point(25, 9));
                gr.DrawString($"{val.Value}", new Font("Arial", 8, FontStyle.Regular), new SolidBrush(Color.White), new Point(24, 8));
            }
            table.Image = bmp;
            wposition += 60;
            if (wposition == 244) //244
            {
                wposition = 4;
                position += 30;
            }
            panel1.Controls.Add(table);*/
        }



        private void fgradioButton_CheckedChanged(object sender, EventArgs e)
        {

            sortby(1);
        }

        private void actradioButton_CheckedChanged(object sender, EventArgs e)
        {

            sortby(2);
        }

        private void decorradioButton_CheckedChanged(object sender, EventArgs e)
        {

            sortby(3);
        }

        private void bgradioButton_CheckedChanged(object sender, EventArgs e)
        {
            sortby(4);
        }
    }
}
