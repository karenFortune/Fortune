using FortuneSystem.Models;
using FortuneSystem.Models.Login;
using FortuneSystem.Models.Usuarios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FortuneSystem.Controllers
{
    public class LoginController : Controller
    {


        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(CatUsuario usuario)
        {
            LoginData objData = new LoginData();

            string actionName="";
            string nameController="";
            if (ModelState.IsValid == false)
            {
                objData.IsValid(usuario.Nombres, usuario.Contrasena, usuario);

                if (usuario.Cargo == 1)
                {
                    actionName = "Index";
                    nameController = "Usuarios";
                   
                }
                else if (usuario.Cargo == 5)
                {
                    actionName = "Index";
                    nameController = "Home";

                }

                return RedirectToAction(actionName, nameController);
            }


            if (ModelState.IsValid)
            {
                if(objData.IsValid(usuario.Nombres, usuario.Contrasena,usuario))
                {
                    //FormsAuthentication.SetAuthCookie(usuario.Nombres, usuario.Contrasena);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Login incorrecto!");
                }
            }
            return View(usuario);

        }

        public ActionResult IniciarSesion()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }




    }
}