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
