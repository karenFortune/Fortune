using FortuneSystem.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers.Catalogos
{
    public class TipoCamisetaController : Controller
    {
        // GET: TipoCamiseta
        CatTipoCamisetaData objCamiseta = new CatTipoCamisetaData();
        public ActionResult Index()
        {
            List<CatTipoCamiseta> listaCamiseta = new List<CatTipoCamiseta>();
            listaCamiseta = objCamiseta.ListaTipoCamiseta().ToList();
            return View(listaCamiseta);
        }

        [HttpGet]
        public ActionResult CrearCamiseta()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearCamiseta([Bind] CatTipoCamiseta camisetas)
        {
            if (ModelState.IsValid)
            {
                objCamiseta.AgregarCamiseta(camisetas);
                TempData["camisetaOK"] = "Se registro correctamente el tipo de camiseta.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["camisetaError"] = "No se pudo registrar el tipo de camiseta, intentelo más tarde.";
            }
            return View(camisetas);
        }

        [HttpGet]
        public ActionResult Detalles(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatTipoCamiseta camisetas = objCamiseta.ConsultarListaCamisetas(id);
            if (camisetas == null)
            {
                return View();
            }
            return View(camisetas);
        }

        [HttpGet]
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatTipoCamiseta camisetas = objCamiseta.ConsultarListaCamisetas(id);
            if (camisetas == null)
            {
                return View();
            }

            return View(camisetas);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, [Bind] CatTipoCamiseta camisetas)
        {
            if (id != camisetas.IdTipo)
            {
                return View();
            }
            if (ModelState.IsValid)
            {
                objCamiseta.ActualizarCamisetas(camisetas);
                TempData["camisetaEditar"] = "Se modifico correctamente el tipo de camiseta.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["camisetaEditarError"] = "No se pudo modificar el tipo de camiseta, intentelo más tarde.";
            }
            return View(camisetas);
        }

        [HttpGet]
        public ActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatTipoCamiseta camisetas = objCamiseta.ConsultarListaCamisetas(id);


            if (camisetas == null)
            {
                return View();
            }
            return View(camisetas);

        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfimacionEliminar(int? id)
        {
            objCamiseta.EliminarCamisetas(id);
            TempData["camisetaEliminar"] = "Se elimino correctamente el tipo de camiseta.";
            return RedirectToAction("Index");
        }
    }
}