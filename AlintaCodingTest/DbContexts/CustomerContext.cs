using AlintaCodingTest.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlintaCodingTest.DbContexts
{
    public class CustomerContext : DbContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options)
          : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
    }
}
