using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Item;
using FortuneSystem.Models.Packing;
using FortuneSystem.Models.Pedidos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers
{
    public class PackingController : Controller
    {
        PedidosData objPedido = new PedidosData();
        CatClienteData objCliente = new CatClienteData();
        CatClienteFinalData objClienteFinal = new CatClienteFinalData();
        CatTallaItemData objTalla = new CatTallaItemData();
        PackingData objPacking = new PackingData();
        ItemTallaData objTallas = new ItemTallaData();
        // GET: Packing
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
            pedido.NombreClienteFinal = pedido.CatClienteFinal.NombreCliente.TrimEnd();
            pedido.IdPedido = Convert.ToInt32(id);

            if (pedido == null)
            {
                return View();
            }
            return View(pedido);
        }

        [HttpPost]
        public JsonResult Obtener_Lista_Tallas_Packing(List<string> ListTalla, int EstiloID)
        {
            PackingM tallaItem = new PackingM();
            PackingSize packingSize = new PackingSize();
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            tallaItem.Usuario = noEmpleado;
            packingSize.IdSummary = EstiloID;
            // int numBatch = objPacking.ObtenerIdBatch(EstiloID);
            // tallaItem.IdBatch = numBatch + 1;
            List<string> tallas = ListTalla[0].Split('*').ToList();
            List<string> calidad = ListTalla[1].Split('*').ToList();
            int i = 0;
            foreach (var item in tallas)
            {
                i++;
            }

            i = i - 1;
            string talla;
            for (int v = 0; v < i; v++)
            {
                talla= tallas[v];
                packingSize.IdTalla = objTalla.ObtenerIdTalla(talla);
                string calidadT = calidad[v];
                if (calidadT == "")
                {
                    calidadT = "0";
                }
                packingSize.Calidad = Int32.Parse(calidadT);
                tallaItem.PackingSize = packingSize;

                objPacking.AgregarTallasP(tallaItem);


            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Obtener_Lista_Tallas_Packing_Bulk(List<string> ListTalla, int EstiloID, string TipoPackID)
        {
            PackingM tallaItem = new PackingM();
            PackingTypeSize packingTSize = new PackingTypeSize();
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            tallaItem.Usuario = noEmpleado;
            packingTSize.IdSummary = EstiloID;
            packingTSize.IdTipoEmpaque = Int32.Parse(TipoPackID);
            

            // int numBatch = objPacking.ObtenerIdBatch(EstiloID);
            // tallaItem.IdBatch = numBatch + 1;
            List<string> tallas = ListTalla[0].Split('*').ToList();
            List<string> piezas = ListTalla[1].Split('*').ToList();
            int i = 0;
            foreach (var item in tallas)
            {
                i++;
            }

            i = i - 1;
            string talla;
            for (int v = 0; v < i; v++)
            {
                talla = tallas[v];
                packingTSize.IdTalla = objTalla.ObtenerIdTalla(talla);
                string piezasT = piezas[v];
                if (piezasT == "")
                {
                    piezasT = "0";
                }
                packingTSize.Pieces = Int32.Parse(piezasT);
                tallaItem.PackingTypeSize = packingTSize;
                

                objPacking.AgregarTallasTypePack(tallaItem);


            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Obtener_Lista_Tallas_Packing_PPK(List<string> ListTalla, int EstiloID, string TipoPackID)
        {
            PackingM tallaItem = new PackingM();
            PackingTypeSize packingTSize = new PackingTypeSize();
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            tallaItem.Usuario = noEmpleado;
            packingTSize.IdSummary = EstiloID;
            packingTSize.IdTipoEmpaque = Int32.Parse(TipoPackID);
            // int numBatch = objPacking.ObtenerIdBatch(EstiloID);
            // tallaItem.IdBatch = numBatch + 1;
            List<string> tallas = ListTalla[0].Split('*').ToList();
            List<string> ratio = ListTalla[1].Split('*').ToList();
            int i = 0;
            foreach (var item in tallas)
            {
                i++;
            }

            i = i - 1;
            string talla;
            for (int v = 0; v < i; v++)
            {
                talla = tallas[v];
                packingTSize.IdTalla = objTalla.ObtenerIdTalla(talla);
                string ratioT = ratio[v];
                if (ratioT == "")
                {
                    ratioT = "0";
                }
                packingTSize.Ratio = Int32.Parse(ratioT);
                tallaItem.PackingTypeSize = packingTSize;


                objPacking.AgregarTallasTypePack(tallaItem);


            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Obtener_Lista_Tallas_Packing_Pallet(List<string> ListTalla, int EstiloID, int TipoTurnoID, int NumCaja, string TipoEmpaque)
        {
            PackingM tallaItem = new PackingM();
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            tallaItem.Usuario = noEmpleado;
            tallaItem.IdSummary = EstiloID;
            tallaItem.IdTurno = TipoTurnoID;//Int32.Parse(TipoTurnoID);           
            int numBatch = objPacking.ObtenerIdBatch(EstiloID);
            tallaItem.IdBatch = numBatch + 1;
            List<string> idPack = ListTalla[0].Split('*').ToList();
            List<string> tallas = ListTalla[1].Split('*').ToList();
            List<string> cajas = new List<string>();
            List<string> piezas = new List<string>(); 
            List<string> totales = new List<string>();
            int i = 0;
            foreach (var item in tallas)
            {
                i++;
            }
            i = i - 1;
            string talla;
            string idPacking;
            if (TipoEmpaque == "BULK")
            {
                
                cajas = ListTalla[2].Split('*').ToList();
                piezas = ListTalla[3].Split('*').ToList();
                totales = ListTalla[4].Split('*').ToList();
                for (int v = 0; v < i; v++)
                {
                    talla = tallas[v];
                    tallaItem.IdTalla = objTalla.ObtenerIdTalla(talla);
                    idPacking = idPack[v];
                    tallaItem.IdPackingTypeSize= Int32.Parse(idPacking);
                    string cantBox = cajas[v];
                    if (cantBox == "")
                    {
                        cantBox = "0";
                    }
                    tallaItem.CantBox = Int32.Parse(cantBox);
                    string totalP = totales[v];
                    if (totalP == "")
                    {
                        totalP = "0";
                    }
                    tallaItem.TotalPiezas = Int32.Parse(totalP);


                    objPacking.AgregarTallasPacking(tallaItem);


                }
            } else if(TipoEmpaque == "PPK")
            {
                int nBox = NumCaja;
                piezas = ListTalla[2].Split('*').ToList();
                totales = ListTalla[3].Split('*').ToList();
                for (int v = 0; v < i; v++)
                {
                    talla = tallas[v];
                    tallaItem.IdTalla = objTalla.ObtenerIdTalla(talla);
                    tallaItem.CantBox = NumCaja;
                    idPacking = idPack[v];
                    tallaItem.IdPackingTypeSize = Int32.Parse(idPacking);
                    string totalP = totales[v];
                    if (totalP == "")
                    {
                        totalP = "0";
                    }
                    tallaItem.TotalPiezas = Int32.Parse(totalP);              


                objPacking.AgregarTallasPacking(tallaItem);


                }
            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Obtener_Lista_Tallas_Packing_Bulk_HT(List<string> ListTalla, int EstiloID, string FormaPackID, int NumberPOID)
        {
            PackingM tallaItem = new PackingM();
            PackingTypeSize packingTSize = new PackingTypeSize();
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            tallaItem.Usuario = noEmpleado;
            packingTSize.IdSummary = EstiloID;
            packingTSize.IdTipoEmpaque = 1;
            packingTSize.IdFormaEmpaque= Int32.Parse(FormaPackID);
            packingTSize.NumberPO = NumberPOID;

            // int numBatch = objPacking.ObtenerIdBatch(EstiloID);
            // tallaItem.IdBatch = numBatch + 1;
            List<string> tallas = ListTalla[0].Split('*').ToList();
            List<string> cantidad = ListTalla[1].Split('*').ToList();
            int i = 0;
            foreach (var item in tallas)
            {
                i++;
            }

            i = i - 1;
            string talla;
            for (int v = 0; v < i; v++)
            {
                talla = tallas[v];
                packingTSize.IdTalla = objTalla.ObtenerIdTalla(talla);
                string cantidadT = cantidad[v];
                if (cantidadT == "")
                {
                    cantidadT = "0";
                }
                packingTSize.Cantidad = Int32.Parse(cantidadT);
                tallaItem.PackingTypeSize = packingTSize;


                //objPacking.AgregarTallasTypePack(tallaItem);


            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Obtener_Lista_Tallas_Packing_PPK_HT(List<string> ListTalla, int EstiloID, int NumberPOID)
        {
            PackingM tallaItem = new PackingM();
            PackingTypeSize packingTSize = new PackingTypeSize();
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            tallaItem.Usuario = noEmpleado;
            packingTSize.IdSummary = EstiloID;
            packingTSize.IdTipoEmpaque = 2;
            packingTSize.NumberPO = NumberPOID;
            // int numBatch = objPacking.ObtenerIdBatch(EstiloID);
            // tallaItem.IdBatch = numBatch + 1;
            List<string> tallas = ListTalla[0].Split('*').ToList();
            List<string> ratio = ListTalla[1].Split('*').ToList();
            int i = 0;
            foreach (var item in tallas)
            {
                i++;
            }

            i = i - 1;
            string talla;
            for (int v = 0; v < i; v++)
            {
                talla = tallas[v];
                packingTSize.IdTalla = objTalla.ObtenerIdTalla(talla);
                string ratioT = ratio[v];
                if (ratioT == "")
                {
                    ratioT = "0";
                }
                packingTSize.Ratio = Int32.Parse(ratioT);
                tallaItem.PackingTypeSize = packingTSize;


                //objPacking.AgregarTallasTypePack(tallaItem);


            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Actualizar_Lista_Tallas_Batch(List<string> ListTalla, int TipoTurnoID, int EstiloID, int IdBatch, int NumCaja, string TipoEmpaque)
        {
            PackingM tallaItem = new PackingM();
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            tallaItem.UsuarioModif = noEmpleado;
            tallaItem.Usuario = noEmpleado;
            tallaItem.IdSummary = EstiloID;
            tallaItem.IdTurno = TipoTurnoID;//Int32.Parse(TipoTurnoID);           
            tallaItem.IdBatch = IdBatch;
            List<string> idPack = ListTalla[0].Split('*').ToList();
            List<string> tallas = ListTalla[1].Split('*').ToList();
            List<string> cajas = new List<string>();
            List<string> piezas = new List<string>();
            List<string> totales = new List<string>();
            int i = 0;
            foreach (var item in tallas)
            {
                i++;
            }
            i = i - 1;
            string talla;
            string idPacking;
            if (TipoEmpaque == "BULK")
            {
                cajas = ListTalla[2].Split('*').ToList();
                piezas = ListTalla[3].Split('*').ToList();
                totales = ListTalla[4].Split('*').ToList();
                for (int v = 0; v < i; v++)
                {
                    talla = tallas[v];
                    tallaItem.IdTalla = objTalla.ObtenerIdTalla(talla);
                    idPacking = idPack[v];
                    tallaItem.IdPacking = Int32.Parse(idPacking);
                    tallaItem.IdPackingTypeSize = objPacking.ObtenerIdPackingSize(tallaItem.IdPacking);
                    tallaItem.Usuario = objPacking.ObtenerIdUsuarioPorBatchEstilo(tallaItem.IdBatch, tallaItem.IdSummary, tallaItem.IdTalla);
                    string cantBox = cajas[v];
                    if (cantBox == "")
                    {
                        cantBox = "0";
                    }
                    tallaItem.CantBox = Int32.Parse(cantBox);
                    string totalP = totales[v];
                    if (totalP == "")
                    {
                        totalP = "0";
                    }
                    tallaItem.TotalPiezas = Int32.Parse(totalP);


                    objPacking.ActualizarTallasPacking(tallaItem);


                }
            }
            else if (TipoEmpaque == "PPK")
            {
                int nBox = NumCaja;
                piezas = ListTalla[2].Split('*').ToList();
                totales = ListTalla[3].Split('*').ToList();
                for (int v = 0; v < i; v++)
                {
                    talla = tallas[v];
                    tallaItem.IdTalla = objTalla.ObtenerIdTalla(talla);
                    tallaItem.CantBox = NumCaja;
                    idPacking = idPack[v];
                    tallaItem.IdPacking = Int32.Parse(idPacking);
                    tallaItem.IdPackingTypeSize = objPacking.ObtenerIdPackingSize(tallaItem.IdPacking);
                    tallaItem.Usuario = objPacking.ObtenerIdUsuarioPorBatchEstilo(tallaItem.IdBatch, tallaItem.IdSummary, tallaItem.IdTalla);
                    string totalP = totales[v];
                    if (totalP == "")
                    {
                        totalP = "0";
                    }
                    tallaItem.TotalPiezas = Int32.Parse(totalP);


                    objPacking.ActualizarTallasPacking(tallaItem);


                }
            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Lista_Batch_Estilo(int? id)
        {
            List<PackingM> listaBatch = objPacking.ListaBatch(id).ToList();

            var result = Json(new { listaTalla = listaBatch });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Packing_IdEstilo_IdBatch(int? idEstilo, int idBatch)
        {
            List<PackingM> listaCantidadesTallasBatch = objPacking.ListaCantidadesTallaPorIdBatchEstilo(idEstilo, idBatch).ToList();
            List<int> listaTallasTBatch = objPacking.ListaTotalTallasPackingBatchEstilo(idEstilo).ToList();
            List<PackingTypeSize> listaTallasEmpaque = objPacking.ObtenerCajasPackingPorEstilo(idEstilo);
            var result = Json(new { listaTalla = listaCantidadesTallasBatch, listaPrint = listaTallasTBatch, listaEmpaqueTallas = listaTallasEmpaque, });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        

        [HttpPost]
        public JsonResult Lista_Tallas_Packing_PPK_IdEstilo_IdBatch(int? idEstilo, int idBatch)
        {
            List<PackingM> listaCantidadesTallasBatch = objPacking.ListaCantidadesTallaPorIdBatchEstilo(idEstilo, idBatch).ToList();
            List<int> listaTallasTBatch = objPacking.ListaTotalTallasPackingBatchEstilo(idEstilo).ToList();
            List<PackingTypeSize> listaTallasEmpaque = objPacking.ObtenerCajasPackingPPKPorEstilo(idEstilo, idBatch);
            var result = Json(new { listaTalla = listaCantidadesTallasBatch, listaPrint = listaTallasTBatch, listaEmpaqueTallas = listaTallasEmpaque, });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Lista_Tallas_Batch(int? id)
        {
            List<PackingM> listaTallas = objPacking.ListaTallasBatchId(id).ToList();

            var result = Json(new { listaTalla = listaTallas });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Por_Estilo(int? id)
        {
            List<PackingM> listaTallasEstilo = objPacking.ObtenerTallas(id).ToList();
            List<ItemTalla> listaTallas = objTallas.ListaTallasPorEstilo(id).ToList();
            List<int> listaCajasPacking = objPacking.ListaTotalTallasPackingBatchEstilo(id).ToList();
            List<PackingSize> listaTallasPacking = objPacking.ObtenerListaPackingSizePorEstilo(id).ToList();
            List<PackingTypeSize> listaTallasEmpaque = objPacking.ObtenerListaPackingTypeSizePorEstilo(id).ToList();
            List<int> listaTallasCBatch = new List<int>();
            if (listaCajasPacking.Count != 0)
            {
                listaTallasCBatch = objPacking.ListaTotalCajasTallasBatchEstilo(id).ToList();
            }
            string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }

            var result = Json(new { lista=listaTallas, listaTalla = listaTallasEstilo, listaPackingS = listaTallasPacking, listaEmpaqueTallas = listaTallasEmpaque, listaTotalCajasPack = listaCajasPacking, listaCajasT = listaTallasCBatch, estilos = estilo });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Empaque_Por_Estilo(int? id)
        {
            List<PackingSize> listaTallasEstilo = objPacking.ListaTallasCalidadPack(id).ToList();
            List<PackingTypeSize> listaTallasPacking = objPacking.ObtenerListaPackingTypeSizePorEstilo(id).ToList();
            var result = Json(new { listaTalla = listaTallasEstilo, listaPackingS = listaTallasPacking });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Empaque_HT_Por_Estilo(int? id)
        {
            List<ItemTalla> listaTallasPO = objTallas.ListaTallasPorEstilo(id).ToList();
            List<PackingSize> listaTallasEstilo = objPacking.ListaTallasCalidadPack(id).ToList();
            List<PackingTypeSize> listaTallasPacking = objPacking.ObtenerListaPackingTypeSizePorEstilo(id).ToList();
            var result = Json(new { lista = listaTallasPO, listaTalla = listaTallasEstilo, listaPackingS = listaTallasPacking });
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Lista_Total_Tallas_Batch(int id)
        {
            List<int> listaTallas = objPacking.ListaTotalTallasPackingBatchEstilo(id).ToList();

            var result = Json(new { listaTallaBatch = listaTallas });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}