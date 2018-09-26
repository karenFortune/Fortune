using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatGenero
    {
        [Display(Name = "No. Género")]
        public int IdGender { get; set; }

        [Required(ErrorMessage = "Ingrese el Género")]
        [Display(Name = "Género")]
        public string Genero { get; set; }

        public virtual CatTallaItem CatTallaItem { get; set; }

        [Display(Name = "Código Género")]
        public string GeneroCode { get; set; }

    }
}