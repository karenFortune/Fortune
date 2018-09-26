using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatTela
    {
        [Display(Name = "No. Tela")]
        public int Id_Tela { get; set; }
        [Display(Name = "Tela")]
        public string Tela { get; set; }
        [Display(Name = "Código Tela")]
        public string CodigoTela { get; set; }
    }
}