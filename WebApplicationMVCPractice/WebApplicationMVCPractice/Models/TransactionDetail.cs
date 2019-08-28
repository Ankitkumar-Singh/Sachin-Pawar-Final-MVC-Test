//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplicationMVCPractice.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class TransactionDetail
    {
        public int AccountNumber { get; set; }

        [Display(Name = "Transfer Amount")]
        [Required(ErrorMessage = "Transfer amount cannot be empty")]
        public Nullable<int> TransferAmount { get; set; }

        public Nullable<System.DateTime> TransferDate { get; set; }
        public Nullable<int> TransferFrom { get; set; }

        [Display(Name = "Transfer To")]
        [Required(ErrorMessage = "Transfer to cannot be empty")]
        public Nullable<int> TransferTo { get; set; }

        public string CustomerName { get; set; }
        public int TransferID { get; set; }
    
        public virtual BanksCustomer BanksCustomer { get; set; }
        public virtual BanksCustomer BanksCustomer1 { get; set; }
    }
}
