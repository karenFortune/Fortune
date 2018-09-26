using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatColores
    {
        [Display(Name = "Id Color")]
        public int IdColor { get; set; }

        [Required(ErrorMessage = "Ingrese el código del color.")]
        [Display(Name = "Código Color")]
        public string CodigoColor { get; set; }

        [Required(ErrorMessage = "Ingrese la descripción del color.")]
        [Display(Name = "Descripción")]
        public string DescripcionColor { get; set; }
    }
}