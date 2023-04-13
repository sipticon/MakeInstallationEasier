using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace MIEConsole 
{
    internal class Program 
    {
        static void Main(string[] args)
        {
            FileService.FileTransferClient fileClient = new FileService.FileTransferClient();
            FileStream fStream = null;
            Console.WriteLine("Please, write full path to file you need to upload");
            string filePath = Console.ReadLine();

            try
            {
                fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine(fileClient.UploadFile(filePath, fStream));
        }
    }
}