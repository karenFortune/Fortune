using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rotativa;
using System.Globalization;
using FortuneSystem.Models.Pedidos;

namespace FortuneSystem.Controllers
{
    public class PDFController : Controller
    {

       /* DatosRecibos recibo = new DatosRecibos();
        DatosEstilos estilos = new DatosEstilos();
        DatosStaging stag = new DatosStaging();
        DatosTallasEstilos tye = new DatosTallasEstilos();

        string filename, footer_alineacion, footer_size,vista;
                
        public ActionResult Index(){            
            return View();
        }

        [ChildActionOnly]
        public ActionResult ListaEstilos(){
            
            estilos.id_pedido = Convert.ToInt32(Session["id_pedido"]);
            List<Estilo> lista = estilos.ListaEstilos();            
            return PartialView(lista);
        }

        public ActionResult imprimir_staging()
        {
            ViewBag.cantidad = Convert.ToInt32(Session["cantidad"]);
            List<Staging> lista = stag.Lista_stag_imprimir(Convert.ToInt32(Session["id_recibo_imprimir"])).ToList();
            filename = Convert.ToString(Session["nombre_pdf"]) + ".pdf";
            vista = Convert.ToString(Session["vista"]);
            stag.buscar_po_estilo_recibo(Convert.ToInt32(Session["id_recibo_imprimir"]));
            ViewBag.po = stag.po;
            ViewBag.estilo = stag.estilo;
            //return View(lista);
            return new ViewAsPdf(vista,lista)
            {
                FileName = filename,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.Letter,
                PageMargins = new Rotativa.Options.Margins(15, 10, 15, 10),
                CustomSwitches = "--page-offset 0 --footer-center [page]/[page] --footer-font-size 8 ",
            };
        }

        public ActionResult Print()//EJEMPLO
        {
            filename = Convert.ToString(Session["nombre_pdf"])+".pdf";
            vista = Convert.ToString(Session["vista"]);
            return new ViewAsPdf(vista){
                FileName = filename,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.Letter,
                PageMargins = new Rotativa.Options.Margins(15, 10, 15, 10),
                CustomSwitches = "--page-offset 0 --footer-center [page]/[page] --footer-font-size 8 ",                
            };
        }

        public ActionResult imprimir_reporte_recibos(){
            estilos.tipo_reporte = Convert.ToString(Session["tipo_reporte"]);
            ViewBag.encabezado = "REPORTE DE RECIBOS ";
            if (estilos.tipo_reporte == "Por fechas"){
                ViewBag.tipo_reporte = 1;
                estilos.fecha_inicio = Convert.ToString(Session["fecha_inicio"]);
                estilos.fecha_final = Convert.ToString(Session["fecha_final"]);               
                ViewBag.fechas= "De " + Convert.ToDateTime(tye.fecha_inicio).ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX")) + "  a  " + Convert.ToDateTime(tye.fecha_final).ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX")); 
            }
            if (estilos.tipo_reporte == "Por PO"){
                estilos.po = Convert.ToString(Session["po"]);
                
                ViewBag.tipo_reporte =2;
            }
            if (estilos.tipo_reporte == "Recibidos hoy") {
                ViewBag.tipo_reporte = 1;
            }
            ViewBag.fecha_consulta = DateTime.Now.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX"));
           List<Estilo> lista = estilos.ListaEstilosReportes();
            //return View(lista);            
            return new ViewAsPdf("imprimir_reporte_recibos", lista){
                FileName = filename,
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageSize = Rotativa.Options.Size.Letter,
                PageMargins = new Rotativa.Options.Margins(15, 10, 15, 10),
                CustomSwitches = "--page-offset 0 --footer-right [page]/[toPage] --footer-font-size 9 ",
            };
        }*/

       



















    }
}