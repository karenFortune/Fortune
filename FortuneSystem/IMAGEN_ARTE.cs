//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
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

    public partial class IMAGEN_ARTE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IMAGEN_ARTE()
        {
            this.ARTE = new HashSet<ARTE>();
        }

        [Display(Name = "ARTE#")]
        public int IdImgArte { get; set; }
        public Nullable<int> IdEstilo { get; set; }
        [Display(Name = "STATE ART")]
        public Nullable<int> StatusArte { get; set; }
        [Display(Name = "STATE PNL")]
        public Nullable<int> StatusPNL { get; set; }
        [Display(Name = "ART IMAGE")]
        public byte[] imgArte { get; set; }
        [Display(Name = "PNL IMAGE")]
        public byte[] imgPNL { get; set; }
        [Display(Name = "CUSTOMER")]
        public string Tienda { get; set; }
        public HttpPostedFileBase FileArte { get; set; }
        public HttpPostedFileBase FilePNL { get; set; }
        public EstatusArte EstadosArte { get; set; }
        public EstatusPNL EstadosPNL { get; set; }
        public List<CatTallaItem> ListaTallas { get; set; }
        [Display(Name = "STYLE")]
        public string Estilo { get; set; }
        public MatchCollection ResultadoK { get; set; }
        public MatchCollection ResultadoW { get; set; }
        public virtual ARTE CATARTE { get; set; }
        public string PO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ARTE> ARTE { get; set; }
    }

    public enum EstatusArte
    {
        APPROVED = 1,
        REVIEWED = 2,
        PENDING = 3
    }

    public enum EstatusPNL
    {
        APPROVED = 1,
        REVIEWED = 2,
        PENDING = 3
    }
}
