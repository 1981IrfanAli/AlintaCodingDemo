using AlintaCodingTest.Entities;
using AlintaCodingTest.Models;
using AutoMapper;

namespace AlintaCodingTest.Profiles
{
    public class CustomersProfile : Profile
    {
        public CustomersProfile()
        {          
            CreateMap<Customer, CustomerReadDto>();
            CreateMap<CustomerCreateDto, Customer>();
            CreateMap<Customer, CustomerUpdateDto>();
            CreateMap<CustomerUpdateDto, Customer>();
        }
    }
}
