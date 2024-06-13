using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileExplorerProject
{
    public partial class EditFile : Form
    {
        public EditFile()
        {
            InitializeComponent();
        }
        public EditFile(string path)
        {
            InitializeComponent();
            richTextBox1.LoadFile(path);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.LoadFile(openFileDialog1.FileName);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.SaveFile(saveFileDialog1.FileName);
                MessageBox.Show("File saved successfully.");
                Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
