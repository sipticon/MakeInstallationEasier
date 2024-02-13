using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;

namespace Service
{
    public class FileTransfer : IFileTransfer
    {
        string pathForUploadFileToServer = @"C:\Users\CURRENTUSER";
        const string pathForInstalling = @"C:\Users\CURRENTUSER\source";
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public EStatusOfOperation FileInstall(string fileName, List<string> pathOfExistsFiles)
        {
            string pathOfFileForInstalling = Path.Combine(pathForUploadFileToServer, fileName);
            List<Process> locks = new List<Process>();
            if (File.Exists(pathOfFileForInstalling))
            {
                try
                {
                    List<string> directoriesWhereFileExists = pathOfExistsFiles;
                    foreach (string oldName in directoriesWhereFileExists)
                    {
                        FileInfo fileInfo = new FileInfo(oldName);
                        if (fileInfo.GetLifetimeService() != null)
                        {
                            try
                            {
                                StopWindowsService(fileInfo.GetLifetimeService().ToString());

                                BackupAndChangeFiles(oldName, directoriesWhereFileExists.ToArray(), pathOfFileForInstalling);

                                StartWindowsService(fileInfo.GetLifetimeService().ToString());
                            }
                            catch (Exception ex)
                            {
                                log.Error("Exception while change file.",ex);
                                throw ex;
                            }
                        }
                        else
                            BackupAndChangeFiles(oldName, directoriesWhereFileExists.ToArray(), pathOfFileForInstalling);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Exception while change file.", ex);
                    return EStatusOfOperation.FAILWHILEBACKUP;
                    throw ex;
                }

                return EStatusOfOperation.SUCCESSFULL;
            }
            else
            {
                return EStatusOfOperation.FILEDOESNOTEXISTS;
            }
        }

        public void BackupAndChangeFiles(string oldName, string[] allDirectoriesFromPath,
            string pathOfFileForInstalling)
        {
            string fileName = Path.GetFileName(oldName);
            string newName = oldName + "_backup";
            List<Process> locks = Win32Processes.GetProcessesLockingFile(newName);
            FileInfo fileInfo = new FileInfo(newName);
            if (fileInfo.GetLifetimeService() != null)
            {
                StopWindowsService(fileInfo.GetLifetimeService().ToString());
                File.Delete(newName);
                log.Info($"File {newName} deleted successfully.");
                StartWindowsService(fileInfo.GetLifetimeService().ToString());
            }
            else if (locks.Count != 0)
            {
                foreach (var proc in locks)
                {
                    proc.Kill();
                    log.Info($"Process {proc} stopped successfully.");
                    proc.WaitForExit(1000);
                }
                File.Delete(newName);
                log.Info($"File {newName} deleted successfully.");
            }
            else if (File.Exists(newName))
            {
                File.Delete(newName);
                log.Info($"File {newName} deleted successfully.");
            }

            File.Move(oldName, newName);
            log.Info($"File {newName} created  successfully.");

            try
            {
                foreach (string directory in allDirectoriesFromPath)
                {
                    if (File.Exists(directory + "_backup") &&
                        !File.Exists(directory))
                    {
                        File.Copy(pathOfFileForInstalling, directory);
                        log.Info($"File {fileName}  successfully moved to {directory}.");
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception while creating backup.", e);
                throw e;
            }
        }

        public void UploadFileToServer(FileData fileData)
        {
            try
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
                log.Info($"File {fileData.fileName} uploaded to server successfully.");
            }
            catch (Exception ex)
            {
                log.Error($"Exception while uploading file {fileData.fileName} to server.", ex);
                throw ex;
            }
        }

        private void StartWindowsService(string serviceName)
        {
            try
            {
                ServiceController serviceController = new ServiceController(serviceName);
                TimeSpan timeout = TimeSpan.FromMilliseconds(1000);
                serviceController.Start();
                serviceController.WaitForStatus(ServiceControllerStatus.Running, timeout);
                log.Info($"Service {serviceName} started  successfully.");
            }
            catch (Exception ex)
            {
                log.Error($"Exception while starting service {serviceName}.", ex);
                throw ex;
            }
        }

        private void StopWindowsService(string serviceName)
        {
            try
            {
                ServiceController serviceController = new ServiceController(serviceName);
                TimeSpan timeout = TimeSpan.FromMilliseconds(1000);
                serviceController.Stop();
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                log.Info($"Service {serviceName} stopped  successfully.");
            }
            catch (Exception ex)
            {
                log.Error($"Exception while stopping service {serviceName}.", ex);
                throw ex;
            }
        }

        public List<string> GetDirectoriesWithFile(string fileName)
        {
            string[] allDirectoriesFromPath =
                Directory.GetDirectories(pathForInstalling, "*", searchOption: SearchOption.AllDirectories);
            List<string> directoriesWhereFileExists = new List<string>();
            foreach (string directory in allDirectoriesFromPath)
            {
                if(!directory.Contains("NPS Agent"))
                    directoriesWhereFileExists.AddRange(Directory.GetFiles(directory, fileName));
            }
            log.Info($"All directories where {fileName} exists sent to client successfully.");
            return directoriesWhereFileExists;
        }
    }
}