using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;

namespace Service
{
    [ServiceContract]
    public interface IFileTransfer
    {
        [OperationContract]
        EStatusOfOperation FileInstall(string filePath, List<string> pathOfExistsFiles);

        [OperationContract]
        void UploadFileToServer(FileData fileData);

        [OperationContract]
        List<string> GetDirectoriesWithFile(string fileName);

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