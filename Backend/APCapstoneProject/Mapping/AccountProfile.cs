using APCapstoneProject.DTO.Account;
using APCapstoneProject.Model;
using AutoMapper;

namespace APCapstoneProject.Mapping
{
    public class AccountProfile: Profile
    {
        public AccountProfile() 
        {
            CreateMap<Account, AccountReadDto>()
                .ForMember(dest => dest.BankName, opt => opt.MapFrom(src => src.Bank.BankName))
                .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => src.AccountType.Type.ToString()))
                .ForMember(dest => dest.ClientUserName, opt => opt.MapFrom(src => src.ClientUser.UserFullName))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.AccountStatus.StatusEnum));

            CreateMap<CreateAccountDto, Account>();
            CreateMap<UpdateAccountDto, Account>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
