using AlintaCodingTest.DbContexts;
using AlintaCodingTest.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlintaCodingTest.Services
{
    public class CustomerRepository : ICustomerRepository, IDisposable
    {
        private readonly CustomerContext _context;

        public CustomerRepository(CustomerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Customer>> GetCustomers(string name)
        {
            return await _context.Customers
                    .Where(c=> c.FirstName.Contains(name) || c.LastName.Contains(name))
                    .ToListAsync();
        }

        public async Task<Customer> GetCustomerById(Guid customerId)
        {
            if (customerId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            return await _context.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
        }

        public async Task<IEnumerable<Customer>> GetCustomerByIds(IEnumerable<Guid> customerIds)
        {
            return await _context.Customers.Where(c => customerIds.Contains(c.Id)).ToListAsync();
        }

        public void AddCustomer(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            _context.Customers.Add(customer);
        }

        public void DeleteCustomer(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            _context.Customers.Remove(customer);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
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

        public void UpdateCustomer(Customer customer)
        {
            //Noting
        }

        public bool CustomerExists(Guid customerId)
        {
            if (customerId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            return  _context.Customers.Any(a => a.Id == customerId);
        }
    }
}
