using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.PrintShop
{
    public class PrintShop
    {
        public int IdPrintShop { get; set; }
        public int IdPedido { get; set; }
        public int Printed { get; set; }
        public int MisPrint { get; set; }
        public int Defect { get; set; }
        public string Maquina { get; set; }
        public int Usuario { get; set; }
        public int Total { get; set; }
    }
}