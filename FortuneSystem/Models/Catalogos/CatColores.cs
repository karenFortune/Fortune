using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatColores
    {
        [Display(Name = "COLOR#")]
        public int IdColor { get; set; }

        [Required(ErrorMessage = "Please enter the color code.")]
        [Display(Name = "COLOR CODE")]
        public string CodigoColor { get; set; }

        [Required(ErrorMessage = "Please enter the color description.")]
        [Display(Name = "DESCRIPTION")]
        public string DescripcionColor { get; set; }
    }
}