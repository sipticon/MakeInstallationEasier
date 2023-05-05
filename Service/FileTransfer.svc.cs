using FileLockInfo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.ServiceProcess;
using System.Web.Services.Description;

namespace Server
{
    public class FileTransfer : IFileTransfer
    {
        string pathForUploadFileToServer = Directory.GetCurrentDirectory(); 
        const string pathForInstalling = @"D:\Program files\NICE Systems";
        public EStatusOfOperation FileInstall(string fileName)
        {
            string pathOfFileForInstalling = Path.Combine(pathForUploadFileToServer, fileName);
            string[] allDirectoriesFromPath = Directory.GetDirectories(pathForInstalling, "*", searchOption: SearchOption.AllDirectories);
            List<string> directoriesWhereFileExists = new List<string>();
            List<Process> locks = new List<Process>();
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
                        FileInfo fileInfo = new FileInfo(oldName);
                        if (fileInfo.GetLifetimeService() != null)
                        {
                            try
                            {
                                StopWindowsService(fileInfo.GetLifetimeService().ToString());

                                BackupAndChangeFiles(oldName, allDirectoriesFromPath, pathOfFileForInstalling);

                                StartWindowsService(fileInfo.GetLifetimeService().ToString());
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                        else
                            BackupAndChangeFiles(oldName, allDirectoriesFromPath, pathOfFileForInstalling);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                    return EStatusOfOperation.FAILWHILEBACKUP;
                }
                return EStatusOfOperation.SUCCESSFULL;
            }
            else
            {
                return EStatusOfOperation.FILEDOESNOTEXISTS;
            }
        }

        public void BackupAndChangeFiles(string oldName, string[] allDirectoriesFromPath, string pathOfFileForInstalling)
        {
            string fileName = Path.GetFileName(oldName);
            string newName = oldName + "_backup";
            List<Process> locks = Win32Processes.GetProcessesLockingFile(newName);
            FileInfo fileInfo = new FileInfo(newName);
            if (fileInfo.GetLifetimeService() != null)
            {
                StopWindowsService(fileInfo.GetLifetimeService().ToString());
                File.Delete(newName);
                StartWindowsService(fileInfo.GetLifetimeService().ToString());
            }
            else if (locks.Count != 0)
            {
                foreach (var proc in locks)
                {
                    proc.Kill(); 
                    proc.WaitForExit(1000);
                    File.Delete(newName);
                    proc.Start();
                }
            }
            else if(File.Exists(newName))
                File.Delete(newName);

            File.Move(oldName, newName);
            try
            {
                foreach (string directory in allDirectoriesFromPath)
                {
                    if (File.Exists(directory + "\\" + fileName + "_backup") && !File.Exists(directory + "\\" + fileName))
                        File.Copy(pathOfFileForInstalling, directory + "\\" + fileName);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void UploadFileToServer(FileData fileData)
        {
            FileStream serverStream = null;
            Stream clientStream = fileData.stream;
            string file = Path.Combine(pathForUploadFileToServer, fileData.fileName);
            using (serverStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                int bufferLen = 65000;
                byte[] buffer = new byte[bufferLen];
                int count = 0;
                while ((count = clientStream.Read(buffer, 0, bufferLen)) > 0)
                {
                    serverStream.Write(buffer, 0, count);
                }
            }
            serverStream.Close();
            clientStream.Close();
        }

        private void StartWindowsService(string serviceName)
        {
            ServiceController serviceController = new ServiceController(serviceName);
            TimeSpan timeout = TimeSpan.FromMilliseconds(1000);
            serviceController.Start();
            serviceController.WaitForStatus(ServiceControllerStatus.Running, timeout);
        }

        private void StopWindowsService(string serviceName)
        {
            ServiceController serviceController = new ServiceController(serviceName);
            TimeSpan timeout = TimeSpan.FromMilliseconds(1000);
            serviceController.Stop();
            serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
        }
    }
}