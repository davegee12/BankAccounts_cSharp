using BankAccounts.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAccounts
{
    public class UsersContext : DbContext
    {
        public UsersContext(DbContextOptions options) : base(options) {}

        public DbSet<RegUser> Users {get;set;}
        public DbSet<Transaction> Transactions {get;set;}
    }
}