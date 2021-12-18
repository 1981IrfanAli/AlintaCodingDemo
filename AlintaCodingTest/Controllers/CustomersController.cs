using AlintaCodingTest.Models;
using AlintaCodingTest.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace AlintaCodingTest.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomersController(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository ??
              throw new ArgumentNullException(nameof(customerRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet()]
        public ActionResult<IEnumerable<CustomerDto>> GetCustomers()
        {
            var customerRepo = _customerRepository.GetCustomers();
            return Ok(_mapper.Map<IEnumerable<CustomerDto>>(customerRepo));
        }
    }
}
