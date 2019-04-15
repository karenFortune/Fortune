using ClosedXML.Excel;
using FortuneSystem.Models.Almacen;
using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Item;
using FortuneSystem.Models.Items;
using FortuneSystem.Models.Packing;
using FortuneSystem.Models.Pedidos;
using FortuneSystem.Models.PNL;
using FortuneSystem.Models.POSummary;
using FortuneSystem.Models.PrintShop;
using FortuneSystem.Models.Revisiones;
using FortuneSystem.Models.Staging;
using FortuneSystem.Models.Trims;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers
{
    public class PedidosController : Controller
    {
        // GET: Pedido
        PedidosData objPedido = new PedidosData();
        CatClienteData objCliente = new CatClienteData();
        CatClienteFinalData objClienteFinal = new CatClienteFinalData();
        CatStatusData objEstados = new CatStatusData();
        CatGeneroData objGenero = new CatGeneroData();
        CatColoresData objColores = new CatColoresData();
        DescripcionItemData objItems = new DescripcionItemData();
        CatTelaData objTela = new CatTelaData();
        CatTipoCamisetaData objTipoC = new CatTipoCamisetaData();
        ItemTallaData objTallas = new ItemTallaData();
        RevisionesData objRevision = new RevisionesData();
        ItemDescripcionData objEst = new ItemDescripcionData();
        PrintShopData objPrint = new PrintShopData();
        PackingData objPacking = new PackingData();
        PnlData objPnl = new PnlData();
        PDFController pdf = new PDFController();
        CatEspecialidadesData objEspecialidad = new CatEspecialidadesData();
        CatTipoOrdenData objTipoOrden = new CatTipoOrdenData();
  
        public int estado;
        public int IdPO;
        public int pedidos;

        public ActionResult Index()
        {
            List<OrdenesCompra> listaPedidos = new List<OrdenesCompra>();
            listaPedidos = objPedido.ListaOrdenCompra().ToList();
            return View(listaPedidos);
        }

        public void Reporte(int? id)
        {
            Session["idPed"] = id;
            pdf.Imprimir_Reporte_PO();
        }

        [HttpPost]
        public JsonResult Imprimir_Reporte_PO(int id)
        {
            //pedido        
            Session["idPed"] = id;

            return Json("0", JsonRequestBehavior.AllowGet);
        }

        /*  [ChildActionOnly]
          public ActionResult StudentList()
          {
              lista = objPedido.ListaOrdenCompra().ToList();
              return PartialView(lista);
          }*/

        /* public ActionResult Lista_Pedido_Por_Fecha(DateTime? fechaCancel, DateTime? fechaOrden)
         {
             List<OrdenesCompra> lista = objPedido.ListaOrdenCompra(fechaCancel, fechaOrden).ToList();
             return PartialView(lista);
         }*/

        [HttpPost]
        public JsonResult Lista_Estilos_PO(int? id)
        {

            List<POSummary> listaItems = objItems.ListaItemsPorPO(id).ToList();
            int cargo = Convert.ToInt32(Session["idCargo"]);
            var result = Json(new { listaItem = listaItems, cargoUser = cargo });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        public ActionResult Listado_Estilos_PO()
        {
            int? id = Convert.ToInt32(Convert.ToInt32(Session["idPedidoNuevo"])); //id_pedido

            List<POSummary> listaItems = objItems.ListaItemsPorPO(id).ToList();

            return PartialView(listaItems);
        }

        [ChildActionOnly]
        public ActionResult Listado_Tallas_Estilo(int? id)
        {
            List<ItemTalla> listaTallas = objTallas.ListaTallasPorEstilo(id).ToList();

            return PartialView(listaTallas);
        }

        public ActionResult HistorialPedidos(int id)
        {
            List<OrdenesCompra> listaPedidosRev = new List<OrdenesCompra>();
            listaPedidosRev = objPedido.ListaRevisionesPO(id).ToList();

            return View(listaPedidosRev);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Estilo(int? id)
        {
            List<ItemTalla> listaTallas = objTallas.ListaTallasPorEstilo(id).ToList();
            List<StagingD> listaTallasStaging = objTallas.ListaTallasStagingPorEstilo(id).ToList();
            List<int> listaTallasTBatch = objPrint.ListaTotalTallasBatchEstilo(id).ToList();
            List<int> listaTallasPBatch = new List<int>();
            List<int> listaTallasMPBatch = new List<int>();
            List<int> listaTallasDBatch = new List<int>();
            List<int> listaTallasRBatch = new List<int>();
            if (listaTallasTBatch.Count != 0)
            {
                listaTallasPBatch = objPrint.ListaTotalPrintedTallasBatchEstilo(id).ToList();
                listaTallasMPBatch = objPrint.ListaTotalMPTallasBatchEstilo(id).ToList();
                listaTallasDBatch = objPrint.ListaTotalDefTallasBatchEstilo(id).ToList();
                listaTallasRBatch = objPrint.ListaTotalRepTallasBatchEstilo(id).ToList();
            }

            string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }
            var result = Json(new
            {
                listaTalla = listaTallas,
                estilos = estilo,
                listTallaStaging = listaTallasStaging,
                listaTallasTotalBatch = listaTallasTBatch,
                listaTallasTotalPBatch = listaTallasPBatch,
                listaTallasTotalMBatch = listaTallasMPBatch,
                listaTallasTotalDBatch = listaTallasDBatch,
                listaTallasTotalRBatch = listaTallasRBatch
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Listado_Tallas_Estilos(int? id)
        {
            List<ItemTalla> listaTallas = objTallas.ListadoTallasPorEstilos(id).ToList();
            List<StagingD> listaTallasStaging = objTallas.ListaTallasStagingPorEstilo(id).ToList();        
            List<int> listaTallasTBatch = objPrint.ListaTotalTallasBatchEstilo(id).ToList();
            List<int> listaTallasPBatch = new List<int>();
            List<int> listaTallasMPBatch = new List<int>();
            List<int> listaTallasDBatch = new List<int>();
            List<int> listaTallasRBatch = new List<int>();
            if (listaTallasTBatch.Count != 0)
            {
                listaTallasPBatch = objPrint.ListaTotalPrintedTallasBatchEstilo(id).ToList();
                listaTallasMPBatch = objPrint.ListaTotalMPTallasBatchEstilo(id).ToList();
                listaTallasDBatch = objPrint.ListaTotalDefTallasBatchEstilo(id).ToList();
                listaTallasRBatch = objPrint.ListaTotalRepTallasBatchEstilo(id).ToList();
            }

            string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }
            var result = Json(new
            {
                listaTalla = listaTallas,
                estilos = estilo,
                listTallaStaging = listaTallasStaging,                
                listaTallasTotalBatch = listaTallasTBatch,
                listaTallasTotalPBatch = listaTallasPBatch,
                listaTallasTotalMBatch = listaTallasMPBatch,
                listaTallasTotalDBatch = listaTallasDBatch,
                listaTallasTotalRBatch = listaTallasRBatch
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Estilo_PrintShop(int? id)
        {
            List<ItemTalla> listaTallas = objTallas.ListaTallasPorEstiloPrint(id).ToList();
            List<StagingD> listaTallasStaging = objTallas.ListaTallasStagingPorEstilo(id).ToList();
            List<StagingDatos> listaDatosStaging = objTallas.ListaTallasStagingDatosPorEstilo(id).ToList();
            List<int> listaTallasTBatch = objPrint.ListaTotalTallasBatchEstilo(id).ToList();
            List<int> listaTallasPBatch = new List<int>();
            List<int> listaTallasMPBatch = new List<int>();
            List<int> listaTallasDBatch = new List<int>();
            List<int> listaTallasRBatch = new List<int>();
            if (listaTallasTBatch.Count != 0)
            {
                listaTallasPBatch = objPrint.ListaTotalPrintedTallasBatchEstilo(id).ToList();
                listaTallasMPBatch = objPrint.ListaTotalMPTallasBatchEstilo(id).ToList();
                listaTallasDBatch = objPrint.ListaTotalDefTallasBatchEstilo(id).ToList();
                listaTallasRBatch = objPrint.ListaTotalRepTallasBatchEstilo(id).ToList();
            }

            string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }
            var result = Json(new
            {
                listaTalla = listaTallas,
                estilos = estilo,
                listTallaStaging = listaTallasStaging,
                listDatosStaging = listaDatosStaging,
                listaTallasTotalBatch = listaTallasTBatch,
                listaTallasTotalPBatch = listaTallasPBatch,
                listaTallasTotalMBatch = listaTallasMPBatch,
                listaTallasTotalDBatch = listaTallasDBatch,
                listaTallasTotalRBatch = listaTallasRBatch
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Estilo_Recibos(int? idSummary, int? idPedido, int? idEstilo)
        {
            List<ItemTalla> listaTallas = objTallas.ListaTallasPorEstiloRecibo(idSummary).ToList();
            List<recibo> listaTallasRecibo = objPedido.ListaRecibos(idSummary).ToList();
            List<recibo> listaTRecibo = objPedido.ListaRecibos(idSummary).ToList(); 
            string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }
            var result = Json(new
            {
                listaTalla = listaTallas,
                estilos = estilo,
                listaRecibos = listaTallasRecibo,
                listadoRecibo = listaTRecibo
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //Trims
        [HttpPost]
        public JsonResult Lista_Trims(int? idSummary)
        {
            List<Trim_requests> listadoTrims = objPedido.ObtenerInformacionTrims(idSummary).ToList();

            foreach (var item in listadoTrims)
            {             
               string DatoTalla = objTallas.ObtenerTallasPorId(item.id_talla);
                if(DatoTalla != "")
                {
                    item.talla = DatoTalla;
                }
                else
                {
                    item.talla = "0";
                }         
            }           
   
            var result = Json(new
            {
                listaTrims = listadoTrims

            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Lista_Price_Tickets_Trims(int? idSummary)
        {
            List<InfoPriceTickets> listadoTrims = objPedido.ObtenerInformacionPriceTicketsTrims(idSummary).ToList();

            foreach (var item in listadoTrims)
            {
                string DatoTalla = objTallas.ObtenerTallasPorId(item.Id_talla);
                if (DatoTalla != "")
                {
                    item.Talla = DatoTalla;
                }
                else
                {
                    item.Talla = "0";
                }
            }

            var result = Json(new
            {
                listaTrims = listadoTrims

            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
       

        [HttpPost]
        public JsonResult Lista_Tallas_Estilo_Det(int? id)
        {
            List<ItemTalla> listaTallas = objTallas.ListaTallasPorEstiloRev(id).ToList();
            List<StagingD> listaTallasStaging = objTallas.ListaTallasStagingPorEstilo(id).ToList();
            List<int> listaTallasTBatch = objPrint.ListaTotalTallasBatchEstilo(id).ToList();
            List<int> listaTallasPBatch = new List<int>();
            List<int> listaTallasMPBatch = new List<int>();
            List<int> listaTallasDBatch = new List<int>();
            List<int> listaTallasRBatch = new List<int>();
            if (listaTallasTBatch.Count != 0)
            {
                listaTallasPBatch = objPrint.ListaTotalPrintedTallasBatchEstilo(id).ToList();
                listaTallasMPBatch = objPrint.ListaTotalMPTallasBatchEstilo(id).ToList();
                listaTallasDBatch = objPrint.ListaTotalDefTallasBatchEstilo(id).ToList();
                listaTallasRBatch = objPrint.ListaTotalRepTallasBatchEstilo(id).ToList();
            }

            string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }
            var result = Json(new
            {
                listaTalla = listaTallas,
                estilos = estilo,
                listTallaStaging = listaTallasStaging,
                listaTallasTotalBatch = listaTallasTBatch,
                listaTallasTotalPBatch = listaTallasPBatch,
                listaTallasTotalMBatch = listaTallasMPBatch,
                listaTallasTotalDBatch = listaTallasDBatch,
                listaTallasTotalRBatch = listaTallasRBatch
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Estilo_Pnl(int? id)
        {
            List<ItemTalla> listaTallas = objTallas.ListaTallasPorEstilopnl(id).ToList();
            List<StagingD> listaTallasStaging = objTallas.ListaTallasStagingPorEstilo(id).ToList();
            List<StagingDatos> listaDatosStaging = objTallas.ListaTallasStagingDatosPorEstilo(id).ToList();
            List<int> listaTallasTPnlBatch = objPnl.ListaTotalTallasPNLBatchEstilo(id).ToList();
            List<int> listaTallasPBatchPnl = new List<int>();
            List<int> listaTallasMPBatchPnl = new List<int>();
            List<int> listaTallasDBatchPnl = new List<int>();
            List<int> listaTallasRBatch = new List<int>();
            if (listaTallasTPnlBatch.Count != 0)
            {
                listaTallasPBatchPnl = objPnl.ListaTotalPrintedTallasBatchEstilo(id).ToList();
                listaTallasMPBatchPnl = objPnl.ListaTotalMPTallasBatchEstilo(id).ToList();
                listaTallasDBatchPnl = objPnl.ListaTotalDefTallasBatchEstilo(id).ToList();
                listaTallasRBatch = objPnl.ListaTotalRepTallasBatchEstilo(id).ToList();
            }
            List<int> listaTallasTBatch = objPrint.ListaTotalTallasBatchEstilo(id).ToList();
            string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }
            var result = Json(new
            {
                listaTalla = listaTallas,
                estilos = estilo,
                listTallaStaging = listaTallasStaging,
                listDatosStaging = listaDatosStaging,
                listaTallasTotalPnlBatch = listaTallasTPnlBatch,
                listaTallasTotalBatch = listaTallasTBatch,
                listaTallasTotalPBatchPNL = listaTallasPBatchPnl,
                listaTallasTotalMBatchPnl = listaTallasMPBatchPnl,
                listaTallasTotalDBatchPnl = listaTallasDBatchPnl,
                listaTallasTotalRBatch = listaTallasRBatch
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Lista_Tallas_Estilo_Packing(int? id)
        {
            List<ItemTalla> listaTallas = objTallas.ListaTallasPorEstilo(id).ToList();
            List<StagingD> listaTallasStaging = objTallas.ListaTallasStagingPorEstilo(id).ToList();
            List<int> listaTallasTPrintShopBatch = objPrint.ListaTotalTallasBatchEstilo(id).ToList();
            List<int> listaTallasTPnlBatch = objPnl.ListaTotalTallasPNLBatchEstilo(id).ToList();
            List<int> listaTallasTPackingBatch = objPacking.ListaTotalTallasPackingBatchEstilo(id).ToList();
            List<int> listaTallasPBatchPacking = new List<int>();
            List<int> listaTallasEBatchPacking = new List<int>();
            List<int> listaTallasDBatchPacking = new List<int>();
            if (listaTallasTPackingBatch.Count != 0)
            {
                listaTallasPBatchPacking = objPacking.ListaTotalCajasTallasBatchEstilo(id).ToList();
                listaTallasEBatchPacking = objPacking.ListaTotalETallasBatchEstilo(id).ToList();
                listaTallasDBatchPacking = objPacking.ListaTotalDefTallasBatchEstilo(id).ToList();
            }

            string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }
            var result = Json(new
            {
                listaTalla = listaTallas,
                estilos = estilo,
                listTallaStaging = listaTallasStaging,
                listaTallasTotalPnlBatch = listaTallasTPnlBatch,
                listaTallasTotalBatch = listaTallasTPrintShopBatch,
                listaTallasTotalPackingBatch = listaTallasTPackingBatch,
                listaTallasTotalPBatchPacking = listaTallasPBatchPacking,
                listaTallasTotalEBatchPacking = listaTallasEBatchPacking,
                listaTallasTotalDBatchPacking = listaTallasDBatchPacking
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Staging_Estilo(int? id)
        {
            List<StagingD> listaTallas = objTallas.ListaTallasStagingPorEstilo(id).ToList();
            /*string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }*/
            var result = Json(new { listaTalla = listaTallas/*, estilos = estilo*/ });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_PrintShop_Estilo(int? id)
        {
            List<PrintShopC> listaTallas = objPrint.ListaTallasPrintShop(id).ToList();
            List<PrintShopC> listaTallasEstilo = objPrint.ObtenerTallas(id).ToList();
            List<int> listaTallasTBatch = objPrint.ListaTotalTallasBatchEstilo(id).ToList();
            var result = Json(new { listaTalla = listaTallas, listaEstiloTallas = listaTallasEstilo, listaPrint = listaTallasTBatch });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Pnl_Estilo(int? id)
        {
            List<Pnl> listaTallas = objPnl.ListaTallasPnl(id).ToList();
            List<Pnl> listaTallasEstilo = objPnl.ObtenerTallas(id).ToList();
            List<int> listaTallasTBatch = objPnl.ListaTotalTallasPNLBatchEstilo(id).ToList();
            var result = Json(new { listaTalla = listaTallas, listaEstiloTallas = listaTallasEstilo, listaPrint = listaTallasTBatch });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Packing_Estilo(int? id)
        {
            List<PackingM> listaTallas = objPacking.ListaTallasPacking(id).ToList();
            List<PackingM> listaTallasEstilo = objPacking.ObtenerTallas(id).ToList();
            List<int> listaTallasTBatch = objPacking.ListaTotalTallasPackingBatchEstilo(id).ToList();
            var result = Json(new { listaTalla = listaTallas, listaEstiloTallas = listaTallasEstilo, listaPrint = listaTallasTBatch });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Total_PrintShop_Estilo(int? id)
        {
            List<PrintShopC> listaTallas = objPrint.ListaTallasTotalPrintShop(id).ToList();
            /*string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }*/
            var result = Json(new { listaTalla = listaTallas/*, estilos = estilo*/ });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult CrearPO()
        {
            OrdenesCompra pedido = new OrdenesCompra();
            POSummary summary = new POSummary();
            ListasClientes(pedido);
            ListaEstados(pedido);
            ListaGenero(summary);
            ListaTela(summary);
            ListaTipoCamiseta(summary);
            ListaEspecialidades(summary);
            ListaTipoOrden(pedido);
            return View();
        }
        [HttpPost]
        public ActionResult RegistrarPO([Bind] OrdenesCompra ordenCompra, string po, string VPO, DateTime FechaCancel, DateTime FechaOrden, int Cliente, int Clientefinal, int TotalUnidades, int IdTipoOrden)
        {
            ListaEstados(ordenCompra);
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            ordenCompra.Usuario = noEmpleado;
            objPedido.AgregarPO(ordenCompra);
            Session["idPedido"] = objPedido.Obtener_Utlimo_po();

            return View(ordenCompra);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearPO([Bind] OrdenesCompra ordenCompra)
        {
            if (ModelState.IsValid)
            {
                ObtenerIdClientes(ordenCompra);
                ListaEstados(ordenCompra);
                //objPedido.AgregarPO(pedido);
            }

            return View();
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

        [HttpGet]
        public ActionResult DetallesRev(int? id)
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

        [HttpGet]
        public int ObtenerPORevision(int? id)
        {
            OrdenesCompra pedido = objPedido.ConsultarListaPO(id);
            SeleccionarClientes(pedido);
            SeleccionarClienteFinal(pedido);
            /*int revisiones = objRevision.ObtenerNumeroRevisiones(id);
            int identificador = 0;
            string rev;
            if (revisiones == 0 && pedido.IdStatus != 3)
            {
                identificador = revisiones + 1;
                rev = pedido.PO + "-REV" + identificador;
            }
            else
            {
                identificador = revisiones + 1;
                rev = pedido.PO + "-REV" + identificador;
            }pedido.PO = rev.Replace(" ", "");*/


            // pedido.IdPedido = Convert.ToInt32(id);
            pedido.FechaOrden = DateTime.Today;
            Session["id_pedido"] = id;

            ObtenerEstadoRevisado(pedido);

            objPedido.AgregarPO(pedido);
            Session["idPedidoNuevo"] = objPedido.Obtener_Utlimo_po();
            //Cambia estado pedido original a 5
            objPedido.ActualizarEstadoPO(Convert.ToInt32(Session["id_pedido"]));

            //Registrar en Revisado el Pedido Nuevo 
            //int PedidosId = objPedido.Obtener_Utlimo_po();
            //Session["idPedidoNuevo"] = PedidosId;
            int PedidoNuevo = Convert.ToInt32(Session["idPedidoNuevo"]);
            if (PedidoNuevo != 0)
            {
                Revision revisionPO = new Revision()
                {
                    IdPedido = Convert.ToInt32(Session["id_pedido"]),
                    IdRevisionPO = Convert.ToInt32(Session["idPedidoNuevo"]),
                    FechaRevision = DateTime.Today,
                    IdStatus = pedido.IdStatus

                };
                objRevision.AgregarRevisionesPO(revisionPO);
            }
            //Obtener los estilos por ID Pedido Anterior
            List<POSummary> listaItems = objItems.ListaEstilosPorPO(Convert.ToInt32(Session["id_pedido"])).ToList();
            POSummary estilos = new POSummary();
            foreach (var item in listaItems)
            {
                estilos.EstiloItem = item.EstiloItem;
                estilos.IdColor = item.CatColores.CodigoColor;
                estilos.Cantidad = item.Cantidad;
                estilos.Precio = item.Precio;
                estilos.PedidosId = item.PedidosId;
                estilos.IdGenero = item.CatGenero.GeneroCode;
                estilos.IdTela = item.IdTela;
                estilos.TipoCamiseta = item.CatTipoCamiseta.TipoProducto;
                estilos.IdItems = item.IdItems;
                estilos.IdEspecialidad = item.CatEspecialidades.IdEspecialidad;
                Session["id_estilo"] = estilos.IdItems;
                int? idEstilo = Convert.ToInt32(Convert.ToInt32(Session["id_estilo"]));
                estilos.PedidosId = Convert.ToInt32(Session["idPedidoNuevo"]);
                objItems.AgregarItems(estilos);
                Session["estiloIdItem"] = objItems.Obtener_Utlimo_Item();
                //Obtener la lista de tallas del item
                List<ItemTalla> listaTallas = objTallas.ListaTallasPorSummary(idEstilo).ToList();
                ItemTalla tallas = new ItemTalla();

                foreach (var itemT in listaTallas)
                {

                    tallas.Talla = itemT.Talla;
                    tallas.Cantidad = itemT.Cantidad;
                    tallas.Extras = itemT.Extras;
                    tallas.Ejemplos = itemT.Ejemplos;
                    tallas.IdSummary = Convert.ToInt32(Session["estiloIdItem"]);

                    objTallas.RegistroTallas(tallas);
                }

            }
            return Convert.ToInt32(Session["idPedidoNuevo"]);
        }

        [HttpGet]
        public ActionResult Revision(int? id)
        {
            int idPedido = ObtenerPORevision(id);
            Session["idPedidoRevision"] = idPedido;
            POSummary summary = new POSummary();
            ListaGenero(summary);
            ListaTela(summary);
            ListaTipoCamiseta(summary);
            if (id == null)
            {
                return View();
            }
            OrdenesCompra pedidos = objPedido.ConsultarListaPO(idPedido);
            ListasClientes(pedidos);


            /* if(id != null)
             {
                 RegistrarRevisionPO(pedidos);
             }   */


            if (pedidos == null)
            {
                return View();
            }

            return View(pedidos);

        }
        [HttpPost]
        public ActionResult RegistrarRevisionPO([Bind] OrdenesCompra pedido)
        {
            List<POSummary> listaItems = objItems.ListaItemsPorPO(pedido.IdPedido).ToList();


            // List<ItemTalla> listaTallas = objTallas.ListaTallasPorEstilo(id).ToList();
            return View(pedido);
        }

        public ActionResult CancelarPO(int id)
        {
            objPedido.ActualizarEstadoPOCancelado(id);
            List<POSummary> listaItems = objItems.ListaItemsPorPO(id).ToList();
            foreach (var item in listaItems)
            {
                objPedido.ActualizarEstadoStyleCancelado(item.IdItems);

            }
            
            TempData["cancelarPO"] = "The purchase order was canceled correctly.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CancelarStyle(int id, int IdPedido)
        {
            objPedido.ActualizarEstadoStyleCancelado(id);
            TempData["cancelarStylePO"] = "The Style was canceled correctly.";
            return Json(new
            {
                redirectUrl = Url.Action("Detalles", "Pedidos", new { id = IdPedido }),
                isRedirect = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Revision(int id, [Bind] OrdenesCompra pedido)
        {
            string cliente = Request.Form["Nombre"].ToString();
            pedido.Cliente = Int32.Parse(cliente);

            string clienteFinal = Request.Form["NombreCliente"].ToString();
            pedido.ClienteFinal = Int32.Parse(clienteFinal);
            /*if (id != pedido.IdPedido)
            {
                return View();
            }*/
            /* if (ModelState.IsValid)
             {
                 objPedido.ActualizarPedidos(pedido);
                 TempData["pedidoRevision"] = "Se registro correctamente la revisión de la orden de compra .";
                 return RedirectToAction("Index");
             }
             else
             {
                 TempData["pedidoRevisionError"] = "No se pudo registrar la revisión de la orden de compra, intentelo más tarde.";
             }*/
            return View(pedido);
        }



        [HttpGet]
        public ActionResult EditarEstilo(int? id)
        {
            if (id == null)
            {
                return View();
            }


            POSummary items = objItems.ConsultarListaEstilos(id);
            ListaGenero(items);
            ListaTela(items);
            ListaTipoCamiseta(items);
            ListaEspecialidades(items);
            items.CatColores = objColores.ConsultarListaColores(items.ColorId);
            items.ItemDescripcion = objEst.ConsultarListaItemDesc(items.IdItems);
            items.CatEspecialidades = objEspecialidad.ConsultarListaEspecialidad(items.IdEspecialidad);
            items.PedidosId = items.PedidosId;
            SeleccionarGenero(items);
            SeleccionarTela(items);
            SeleccionarTipoCamiseta(items);
            SeleccionarTipoEspecialidad(items);


            if (items == null)
            {

                return View();
            }

            return PartialView(items);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEstilo(int id, [Bind] POSummary items)
        {
            items.IdItems = id;
            /* if (id != items.IdItems)
             {
                 return View();
             }*/
            string genero = Request.Form["Genero"].ToString();
            items.Id_Genero = objGenero.ObtenerIdGenero(genero);
            string tipoCamiseta = Request.Form["DescripcionTipo"].ToString();
            items.IdCamiseta = objTipoC.ObtenerIdTipoCamiseta(tipoCamiseta);
            string tela = Request.Form["Tela"].ToString();
            items.IdTela = Int32.Parse(tela);
            string estilo = items.ItemDescripcion.ItemEstilo;
            items.IdEstilo = objEst.ObtenerIdEstilo(estilo);
            string color = items.CatColores.CodigoColor;
            items.ColorId = objColores.ObtenerIdColor(color);
            if (items.IdItems != 0)
            {
                objItems.ActualizarEstilos(items);
                TempData["itemEditar"] = "The style was modified correctly.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["itemEditarError"] = "The style could not be modified, try it later.";
            }
            return View(items);
        }

        [HttpGet]
        public ActionResult EditarEstiloNuevo(int? id)
        {
            if (id == null)
            {
                return View();
            }

            POSummary items = objItems.ConsultarListaEstilos(id);
            ListaGenero(items);
            ListaTela(items);
            ListaTipoCamiseta(items);
            ListaEspecialidades(items);
            items.CatColores = objColores.ConsultarListaColores(items.ColorId);
            items.ItemDescripcion = objEst.ConsultarListaItemDesc(items.IdItems);
            items.CatEspecialidades = objEspecialidad.ConsultarListaEspecialidad(items.IdEspecialidad);
            items.PedidosId = items.PedidosId;
            SeleccionarGenero(items);
            SeleccionarTela(items);
            SeleccionarTipoCamiseta(items);
            SeleccionarTipoEspecialidad(items);

            if (items == null)
            {

                return View();
            }

            return PartialView(items);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEstiloNuevo(int id, [Bind] POSummary items)
        {
            items.IdItems = id;
            /* if (id != items.IdItems)
             {
                 return View();
             }*/
            string genero = Request.Form["Genero"].ToString();
            items.Id_Genero = objGenero.ObtenerIdGenero(genero);
            string tipoCamiseta = Request.Form["DescripcionTipo"].ToString();
            items.IdCamiseta = objTipoC.ObtenerIdTipoCamiseta(tipoCamiseta);
            string tela = Request.Form["Tela"].ToString();
            items.IdTela = Int32.Parse(tela);
            string estilo = items.ItemDescripcion.ItemEstilo;
            items.IdEstilo = objEst.ObtenerIdEstilo(estilo);
            string color = items.CatColores.CodigoColor;
            items.ColorId = objColores.ObtenerIdColor(color);
            if (items.IdItems != 0)
            {
                objItems.ActualizarEstilos(items);
                TempData["itemEditar"] = "The style was modified correctly.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["itemEditarError"] = "The style could not be modified, try it later.";
            }
            return View(items);
        }

        [HttpGet]
        public ActionResult EditarPO(int? id)
        {
            if (id == null)
            {
                return View();
            }

            OrdenesCompra pedido = objPedido.ConsultarListaPO(id);
            pedido.FechaCancelada = String.Format("{0:MM/dd/yyyy}", pedido.FechaCancel);
            pedido.FechaOrdenFinal = String.Format("{0:MM/dd/yyyy}", pedido.FechaOrden);
            pedido.NombrePO=pedido.PO.TrimEnd(' ');
            pedido.PO = pedido.NombrePO;
            ListasClientes(pedido);      
            ListaTipoOrden(pedido);           
            pedido.IdEstilo = pedido.IdPedido;
            pedido.CatCliente = objCliente.ConsultarListaClientes(pedido.Cliente);
            pedido.CatClienteFinal = objClienteFinal.ConsultarListaClientesFinal(pedido.ClienteFinal);
            pedido.CatTipoOrden = objTipoOrden.ConsultarListaTipoOrden(pedido.IdTipoOrden);

            if (pedido == null)
            {

                return View();
            }

            return View(pedido);

        }

        [HttpGet]
        public ActionResult ActualizarInfPO( [Bind] OrdenesCompra pedido,int id, string po, string VPO, DateTime FechaCancel, DateTime FechaOrden, int Cliente, int Clientefinal, int TotalUnidades, int IdTipoOrden)
        {
            pedido.IdPedido = id;

            if (pedido.IdPedido != 0)
            {
                objPedido.ActualizarPedidos(pedido);
                TempData["itemEditar"] = "The purchase order was modified correctly.";
               return RedirectToAction("Index");
            }
            else
            {
                TempData["itemEditarError"] = "The purchase order could not be modified, try it later.";
            }
            return View(pedido);
        }


        [HttpPost]
        public ActionResult Eliminar(int? id)
        {
            objItems.EliminarTallasEstilo(id);
            objItems.EliminarEstilos(id);
            TempData["eliminarEstilo"] = "The style was removed correctly.";
            return View();
        }



        public void SeleccionarClientes(OrdenesCompra pedido)
        {
            List<CatCliente> listaClientes = pedido.LCliente;
            listaClientes = objCliente.ListaClientes().ToList();
            pedido.CatCliente = objCliente.ConsultarListaClientes(pedido.Cliente);
            pedido.CatCliente.Customer = pedido.Cliente;
            ViewBag.listCliente = new SelectList(listaClientes, "Customer", "Nombre", pedido.Cliente);


        }

        public void SeleccionarClienteFinal(OrdenesCompra pedido)
        {

            List<CatClienteFinal> listaClientesFinal = pedido.LClienteFinal;
            listaClientesFinal = objClienteFinal.ListaClientesFinal().ToList();
            pedido.CatClienteFinal = objClienteFinal.ConsultarListaClientesFinal(pedido.ClienteFinal);
            pedido.CatClienteFinal.CustomerFinal = pedido.ClienteFinal;
            ViewBag.listClienteFinal = new SelectList(listaClientesFinal, "CustomerFinal", "NombreCliente", pedido.ClienteFinal);

        }

        public void SeleccionarGenero(POSummary items)
        {

            List<CatGenero> listaGenero = items.ListaGeneros;
            listaGenero = objGenero.ListaGeneros().ToList();
            items.CatGenero = objGenero.ConsultarListaGenero(items.Id_Genero);
            items.CatGenero.IdGender = items.Id_Genero;
            ViewBag.listGenero = new SelectList(listaGenero, "GeneroCode", "Genero", items.CatGenero.GeneroCode);

        }

        public void SeleccionarTela(POSummary items)
        {

            List<CatTela> listaTela = items.ListaTelas;
            listaTela = objTela.ListaTela().ToList();
            items.CatTela = objTela.ConsultarListaTelas(items.IdTela);
            items.CatTela.Id_Tela = items.IdTela;
            ViewBag.listTela = new SelectList(listaTela, "Id_Tela", "Tela", items.IdTela);
        }

        public void SeleccionarTipoCamiseta(POSummary items)
        {

            List<CatTipoCamiseta> listaTipoCamiseta = items.ListaTipoCamiseta;
            listaTipoCamiseta = objTipoC.ListaTipoCamiseta().ToList();
            items.CatTipoCamiseta = objTipoC.ConsultarListaCamisetas(items.IdCamiseta);
            items.CatTipoCamiseta.IdTipo = items.IdCamiseta;
            ViewBag.listTipoCamiseta = new SelectList(listaTipoCamiseta, "TipoProducto", "DescripcionTipo", items.CatTipoCamiseta.TipoProducto);
        }

        public void SeleccionarTipoEspecialidad(POSummary items)
        {

            List<CatEspecialidades> listaEspecialidades = items.ListaEspecialidades;
            listaEspecialidades = objEspecialidad.ListaEspecialidades().ToList();
            items.CatEspecialidades = objEspecialidad.ConsultarListaEspecialidad(items.IdEspecialidad);
            items.CatEspecialidades.IdEspecialidad = items.IdEspecialidad;
            ViewBag.listEspecialidad = new SelectList(listaEspecialidades, "IdEspecialidad", "Especialidad", items.IdEspecialidad);
        }

        public void ListasClientes(OrdenesCompra pedido)
        {
            List<CatCliente> listaClientes = pedido.ListaClientes;
            listaClientes = objCliente.ListaClientes().ToList();

            ViewBag.listCliente = new SelectList(listaClientes, "Customer", "Nombre", pedido.Cliente);

            List<CatClienteFinal> listaClientesFinal = pedido.ListaClientesFinal;
            listaClientesFinal = objClienteFinal.ListaClientesFinal().ToList();
            ViewBag.listClienteFinal = new SelectList(listaClientesFinal, "CustomerFinal", "NombreCliente", pedido.ClienteFinal);
        }

        public void ListaTipoOrden(OrdenesCompra pedido)
        {
            List<CatTipoOrden> listaTipoOrden = pedido.ListadoTipoOrden;
            listaTipoOrden = objTipoOrden.ListaTipoOrden().ToList();

            ViewBag.listTipoOrden = new SelectList(listaTipoOrden, "IdTipoOrden", "TipoOrden", pedido.IdTipoOrden);

        }

        public void ObtenerIdClientes(OrdenesCompra pedido)
        {
            string cliente = Request.Form["listCliente"].ToString();
            pedido.Cliente = Int32.Parse(cliente);
            pedido.CatCliente = objCliente.ConsultarListaClientes(pedido.Cliente);


            string clienteFinal = Request.Form["listClienteFinal"].ToString();
            pedido.ClienteFinal = Int32.Parse(clienteFinal);
            pedido.CatClienteFinal = objClienteFinal.ConsultarListaClientesFinal(pedido.ClienteFinal);



        }

        public void ListaGenero(POSummary summary)
        {
            List<CatGenero> listaGenero = summary.ListaGeneros;
            listaGenero = objGenero.ListaGeneros().ToList();

            ViewBag.listGenero = new SelectList(listaGenero, "GeneroCode", "Genero", summary.IdGenero);

        }

        public void ListaTela(POSummary summary)
        {
            List<CatTela> listaTela = summary.ListaTelas;
            listaTela = objTela.ListaTela().ToList();

            ViewBag.listTela = new SelectList(listaTela, "Id_Tela", "Tela", summary.IdTela);

        }

        public void ListaTipoCamiseta(POSummary summary)
        {
            List<CatTipoCamiseta> listaTipoCamiseta = summary.ListaTipoCamiseta;
            listaTipoCamiseta = objTipoC.ListaTipoCamiseta().ToList();

            ViewBag.listTipoCamiseta = new SelectList(listaTipoCamiseta, "TipoProducto", "DescripcionTipo", summary.TipoCamiseta);

        }

        public void ListaEspecialidades(POSummary summary)
        {
            List<CatEspecialidades> listaEspecialidades = summary.ListaEspecialidades;
            listaEspecialidades = objEspecialidad.ListaEspecialidades().ToList();

            ViewBag.listEspecialidad = new SelectList(listaEspecialidades, "IdEspecialidad", "Especialidad", summary.IdEspecialidad);

        }

       

        public void ListaEstados(OrdenesCompra pedido)
        {
            List<CatStatus> listaEstados = pedido.ListaCatStatus;
            listaEstados = objEstados.ListarEstados().ToList();

            ViewBag.listEstados = new SelectList(listaEstados, "IdStatus", "Estado", pedido.IdStatus);
            foreach (var item in listaEstados)
            {
                if (item.IdStatus == 1)
                {
                    pedido.IdStatus = item.IdStatus;
                }

            }

        }
        public void ObtenerEstadoRevisado(OrdenesCompra pedido)
        {
            List<CatStatus> listaEstados = pedido.ListaCatStatus;
            listaEstados = objEstados.ListarEstados().ToList();

            ViewBag.listEstados = new SelectList(listaEstados, "IdStatus", "Estado", pedido.IdStatus);
            foreach (var item in listaEstados)
            {
                if (item.IdStatus == 1)
                {
                    pedido.IdStatus = item.IdStatus;
                }

            }
        }
        

        public ActionResult ReportWIP()
        {
            FileContentResult robj;
            //string year = Convert.ToString(Session["year_reporte"]);
            List<OrdenesCompra> listaPedidos = objPedido.ListaOrdenCompraWIP(1).ToList();
            List<OrdenesCompra> listaShipped = objPedido.ListaOrdenCompraWIP(2).ToList();
            List<OrdenesCompra> listaCancelled = objPedido.ListaOrdenCompraWIP(3).ToList();


            int row = 1, column = 1;
            using (XLWorkbook libro_trabajo = new XLWorkbook())
            { //Regex.Replace(pedido, @"\s+", " "); 
                var wp = libro_trabajo.Worksheets.Add("WIP");
                var ws = libro_trabajo.Worksheets.Add("SHIPPED");
                var wc = libro_trabajo.Worksheets.Add("CANCELLED");
                wp.TabColor = XLColor.Green;
                ws.TabColor = XLColor.Yellow;
                wc.TabColor = XLColor.Red;



                //CABECERAS TABLA
                var headers = new List<String[]>();
                List<String> titulos = new List<string>();
                titulos.Add("CUSTOMER"); titulos.Add("RETAILER"); titulos.Add("P.O. RECVD DATA"); titulos.Add("PO NO."); titulos.Add("BRAND NAME"); titulos.Add("AMT PO"); titulos.Add("REG/BULK"); titulos.Add("BALANCE QTY");
                titulos.Add("EXPECTED SHIP DATE"); titulos.Add("ORIGINAL CUST DUE DATE"); titulos.Add("DESIGN NAME"); titulos.Add("STYLE"); titulos.Add("MillPO"); titulos.Add("COLOR"); titulos.Add("GENDER");
                titulos.Add("BLANKS RECEIVED"); titulos.Add("PARTIAL/COMPLETE BLANKS"); titulos.Add("ART RECEIVED"); titulos.Add("TRIM RECEIVED"); titulos.Add("PACK INST.RCVD"); titulos.Add("PRICE TICKET RECEIVED");
                titulos.Add("UCC RECEIVED"); titulos.Add("COMMENTS UPDATE"); titulos.Add("COMMENTS");
                headers.Add(titulos.ToArray());
                ws.Cell(row, 1).Value = headers;
                ws.Range(row, 1, row, 24).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 0, 0);
                ws.Range(row, 1, row, 24).Style.Font.FontColor = XLColor.FromArgb(255, 255, 255);
                wp.Cell(row, 1).Value = headers;
                wp.Range(row, 1, row, 24).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 0, 0);
                wp.Range(row, 1, row, 24).Style.Font.FontColor = XLColor.FromArgb(255, 255, 255);
                wc.Cell(row, 1).Value = headers;
                wc.Range(row, 1, row, 24).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 0, 0);
                wc.Range(row, 1, row, 24).Style.Font.FontColor = XLColor.FromArgb(255, 255, 255);
                wp.RangeUsed().SetAutoFilter().Column(1);
                ws.RangeUsed().SetAutoFilter().Column(1);
                wc.RangeUsed().SetAutoFilter().Column(1);
                row++; //AGREGAR DATOS LISTAS
                SheetWIP(listaPedidos, wp, row);
                SheetShipped(listaShipped, ws, row);
                SheetCancelled(listaCancelled, wc, row);

                wp.Rows().AdjustToContents();
                wp.Columns().AdjustToContents();
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();
                wc.Rows().AdjustToContents();
                wc.Columns().AdjustToContents();
                //ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
               /* HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"WIP.xlsx\"");*/
                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    libro_trabajo.SaveAs(memoryStream);
                    var bytesdata = File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "WIP.xlsx");
                    robj = bytesdata;
                }
             
               // return File(httpResponse.OutputStream);
            }
          
            return Json(robj, JsonRequestBehavior.AllowGet);
        }

      /*  public void ReportWIP()
        {
            
        }*/

        public void SheetWIP(List<OrdenesCompra> listaPedidos, IXLWorksheet wp, int row)
        {
            foreach (OrdenesCompra e in listaPedidos)
            {
                var celdas = new List<String[]>();
                List<String> datos = new List<string>();
                DateTime fechaActual = DateTime.Today;
                string fechaHoy = String.Format("{0:dd/MM/yyyy}", fechaActual);
                if (e.CatComentarios.FechaComents == fechaHoy)
                {
                    wp.Range(row, 1, row, 24).Style.Fill.BackgroundColor = XLColor.FromArgb(254, 232, 200);
                }
                datos.Add(e.CatCliente.Nombre);

                datos.Add(e.CatClienteFinal.NombreCliente);
                datos.Add(e.FechaRecOrden);
                //PO NO
                if (e.RestaPrintshop <= 10)
                {
                    datos.Add(e.PO);
                    wp.Range(row, 4, row, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(95, 157, 205);
                    wp.Range(row, 4, row, 4).Style.Font.Bold = true;
                    wp.Range(row, 4, row, 4).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else
                {
                    datos.Add(e.PO);
                    wp.Range(row, 4, row, 4).Style.Font.Bold = true;
                    wp.Range(row, 4, row, 4).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }

                datos.Add(e.CatTipoBrand.TipoBrandName);
                wp.Range(row, 5, row, 5).Style.Font.Bold = true;
                datos.Add(e.VPO);
                datos.Add(e.CatTipoOrden.TipoOrden);
                wp.Range(row, 7, row, 7).Style.Font.Bold = true;
                datos.Add((e.InfoSummary.CantidadEstilo).ToString());
                datos.Add(e.FechaOrdenFinal);
                datos.Add(e.FechaCancelada);
                //DESIGN NAME
                if (e.DestinoSalida == 2)
                {
                    datos.Add(e.InfoSummary.ItemDesc.Descripcion);
                    wp.Range(row, 11, row, 11).Style.Fill.BackgroundColor = XLColor.FromArgb(190, 174, 241);
                    wp.Range(row, 11, row, 11).Style.Font.Bold = true;
                    wp.Range(row, 11, row, 11).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else
                {
                    datos.Add(e.InfoSummary.ItemDesc.Descripcion);
                    wp.Range(row, 11, row, 11).Style.Font.Bold = true;
                    wp.Range(row, 11, row, 11).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                datos.Add(e.InfoSummary.ItemDesc.ItemEstilo);
                datos.Add(e.MillPO);
                datos.Add(e.InfoSummary.CatColores.DescripcionColor);
                datos.Add(e.InfoSummary.CatGenero.Genero);
                // BLANKS RECEIVED 
                if (e.TotalRestante == 0)
                {
                    datos.Add((e.TotalRestante).ToString());
                    wp.Range(row, 16, row, 16).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else if (e.TotalRestante != e.InfoSummary.TotalEstilo)
                {
                    datos.Add((e.TotalRestante).ToString());
                    wp.Range(row, 16, row, 16).Style.Font.FontColor = XLColor.FromArgb(246, 57, 57);
                }
                //PARTIAL/COMPLETE BLANKS
                if (e.TipoPartial == "PARTIAL")
                {

                    datos.Add(e.TipoPartial);
                    wp.Range(row, 17, row, 17).Style.Fill.BackgroundColor = XLColor.FromArgb(249, 136, 29);
                    wp.Range(row, 17, row, 17).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);

                }
                else if (e.TipoPartial == "COMPLETE")
                {
                    datos.Add(e.TipoPartial);
                    wp.Range(row, 17, row, 17).Style.Fill.BackgroundColor = XLColor.FromArgb(64, 191, 128);
                    wp.Range(row, 17, row, 17).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else if (e.TipoPartial == null)
                {
                    e.TipoPartial = "";
                    datos.Add(e.TipoPartial);
                    wp.Range(row, 17, row, 17).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }

                //ART RECEIVED
                if (e.ImagenArte.StatusArteInf == "IN HOUSE")
                {
                    datos.Add((e.ImagenArte.StatusArteInf).ToString());
                    wp.Range(row, 18, row, 18).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 193, 116);
                    wp.Range(row, 18, row, 18).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);

                }
                else if (e.ImagenArte.StatusArteInf == "REVIEWED")
                {
                    datos.Add((e.ImagenArte.StatusArteInf).ToString());
                    wp.Range(row, 18, row, 18).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 193, 116);
                    wp.Range(row, 18, row, 18).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else if (e.ImagenArte.StatusArteInf == "PENDING")
                {
                    datos.Add((e.ImagenArte.StatusArteInf).ToString());
                    wp.Range(row, 18, row, 18).Style.Fill.BackgroundColor = XLColor.FromArgb(236, 95, 95);
                    wp.Range(row, 18, row, 18).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else if (e.ImagenArte.StatusArteInf == "APPROVED")
                {
                    string infoArte = e.ImagenArte.StatusArteInf + e.ImagenArte.FechaArte;
                    datos.Add(infoArte);
                    wp.Range(row, 18, row, 18).Style.Fill.BackgroundColor = XLColor.FromArgb(246, 129, 51);
                    wp.Range(row, 18, row, 18).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }

                //TRIM RECEIVED
                if (e.Trims.restante <= 0 && e.Trims.estado == "1")
                {
                    datos.Add(e.Trims.fecha_recibo);
                    wp.Range(row, 19, row, 19).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 193, 116);
                    wp.Range(row, 19, row, 19).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else if (e.Trims.restante >= 1 && e.Trims.estado == "1")
                {
                    datos.Add(e.Trims.fecha_recibo);
                    wp.Range(row, 19, row, 19).Style.Fill.BackgroundColor = XLColor.FromArgb(245, 213, 67);
                    wp.Range(row, 19, row, 19).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else
                {
                    datos.Add(e.Trims.fecha_recibo);
                    wp.Range(row, 19, row, 19).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                //PACK INSTRUCTION
                if (e.InfoPackInstruction.Fecha_Pack != "" && e.InfoPackInstruction.EstadoPack == 1)
                {
                    datos.Add(e.InfoPackInstruction.Fecha_Pack);
                    wp.Range(row, 20, row, 20).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 193, 116);
                    wp.Range(row, 20, row, 20).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }                
                else
                {
                    datos.Add(e.InfoPackInstruction.Fecha_Pack);
                    wp.Range(row, 20, row, 20).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
     
                //PRICE TICKET RECEIVED
                if (e.InfoPriceTickets.Restante <= 0 && e.InfoPriceTickets.Estado == "1")
                {
                    datos.Add(e.InfoPriceTickets.Fecha_recibo);
                    wp.Range(row, 21, row, 21).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 193, 116);
                    wp.Range(row, 21, row, 21).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else if (e.InfoPriceTickets.Restante >= 1 && e.InfoPriceTickets.Estado == "1")
                {
                    datos.Add(e.InfoPriceTickets.Fecha_recibo);
                    wp.Range(row, 21, row, 21).Style.Fill.BackgroundColor = XLColor.FromArgb(245, 213, 67);
                    wp.Range(row, 21, row, 21).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else
                {
                    datos.Add(e.InfoPriceTickets.Fecha_recibo);
                    wp.Range(row, 21, row, 21).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                //UCC RECEIVED
                if (e.InfoSummary.FechaUCC != "")
                {
                    datos.Add(e.InfoSummary.FechaUCC);
                    wp.Range(row, 22, row, 22).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 193, 116);
                    wp.Range(row, 22, row, 22).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else
                {
                    datos.Add(e.InfoSummary.FechaUCC);
                    wp.Range(row, 22, row, 22).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                datos.Add(e.CatComentarios.FechaComents);
                datos.Add(e.CatComentarios.Comentario);
                celdas.Add(datos.ToArray());
                wp.Cell(row, 1).Value = celdas;


                row++;
            }
        }

        public void SheetShipped(List<OrdenesCompra> listaShipped, IXLWorksheet ws, int row)
        {
            foreach (OrdenesCompra e in listaShipped)
            {
                var celdas = new List<String[]>();
                List<String> datos = new List<string>();
                datos.Add(e.CatCliente.Nombre);

                datos.Add(e.CatClienteFinal.NombreCliente);
                datos.Add(e.FechaRecOrden);
                //PO NO
                if (e.RestaPrintshop <= 10)
                {
                    datos.Add(e.PO);
                    ws.Range(row, 4, row, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(95, 157, 205);
                    ws.Range(row, 4, row, 4).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else
                {
                    datos.Add(e.PO);
                    ws.Range(row, 4, row, 4).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }

                datos.Add(e.CatTipoBrand.TipoBrandName);
                datos.Add(e.VPO);
                datos.Add(e.CatTipoOrden.TipoOrden);
                datos.Add((e.Shipped.Cantidad).ToString());
                datos.Add(e.FechaOrdenFinal);
                datos.Add(e.FechaCancelada);
                //DESIGN NAME
                if (e.DestinoSalida == 2)
                {
                    datos.Add(e.InfoSummary.ItemDesc.Descripcion);
                    ws.Range(row, 11, row, 11).Style.Fill.BackgroundColor = XLColor.FromArgb(190, 174, 241);
                    ws.Range(row, 11, row, 11).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else
                {
                    datos.Add(e.InfoSummary.ItemDesc.Descripcion);
                    ws.Range(row, 11, row, 11).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                datos.Add(e.InfoSummary.ItemDesc.ItemEstilo);
                datos.Add(e.MillPO);
                datos.Add(e.InfoSummary.CatColores.DescripcionColor);
                datos.Add(e.InfoSummary.CatGenero.Genero);
                // BLANKS RECEIVED 
                if (e.TotalRestante == 0)
                {
                    datos.Add((e.TotalRestante).ToString());
                    ws.Range(row, 16, row, 16).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else if (e.TotalRestante != e.InfoSummary.TotalEstilo)
                {
                    datos.Add((e.TotalRestante).ToString());
                    ws.Range(row, 16, row, 16).Style.Font.FontColor = XLColor.FromArgb(246, 57, 57);
                }
                //PARTIAL/COMPLETE BLANKS
                if (e.TipoPartial == "PARTIAL")
                {

                    datos.Add(e.TipoPartial);
                    ws.Range(row, 17, row, 17).Style.Fill.BackgroundColor = XLColor.FromArgb(249, 136, 29);
                    ws.Range(row, 17, row, 17).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);

                }
                else if (e.TipoPartial == "COMPLETE")
                {
                    datos.Add(e.TipoPartial);
                    ws.Range(row, 17, row, 17).Style.Fill.BackgroundColor = XLColor.FromArgb(64, 191, 128);
                    ws.Range(row, 17, row, 17).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else if (e.TipoPartial == null)
                {
                    e.TipoPartial = "";
                    datos.Add(e.TipoPartial);
                    ws.Range(row, 17, row, 17).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }

                //ART RECEIVED
                if (e.ImagenArte.StatusArteInf == "IN HOUSE")
                {
                    datos.Add((e.ImagenArte.StatusArteInf).ToString());
                    ws.Range(row, 18, row, 18).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 193, 116);
                    ws.Range(row, 18, row, 18).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);

                }
                else if (e.ImagenArte.StatusArteInf == "REVIEWED")
                {
                    datos.Add((e.ImagenArte.StatusArteInf).ToString());
                    ws.Range(row, 18, row, 18).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 193, 116);
                    ws.Range(row, 18, row, 18).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else if (e.ImagenArte.StatusArteInf == "PENDING")
                {
                    datos.Add((e.ImagenArte.StatusArteInf).ToString());
                    ws.Range(row, 18, row, 18).Style.Fill.BackgroundColor = XLColor.FromArgb(236, 95, 95);
                    ws.Range(row, 18, row, 18).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else if (e.ImagenArte.StatusArteInf == "APPROVED")
                {
                    string infoArte = e.ImagenArte.StatusArteInf + "-" + e.ImagenArte.FechaArte;
                    datos.Add(infoArte);
                    ws.Range(row, 18, row, 18).Style.Fill.BackgroundColor = XLColor.FromArgb(246, 129, 51);
                    ws.Range(row, 18, row, 18).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }

                //TRIM RECEIVED
                if (e.Trims.restante <= 0 && e.Trims.estado == "1")
                {
                    datos.Add(e.Trims.fecha_recibo);
                    ws.Range(row, 19, row, 19).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 193, 116);
                    ws.Range(row, 19, row, 19).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else if (e.Trims.restante >= 1 && e.Trims.estado == "1")
                {
                    datos.Add(e.Trims.fecha_recibo);
                    ws.Range(row, 19, row, 19).Style.Fill.BackgroundColor = XLColor.FromArgb(245, 213, 67);
                    ws.Range(row, 19, row, 19).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else
                {
                    datos.Add(e.Trims.fecha_recibo);
                    ws.Range(row, 19, row, 19).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                //PACK INSTRUCTION
                if (e.InfoPackInstruction.Fecha_Pack != "" && e.InfoPackInstruction.EstadoPack == 1)
                {
                    datos.Add(e.InfoPackInstruction.Fecha_Pack);
                    ws.Range(row, 20, row, 20).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 193, 116);
                    ws.Range(row, 20, row, 20).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else
                {
                    datos.Add(e.InfoPackInstruction.Fecha_Pack);
                    ws.Range(row, 20, row, 20).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                //PRICE TICKET RECEIVED
                if (e.InfoPriceTickets.Restante <= 0 && e.InfoPriceTickets.Estado == "1")
                {
                    datos.Add(e.InfoPriceTickets.Fecha_recibo);
                    ws.Range(row, 21, row, 21).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 193, 116);
                    ws.Range(row, 21, row, 21).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else if (e.InfoPriceTickets.Restante >= 1 && e.InfoPriceTickets.Estado == "1")
                {
                    datos.Add(e.InfoPriceTickets.Fecha_recibo);
                    ws.Range(row, 21, row, 21).Style.Fill.BackgroundColor = XLColor.FromArgb(245, 213, 67);
                    ws.Range(row, 21, row, 21).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else
                {
                    datos.Add(e.InfoPriceTickets.Fecha_recibo);
                    ws.Range(row, 21, row, 21).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                //UCC RECEIVED
                if (e.InfoSummary.FechaUCC != "")
                {
                    datos.Add(e.InfoSummary.FechaUCC);
                    ws.Range(row, 22, row, 22).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 193, 116);
                    ws.Range(row, 22, row, 22).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else
                {
                    datos.Add(e.InfoSummary.FechaUCC);
                    ws.Range(row, 22, row, 22).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                datos.Add(e.CatComentarios.FechaComents);
                datos.Add(e.CatComentarios.Comentario);
                celdas.Add(datos.ToArray());
                ws.Cell(row, 1).Value = celdas;


                row++;
            }

        }

        public void SheetCancelled(List<OrdenesCompra> listaCancelled, IXLWorksheet wc, int row)
        {
            foreach (OrdenesCompra e in listaCancelled)
            {
                var celdas = new List<String[]>();
                List<String> datos = new List<string>();
                datos.Add(e.CatCliente.Nombre);

                datos.Add(e.CatClienteFinal.NombreCliente);
                datos.Add(e.FechaRecOrden);
                //PO NO
                if (e.RestaPrintshop <= 10)
                {
                    datos.Add(e.PO);
                    wc.Range(row, 4, row, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(95, 157, 205);
                    wc.Range(row, 4, row, 4).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else
                {
                    datos.Add(e.PO);
                    wc.Range(row, 4, row, 4).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }

                datos.Add(e.CatTipoBrand.TipoBrandName);
                datos.Add(e.VPO);
                datos.Add(e.CatTipoOrden.TipoOrden);
                datos.Add((e.InfoSummary.CantidadEstilo).ToString());
                datos.Add(e.FechaOrdenFinal);
                datos.Add(e.FechaCancelada);
                //DESIGN NAME
                if (e.DestinoSalida == 2)
                {
                    datos.Add(e.InfoSummary.ItemDesc.Descripcion);
                    wc.Range(row, 11, row, 11).Style.Fill.BackgroundColor = XLColor.FromArgb(190, 174, 241);
                    wc.Range(row, 11, row, 11).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else
                {
                    datos.Add(e.InfoSummary.ItemDesc.Descripcion);
                    wc.Range(row, 11, row, 11).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                datos.Add(e.InfoSummary.ItemDesc.ItemEstilo);
                datos.Add(e.MillPO);
                datos.Add(e.InfoSummary.CatColores.DescripcionColor);
                datos.Add(e.InfoSummary.CatGenero.Genero);
                // BLANKS RECEIVED 
                if (e.TotalRestante == 0)
                {
                    datos.Add((e.TotalRestante).ToString());
                    wc.Range(row, 16, row, 16).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else if (e.TotalRestante != e.InfoSummary.TotalEstilo)
                {
                    datos.Add((e.TotalRestante).ToString());
                    wc.Range(row, 16, row, 16).Style.Font.FontColor = XLColor.FromArgb(246, 57, 57);
                }
                //PARTIAL/COMPLETE BLANKS
                if (e.TipoPartial == "PARTIAL")
                {

                    datos.Add(e.TipoPartial);
                    wc.Range(row, 17, row, 17).Style.Fill.BackgroundColor = XLColor.FromArgb(249, 136, 29);
                    wc.Range(row, 17, row, 17).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);

                }
                else if (e.TipoPartial == "COMPLETE")
                {
                    datos.Add(e.TipoPartial);
                    wc.Range(row, 17, row, 17).Style.Fill.BackgroundColor = XLColor.FromArgb(64, 191, 128);
                    wc.Range(row, 17, row, 17).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else if (e.TipoPartial == null)
                {
                    e.TipoPartial = "";
                    datos.Add(e.TipoPartial);
                    wc.Range(row, 17, row, 17).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }

                //ART RECEIVED
                if (e.ImagenArte.StatusArteInf == "IN HOUSE")
                {
                    datos.Add((e.ImagenArte.StatusArteInf).ToString());
                    wc.Range(row, 18, row, 18).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 193, 116);
                    wc.Range(row, 18, row, 18).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);

                }
                else if (e.ImagenArte.StatusArteInf == "REVIEWED")
                {
                    datos.Add((e.ImagenArte.StatusArteInf).ToString());
                    wc.Range(row, 18, row, 18).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 193, 116);
                    wc.Range(row, 18, row, 18).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else if (e.ImagenArte.StatusArteInf == "PENDING")
                {
                    datos.Add((e.ImagenArte.StatusArteInf).ToString());
                    wc.Range(row, 18, row, 18).Style.Fill.BackgroundColor = XLColor.FromArgb(236, 95, 95);
                    wc.Range(row, 18, row, 18).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else if (e.ImagenArte.StatusArteInf == "APPROVED")
                {
                    string infoArte = e.ImagenArte.StatusArteInf + "-" + e.ImagenArte.FechaArte;
                    datos.Add(infoArte);
                    wc.Range(row, 18, row, 18).Style.Fill.BackgroundColor = XLColor.FromArgb(246, 129, 51);
                    wc.Range(row, 18, row, 18).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }

                //TRIM RECEIVED
                if (e.Trims.restante <= 0 && e.Trims.estado == "1")
                {
                    datos.Add(e.Trims.fecha_recibo);
                    wc.Range(row, 19, row, 19).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 193, 116);
                    wc.Range(row, 19, row, 19).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else if (e.Trims.restante >= 1 && e.Trims.estado == "1")
                {
                    datos.Add(e.Trims.fecha_recibo);
                    wc.Range(row, 19, row, 19).Style.Fill.BackgroundColor = XLColor.FromArgb(245, 213, 67);
                    wc.Range(row, 19, row, 19).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else
                {
                    datos.Add(e.Trims.fecha_recibo);
                    wc.Range(row, 19, row, 19).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                //PACK INSTRUCTION
                if (e.InfoPackInstruction.Fecha_Pack != "" && e.InfoPackInstruction.EstadoPack == 1)
                {
                    datos.Add(e.InfoPackInstruction.Fecha_Pack);
                    wc.Range(row, 20, row, 20).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 193, 116);
                    wc.Range(row, 20, row, 20).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else
                {
                    datos.Add(e.InfoPackInstruction.Fecha_Pack);
                    wc.Range(row, 20, row, 20).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }

                //PRICE TICKET RECEIVED
                if (e.InfoPriceTickets.Restante <= 0 && e.InfoPriceTickets.Estado == "1")
                {
                    datos.Add(e.InfoPriceTickets.Fecha_recibo);
                    wc.Range(row, 21, row, 21).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 193, 116);
                    wc.Range(row, 21, row, 21).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else if (e.InfoPriceTickets.Restante >= 1 && e.InfoPriceTickets.Estado == "1")
                {
                    datos.Add(e.InfoPriceTickets.Fecha_recibo);
                    wc.Range(row, 21, row, 21).Style.Fill.BackgroundColor = XLColor.FromArgb(245, 213, 67);
                    wc.Range(row, 21, row, 21).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else
                {
                    datos.Add(e.InfoPriceTickets.Fecha_recibo);
                    wc.Range(row, 21, row, 21).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                //UCC RECEIVED
                if (e.InfoSummary.FechaUCC != "")
                {
                    datos.Add(e.InfoSummary.FechaUCC);
                    wc.Range(row, 22, row, 22).Style.Fill.BackgroundColor = XLColor.FromArgb(68, 193, 116);
                    wc.Range(row, 22, row, 22).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                else
                {
                    datos.Add(e.InfoSummary.FechaUCC);
                    wc.Range(row, 22, row, 22).Style.Font.FontColor = XLColor.FromArgb(0, 0, 0);
                }
                datos.Add(e.CatComentarios.FechaComents);
                datos.Add(e.CatComentarios.Comentario);
                celdas.Add(datos.ToArray());
                wc.Cell(row, 1).Value = celdas;


                row++;
            }

        }







    }
}