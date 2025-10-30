using APCapstoneProject.DTO.User.ClientUser;
using APCapstoneProject.Model;
using AutoMapper;

namespace APCapstoneProject.Mapping
{
    public class ClientUserProfile:Profile
    {
        public ClientUserProfile()
        {
            CreateMap<CreateClientUserDto, ClientUser>();
            CreateMap<ClientUser, ReadClientUserDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Role.ToString()))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.VerificationStatus.StatusEnum.ToString()))
                .ForMember(dest => dest.BankId, opt => opt.MapFrom(src => src.BankUser.BankId))
                .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.Account != null ? src.Account.AccountNumber : null));
            CreateMap<UpdateClientUserDto, ClientUser>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
