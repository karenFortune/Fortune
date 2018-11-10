using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Packing
{
    public class PackingTypeSize
    {
        public int IdPackingTypeSize { get; set; }
        public int IdTalla { get; set; }
        public int Pieces { get; set; }
        public int Ratio { get; set; }
        public int IdSummary { get; set; }
        public int IdTipoEmpaque { get; set; }
        public string NombreTipoPak{ get; set; }
        public string Talla { get; set; }
        [Display(Name = "TYPE OF PACKAGING ")]
        public TipoEmpaque TipoEmpaque { get; set; }
        [Display(Name = "PACKAGING FORM")]
        public FormaEmpaque FormaEmpaque { get; set; }
        public int IdFormaEmpaque { get; set; }
        [Display(Name = "PO#")]
        public int NumberPO { get; set; }
        [Display(Name = "QTY")]
        public int Cantidad { get; set; }
        public virtual PackingM PackingM { get; set; }

    }

    public enum TipoEmpaque
    {
        BULK = 1,
        PPK = 2
        //ASSORTMENT = 3
    }

    public enum FormaEmpaque
    {
        STORE = 1,
        ECOM = 2
        //ASSORTMENT = 3
    }
}