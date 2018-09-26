using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using FortuneSystem.Models.Sizes;

namespace FortuneSystem.Controllers
{
    public class SizesController : Controller
    {
        DatosSizes dt = new DatosSizes();
        // GET: Tallas
        public ActionResult Index()
        {
            dt.id_recibo = Convert.ToInt32(Session["id_recibo"]); //O ALGO ASÍ
            List<Size> listaTallas = new List<Size>();
            listaTallas= dt.ListaTallas(dt.id_recibo).ToList();
            return View();
        }

        public ActionResult PorOrden()
        {
            dt.id_po_summary = Convert.ToInt32(Session["id_po_summary"]); //O ALGO ASÍ
            List<Size> listaTallasPorOrden = new List<Size>();
            listaTallasPorOrden = dt.ListaTallasPorOrden(dt.id_po_summary).ToList();
            return View(listaTallasPorOrden);
        }
    }
}