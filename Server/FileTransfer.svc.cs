using System;
using System.Collections.Generic;
using System.IO;

namespace Server
{
    public class FileTransfer : IFileTransfer
    {
        private const string pathForInstalling = @"D:\C#\Test";
        public EStatusOfOperation UploadFile(string filePath, FileStream stream)
        {
            Stream file = stream;
            string fileName = Path.GetFileName(filePath);
            if (file != null)
            {
                
                string[] directories = Directory.GetDirectories(pathForInstalling);
                List<string> filePaths = new List<string>();
                try
                {
                    foreach (string directory in directories)
                    {
                        filePaths.AddRange(Directory.GetFiles(directory, fileName));
                    }

                    foreach (string oldName in filePaths)
                    {
                        string newName = oldName.Replace(".", "backup.");
                        File.Move(oldName, newName);
                    }

                    foreach (string directory in directories)
                    {
                        File.Copy(filePath, directory +"\\" + fileName);
                    }
                    return EStatusOfOperation.SUCCESSFULL;
                }
                catch (Exception e)
                {
                    return EStatusOfOperation.FAILURE;
                }
            }
            else
            {
                return EStatusOfOperation.FAILURE;
            }
        }
    }
}