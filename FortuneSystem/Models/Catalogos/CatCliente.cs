using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatCliente
    {
        [Display(Name = "No. Cliente")] 
        public int Customer { get; set; }

        [Required(ErrorMessage = "Ingrese el nombre del Cliente.")]
        [Display(Name = "Cliente")]
        [Column("NAME")]
        public string Nombre { get; set; }
    }
}