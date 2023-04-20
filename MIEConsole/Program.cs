using System;
using System.Collections.Generic;
using System.IO;
using MIEConsole.FileService;

namespace MIEConsole 
{
    internal class Program 
    {
        static void Main(string[] args)
        {
            Client.Client client = new Client.Client();
            FileTransferClient fileService = new FileTransferClient();
            
            Console.WriteLine("Please, write full path to file you need to upload");
            string filePath = @"C:\Users\oleksandrm\inputfile.txt";

            FileStream openedFileStream = client.OpenFileFromDir(filePath) as FileStream;

            FileData fileData = new FileData();
            fileData.stream = openedFileStream;
            fileData.fileName = Path.GetFileName(openedFileStream.Name);

            fileService.UploadFileToServer(fileData.fileName, fileData.stream);
            string resultOfOperation = fileService.BackupAndChangeFiles(fileData.fileName).ToString();
            Console.WriteLine("The result of operation is " + resultOfOperation);

            openedFileStream.Close();
        }
    }
}