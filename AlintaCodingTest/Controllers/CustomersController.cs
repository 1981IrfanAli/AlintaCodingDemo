using AlintaCodingTest.ModelBinders;
using AlintaCodingTest.Models;
using AlintaCodingTest.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public CustomersController(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        //Get Customers
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet()]
        public async Task<IActionResult> GetCustomers()
        {
            var customerRepo = await _customerRepository.GetCustomers();
            return Ok(_mapper.Map<IEnumerable<CustomerReadDto>>(customerRepo));
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

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{customerId}", Name = "GetCustomerById")]
        public async Task<IActionResult> GetCustomerById(Guid customerId)
        {
            var customer = await _customerRepository.GetCustomerById(customerId);
            if (customer == null)
            {
                return NotFound();
            }
            
            return Ok(_mapper.Map<CustomerReadDto>(customer));
        }

        //Add Customers collecction
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> AddCustomers(IEnumerable<CustomerCreateDto> customers)
        {
            var customerEntities = _mapper.Map<IEnumerable<Entities.Customer>>(customers);

            foreach (var customerEntity in customerEntities)
            {
                _customerRepository.AddCustomer(customerEntity);
            }

            await _customerRepository.SaveChangesAsync();

            var customerToReturn = await _customerRepository.GetCustomerByIds(customerEntities.Select(b => b.Id).ToList());

            var customerIds = string.Join(",", customerToReturn.Select(a => a.Id));

            return CreatedAtRoute("GetCustomerCollection", new { customerIds }, customerToReturn);
        }

        //Update Customers collection
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPut]
        public async Task<IActionResult> UpdateCustomers(IEnumerable<CustomerUpdateDto> customers)
        {
            foreach (var customerDto in customers)
            {
                var customerEntity = await _customerRepository.GetCustomerById(customerDto.Id);
                if (customerEntity == null)
                {
                    return NotFound();
                }
                _mapper.Map(customerDto, customerEntity);
                _customerRepository.UpdateCustomer(customerEntity);
            }
            await _customerRepository.SaveChangesAsync();
            return NoContent();
        }

        //Update individual customer
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPut("{customerId}")]
        public async Task<IActionResult> UpdateCustomer(Guid customerId, CustomerUpdateDto customerUpdateDto)
        {
            var customer = await _customerRepository.GetCustomerById(customerId);
            if (customer == null)
            {
                return NotFound();
            }
            _mapper.Map(customerUpdateDto, customer);

            _customerRepository.UpdateCustomer(customer);

            await _customerRepository.SaveChangesAsync();

            return NoContent();
        }

        //Delete Customers collection 
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

        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        //Delete individual customer
        [HttpDelete("{customerId}")]
        public async Task<IActionResult> DeleteCustomer(Guid customerId)
        {
            var customer = await _customerRepository.GetCustomerById(customerId);

            if (customer == null)
            {
                return NotFound();
            }

            _customerRepository.DeleteCustomer(customer);

            await _customerRepository.SaveChangesAsync();

            return NoContent();
        }

    }
}
