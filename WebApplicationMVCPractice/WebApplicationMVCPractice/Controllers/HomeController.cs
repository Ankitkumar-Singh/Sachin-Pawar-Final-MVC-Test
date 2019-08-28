using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using WebApplicationMVCPractice.Models;

namespace WebApplicationMVCPractice.Controllers
{
    public class HomeController : Controller
    {
        #region "SignUp Action Method to SignUp For New Customers [HttpGet]"
        /// <summary>
        /// SignUp action method to sign up for the new customers [HttpGet]
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }

        /// <summary>
        /// SignUp action method to sign up for the new customers [HttpPost]
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(BanksCustomer banks_Customer)
        {
            if (ModelState.IsValid)
            {
                using (var db = new BankEntities1())
                {
                    banks_Customer.UserRole = "Customer";
                    banks_Customer.CurrentBalance = 0;
                    banks_Customer.Fund = 0;
                    db.BanksCustomers.Add(banks_Customer);

                    db.SaveChanges();
                    return RedirectToAction("Login", "Login");
                }
            }
            return View();
        }
        #endregion

        #region "Check UserName Availability Wheather It's Already in Use Or Not"
        /// <summary>
        /// UserName is checking wheather its already in use or not
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public JsonResult IsUserNameAvailable(string UserName)
        {
            using (var db = new BankEntities1())
            {
                return Json(!db.BanksCustomers.Any(x => x.UserName == UserName), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region "Check Account Number Availability Wheather It's Already in Use Or Not"
        /// <summary>
        /// Account Number is checking wheather its already in use or not
        /// </summary>
        /// <param name="AccountNumber"></param>
        /// <returns></returns>
        public JsonResult IsAccountNumberAvailable(int AccountNumber)
        {
            using (var db = new BankEntities1())
            {
                return Json(!db.BanksCustomers.Any(x => x.AccountNumber == AccountNumber), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region "Sign Out"
        /// <summary>Represents an event that is raised when the sign-out operation is complete.</summary>
        /// <returns></returns>
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            HttpContext.Session.Abandon();
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Login");
        }
        #endregion
    }
}