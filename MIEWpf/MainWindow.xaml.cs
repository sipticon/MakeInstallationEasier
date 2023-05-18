using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using MIEWpf.FileService;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;
namespace MIEWpf
{
    public partial class MainWindow : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MainWindow()
        {
            log.Info("Application started.");
            InitializeComponent();
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                textFilePath.Text = file;
                log.Info($"File {file} successfully chosen.");
            }
        }

        private void Window_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
        }

        private void ReplaceButton_OnClick(object sender, RoutedEventArgs e)
        {
            statusOfOperation.Text = "";
            Client.Client client = new Client.Client();
            FileTransferClient fileService = new FileTransferClient();
            string filePath = textFilePath.Text;
            if (filePath == "" || filePath == null)
            {
                MessageBox.Show(
                    "Please choose file you need to replace!",
                    "Important!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
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
            if (checkedListBox.SelectedItems.Count > 0)
            {
                foreach (var ob in checkedListBox.SelectedItems)
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
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
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
            statusOfOperation.Text = resultOfOperation;
            openedFileStream.Close();
        }

        private void FindDirectoriesButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                checkedListBox.UnSelectAll();
                Client.Client client = new Client.Client();
                FileTransferClient fileService = new FileTransferClient();
                string filePath = textFilePath.Text;
                if (filePath == "" || filePath == null)
                {
                    MessageBox.Show(
                        "Please choose file you need to replace!",
                        "Important!",
                        MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);
                    return;
                }

                FileStream openedFileStream = client.OpenFileFromDir(filePath) as FileStream;

                FileData fileData = new FileData();
                fileData.stream = openedFileStream;
                fileData.fileName = Path.GetFileName(openedFileStream.Name);
                checkedListBox.ItemsSource = fileService.GetDirectoriesWithFile(fileData.fileName);
                if (checkedListBox.Items.Count == 0)
                {
                    MessageBox.Show(
                        "There are no directories where file exists.",
                        "Important!",
                        MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);
                    log.Info("No directories found where file exists.");
                    textFilePath.Text = "";
                }
                else
                    log.Info("Paths of file exists on server successfully got.");
            }
            catch (Exception ex)
            {
                log.Error("Exception while trying to get paths where file exists on server.", ex);
            }
        }

        private void SelectAllButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (checkedListBox.Items.Count > 0)
            {
                checkedListBox.SelectAll();
            }
            else
            {
                MessageBox.Show(
                    "Sorry, but list is empty.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ClearSelectionButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (checkedListBox.SelectedItems.Count > 0)
            {
                checkedListBox.UnSelectAll();
            }
            else
            {
                MessageBox.Show(
                    "Sorry, but checked items count is 0.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Open_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                    textFilePath.Text = openFileDialog.FileName;

                log.Info("File successfully opened.");
            }
            catch (Exception ex)
            {
                log.Error("Error while opening file.", ex);
            }
        }
        private void Help_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "To replace file you need to drag and drop it to window or select File->Open, than click Find directories and select needed. Choose Replace and wait for the result of operation. Good luck!",
                "Help",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            log.Info("Help window successfully opened.");
        }
        private void About_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "MakeInsrallationEasier it is a program for backup and replace files on server side. It also can stop and start services/processes.",
                "About",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            log.Info("About window successfully opened.");
        }
    }
}