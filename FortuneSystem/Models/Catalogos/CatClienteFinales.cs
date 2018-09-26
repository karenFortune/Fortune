using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatClienteFinal
    {
        [Display(Name = "No. Cliente Final")]
        public int CustomerFinal { get; set; }
        [Required(ErrorMessage = "Ingrese el nombre del Cliente Final.")]
        [Display(Name = "Cliente Final")]
        public string NombreCliente{ get; set; }
    }
}