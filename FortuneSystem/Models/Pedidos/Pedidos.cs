﻿using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Items;
using FortuneSystem.Models.Packing;
using FortuneSystem.Models.PNL;
using FortuneSystem.Models.POSummary;
using FortuneSystem.Models.PrintShop;
using FortuneSystem.Models.Revisiones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Pedidos
{

    public class OrdenesCompra
    {

        [Display(Name = "#")]
        public int IdPedido { get; set; }

        //[Required(ErrorMessage = "Ingrese el número de referencia de la orden.")]
        //[RegularExpression("/[^A-Z\u00f1\u00d1\u0020\0-9]/g", ErrorMessage = "El Orden Ref. debe contener sólo números y letras.")]
        [Display(Name = "ORDEN REF")]
        public string PO { get; set; }

        [Required]
        [Display(Name = "PO")]
        public int VPO { get; set; }

        [Required]
        [Display(Name = "CUSTOMER")]
        [ForeignKey("CUSTOMER")]
        [Column("CUSTOMER")]
        public int Cliente { get; set; }

        public virtual CatCliente CatCliente { get; set; }
        public List<CatCliente> LCliente { get; set; }

        [Required]
        [Display(Name = "ORDEN CUSTOMER")]
        [ForeignKey("CUSTOMER_FINAL")]
        [Column("CUSTOMER_FINAL")]
        public int ClienteFinal { get; set; }

        public virtual CatClienteFinal CatClienteFinal { get; set; }
        public List<CatClienteFinal> LClienteFinal { get; set; }

        [Required]
        [Display(Name = "STATUS")]
        [ForeignKey("ID_STATUS")]
        [Column("ID_STATUS")]
        public int IdStatus { get; set; }

        public virtual CatStatus CatStatus { get; set; }


        [Required]
        
        [RegularExpression("^[0-9]{4}-[0-1][0-9]-[0-3][0-9]$")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        [Display(Name = "CANCEL DATE")]
        public DateTime FechaCancel { get; set; }

        [RegularExpression("^[0-9]{4}-[0-1][0-9]-[0-3][0-9]$")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        [Display(Name = "DATE")]
        public DateTime FechaFinalOrden { get; set; }

        [Required]
        // [RegularExpression("^[0-9]{4}-[0-1][0-9]-[0-3][0-9]$", ErrorMessage = "Formato de fecha incorrecta.")]
        [RegularExpression("^[0-9]{4}-[0-1][0-9]-[0-3][0-9]$")]
        [Display(Name = "REGISTRATION DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime FechaOrden { get; set; }

        //[Required(ErrorMessage = "Ingrese el total de unidades.")]
        [Required]
        [Display(Name = "TOTAL UNITS")]
        public int TotalUnidades { get; set; }

        public List<CatCliente> ListaClientes { get; set; }

        public List<CatClienteFinal> ListaClientesFinal { get; set; }

        public List<CatStatus> ListaCatStatus { get; set; }

        public List<CatGenero> ListarTallasPorGenero { get; set; }

        public int Historial { get; set; }

        public virtual Revision Revision { get; set; }

        public virtual PrintShopC PrintShopC { get; set; }

        public virtual Pnl PNL { get; set; }

        public virtual PackingM Packing { get; set; }       

        public int Usuario { get; set; }
        public string NombreUsr { get; set; }

        public string NombreClienteFinal { get; set; }

        public int HistorialPacking { get; set; }

        public List<PackingTypeSize> ListPack { get; set; }

        public List<ItemDescripcion> ListItems { get; set; }
        [Display(Name = "STYLE")]
        public int IdEstilo { get; set; }


    }
}