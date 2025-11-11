using APCapstoneProject.DTO.Beneficiary;
using APCapstoneProject.Model;
using AutoMapper;

namespace APCapstoneProject.Mapping
{
    public class BeneficiaryProfile: Profile
    {
        public BeneficiaryProfile()
        {

            CreateMap<Beneficiary, BeneficiaryReadDto>();
            CreateMap<CreateBeneficiaryDto, Beneficiary>();
            CreateMap<UpdateBeneficiaryDto, Beneficiary>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}
