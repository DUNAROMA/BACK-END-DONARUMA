using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonarumaAPI_Data.Interfaces
{
    public interface IEncriptacionService
    {
        string Encriptar(string texto);
        string Desencriptar(string textoCifrado);
    }
}
