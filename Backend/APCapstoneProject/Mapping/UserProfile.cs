using APCapstoneProject.DTO.Beneficiary;
using APCapstoneProject.DTO.Employee;
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
            CreateMap<CreateClientUserDto, ClientUser>();
            CreateMap<UpdateClientUserDto, ClientUser>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ---  BENEFICIARY MAPS ---
            CreateMap<Beneficiary, BeneficiaryReadDto>();
            CreateMap<CreateBeneficiaryDto, Beneficiary>();
            CreateMap<UpdateBeneficiaryDto, Beneficiary>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));




            // --- ADD THESE EMPLOYEE MAPS ---
            CreateMap<Employee, EmployeeReadDto>();
            CreateMap<CreateEmployeeDto, Employee>();
            CreateMap<UpdateEmployeeDto, Employee>()
                 .ForAllMembers(opts =>
                     opts.Condition((src, dest, srcMember, destMember, context) =>
                     {
                         // ignore nulls
                         if (srcMember == null)
                             return false;

                         // special handling for salary — ignore if 0
                         if (srcMember is decimal salary && salary == 0)
                             return false;

                         return true;
                     }));






        }
    }
}
