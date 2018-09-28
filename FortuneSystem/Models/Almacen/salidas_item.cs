using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Almacen
{
    public class salidas_item
    {
        public int id_salida { get; set; }
        public int id_salida_item { get; set; }
        public string amt_item { get; set; }
        public string estilo { get; set; }
        public int total { get; set; }
        public string talla { get; set; }
    }
}