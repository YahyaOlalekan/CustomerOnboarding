using AutoMapper;
using CustomerOnboarding.Api.Domain.Entities;
using CustomerOnboarding.Api.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CustomerOnboarding.Api.Mapping
{
    public sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerDto>();
        }
    }
}
