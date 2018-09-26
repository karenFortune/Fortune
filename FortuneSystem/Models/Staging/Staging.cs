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

namespace FortuneSystem.Models.Staging
{
    /*public class Recibo {
        [Display(Name = "RECIBO")]
        public int id_recibo { get; set; }//PARA LAS TABLAS
        [Display(Name = "# PEDIDO")]
        public string id_po { get; set; }
        [Display(Name = "TOTAL")]
        public int total { get; set; }        
    }*/
    public class Staging
    {
        [Display(Name = "RECIBO")]
        public int id_recibo{ get; set; }//PARA LAS TABLAS
        [Display(Name = "# PEDIDO")]
        public int id_pedido { get; set; }
        [Display(Name = "STAGING")]
        public int id_staging { get; set; }
        [Display(Name = "FECHA ")]
        public string fecha { get; set; }
        [Display(Name = "USUARIO")]
        public int id_usuario { get; set; }
        [Display(Name = "TALLA")]
        public int id_talla { get; set; }
        [Display(Name = "PAIS")]
        public string pais{ get; set; }
        [Display(Name = "COLOR")]
        public int color { get; set; }
        [Display(Name = "PORCENTAJE")]
        public string porcentaje { get; set; }
        public int usuario { get; set;}
        [Display(Name = "ORDEN")]
        public string po { get; set; }
        [Display(Name = "TOTAL")]
        public int total { get; set; }
        public int id_size { get; set; }
        public string talla { get; set; }
        public string estilo { get; set; }
        public int id_po_summary { get; set; }
        public string color_description { get; set; }
        public string nombre_usuario { get; set; }
    }

    /*public class Caja_staging {
        public int cantidad { get; set; }
        public int id_staging { get; set; }
    }*/
}