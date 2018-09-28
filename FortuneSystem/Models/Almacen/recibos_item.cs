using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Almacen
{
    public class recibos_item
    {
        public int id_recibo { get; set; }
        public int id_recibo_item { get; set; }
        public int id_inventario { get; set; }
        public string amt_item { get; set; }
        public string estilo { get; set; }
        public int total { get; set; }
        public string mill_po { get; set; }
        public virtual recibos_cajas rc { get; set; }
        public List<recibos_cajas> lista_recibos_cajas { get; set; }
    }
}