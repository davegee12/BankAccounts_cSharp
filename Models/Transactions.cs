using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BankAccounts.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId{get;set;}

        // Amount
        [Required(ErrorMessage="Please input transaction details")]
        public double Amount{get;set;}

        public DateTime CreatedAt{get;set;} = DateTime.Now;
        public DateTime UpdateAt{get;set;} = DateTime.Now;

        // Foreign Key
        public int RegUserId{get;set;}

    }
}