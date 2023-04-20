using System;
using System.IO;
using System.ServiceModel;

namespace Client
{
    public class Client
    {

        public Stream OpenFileFromDir(string dir)
        {
            Stream fileStream = null;
            try
            {
                fileStream = new FileStream(dir, FileMode.Open, FileAccess.Read);
            }
            catch (Exception)
            {
                return null;
            }
            return fileStream;
        }
    }
}