using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pojjaman2_EncryptionFile.Interfaces
{
    public interface IServiceEncryptionFile
    {
        Task<string> Encryption(string filename, string textFile);
       
    }
}