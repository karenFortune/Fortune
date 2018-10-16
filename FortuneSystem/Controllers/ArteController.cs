using FortuneSystem.Models;
using FortuneSystem.Models.Arte;
using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Item;
using FortuneSystem.Models.Items;
using FortuneSystem.Models.PrintShop;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers
{
    public class ArteController : Controller
    {
        ArteData objArte = new ArteData();
        CatTallaItemData objItem = new CatTallaItemData();
        ItemDescripcionData objDesc = new ItemDescripcionData();
        private MyDbContext db = new MyDbContext();
        // GET: Arte

        public ActionResult Index()
        {
            List<IMAGEN_ARTE> listaArtes = new List<IMAGEN_ARTE>();
            listaArtes = objArte.ListaInvArtes().ToList();          
            return View(listaArtes);

        }

        // GET: Arte
       
        public ActionResult ListaImgArte(int id)
        {
            List<IMAGEN_ARTE> listaArtes = new List<IMAGEN_ARTE>();
            listaArtes = objArte.ListaArtes(id).ToList();
            return PartialView(listaArtes);
        }

        public ActionResult ConvertirImagen(int arteCodigo)
        {
            var arte = db.ImagenArte.Where(x => x.IdImgArte == arteCodigo).FirstOrDefault();
            if (arte != null)
            {
                if (arte.imgArte != null)
                {
                    return File(arte.imgArte, "image/jpg");
                }
                else
                {
                    return File("~/Content/img/noImagen.png", "image/png");
                }
              
          
                
            }
            else
            {
                return File("~/Content/img/noImagen.png", "image/png");
            }
        }

        public ActionResult ConvertirImagenPNL(int pnlCodigo)
        {
            var arte = db.ImagenArte.Where(x => x.IdImgArte == pnlCodigo).FirstOrDefault();
            if (arte != null)
            {
                                 

                if (arte.imgPNL != null)
                {
                    return File(arte.imgPNL, "image/jpg");
                }
                else
                {
                    return File("~/Content/img/noImagen.png", "image/png");
                }

            }
            else
            {
                return File("~/Content/img/noImagen.png", "image/png");
            }
        }

        public ActionResult ObtenerIdEstilo(int id)
        {
            Session["idSummary"] = id;
            return View();
        }

            public ActionResult ConvertirImagenArteEstilo(string nombreEstilo)
        {
            int idEstilo = objDesc.ObtenerIdEstilo(nombreEstilo);
            var arte = db.ImagenArte.Where(x => x.IdEstilo == idEstilo).FirstOrDefault();
            if (arte != null)
            {
                if (arte.imgArte != null)
                {
                    return File(arte.imgArte, "image/jpg");
                }
                else
                {
                    return File("~/Content/img/noImagen.png", "image/png");
                }
                
            }
            else
            {
                return File("~/Content/img/noImagen.png", "image/png");
            }
            
        }

        public ActionResult ConvertirImagenPNLEstilo(string nombreEstilo)
        {
            int idEstilo= objDesc.ObtenerIdEstilo(nombreEstilo);
            var arte = db.ImagenArte.Where(x => x.IdEstilo == idEstilo).FirstOrDefault();
            if(arte != null)
            {
                if(arte.imgPNL != null)
                {
                    return File(arte.imgPNL, "image/jpg");
                }
                else
                {
                    return File("~/Content/img/noImagen.png", "image/png");
                }
                
            } else
            {
                return File("~/Content/img/noImagen.png", "image/png");
            }
           
        }



        public ActionResult Create(int? id, int idArte)
        {
            IMAGEN_ARTE IArte = db.ImagenArte.Find(idArte);

           ARTE art = db.Arte.Where(x => x.IdImgArte == idArte).FirstOrDefault();
            Session["id"]= id;   
            int Summary = Convert.ToInt32(Session["id"]);
            art.IdEstilo = Summary;
            IArte.CATARTE = art;
            IArte.Tienda = objArte.ObtenerclienteEstilo(id, idArte);
            //arte.Tienda = "BRAVADO KOHL's";
            Regex kohl = new Regex("KOHL");
            Regex walmart = new Regex("WAL-");
            IArte.ResultadoK = kohl.Matches(IArte.Tienda);
            IArte.ResultadoW = walmart.Matches(IArte.Tienda);
            ObtenerEstados(IArte.StatusArte, IArte.StatusPNL, IArte);     


                return View(IArte);
        }

        public void ObtenerEstados(int? idEstadoArte, int? idEstadoPNL, IMAGEN_ARTE arte)
        {
            //Obtener el idEstado Arte 
            if (idEstadoArte == 1)
            {
                arte.EstadosArte = EstatusArte.APPROVED;
            }
            else if (idEstadoArte == 2)
            {
                arte.EstadosArte = EstatusArte.REVIEWED;
            }
            else if (idEstadoArte == 3)
            {
                arte.EstadosArte = EstatusArte.PENDING;
            }
            //Obtener el idEstado PNL
            if (idEstadoPNL == 1)
            {
                arte.EstadosPNL = EstatusPNL.APPROVED;
            }
            else if (idEstadoPNL == 2)
            {
                arte.EstadosPNL = EstatusPNL.REVIEWED;
            }
            else if (idEstadoPNL == 3)
            {
                arte.EstadosPNL = EstatusPNL.PENDING;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind] IMAGEN_ARTE Arte, HttpPostedFileBase imgArte, HttpPostedFileBase imgPNL)
        {
            byte[] iArte = objArte.ObtenerImagenArte(Arte.IdImgArte);
            if (iArte == null)
            {
                imgArte = Arte.FileArte;
                if (imgArte != null && imgArte.ContentLength > 0)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(imgArte.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(imgArte.ContentLength);
                    }
                    //setear la imagen a la entidad que se creara
                    Arte.imgArte = imageData;
                }
            }
            else
            {
                Arte.imgArte = iArte;
            }
            imgPNL = Arte.FilePNL;
            if (imgPNL != null && imgPNL.ContentLength > 0)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(imgPNL.InputStream))
                {
                    imageData = binaryReader.ReadBytes(imgPNL.ContentLength);
                }
                //setear la imagen a la entidad que se creara
                Arte.imgPNL = imageData;
            }

            if(Arte.EstadosArte == EstatusArte.APPROVED)
            {
                Arte.StatusArte = 1;
            }else if(Arte.EstadosArte == EstatusArte.REVIEWED)
            {
                Arte.StatusArte = 2;
            }else if (Arte.EstadosArte == EstatusArte.PENDING)
            {
                Arte.StatusArte = 3;
            }

            if (Arte.EstadosPNL == EstatusPNL.APPROVED)
            {
                Arte.StatusPNL = 1;
            }
            else if (Arte.EstadosPNL == EstatusPNL.REVIEWED)
            {
                Arte.StatusPNL = 2;
            }
            else if (Arte.EstadosPNL == EstatusPNL.PENDING)
            {
                Arte.StatusPNL = 3;
            }

            if (ModelState.IsValid)
            {
                
               
                db.Entry(Arte).State = EntityState.Modified;
                db.SaveChanges();
                //objArte.ActualizarArteEstilo(Arte.IdImgArte, Arte.imgArte, Arte.imgPNL, Arte.StatusArte);
                /*   db.ImagenArte.Add(Arte);
                   db.SaveChanges();*/


                return RedirectToAction("Index");
            }

         
            return View(Arte);
        }

    

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IMAGEN_ARTE arte = db.ImagenArte.Find(id);
            arte.Tienda = objArte.ObtenerclienteEstilo(id, arte.IdImgArte);
            if (arte == null)
            {
                return HttpNotFound();
            }
        
            return View(arte);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind] IMAGEN_ARTE Arte, HttpPostedFileBase imgArte, HttpPostedFileBase imgPNL)
        {
            imgArte = Arte.FileArte;
            if (imgArte != null && imgArte.ContentLength > 0)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(imgArte.InputStream))
                {
                    imageData = binaryReader.ReadBytes(imgArte.ContentLength);
                }
                //setear la imagen a la entidad que se creara
                Arte.imgArte = imageData;
            }
            imgPNL = Arte.FilePNL;
            if (imgPNL != null && imgPNL.ContentLength > 0)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(imgPNL.InputStream))
                {
                    imageData = binaryReader.ReadBytes(imgPNL.ContentLength);
                }
                //setear la imagen a la entidad que se creara
                Arte.imgPNL = imageData;
            }
            if (ModelState.IsValid)
            {
                //db.Entry(Arte).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
     
            return View(Arte);
        }
        

        public JsonResult Lista_Tallas_Estilo()
        {
            IMAGEN_ARTE arte = new IMAGEN_ARTE();
            int idEstilo = Convert.ToInt32(Session["id"]);
            List<CatTallaItem> listaT = objItem.Lista_tallas_Estilo_Arte(idEstilo).ToList();
            arte.ListaTallas = listaT;

            List<UPC> listaU = objItem.Lista_tallas_upc(idEstilo).ToList();
            var result = Json(new { listaTalla = listaT, listaUPC = listaU});
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        

    }
    
}