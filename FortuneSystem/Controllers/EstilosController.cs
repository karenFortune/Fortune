using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FortuneSystem.Models.Estilos;
using FortuneSystem.Models.Sizes;
using FortuneSystem.Models.TallasEstilos;
using System.Globalization;

namespace FortuneSystem.Controllers
{
    public class EstilosController : Controller{
        DatosEstilos estilos = new DatosEstilos();
        DatosSizes tallas = new DatosSizes();
        DatosTallasEstilos tallasEstilos = new DatosTallasEstilos();
        List<Estilo> listaEstilos = new List<Estilo>();

        public ActionResult Index(){
            
            //Session["id_pedido"] = 17; //ID PO
            int id = Convert.ToInt32(Session["id_pedido"]);
            estilos.Obtener_informacion_po(id);
            @ViewData["PO"] = estilos.id_po;
            @ViewData["cancel_date"] = (estilos.fecha_cancelacion).ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX"));
            @ViewData["total_units"] = estilos.total_unidades;
            estilos.id_pedido = Convert.ToInt32(Session["id_pedido"]);
            return View();
        }
        //Lista de estilos de toda la orden
        public JsonResult List(){
            //Session["id_pedido"] = 17;
            estilos.id_pedido = Convert.ToInt32(Session["id_pedido"]);
            return Json(estilos.ListaEstilos(), JsonRequestBehavior.AllowGet);
        }
        //Lista de TALLAS de toda la orden
        public JsonResult InformacionEstilo(int ID){
            Session["id_po_summary"] = ID;
            return Json(tallas.ListaTallasPorOrden(ID), JsonRequestBehavior.AllowGet);
        }
        //Lista de los recibos que ha tenido esta orden
        public JsonResult RecibosAnteriores(int ID){
            tallasEstilos.id_po_summary= Convert.ToInt32(Session["id_po_summary"]);
            //REVISAR SI EL PEDIDO YA SE RECIBIÓ POR COMPLETO

            ViewBag.estado_pedido_estilo = estilos.buscar_estado_estilo_pedido(Convert.ToInt32(Session["id_po_summary"]));
            var result = Json(new { listaTallasyEstilos = tallasEstilos.ListaTallasEstilos(), listaTallas = tallas.ListaTallasPorOrden(ID) });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Guardar_recibo_nuevo(string tallas_arr, string cantidades_arr, string packing,string origen, string material, string notas){
            int total = 0,id_recibo,id_talla;
            string[] tallaS = tallas_arr.Split(',');
            string[] cantidadeS = cantidades_arr.Split(',');
            //OBTENER CANTIDAD TOTAL RECIBIDA
            for (int i = 1; i < cantidadeS.Length; i++) {
                total += Convert.ToInt32(cantidadeS[i]);
            }
            if (notas == "undefined")notas= "N/A";
            estilos.id_usuario= Convert.ToInt32(Session["id_usuario"]);
            estilos.id_pedido = Convert.ToInt32(Session["id_pedido"]);
            estilos.id_po_summary= Convert.ToInt32(Session["id_po_summary"]);
            //GUARDAR EL RECIBO EN GENERAL
            estilos.registrar_recibo(packing,origen,material,notas,total);
            //OBTENER EL ID DEL ÚLTIMO RECIBO
            id_recibo = estilos.obtener_ultimo_recibo();
            //GUARDAR TODAS LAS TALLAS EN LOS RECIBOS EN UN CICLO
            for (int i = 1; i < cantidadeS.Length; i++){
                //BUSCAR ID DE LA TALLA
                id_talla = estilos.buscar_talla(tallaS[i]);
                //GUARDAR EL REGISTRO
                estilos.registrar_tallas_recibo(id_recibo,id_talla,cantidadeS[i]);
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }




    }//NO
}//NO



