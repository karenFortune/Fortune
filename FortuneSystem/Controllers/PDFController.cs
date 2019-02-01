using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rotativa;
using System.Globalization;

using FortuneSystem.Models.Almacen;
using FortuneSystem.Models.Staging;
using FortuneSystem.Models.Shipping;
using FortuneSystem.Models.POSummary;

namespace FortuneSystem.Controllers
{
    public class PDFController : Controller
    {


        DatosInventario di = new DatosInventario();
        DatosTransferencias dt = new DatosTransferencias();
        DatosStaging ds = new DatosStaging();
        DatosShipping dsh = new DatosShipping();
        StagingGeneral sg = new StagingGeneral();
        DescripcionItemData de = new DescripcionItemData();
        string filename, footer_alineacion, footer_size,vista;
                
        public ActionResult Index(){            
            return View();
        }            

        public ActionResult imprimir_etiquetas_recibos()
        {
            int recibo = Convert.ToInt32(Session["id_recibo_nuevo"]);
            //return View("etiquetas_cajas_recibo", di.lista_recibo_etiqueta(recibo.ToString()));            
            return new ViewAsPdf("etiquetas_cajas_recibo", di.lista_recibo_etiqueta(recibo.ToString()))
            {
                FileName = filename,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.Letter,               
                PageMargins = new Rotativa.Options.Margins(5, 5, 5, 5),
                CustomSwitches = "--page-offset 0 ",
                PageHeight=40,
                PageWidth=120,
            };
        }
        [AllowAnonymous]
        public ActionResult transfer_ticket()
        {
            int salida = Convert.ToInt32(Session["id_transfer_ticket"]);
            //return View("transfer_ticket", dt.lista_transfer_ticket(salida));              
            int tipo = dt.buscar_tipo_salida(salida);
            if (tipo == 0){
                return new ViewAsPdf("transfer_ticket", dt.lista_transfer_ticket(salida)){
                    FileName = filename,
                    PageOrientation = Rotativa.Options.Orientation.Landscape,
                    PageSize = Rotativa.Options.Size.Letter,
                    PageMargins = new Rotativa.Options.Margins(15, 10, 15, 10),
                    CustomSwitches = "--page-offset 0 --footer-right [page]/[toPage] --footer-font-size 9 ",
                };
            }else {
                //return View("transfer_ticket_contratista", dt.lista_transfer_ticket(salida));
                return new ViewAsPdf("transfer_ticket_contratista", dt.lista_transfer_ticket(salida)){
                    FileName = filename,
                    PageOrientation = Rotativa.Options.Orientation.Landscape,
                    PageSize = Rotativa.Options.Size.Letter,
                    PageMargins = new Rotativa.Options.Margins(15, 10, 15, 10),
                    CustomSwitches = "--page-offset 0 --footer-right [page]/[toPage] --footer-font-size 9 ",
                };
            }
        }       
        [AllowAnonymous]
        public ActionResult papeleta_staging_vacias()
        {
            int salida = Convert.ToInt32(Session["id_transfer_ticket"]);            
            ViewBag.color = sg.obtener_color_item(Convert.ToInt32(Session["id_inventario"]));
            ViewBag.pais = sg.obtener_pais_item(Convert.ToInt32(Session["id_inventario"]));
            return new ViewAsPdf("papeleta_staging_vacias", ds.lista_papeleta(Convert.ToInt32(Session["id_inventario"]), Convert.ToInt32(Session["turno"])))
            {
                FileName = filename,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.Letter,
                PageMargins = new Rotativa.Options.Margins(15, 10, 15, 10),
                CustomSwitches = "--page-offset 0 --footer-right [page]/[toPage] --footer-font-size 9 ",
            };
        }
        //papeleta_staging
        [AllowAnonymous]
        public ActionResult papeleta_staging(){
            return new ViewAsPdf("papeleta_staging", ds.lista_papeleta_staging(Convert.ToInt32(Session["id_staging"]), Convert.ToInt32(Session["turno"])))
            {
                FileName = filename,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.Letter,
                PageMargins = new Rotativa.Options.Margins(15, 10, 15, 10),
                CustomSwitches = "--page-offset 0 --footer-right [page]/[toPage] --footer-font-size 9 ",
            };
        }
        //*************************
        [AllowAnonymous]
        public ActionResult imprimir_pk(){
            //return View("packing_list", dsh.obtener_packing_list(Convert.ToInt32(Session["pk"])));
            return new ViewAsPdf("packing_list", dsh.obtener_packing_list(Convert.ToInt32(Session["pk"]))){
                FileName = filename,
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageSize = Rotativa.Options.Size.Letter,
                PageMargins = new Rotativa.Options.Margins(8, 10, 15, 10),
                CustomSwitches = "--page-offset 0 --footer-right [page]/[toPage] --footer-font-size 9 ",
            };
        }
        //*************************
        [AllowAnonymous]
        public ActionResult imprimir_bol()
        {
            //return View("bol", dsh.obtener_packing_list(Convert.ToInt32(Session["pk"])));
            return new ViewAsPdf("bol", dsh.obtener_packing_list(Convert.ToInt32(Session["pk"])))
            {
                FileName = filename,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.Letter,
                PageMargins = new Rotativa.Options.Margins(8, 10, 15, 10),
                CustomSwitches = "--page-offset 0  ",
            };
        }

        [AllowAnonymous]
        public ActionResult Imprimir_Reporte_PO()
        {
            int id = Convert.ToInt32(Session["idPed"]);
            return new ViewAsPdf("Imprimir_Reporte_PO", de.ListadoInfEstilo(id))
            {
                FileName = filename,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.Letter,
                PageMargins = new Rotativa.Options.Margins(5, 10, 15, 10),
                CustomSwitches = "--page-offset 0 --footer-right [page]/[toPage] --footer-font-size 9 ",
            };
           
        }











    }
}