using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using MIEForm.FileService;

namespace MIEForm
{
    public partial class Form1 : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Form1()
        {
            log.Info("Application started.");
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
            {
                label2.Text = file;
                log.Info($"File {file} successfully chosen.");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel2.Text = "";
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
                log.Error("Error because file for changing was not chosen.");
                return;
            }

            FileStream openedFileStream = client.OpenFileFromDir(filePath) as FileStream;

            FileData fileData = new FileData();
            try
            {
                fileData.stream = openedFileStream;
                fileData.fileName = Path.GetFileName(openedFileStream.Name);
                log.Info("FileData successfully created.");
            }
            catch (Exception ex)
            {
                log.Error("Exception while creation FileData.", ex);
                throw ex;
            }

            List<string> selectedItems = new List<string>();
            if (checkedListBox1.CheckedItems.Count > 0)
            {
                foreach (var ob in checkedListBox1.CheckedItems)
                {
                    selectedItems.Add(ob.ToString());
                    log.Info($"{ob.ToString()} was chosen as a directory for installation.");
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
                log.Error("Directories for file installation was not chosen.");
                return;
            }

            try
            {
                fileService.UploadFileToServer(fileData.fileName, fileData.stream);
                log.Info($"File {fileData.fileName} successfully uploaded to server.");
            }
            catch (Exception ex)
            {
                log.Error($"Exception while uploading {fileData.fileName} to server.", ex);
            }

            string resultOfOperation = "NONE";
            try
            {
                resultOfOperation = fileService.FileInstall(fileData.fileName, selectedItems.ToArray()).ToString();
                log.Info($"Operation finished with status {resultOfOperation}.");
            }
            catch (Exception ex)
            {
                log.Error("Exception while change file on server (see server logs).", ex);
            }
            toolStripStatusLabel2.Text = resultOfOperation;
            openedFileStream.Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                DialogResult result = openFileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    label2.Text = openFileDialog.FileName;
                }

                log.Info("File successfully opened.");
            }
            catch (Exception ex)
            {
                log.Error("Error while opening file.", ex);
            }


        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "there will be instruction how to use this programm...",
                "Help",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);
            log.Info("Help window successfully opened.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
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
                if (checkedListBox1.Items.Count == 0)
                {
                    MessageBox.Show(
                        "There are no directories where file exists.",
                        "Important!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.DefaultDesktopOnly);
                    log.Info("No directories found where file exists.");
                    label2.Text = "";
                }
                else
                    log.Info("Paths of file exists on server successfully got.");
            }
            catch (Exception ex)
            {
                log.Error("Exception while trying to get paths where file exists on server.", ex);
            }
            
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "there will be information about this programm...",
                "About",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);
            log.Info("About window successfully opened.");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.Items.Count > 0)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, true);
                }
            }
            else
            {
                MessageBox.Show(
                    "Sorry, but list is empty.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.CheckedItems.Count > 0)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                }
            }
            else
            {
                MessageBox.Show(
                    "Sorry, but checked items count is 0.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
            }
        }
    }
}
