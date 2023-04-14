using System;
using System.IO;

namespace Client
{
    public class Client
    {

        public FileStream OpenFileFromDir(string dir)
        {
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(dir, FileMode.Open, FileAccess.Read);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return fileStream;
        }
    }
}