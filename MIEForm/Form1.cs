using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MIEForm.FileService;

namespace MIEForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) 
                e.Effect = DragDropEffects.Copy;
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
                label2.Text = file;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Client.Client client = new Client.Client();
            FileTransferClient fileService = new FileTransferClient();
            string filePath = label2.Text;
            if (filePath == "" || filePath == null)
            {
                MessageBox.Show(
                    "Please choose file you need to replace!",
                    "Important!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            FileStream openedFileStream = client.OpenFileFromDir(filePath) as FileStream;

            FileData fileData = new FileData();
            fileData.stream = openedFileStream;
            fileData.fileName = Path.GetFileName(openedFileStream.Name);


            List<string> selectedItems = new List<string>();
            if (checkedListBox1.CheckedItems.Count > 0)
            {
                foreach (var ob in checkedListBox1.CheckedItems)
                {
                    selectedItems.Add(ob.ToString());
                }
            }
            else
            {
                MessageBox.Show(
                    "Please choose directory!",
                    "Important!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            fileService.UploadFileToServer(fileData.fileName, fileData.stream);
            string resultOfOperation = fileService.FileInstall(fileData.fileName, selectedItems.ToArray()).ToString();
            toolStripStatusLabel2.Text = resultOfOperation;
            openedFileStream.Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                label2.Text = openFileDialog.FileName;
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "For replacing file just drag and drop it to the program window or select File->Open and choose needed one. Then press Replace button and wait for the result.",
                "Help",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
            Client.Client client = new Client.Client();
            FileTransferClient fileService = new FileTransferClient();
            string filePath = label2.Text;
            if (filePath == "" || filePath == null)
            {
                MessageBox.Show(
                    "Please choose file you need to replace!",
                    "Important!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            FileStream openedFileStream = client.OpenFileFromDir(filePath) as FileStream;

            FileData fileData = new FileData();
            fileData.stream = openedFileStream;
            fileData.fileName = Path.GetFileName(openedFileStream.Name);
            checkedListBox1.Items.AddRange(fileService.GetDirectoriesWithFile(fileData.fileName));

           
        }
    }
}
