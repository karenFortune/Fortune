using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using FortuneSystem.Models.TallasEstilos;

namespace FortuneSystem.Controllers
{
    public class TallasEstilosController : Controller
    {
        DatosTallasEstilos dte = new DatosTallasEstilos();
        // GET: TallasEstilos
        public ActionResult Index()
        {
            Session["id_po_summary"] = 1;
            dte.id_po_summary=Convert.ToInt32(Session["id_po_summary"]); 
            ViewData["estilo"] = dte.buscar_estilo_po_summary(dte.id_po_summary);
            List<TallaEstilo> listaTallasEstilos = new List<TallaEstilo>();
            listaTallasEstilos = dte.ListaTallasEstilos().ToList();
            return View(listaTallasEstilos);
        }

        public PartialViewResult Details(int id)
        {
            Session["id_po_summary"] = id;
            return PartialView("Tallas/TallasOrden");
        }


    }
}