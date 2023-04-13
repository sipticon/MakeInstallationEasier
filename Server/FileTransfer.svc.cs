using System;
using System.Collections.Generic;
using System.IO;

namespace Server
{
    public class FileTransfer : IFileTransfer
    {
        public EStatusOfOperation UploadFile(string fileName, FileStream stream)
        {
            FileStream file = stream;
            const string pathForInstalling = @"D:\C#\Test";
            if (file != null)
            {
                return EStatusOfOperation.SUCCESSFULL;

                string[] directories = Directory.GetDirectories(pathForInstalling);
                string[] fileNames = null;
                List<string> n = new List<string>();
                foreach (string directory in directories)
                {
                    fileNames = Directory.GetFiles(directory, fileName);
                    n.AddRange(fileNames);
                }
            }
            else
            {
                return EStatusOfOperation.FAILURE;
            }
        }
    }
}