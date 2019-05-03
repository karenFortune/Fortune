//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FortuneSystem
{
    using FortuneSystem.Models.Catalogos;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;
    using System.Web;

    public partial class IMAGEN_ARTE_PNL
    {
        [Display(Name = "ARTE#")]
        public int IdImgArtePNL { get; set; }
        public Nullable<int> IdEstilo { get; set; }
        [Display(Name = "STATE PNL")]
        public Nullable<int> StatusPNL { get; set; }
        public string ExtensionPNL { get; set; }
        public Nullable<int> IdSummary { get; set; }
        [Display(Name = "PNL IMAGE")]
        public byte[] imgPNL { get; set; }
        [Display(Name = "CUSTOMER")]
        public string Tienda { get; set; }
        public HttpPostedFileBase FilePNL { get; set; }
        public EstatusImgPNL EstadosPNL { get; set; }
        public List<CatTallaItem> ListaTallas { get; set; }
        [Display(Name = "STYLE")]
        public string Estilo { get; set; }
        [Display(Name = "DESCRIPTION")]
        public string DescripcionEstilo { get; set; }
        public MatchCollection ResultadoK { get; set; }
        public MatchCollection ResultadoW { get; set; }
        public string PO { get; set; }
		public string StatusArtePnlInf { get; set; }
		public DateTime Fecha { get; set; }
		public string FechaArtePnl { get; set; }
	}

    public enum EstatusImgPNL
    {
        APPROVED = 1,
        INHOUSE = 2,
        REVIEWED = 3,
        PENDING = 4
    }
}

