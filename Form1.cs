using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scanner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CodeBox.AcceptsTab = true;
        }

        private void ScanButton_Click(object sender, EventArgs e)
        {
            TokensTable.Rows.Clear(); // Reset the tokens table at each scan
            ErrorsTable.Rows.Clear(); // Reset the errors table at each scan
            ScanLogic.ScanCode(CodeBox.Text);
            foreach (KeyValuePair<string, string> t in ScanLogic.ShowTokens())
            {
                TokensTable.Rows.Add(t.Key, t.Value);
            }
            foreach (KeyValuePair<string, string> er in ScanLogic.ShowErrors())
            {
                ErrorsTable.Rows.Add(er.Key, er.Value);
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            CodeBox.Clear(); //  Clear the code box
            TokensTable.Rows.Clear(); // Clear the tokens table
            ErrorsTable.Rows.Clear(); // Clear the errors table
        }

        private void UploadCodeButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                CodeBox.Clear();
                TokensTable.Rows.Clear();
                CodeBox.Text = File.ReadAllText(Dialog.FileName);
            }
        }

        private void TokensTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
