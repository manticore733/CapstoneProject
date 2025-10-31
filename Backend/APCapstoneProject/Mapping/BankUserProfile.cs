using APCapstoneProject.DTO.User.BankUser;
using APCapstoneProject.Model;
using AutoMapper;
using System.Linq;

namespace APCapstoneProject.Mapping
{
    public class BankUserProfile : Profile
    {
        public BankUserProfile()
        {
            CreateMap<CreateBankUserDto, BankUser>();

            CreateMap<BankUser, ReadBankUserDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Role.ToString()))
                .ForMember(dest => dest.BankName, opt => opt.MapFrom(src => src.Bank.BankName))
                .ForMember(dest => dest.ClientCount,
                    opt => opt.MapFrom(src => src.Clients.Count(c => c.IsActive))) // only active clients
                .ForSourceMember(src => src.Clients, opt => opt.DoNotValidate()); // prevent full mapping validation

            CreateMap<UpdateBankUserDto, BankUser>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
