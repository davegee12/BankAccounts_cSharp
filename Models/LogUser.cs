using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace BankAccounts.Models
{
    public class LogUser
    {
        // Email
        [Required(ErrorMessage="Email is required")]
        [EmailAddress]
        public string LogEmail{get;set;}


        // Password
        [Required(ErrorMessage="Password is required")]
        [DataType(DataType.Password)]
        public string LogPassword{get;set;}
    }
}