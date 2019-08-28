using System;
using System.Linq;
using System.Web.Mvc;
using WebApplicationMVCPractice.Common;
using LoginDetails = WebApplicationMVCPractice.Common.LoginDetails;

namespace WebApplication5.Controllers
{
    public class LoginController : Controller
    {
        #region "Login Action Method [HttpGet] Contains Error Log File"
        /// <summary>
        /// Login Action Method [HttpGet]
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            log4net.ILog logger = log4net.LogManager.GetLogger(typeof(LoginController));  //Declaring Log4Net  
            try
            {
                int x, y, z;
                x = 5; y = 0;
                z = x / y;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            return View();
        }
        #endregion

        #region "Login Action Method [HttpPost] Checks For Valid Username And Password"
        /// <summary>
        /// Login Action Method Checks for valid UserName and Password
        /// </summary>
        /// <param name="userProfile"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(LoginDetails loginDetails)
        {
            using (var db = new LoginEntities())
            {
                var UserName1 = loginDetails.UserName1;
                var Passsword1 = loginDetails.Password1;

                if (db.LoginDetails.Any(x => x.UserName1.Equals(loginDetails.UserName1, StringComparison.Ordinal) && x.Password1.Equals(loginDetails.Password1, StringComparison.Ordinal)))
                {
                    var user = db.LoginDetails.Where(a => a.UserName1.Equals(loginDetails.UserName1) && a.Password1.Equals(loginDetails.Password1)).FirstOrDefault();
                    if (user != null)
                    {
                        Session["UserName"] = user.UserName1;
                        Session["Password"] = user.Password1;
                        Session["TransactionPassword"] = user.TransactionPassword;
                        Session["UserRole"] = user.UserRole;
                        Session["CustomerName"] = user.CustomerName;
                        Session["AccountNumber"] = user.AccountNumber1;

                        return RedirectToAction("UserDashBoard");
                    }
                    else
                    {
                        return View();
                    }
                }
            }

            ModelState.AddModelError("", "Invalid email and password");
            return View();
        }
        #endregion

        #region "Dashboard Redirecting"
        /// <summary>
        /// User Dashboard
        /// </summary>
        /// <returns></returns>
        public ActionResult UserDashBoard()
        {
            if (Session["UserRole"].ToString().Contains("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            if (Session["UserRole"].ToString().Contains("Customer"))
            {
                return RedirectToAction("Index", "BanksCustomer");
            }
            else
            {
                return View();
            }
        }
        #endregion
    }
}