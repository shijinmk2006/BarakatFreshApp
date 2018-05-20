using BarakatFresh.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
namespace BarakatFresh.Controllers
{
    public class TransactionController : Controller
    {
       
        [AllowAnonymous]
        [HttpPost]
        public ActionResult ResetPassword(ResetPassword model)
        {
            if (ModelState.IsValid)
            {
                WebMatrix.WebData.WebSecurity.ResetPassword(model.UserName, model.Password);
                return RedirectToAction("Login");
            }
            return View();
        }

       
        // GET: Transaction/Details/5
        public ActionResult Reset(string _auth)
        {
            //string auth = "MXJpVkFZZjZhWDFNSUFJQXVFekNxUTI=";
            PassProtection.Protection _obj = new PassProtection.Protection();
            string _decoded = _obj.Decoding(_auth);
            ResetPassword model = new ResetPassword();
            model.UserName = _decoded;

            return View(model);

        }
        [HttpPost]
        public ActionResult Transaction(int id)
        {
            string status = string.Empty;
            if (id > 0)
                status = "Order Successful. You will receive an email shortly";
            else
                status = "Something went wrong!!";
            return View((object)status);
        }
        public ActionResult Login()
        {
           
            return View();

        }

    }
}
