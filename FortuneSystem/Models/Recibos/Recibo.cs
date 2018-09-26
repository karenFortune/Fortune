using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace FortuneSystem.Models.Recibos
{
    public class Recibo
    {
        //RECIBO GENERAL
        [Display(Name = "PO")]
        public string id_po { get; set; }
        [Display(Name = "FECHA DE ORDEN")]
        public string fecha { get; set; }
        [Display(Name = "TOTAL DE UNIDADES")]
        public int total { get; set; }
        [Display(Name = "PEDIDO")]
        public int id_pedido { get; set; }
        public DateTime fecha_cancelacion { get; set; }
    }
}
