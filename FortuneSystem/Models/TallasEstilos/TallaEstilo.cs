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

namespace FortuneSystem.Models.TallasEstilos
{
    public class TallaEstilo
    {
        [Display(Name = "")]
        public int id_po_summary { get; set; }
        [Display(Name = "ID")]
        public int id_recibo { get; set; }
        [Display(Name = "TOTAL")]
        public int total { get; set; }
        [Display(Name = "PACKING")]
        public string packing { get; set; }
        [Display(Name = "FECHA")]
        public string fecha { get; set; }
        [Display(Name = "ORIGEN")]
        public string origen { get; set; }
        [Display(Name = "MATERIAL")]
        public string material { get; set; }
        [Display(Name = "NOTAS")]
        public string notas{ get; set; }
        [Display(Name = "RECIBIÓ")]
        public string usuario_recibio { get; set; }
        public string staging { get; set; }
        public virtual Size Tallas { get; set; }
        public List<Size> ListaTallas { get; set; }

    }
}