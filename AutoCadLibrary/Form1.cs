using System;
//using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Data.SqlClient;
//using System.ComponentModel;
//using System.Collections;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

//using Autodesk.AutoCAD.Runtime;
//using Autodesk.Windows;
//using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using acd = Autodesk.AutoCAD.ApplicationServices.Application;



namespace AutoCadLibrary
{

    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        extern static IntPtr
            SetFocus(IntPtr hWnd);
        Editor ed = acd.DocumentManager.MdiActiveDocument.Editor;
        public Form1()
        {
            InitializeComponent();
        }
        public static Form1 s_ThisForm = null;
        public static Form1 ThisForm
        {
            get
            {
                if (s_ThisForm == null || s_ThisForm.IsDisposed)
                    s_ThisForm = new Form1();
                return s_ThisForm;
            }
        }
        static public void FormShow()
        {
            acd.ShowModelessDialog(ThisForm);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            SetFocus(acd.DocumentManager.MdiActiveDocument.Window.Handle);
            string[] strarr = new string[ThisForm.dataGridView1.Rows.Count];
            for (int i = 0; i<ThisForm.dataGridView1.Rows.Count - 1; i++)
            {
                strarr[i] = ThisForm.dataGridView1.Rows[i].Cells[0].Value.ToString() + "(" + ThisForm.textBox1.Text + ")" + ";" + ThisForm.dataGridView1.Rows[i].Cells[1].Value.ToString();
            }
            File.WriteAllLines(Path.GetTempPath() + "ViewArr("+ Environment.UserName +").txt", strarr, Encoding.UTF8);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetFocus(acd.DocumentManager.MdiActiveDocument.Window.Handle);
            ed.Document.SendStringToExecute("SELVIEW ", false, false, true);
        }
        public void dataGridPull(string zone, string[] data)
        {
            ed.WriteMessage("\nВЫПОЛНИЛСЯ...  !!! {0} !!!\n", zone);
            int[] intarr = Array.ConvertAll(data, int.Parse);
            Array.Sort(intarr);
            data = Array.ConvertAll(intarr, x => x.ToString());
            string[] arrstr = new string[2];
            arrstr[0] = zone;
            arrstr[1] = String.Join(";", intarr.Distinct());
            ThisForm.dataGridView1.Rows.Add(arrstr);
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (ThisForm.dataGridView1.Rows.Count > 1 && ThisForm.textBox1.Text != "")
                ThisForm.button1.Enabled = true;
            else
                ThisForm.button1.Enabled = false;
        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (ThisForm.dataGridView1.Rows.Count > 1 && ThisForm.textBox1.Text != "")
                ThisForm.button1.Enabled = true;
            else
                ThisForm.button1.Enabled = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (ThisForm.dataGridView1.Rows.Count > 1 && ThisForm.textBox1.Text != "")
                ThisForm.button1.Enabled = true;
            else
                ThisForm.button1.Enabled = false;
        }
    }
}
