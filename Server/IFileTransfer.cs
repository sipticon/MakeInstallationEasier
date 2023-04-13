using System.IO;
using System.ServiceModel;

namespace Server
{
    [ServiceContract]
    public interface IFileTransfer
    {
        [OperationContract]
        EStatusOfOperation UploadFile(string fileName, FileStream stream);
    }
}