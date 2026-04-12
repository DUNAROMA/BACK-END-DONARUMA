using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// DonarumaAPI_Model/ComprasModel/Compra.cs
namespace DonarumaAPI_Model.ComprasModel
{
    public class Compra
    {
        public long IdCompra { get; set; }
        public long FkIdUsuario { get; set; }
        public string Direccion { get; set; }
        public int? FkIdTarjeta { get; set; } // nullable, columna vieja
        public int? FkIdMetodo { get; set; } // nueva FK a MetodosDePago
    }
}
