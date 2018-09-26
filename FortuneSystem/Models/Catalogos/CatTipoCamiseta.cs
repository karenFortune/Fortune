using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatTipoCamiseta
    {
        [Display(Name = "No. Tipo de Camiseta")]
        public int IdTipo { get; set; }
        [Display(Name = "Código Camiseta")]
        public string TipoProducto { get; set; }
        [Display(Name = "Descripción")]
        public string DescripcionTipo { get; set; }
        [Display(Name = "Tipo de Grupo")]
        public string TipoGrupo { get; set; }
    }
}