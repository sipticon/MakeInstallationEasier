using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using static System.Net.WebRequestMethods;

namespace MIEConsole 
{
    internal class Program 
    {
        static void Main(string[] args)
        {
            FileService.FileTransferClient fileClient = new FileService.FileTransferClient();
            FileStream fStream = null;

            const string pathForInstalling = @"D:\C#\Test";
           
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
            Console.WriteLine(fileClient.UploadFile(Path.GetFileName(filePath), fStream));

            string fileName = Path.GetFileName(filePath);

            string[] directories = Directory.GetDirectories(@"D:\C#\Test");
            string[] fileNames = null;
            List<string> n = new List<string>();
            foreach (string directory in directories)
            {
                fileNames = Directory.GetFiles(directory, fileName);
                n.AddRange(fileNames);
            }

            string str = "";
            foreach (string name in n)
            {
                str += name + ",";
            }
            Console.WriteLine(str);
        }
    }
}