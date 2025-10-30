using APCapstoneProject.DTO.Payment;
using APCapstoneProject.Model;
using AutoMapper;

namespace APCapstoneProject.Mapping
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<CreatePaymentDto, Payment>();
            CreateMap<Payment, ReadPaymentDto>()
                .ForMember(dest => dest.TransactionStatus, opt => opt.MapFrom(src => src.TransactionStatus.StatusEnum.ToString()))
                .ForMember(dest => dest.BeneficiaryName, opt => opt.MapFrom(src => src.Beneficiary.BeneficiaryName));
        }
    }
}
