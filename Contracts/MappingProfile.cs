using AutoMapper;
using Contracts.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Contracts
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, RegisterDto>().ReverseMap();
            CreateMap<IdentityRole, RoleDto>().ReverseMap();
            CreateMap<Contact, ContactRequestDto>().ReverseMap();
            CreateMap<Reservation, ReservationRequestDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
