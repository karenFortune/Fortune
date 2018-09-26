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
using FortuneSystem.Models.Sizes;

namespace FortuneSystem.Models.Sizes
{
    public class Size
    {
        [Display(Name = "")]
        public int id_recibo{ get; set; }//PARA LAS TABLAS
        [Display(Name = "")]
        public int id_size { get; set; }
        [Display(Name = "TALLA")]
        public string talla { get; set; }
        [Display(Name = "CANTIDAD")]
        public int cantidad { get; set; }
        [Display(Name = "")]
        public string estilo { get; set; }
        [Display(Name = "PRUEBA")]
        public int pruebas { get; set; }
        [Display(Name = "EXTRAS")]
        public int extras{ get; set; }
        public int ejemplos { get; set; }
        public int id_po_summary { get; set; }
        public int totales { get; set; }
        public int total_talla { get; set; }


    }
}