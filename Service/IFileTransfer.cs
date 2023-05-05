﻿using System;
using System.IO;
using System.ServiceModel;

namespace Server
{
    [ServiceContract]
    public interface IFileTransfer
    {
        [OperationContract]
        EStatusOfOperation FileInstall(string filePath);

        [OperationContract]
        void UploadFileToServer(FileData fileData);

        [OperationContract]
        void BackupAndChangeFiles(string oldName, string[] allDirectoriesFromPath, string pathOfFileForInstalling);
    }
    [MessageContract]
    public class FileData : IDisposable
    {
        [MessageHeader(MustUnderstand = true)]
        public string fileName;

        [MessageBodyMember(Order = 1)]
        public Stream stream;

        public void Dispose()
        {
            if (stream != null)
            {
                stream.Close();
                stream = null;
            }
        }
    }
}