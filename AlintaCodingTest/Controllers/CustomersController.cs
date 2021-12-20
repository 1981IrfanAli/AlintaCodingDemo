using AlintaCodingTest.Entities;
using AlintaCodingTest.ModelBinders;
using AlintaCodingTest.Models;
using AlintaCodingTest.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlintaCodingTest.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICustomerRepository customerRepository, IMapper mapper, ILogger<CustomersController> logger)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger;
        }

        [ProducesResponseType(typeof(IEnumerable<CustomerReadDto>), StatusCodes.Status200OK)]
        [HttpGet()]
        public async Task<IEnumerable<CustomerReadDto>> GetCustomers(string name = "")
        {
            var customerRepo = await _customerRepository.GetCustomers(name);
            _logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {customerRepo.Count()} customers");
            return _mapper.Map<IEnumerable<CustomerReadDto>>(customerRepo);
        }


        [ProducesResponseType(typeof(CustomerReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{customerId}", Name = "GetCustomerById")]
        public async Task<ActionResult<CustomerReadDto>> GetCustomerById(Guid customerId)
        {
            var customer = await _customerRepository.GetCustomerById(customerId);
            if (customer is null)
            {
                return NotFound();
            }
            _logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {customer.Id} customer");
            return _mapper.Map<CustomerReadDto>(customer);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost(Name = "AddCustomer")]
        public async Task<ActionResult<CustomerReadDto>> AddCustomer(CustomerCreateDto customerCreateDto)
        {
            var customer = _mapper.Map<Customer>(customerCreateDto);
            _customerRepository.AddCustomer(customer);
            await _customerRepository.SaveChangesAsync();

            _logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Created {customer.Id} customer");

            var commandReadDto = _mapper.Map<CustomerReadDto>(customer);
            return CreatedAtRoute(nameof(GetCustomerById), new { Id = commandReadDto.Id }, commandReadDto);
        }

        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPut("{customerId}")]
        public async Task<IActionResult> UpdateCustomer(Guid customerId, CustomerUpdateDto customerUpdateDto)
        {
            var customer = await _customerRepository.GetCustomerById(customerId);
            if (customer is null)
            {
                return NotFound();
            }
            _mapper.Map(customerUpdateDto, customer);

            _customerRepository.UpdateCustomer(customer);

            await _customerRepository.SaveChangesAsync();

            _logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Updated {customer.Id} customer");

            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpDelete("{customerId}")]
        public async Task<IActionResult> DeleteCustomer(Guid customerId)
        {
            var customer = await _customerRepository.GetCustomerById(customerId);

            if (customer is null)
            {
                return NotFound();
            }

            _customerRepository.DeleteCustomer(customer);

            await _customerRepository.SaveChangesAsync();

            _logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Deleted {customerId} customer");

            return NoContent();
        }

    }
}
