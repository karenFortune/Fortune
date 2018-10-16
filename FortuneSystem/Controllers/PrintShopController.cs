﻿using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Pedidos;
using FortuneSystem.Models.PrintShop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers
{
    public class PrintShopController : Controller
    {
        PedidosData objPedido = new PedidosData();
        CatClienteData objCliente = new CatClienteData();
        CatClienteFinalData objClienteFinal = new CatClienteFinalData();
        CatTallaItemData objTalla = new CatTallaItemData();
        PrintShopData objPrint = new PrintShopData();
        // GET: PrintShop
        public ActionResult Index()
        {
            List<OrdenesCompra> listaPedidos = new List<OrdenesCompra>();
            listaPedidos = objPedido.ListaOrdenCompra().ToList();
            return View(listaPedidos);
        }

        [HttpGet]
        public ActionResult Detalles(int? id)
        {
            if (id == null)
            {
                return View();
            }

            OrdenesCompra pedido = objPedido.ConsultarListaPO(id);
            pedido.CatCliente = objCliente.ConsultarListaClientes(pedido.Cliente);
            pedido.CatClienteFinal = objClienteFinal.ConsultarListaClientesFinal(pedido.ClienteFinal);
            pedido.IdPedido = Convert.ToInt32(id);

            if (pedido == null)
            {
                return View();
            }
            return View(pedido);
        }

        [HttpPost]
        public JsonResult Obtener_Lista_Tallas_PrintShop(List<string> ListTalla, int TurnoID, int EstiloID)
        {
            PrintShopC tallaItem = new PrintShopC();
            int noEmpleado = 1;//Convert.ToInt32(Session["id_Empleado"]);
            tallaItem.Usuario = noEmpleado;
            tallaItem.TipoTurno = 1; //TurnoID;
            tallaItem.IdSummary = EstiloID;
            int numBatch = objPrint.ObtenerIdBatch(EstiloID);
            tallaItem.IdBatch = numBatch + 1;
            int i = 1;
            foreach (var item in ListTalla)
            {
                i++;
            }

            i = i - 2;
            for (int v = 0; v < i; v++)
            {
                List<string> tallas = ListTalla[v].Split('*').ToList();
                string talla;
                string printed;
                string misPrint;
                string defecto;
                int c = 0;


                foreach (var item in tallas)
                {
                    if (c == 0)
                    {
                        talla = tallas[0];
                        printed = tallas[1];
                        misPrint = tallas[2];
                        defecto = tallas[3];
                        tallaItem.IdTalla = objTalla.ObtenerIdTalla(talla);
                        tallaItem.Printed = Int32.Parse(printed);
                        tallaItem.MisPrint = Int32.Parse(misPrint);
                        tallaItem.Defect= Int32.Parse(defecto);


                        objPrint.AgregarTallasPrintShop(tallaItem);

                    }                   
                    c++;
                }                            
            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Actualizar_Lista_Tallas_Batch(List<string> ListTalla, int TurnoID, int EstiloID, int IdBatch)
        {
            PrintShopC tallaItem = new PrintShopC();
            int noEmpleado = 1;//Convert.ToInt32(Session["id_Empleado"]);
            tallaItem.Usuario = noEmpleado;
            tallaItem.TipoTurno =TurnoID;
            tallaItem.IdSummary = EstiloID;
            tallaItem.IdBatch = IdBatch;
            int i = 1;
            foreach (var item in ListTalla)
            {
                i++;
            }

            i = i - 2;
            for (int v = 0; v < i; v++)
            {
                List<string> tallas = ListTalla[v].Split('*').ToList();
                string talla;
                string printed;
                string misPrint;
                string defecto;
                int c = 0;


                foreach (var item in tallas)
                {
                    if (c == 0)
                    {
                        talla = tallas[0];
                        printed = tallas[1];
                        misPrint = tallas[2];
                        defecto = tallas[3];
                        tallaItem.IdTalla = objTalla.ObtenerIdTalla(talla);
                        tallaItem.Printed = Int32.Parse(printed);
                        tallaItem.MisPrint = Int32.Parse(misPrint);
                        tallaItem.Defect = Int32.Parse(defecto);
                        tallaItem.IdPrintShop = objPrint.ObtenerIdPrintShopPorBatchEstilo(tallaItem.IdBatch, tallaItem.IdSummary, tallaItem.IdTalla);
                        objPrint.ActualizarTallasPrintShop(tallaItem);

                    }
                    c++;
                }
            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Lista_Batch_Estilo(int? id)
        {
            List<PrintShopC> listaBatch = objPrint.ListaBatch(id).ToList();
           
            var result = Json(new { listaTalla = listaBatch });
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Lista_Tallas_PrintShop_IdEstilo_IdBatch(int? idEstilo, int idBatch)
        {
            List<PrintShopC> listaCantidadesTallasBatch = objPrint.ListaCantidadesTallaPorIdBatchEstilo(idEstilo, idBatch).ToList();

            var result = Json(new { listaTalla = listaCantidadesTallasBatch });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Batch(int? id)
        {
            List<PrintShopC> listaTallas = objPrint.ListaTallasBatchId(id).ToList();
    
            var result = Json(new { listaTalla = listaTallas });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Por_Estilo(int? id)
        {
            List<PrintShopC> listaTallasEstilo = objPrint.ObtenerTallas(id).ToList();

            var result = Json(new { listaTalla = listaTallasEstilo });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Total_Tallas_Batch(int id)
        {
            List<int> listaTallas = objPrint.ListaTotalTallasBatchEstilo(id).ToList();

            var result = Json(new { listaTallaBatch = listaTallas });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}