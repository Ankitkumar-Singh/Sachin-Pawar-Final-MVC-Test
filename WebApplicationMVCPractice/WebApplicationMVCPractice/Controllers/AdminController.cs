using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplicationMVCPractice.Common;
using WebApplicationMVCPractice.Models;

namespace WebApplicationMVCPractice.Controllers
{
    public class AdminController : Controller
    {
        #region "Database Object Is Created"
        /// <summary>
        /// Database Object Is Created
        /// </summary>
        private BankEntities1 db = new BankEntities1();
        private LoginEntities db2 = new LoginEntities();
        #endregion

        #region "Index Action Method Lists All Customer's"
        /// <summary>
        /// Index Action Method Lists All Customers
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var banksCustomers = db.BanksCustomers;
            return View(banksCustomers.ToList().Where(u => u.UserRole.ToString().Contains("Customer")));
        }
        #endregion

        #region "Details Action Method Shows Personal Details of Customwers"
        /// <summary>
        /// Details Action Method Shows Personal Details of Customwers
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BanksCustomer banksCustomer = db.BanksCustomers.SingleOrDefault(m => m.AccountNumber == id);
            if (banksCustomer == null)
            {
                return HttpNotFound();
            }
            return View(banksCustomer);
        }
        #endregion

        #region "Add funds to Customers Account"
        /// <summary>
        /// Add funds to Customers Account [HttpGet]
        /// </summary>
        /// <returns></returns>
        public ActionResult AddFund()
        {
            return View();
        }

        /// <summary>
        /// Add funds to Customers Account [HttpPost]
        /// </summary>
        /// <param name="fund"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFund( Fund fund)
        {
            if (ModelState.IsValid)
            {
                //Customers Fund Increases into BanksCustomer Table
                BanksCustomer banksCustomer = db.BanksCustomers.Where(u => u.AccountNumber == fund.AccountNumber).FirstOrDefault();
                LoginDetails updateFund = db2.LoginDetails.Where(u => u.AccountNumber1 == fund.AccountNumber).FirstOrDefault();
                updateFund.Fund = banksCustomer.Fund + fund.Fund1;
                db2.Entry(updateFund).Property("Fund").IsModified = true;
                db2.SaveChanges();
                
                //Save Funds History into Fund Table            
                db.Funds.Add(fund);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View();
        }
        #endregion

        #region "Dispose Database Object"
        /// <summary>
        /// Dispose Database Object
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
