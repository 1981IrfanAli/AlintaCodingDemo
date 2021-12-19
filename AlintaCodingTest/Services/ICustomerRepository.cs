using AlintaCodingTest.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlintaCodingTest.Services
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetCustomers(string name);
        Task<Customer> GetCustomerById(Guid customerId);
        Task<IEnumerable<Customer>> GetCustomerByIds(IEnumerable<Guid> customerIds);
        void AddCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        void DeleteCustomer(Customer customer);
        Task<bool> SaveChangesAsync();
    }
}
