using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace EEditor
{
    public partial class ColorSelection : Form
    {
        public Color color { get; set; }
        private Bitmap bmp = new Bitmap(256, 256);
        public ColorSelection()
        {
            InitializeComponent();
        }

        private void txtbHex_TextChanged(object sender, EventArgs e)
        {
            
            if (Regex.IsMatch(txtbHex.Text, "^#[0-9A-Fa-f]{6}$"))
            {
                color = ColorTranslator.FromHtml(txtbHex.Text);
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    gr.Clear(color);
                }
                pictureBox1.Image = bmp;
            }


        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                color = colorDialog.Color;
                txtbHex.Text = String.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    gr.Clear(color);
                }
                pictureBox1.Image = bmp;
            }
        }
    }
}
