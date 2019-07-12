using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace BankAccounts.Models
{
    public class RegUser
    {
        [Key]
        public int RegUserId{get;set;}
        // First Name
        [Required(ErrorMessage="First Name is required")]
        [MinLength(2, ErrorMessage="First name must be longer than two characters")]
        public string FName{get;set;}

        //Last Name
        [Required(ErrorMessage="Last Name is required")]
        [MinLength(2, ErrorMessage="Last name must be longer than two characters")]
        public string LName{get;set;}

        // Email
        [Required(ErrorMessage="Email is required")]
        [EmailAddress(ErrorMessage="Must be valid email format")]
        public string Email{get;set;}

        // Password
        [Required(ErrorMessage="Password is required")]
        [MinLength(8, ErrorMessage="Password must be eight or more characters")]
        [DataType(DataType.Password)]
        public string Password{get;set;}

        // Confirm
        [NotMapped]
        [Required(ErrorMessage="Please confirm your password")]
        [MinLength(8)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string Confirm{get;set;}

        // Transactions Made
        public List<Transaction> MadeTransactions {get;set;}

        public DateTime CreatedAt{get;set;} = DateTime.Now;
        public DateTime UpdatedAt{get;set;} = DateTime.Now;
    }
}
