using System;
using System.Collections.Generic;
using System.IO;

namespace Server
{
    public class FileTransfer : IFileTransfer
    {
        const string pathForUploadFileToServer = @"C:\Users\oleksandrm\Test\";
        const string pathForInstalling = @"C:\Users\oleksandrm\Test";
        public EStatusOfOperation BackupAndChangeFiles(string fileName)
        {
            string pathOfFileForInstalling = Path.Combine(pathForUploadFileToServer, fileName);
            string[] allDirectoriesFromPath = Directory.GetDirectories(pathForInstalling);
            List<string> directoriesWhereFileExists = new List<string>();
            if (File.Exists(pathOfFileForInstalling))
            {
                try
                {
                    foreach (string directory in allDirectoriesFromPath)
                    {
                        directoriesWhereFileExists.AddRange(Directory.GetFiles(directory, fileName));
                    }

                    foreach (string oldName in directoriesWhereFileExists)
                    {
                        string newName = oldName + "_backup";
                        if (File.Exists(newName))
                            File.Delete(newName); 
                        File.Move(oldName, newName);
                    }

                    try
                    {
                        foreach (string directory in allDirectoriesFromPath)
                        {
                            if (File.Exists(directory + "\\" + fileName + "_backup"))
                                File.Copy(pathOfFileForInstalling, directory + "\\" + fileName);
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                        return EStatusOfOperation.FAILWHILECHANGEFILES;
                    }
                }
                catch
                {
                    return EStatusOfOperation.FAILWHILEBACKUP;
                }
                return EStatusOfOperation.SUCCESSFULL;
            }
            else
            {
                return EStatusOfOperation.FILEDOESNOTEXISTS;
            }
        }

        public void UploadFileToServer(FileData fileData)
        {
            FileStream serverStream = null;
            Stream clientStream = fileData.stream;
            string file = Path.Combine(pathForUploadFileToServer, fileData.fileName);
            try
            {
                using (serverStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    const int bufferLen = 1024;
                    byte[] buffer = new byte[bufferLen];
                    int count = 0;
                    while ((count = clientStream.Read(buffer, 0, bufferLen)) > 0)
                    {
                        serverStream.Write(buffer, 0, count);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                serverStream.Close();
                clientStream.Close();
            }
        }
    }
}