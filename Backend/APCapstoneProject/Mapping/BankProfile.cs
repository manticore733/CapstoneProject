using APCapstoneProject.DTO.Bank;
using APCapstoneProject.Model;
using AutoMapper;

namespace APCapstoneProject.Mapping
{
    public class BankProfile : Profile
    {
        public BankProfile()
        {
            // Entity → DTO
            CreateMap<Bank, BankReadDto>()
                .ForMember(dest => dest.TotalUsers, opt => opt.MapFrom(src => src.Users.Count))
                .ForMember(dest => dest.TotalAccounts, opt => opt.MapFrom(src => src.Accounts.Count));

            // DTO → Entity (for Create)
            CreateMap<BankCreateDto, Bank>();

            // DTO → Entity (for Update)
            // If null values are sent, the existing ones do not change.
            CreateMap<BankUpdateDto, Bank>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
