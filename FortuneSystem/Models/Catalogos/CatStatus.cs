using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatStatus
    {
        [Display(Name = "No. Estado")]
        public int IdStatus { get; set; }

        [Required(ErrorMessage = "Ingrese el nombre del estado.")]
        [Display(Name = "Estado")]
        [Column("ESTADO")]
        public string Estado { get; set; }

    }
}