using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonarumaAPI_Model.MetodosdepagoModel
{
    public class MetodosDePago
    {
        public int IdMetodo { get; set; }
        public long IdUsuario { get; set; }
        public string StripePaymentMethodId { get; set; }
        public string Ultimos4 { get; set; }
        public string Marca { get; set; }
        public string VencimientoEncriptado { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
