using System;
using System.IO;
using System.ServiceProcess;
using Client;
using MIEConsole.FileService;
using static System.Net.WebRequestMethods;

namespace MIEConsole 
{
    internal class Program 
    {
        static void Main(string[] args)
        {
            Client.Client client = new Client.Client();
            FileTransferClient fileService = new FileTransferClient();
            Console.WriteLine("Please, write full path to file you need to upload");
            string filePath = Console.ReadLine();
            FileStream openedFileStream = client.OpenFileFromDir(filePath) as FileStream;

            FileData fileData = new FileData();
            fileData.stream = openedFileStream;
            fileData.fileName = Path.GetFileName(openedFileStream.Name);

            fileService.UploadFileToServer(fileData.fileName, fileData.stream);
            string resultOfOperation = fileService.FileInstall(fileData.fileName).ToString();
            Console.WriteLine("The result of operation is " + resultOfOperation);
            openedFileStream.Close();
        }
    }
}