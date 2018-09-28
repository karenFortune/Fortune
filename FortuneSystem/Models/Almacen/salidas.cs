using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Almacen
{
    public class salidas
    {
        public int id_salida { get; set; }
        public string po { get; set; }
        public string fecha { get; set; }
        public int total { get; set; }
        public int id_usuario { get; set; }
        public int id_sucursal { get; set; }
        public int id_destino { get; set; }
        public int estado_probacion { get; set; }

        public virtual salidas_item si { get; set; }
        public List<salidas_item> lista_salidas_item { get; set; }
    }
}