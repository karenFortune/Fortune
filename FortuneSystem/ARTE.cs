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
    using System;
    using System.Collections.Generic;
    
    public partial class ARTE
    {
        public int IdArte { get; set; }
        public Nullable<int> IdImgArte { get; set; }
        public Nullable<int> IdSummary { get; set; }
		public int IdEstilo { get; set; }
		public virtual IMAGEN_ARTE IMAGEN_ARTE { get; set; }
    }
}
