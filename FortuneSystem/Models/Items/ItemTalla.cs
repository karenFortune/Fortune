using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Item
{
    public class ItemTalla
    {
        public int Id { get; set; }
        public string Talla { get; set; }
        public int IdTalla { get; set; }
        public int Cantidad { get; set; }
        public int Ejemplos { get; set; }
        public int Extras { get; set; }
        public int IdSummary { get; set; }
        public string Estilo { get; set; }
    }
}