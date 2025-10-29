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
                .ForMember(dest => dest.ClientUserName, opt => opt.MapFrom(src => src.ClientUser.UserFullName));

            //CreateMap<CreateAccountDto, Account>();
            //CreateMap<UpdateAccountDto, Account>()
            //    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
