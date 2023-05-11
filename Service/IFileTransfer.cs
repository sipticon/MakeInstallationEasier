﻿using System;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Service
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