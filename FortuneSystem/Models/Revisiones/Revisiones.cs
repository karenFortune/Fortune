using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Revisiones
{
    public class Revisiones
    {
        [Display(Name = "Id")]
        public int Id {get; set;}
        [Display(Name = "Id Pedido")]
        public int IdPedido { get; set; }
        [Display(Name = "Id Revision")]
        public int IdRevisionPO { get; set; }
        [Display(Name = "Fecha Revisión")]
        public DateTime FechaRevision { get; set; }
        [Display(Name = "Id Estado")]
        public int IdStatus { get; set; }

    }
}