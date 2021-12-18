using AlintaCodingTest.Entities;
using System;
using System.Collections.Generic;

namespace AlintaCodingTest.Services
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetCustomers();
    }
}
