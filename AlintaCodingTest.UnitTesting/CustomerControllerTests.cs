using AlintaCodingTest.Controllers;
using AlintaCodingTest.Entities;
using AlintaCodingTest.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using AlintaCodingTest.Profiles;
using AlintaCodingTest.Models;
using FluentAssertions;

namespace AlintaCodingTest.UnitTesting
{
    public class CustomerControllerTests
    {
        private readonly Mock<ICustomerRepository> repositoryStub = new();
        private readonly Mock<ILogger<CustomersController>> loggerStub = new();
        private static IMapper _mapper;

        public CustomerControllerTests()
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new CustomersProfile());
                });
                IMapper mapper = mappingConfig.CreateMapper();
                _mapper = mapper;
            }
        }

        [Fact]
        public async Task GetCustomerById_WithUnexistingCustomer_ReturnsNotFound()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.GetCustomerById(It.IsAny<Guid>()))
                .ReturnsAsync((Customer)null);

            var controller = new CustomersController(repositoryStub.Object, _mapper, loggerStub.Object);

            // Act
            var result = await controller.GetCustomerById(Guid.NewGuid());

            // Assert
            result.Value.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetCustomerById_WithExistingCustomer_ReturnsExpectedCustomer()
        {
            // Arrange
            Customer expectedCustomer = CreateCustomer();

            repositoryStub.Setup(repo => repo.GetCustomerById(It.IsAny<Guid>()))
                .ReturnsAsync(expectedCustomer);

            var controller = new CustomersController(repositoryStub.Object, _mapper,loggerStub.Object);

            // Act
            var result = await controller.GetCustomerById(Guid.NewGuid());

            // Assert
            result.Value.Should().BeEquivalentTo(expectedCustomer,options => options.ComparingByMembers<Customer>());
        }

        [Fact]
        public async Task GetCustomers_WithExistingCustomers_ReturnsAllCustomers()
        {
            // Arrange
            var expectedItems = new[] { CreateCustomer(), CreateCustomer(), CreateCustomer() };

            repositoryStub.Setup(repo => repo.GetCustomers(string.Empty))
                .ReturnsAsync(expectedItems);

            var controller = new CustomersController(repositoryStub.Object, _mapper, loggerStub.Object);

            // Act
            var actualItems = await controller.GetCustomers();

            // Assert
            actualItems.Should().BeEquivalentTo(expectedItems, options => options.ComparingByMembers<Customer>());
        }

        [Fact]
        public async Task GetCustomers_WithMatchingCustomers_ReturnsMatchingCustomers()
        {
            // Arrange
            var allCustomer = new[]
            {
                new Customer(){ FirstName = "Irfan", LastName = "Ali", DateOfBirth= DateTime.Now, Id = Guid.NewGuid()},
                new Customer(){ FirstName = "John", LastName = "Mark", DateOfBirth= DateTime.Now, Id = Guid.NewGuid()},
                new Customer(){ FirstName = "Steve", LastName = "Scott", DateOfBirth= DateTime.Now, Id = Guid.NewGuid()},
            };

            var nameToMatch = "Irfan";

            repositoryStub.Setup(repo => repo.GetCustomers(""))
                .ReturnsAsync(allCustomer);

            var controller = new CustomersController(repositoryStub.Object, _mapper, loggerStub.Object);

            // Act
            IEnumerable<CustomerReadDto> foundCustomers = await controller.GetCustomers(nameToMatch);

            // Assert
            foundCustomers.Should().OnlyContain(
                item => item.FirstName == allCustomer[0].FirstName);
        }

        [Fact]
        public async Task AddCustomer_WithCustomerToCreate_ReturnsCreateCustomer()
        {
            // Arrange
            var customerToCreate = new CustomerCreateDto()
            {
                Id = Guid.NewGuid(),
                FirstName = "Irfan",
                LastName = "Ali",
                DateOfBirth = DateTime.Now,
            };

            var controller = new CustomersController(repositoryStub.Object, _mapper, loggerStub.Object);

            // Act
            var result = await controller.AddCustomer(customerToCreate);

            // Assert
            var createdItem = (((Microsoft.AspNetCore.Mvc.ObjectResult)(result.Result)).Value as CustomerReadDto); 
            customerToCreate.Should().BeEquivalentTo(createdItem,
                options => options.ComparingByMembers<CustomerReadDto>().ExcludingMissingMembers()
            );
            createdItem.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task UpdateCustomer_WithExistingCustomer_ReturnsNoContent()
        {
            // Arrange
            Customer existingCustomer = CreateCustomer();
            repositoryStub.Setup(repo => repo.GetCustomerById(It.IsAny<Guid>()))
                .ReturnsAsync(existingCustomer);

            var customerId = existingCustomer.Id;

            var customerToUpdate = new CustomerUpdateDto()
            {
                FirstName = "Irfan",
                LastName = "Ali",
                DateOfBirth = DateTime.Now,
            };


            var controller = new CustomersController(repositoryStub.Object, _mapper, loggerStub.Object);

            // Act
            var result = await controller.UpdateCustomer(customerId, customerToUpdate);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteCustomer_WithExistingCustomer_ReturnsNoContent()
        {
            // Arrange
            Customer existingCustomer = CreateCustomer();
            repositoryStub.Setup(repo => repo.GetCustomerById(It.IsAny<Guid>()))
                .ReturnsAsync(existingCustomer);

            var controller = new CustomersController(repositoryStub.Object, _mapper, loggerStub.Object);

            // Act
            var result = await controller.DeleteCustomer(existingCustomer.Id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        private Customer CreateCustomer()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                DateOfBirth = DateTime.Now
            };
        }

    }
}
