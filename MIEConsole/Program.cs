using System;
using System.IO;

namespace MIEConsole 
{
    internal class Program 
    {
        static void Main(string[] args)
        {
            Client.Client client = new Client.Client();
            FileService.FileTransferClient fileService = new FileService.FileTransferClient();

            Console.WriteLine("Please, write full path to file you need to upload");
            string filePath = Console.ReadLine();
            
            FileStream openedFile = client.OpenFileFromDir(filePath);
            Console.WriteLine(fileService.UploadFile(filePath, openedFile));
            
        }
    }
}