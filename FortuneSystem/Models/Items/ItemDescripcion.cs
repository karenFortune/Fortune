using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Items
{
    public class ItemDescripcion
    {
        public int ItemId { get; set; }
        [Display(Name = "Estilo")]
        public string ItemEstilo { get; set; }
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
    }
}