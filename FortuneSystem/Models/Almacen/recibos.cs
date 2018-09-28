using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace FortuneSystem.Models.Almacen
{
    public class recibos
    {
        public int id_recibo { get; set; }
        public string po { get; set; }
        public string fecha { get; set; }
        public int total { get; set; }
        public int id_usuario { get; set; }
        public int id_sucursal { get; set; }
        public int id_origen { get; set; }

        public virtual recibos_item ri { get; set; }
        public List<recibos_item> lista_recibos_item { get; set; }

    }
}