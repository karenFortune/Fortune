using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FortuneSystem.Models.Estilos;
using FortuneSystem.Models.Sizes;
using FortuneSystem.Models.Staging;
using FortuneSystem.Models.Recibos;
using FortuneSystem.Models.TallasEstilos;
using System.Globalization;
using Rotativa;

namespace FortuneSystem.Controllers
{
    public class StagingController : Controller
    {
        // GET: Staging
        DatosStaging stag = new DatosStaging();
        DatosSizes tallas = new DatosSizes();
        Staging stagg = new Staging();
        DatosSizes sizes = new DatosSizes();
        DatosRecibos invoices = new DatosRecibos();
        DatosEstilos estilos = new DatosEstilos();
        DatosTallasEstilos tallasEstilos = new DatosTallasEstilos();

        public ActionResult Index(){
            List<Recibo> listaRecibos = new List<Recibo>();
            listaRecibos = invoices.ListaRecibos().ToList();
            return View(listaRecibos);
        }
        public JsonResult obtener_ordenes() {
            return Json(invoices.ListaRecibos(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult obtener_estilos(int ID){
            Session["id_pedido"] = ID;
            estilos.Obtener_informacion_po(ID);
            estilos.id_pedido = Convert.ToInt32(Session["id_pedido"]);
            var result = Json(new { estilos= estilos.ListaEstilos(),total_u=estilos.total_unidades ,po=estilos.id_po, cancel_date= (estilos.fecha_cancelacion).ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX")) });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult recibos_anteriores(int ID){
            Session["id_po_summary"] = ID;
            tallasEstilos.id_po_summary = ID;
            estilos.Obtener_informacion_po(Convert.ToInt32(Session["id_pedido"]));            
            ViewBag.estado_pedido_estilo = estilos.buscar_estado_estilo_pedido(Convert.ToInt32(Session["id_po_summary"]));
            var result = Json(new { listaTallasyEstilos = tallasEstilos.ListaTallasEstilos(), listaTallas = tallas.ListaTallasPorOrden(ID), po = estilos.id_po , estilo= tallasEstilos.buscar_estilo_po_summary(ID) });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //BUSCAR LAS TALLAS PARA EL FORMULARIO
        public JsonResult Obtener_tallas_recibo(int ID)
        {
            Session["id_recibo"] = ID;
            return Json(stag.Lista_tallas_recibo(ID), JsonRequestBehavior.AllowGet);
        }
        public JsonResult obtener_stag(int ID)
        {
            return Json(stag.Lista_stag_pedido(ID), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Guardar_stag(string porcentaje, string talla, string color, string pais, string cajas){
            Session["id_usuario"] = 2;
            int id_recibo = Convert.ToInt32(Session["id_recibo"]);
            int id_usuario = Convert.ToInt32(Session["id_usuario"]);
            string[] cajaS = cajas.Split('*');
            //GUARDAR RECIBO PRIMERO //OBTENER EL NÚMERO DE STAG
            int id_stag = stag.guardar_stag(id_recibo, id_usuario, porcentaje, talla, color, pais);
            //GUARDAR LAS CAJAS
            for (int i = 1; i < cajaS.Length; i++){
                stag.guardar_cajas_stag(id_stag, cajaS[i]);
            }
            stag.marcar_talla_recibo(id_recibo, talla);
            stag.buscar_estado_stag_recibo(id_recibo);
            var result = Json(new { pedido = Convert.ToInt32(Session["id_pedido"]), summary = Convert.ToInt32(Session["id_po_summary"]), recibo = Convert.ToInt32(Session["id_recibo"]) });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete(string term)
        {
            //var items = new[] { "Apple", "Pear", "Banana", "Pineapple", "Peach" };

            var items = stag.Lista_colores();
           
            var filteredItems = items.Where(
                item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0
                );
           
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        //obtener_datos_imprimir_stag
        public JsonResult obtener_datos_imprimir_stag(int ID)
        {
            Session["id_stag_imprimir"] = ID;
            return Json(stag.Lista_stag_id(ID), JsonRequestBehavior.AllowGet);
        }
      
    }
}