using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatTallaItem
    {
        [Display(Name = "No. Talla")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Ingrese la Talla.")]
        [Display(Name = "Talla")]
        public string Talla { get; set; }
    }
}