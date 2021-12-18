using AlintaCodingTest.DbContexts;
using AlintaCodingTest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlintaCodingTest.Services
{
    public class CustomerRepository : ICustomerRepository, IDisposable
    {
        private readonly CustomerContext _context;

        public CustomerRepository(CustomerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IEnumerable<Customer> GetCustomers()
        {
            return _context.Customers.ToList<Customer>();
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose resources when needed
            }
        }
    }
}
