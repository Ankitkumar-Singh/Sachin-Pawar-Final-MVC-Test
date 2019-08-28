using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using WebApplicationMVCPractice.Common;
using WebApplicationMVCPractice.Models;

namespace WebApplicationMVCPractice.Controllers
{
    public class BanksCustomerController : Controller
    {
        #region "Database Object Is Created"
        /// <summary>
        /// Database Object Is Created
        /// </summary>
        private LoginEntities db = new LoginEntities();
        public BankEntities1 db2 = new BankEntities1();
        #endregion

        #region "Index Action Method shows Main Panel to Customer"
        /// <summary>
        /// Index Action Method shows Main Panel to Customer
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region "View Details Action Method Shows All Personal details"
        /// <summary>
        /// View Details Action Method Shows All Personal details
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        public ActionResult ViewDetails(int? accountNumber)
        {
            if (Session["AccountNumber"] != null)
            {
                if (accountNumber == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                LoginDetails loginDetails = db.LoginDetails.SingleOrDefault(c => c.AccountNumber1 == accountNumber);

                if (loginDetails == null)
                {
                    return HttpNotFound();
                }
                return View(loginDetails);
            }
            else
            {
                Response.Write("<script>alert('Session logged out. Sign in again');</script>");
                FormsAuthentication.SignOut();
                Session.Clear();
                return RedirectToAction("Login", "Login");
            }
        }
        #endregion

        #region "Check Transaction Password for transfer money [HttpGet]"
        /// <summary>
        /// "Check Transaction Password for transfer money [HttpGet]"
        /// </summary>
        /// <returns></returns>        
        [HttpGet]
        public ActionResult CheckTransactionPassword()
        {
            return View();
        }
        #endregion

        #region "Check Transaction Password for transfer money [HttpPost]"
        /// <summary>
        /// "Check Transaction Password for transfer money [HttpPost]"
        /// </summary>
        /// <param name="loginDetails"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckTransactionPassword(LoginDetails loginDetails)
        {
            using (var db = new LoginEntities())
            {
                
                if (Session["TransactionPassword"].ToString().Equals(loginDetails.TransactionPassword))
                {
                    var user = db.LoginDetails.Where(a => a.TransactionPassword.Equals(Session["TransactionPassword"].ToString())).Distinct();
                    if (user != null)
                    {
                        return RedirectToAction("Transfer");
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

        #region "Transfer Money From Login User to Other Users"     
        /// <summary>
        /// Transfer Money From Login User to Other Users[HttpGet] Action Method
        /// </summary>
        /// <returns></returns>
        public ActionResult Transfer()
        {
            if (Session["AccountNumber"] != null)
            {
                ViewBag.TransferTo = new SelectList(db2.BanksCustomers.Where(u => u.UserRole.ToString().Contains("Customer")), "AccountNumber", "AccountNumber");
                return View();
            }
            Response.Write("<script>alert('Session logged out. Sign in again');</script>");
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login", "Login");
        }

        /// <summary>
        /// Transfer Money From Login User to Other Users[HttpPost] Action Method
        /// </summary>
        /// <param name="transactionDetail"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transfer([Bind(Include = "AccountNumber,TransferFrom,TransferTo,TransferAmount,TransferDate,CurrentBalance,Funds,CustomerName")] TransactionDetail transactionDetail)
        {
            if (Session["UserName"] != null && Session["UserRole"].ToString().Contains("Customer"))
            {
                if (ModelState.IsValid)
                {
                    transactionDetail.TransferDate = DateTime.Now;
                    transactionDetail.AccountNumber = Convert.ToInt32(Session["AccountNumber"]);
                    transactionDetail.TransferFrom = Convert.ToInt32(Session["AccountNumber"]);
                    transactionDetail.CustomerName = Convert.ToString(Session["CustomerName"]);
                    db2.TransactionDetails.Add(transactionDetail);
                    db2.SaveChanges();

                    //Receivers Balance Increases
                    BanksCustomer banksCustomer = new BanksCustomer();
                    banksCustomer = db2.BanksCustomers.Where(u => u.AccountNumber == transactionDetail.TransferTo).FirstOrDefault();
                    LoginDetails updateBalanceReveiver = db.LoginDetails.Where(u => u.AccountNumber1 == transactionDetail.TransferTo).FirstOrDefault();
                    updateBalanceReveiver.CurrentBalance += transactionDetail.TransferAmount;
                    db.Entry(updateBalanceReveiver).Property("CurrentBalance").IsModified = true;
                    db.SaveChanges();

                    //Senders Balance Decreases
                    banksCustomer = db2.BanksCustomers.Where(u => u.AccountNumber == transactionDetail.TransferFrom).FirstOrDefault();
                    LoginDetails updateBalanceSender = db.LoginDetails.Where(u => u.AccountNumber1 == transactionDetail.TransferFrom).FirstOrDefault();
                    updateBalanceSender.CurrentBalance -= transactionDetail.TransferAmount;
                    db.Entry(updateBalanceSender).Property("CurrentBalance").IsModified = true;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                ViewBag.ReceiversAccountNumber = new SelectList(db2.BanksCustomers.Where(u => u.UserRole.ToString().Contains("Customer")), "AccountNumber", "AccountNumber", transactionDetail.AccountNumber);
                return View(transactionDetail);
            }

            Response.Write("<script>alert('Session logged out. Sign in again');</script>");
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("SignIn", "Auth");
        }
        #endregion

        #region "Change Password Action Method Changes Password"
        /// <summary>
        /// Change Password Action Method [HttpGet] Changes Password 
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangePassword(int? accountNumber)
        {
            if (Session["AccountNumber"] != null)
            {
                LoginDetails loginDetails = db.LoginDetails.SingleOrDefault(x => x.AccountNumber1 == accountNumber);

                if (accountNumber == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (loginDetails == null)
                {
                    return HttpNotFound();
                }

                return View(loginDetails);
            }
            else
            {
                Response.Write("<script>alert('Session logged out. Sign in again');</script>");
                FormsAuthentication.SignOut();
                Session.Clear();
                return RedirectToAction("Login", "Login");
            }
        }

        /// <summary>
        /// Change Password Action Method [HttpPost] Changes Password
        /// </summary>
        /// <param name="loginDetails"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(LoginDetails loginDetails)
        {
            if (Session["UserName"] != null && Session["UserRole"].ToString().Contains("Customer"))
            {
                if (ModelState.IsValid)
                {
                    LoginDetails objLoginDetails = db.LoginDetails.Where(x => x.AccountNumber1 == loginDetails.AccountNumber1).FirstOrDefault();
                    objLoginDetails.Password1 = loginDetails.Password1;

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(loginDetails);
            }

            Response.Write("<script>alert('Session logged out. Sign in again');</script>");
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("SignIn", "Auth");
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
