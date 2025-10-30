using APCapstoneProject.DTO.User;
using APCapstoneProject.Model;
using AutoMapper;

namespace APCapstoneProject.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            //assigns Rolename var in DTO with Enum Role from entity of user
            CreateMap<User, UserReadDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Role.ToString()));

            CreateMap<UserCreateDto, User>();
            CreateMap<UserUpdateDto, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            // Bank User Maps
            CreateMap<CreateBankUserDto, BankUser>();
            CreateMap<UpdateBankUserDto, BankUser>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Client User Maps
            CreateMap<ClientUser, ClientStatusReadDto>()
                // Only specify fields that AutoMapper can’t match automatically
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Role.ToString()))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.VerificationStatus.StatusEnum.ToString()))
                // --- ADD THIS LINE ---
                .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.Account != null ? src.Account.AccountNumber : null));





            CreateMap<CreateClientUserDto, ClientUser>();
            CreateMap<UpdateClientUserDto, ClientUser>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}
