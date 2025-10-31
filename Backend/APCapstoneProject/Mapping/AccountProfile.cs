using APCapstoneProject.DTO.Account;
using APCapstoneProject.Model;
using AutoMapper;

namespace APCapstoneProject.Mapping
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Account, ReadAccountDto>()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.ClientUser.UserFullName))
                .ForMember(dest => dest.BankName, opt => opt.MapFrom(src => src.Bank.BankName));
        }
    }
}
