using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.PrintShop
{
    public class PrintShopC
    {
        public int IdPrintShop { get; set; }
        public int IdSummary { get; set; }
        [Display(Name = "SIZE")]
        public string Talla { get; set; }
        public int IdTalla { get; set; }
        public int Printed { get; set; }
        public int MisPrint { get; set; }
        public int Defect { get; set; }
        public int Maquina { get; set; }
        [Display(Name = "TURN")]
        public Turno Turnos { get; set; }
        public int TipoTurno { get; set; }
        public int Usuario { get; set; }
        public string NombreUsr { get; set; }
        public int Total { get; set; }
        public int IdBatch { get; set; }

        public List<PrintShopC> Batch { get; set; }

        public virtual IMAGEN_ARTE ImagenArte { get; set; }
    }

    public enum Turno
    {
        First = 1,
        Second = 2
    }
}