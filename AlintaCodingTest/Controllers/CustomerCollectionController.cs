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
    [Route("api/CustomerCollection")]
    [ApiController]
    public class CustomerCollectionController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomersController> _logger;

        public CustomerCollectionController(ICustomerRepository customerRepository, IMapper mapper, ILogger<CustomersController> logger)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger;
        }

        [ProducesResponseType(typeof(IEnumerable<CustomerReadDto>), StatusCodes.Status200OK)]
        [HttpPost()]
        public async Task<IActionResult> AddCustomers(IEnumerable<CustomerCreateDto> customers)
        {
            var customerEntities = _mapper.Map<IEnumerable<Entities.Customer>>(customers);

            foreach (var customerEntity in customerEntities)
            {
                _customerRepository.AddCustomer(customerEntity);
            }

            await _customerRepository.SaveChangesAsync();

            var customerCollectionToReturn = _mapper.Map<IEnumerable<CustomerReadDto>>(customerEntities);
            var idsAsString = string.Join(",", customerCollectionToReturn.Select(a => a.Id));
            return CreatedAtRoute("GetCustomerCollection", new { customerIds = idsAsString }, customerCollectionToReturn);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("({customerIds})", Name = "GetCustomerCollection")]
        public async Task<IActionResult> GetCustomerCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> customerIds)
        {
            var customerEntities = await _customerRepository.GetCustomerByIds(customerIds);

            if (customerIds.Count() != customerEntities.Count())
            {
                return NotFound();
            }

            return Ok(customerEntities);
        }

        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomers(IEnumerable<CustomerDeleteDto> customers)
        {
            foreach (var customerDto in customers)
            {
                var customerEntity = await _customerRepository.GetCustomerById(customerDto.Id);
                if (customerEntity == null)
                {
                    return NotFound();
                }
                _customerRepository.DeleteCustomer(customerEntity);
            }
            await _customerRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}
