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
            CreateMap<UserUpdateDto, User>();



            // Bank User Maps
            CreateMap<CreateBankUserDto, BankUser>();
            CreateMap<UpdateBankUserDto, BankUser>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Client User Maps
            CreateMap<CreateClientUserDto, ClientUser>();
            CreateMap<UpdateClientUserDto, ClientUser>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));





        }
    }
}
