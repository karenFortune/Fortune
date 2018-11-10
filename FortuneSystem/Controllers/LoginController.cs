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

        CatUsuarioData objUsr = new CatUsuarioData();
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
            string empleado= usuario.NoEmpleado.ToString(); 
            string actionName="";
            string nameController="";
            if (ModelState.IsValid == false)
            {
                if(empleado != "0" && usuario.Contrasena != null)
                {
                    objData.IsValid(empleado, usuario.Contrasena, usuario);
                    usuario.Nombres = objUsr.Obtener_Nombre_Usuario(empleado);
                    Session["nombre"] = usuario.Nombres;
                    int noEmpleado = objUsr.Obtener_Datos_Usuarios(empleado);
                    Session["id_Empleado"] = noEmpleado;
                    Session["idCargo"] = usuario.Cargo;
                    if (noEmpleado != 0)
                    {
                        if (usuario.Cargo == 1)
                        {
                            actionName = "Index";
                            nameController = "Usuarios";

                        }
                        else if (usuario.Cargo == 4)
                        {
                            actionName = "Index";
                            nameController = "Recibos";

                        }
                        else if (usuario.Cargo == 5)
                        {
                            actionName = "Index";
                            nameController = "PrintShop";

                        }
                        else if (usuario.Cargo == 6)
                        {
                            actionName = "Index";
                            nameController = "Shipping";

                        }
                        else if (usuario.Cargo == 7)
                        {
                            actionName = "Index";
                            nameController = "Staging";

                        }
                        else if (usuario.Cargo == 8)
                        {
                            actionName = "Index";
                            nameController = "PNL";

                        }
                        else if (usuario.Cargo == 9)
                        {
                            actionName = "Index";
                            nameController = "Packing";

                        }
                        else if (usuario.Cargo == 12)
                        {
                            actionName = "Index";
                            nameController = "Arte";

                        }
                    }
                    else
                    {
                        actionName = "Login";
                        nameController = "Login";
                        TempData["loginError"] = "The employee number or password is incorrect.";
                    }
                   
                }
                else
                {
                    actionName = "Login";
                    nameController = "Login";
                    TempData["loginError"] = "Please enter your employee number and password.";
                }
                return RedirectToAction(actionName, nameController);


            }


            if (ModelState.IsValid)
            {
                if(objData.IsValid(empleado, usuario.Contrasena,usuario))
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