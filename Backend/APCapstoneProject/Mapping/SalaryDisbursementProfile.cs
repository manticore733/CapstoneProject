using APCapstoneProject.DTO.SalaryDisbursement;
using APCapstoneProject.Model;
using AutoMapper;

namespace APCapstoneProject.Mapping
{
    public class SalaryDisbursementProfile : Profile
    {
        public SalaryDisbursementProfile()
        {
            // ✅ CreateSalaryDisbursementDto → SalaryDisbursement
            CreateMap<CreateSalaryDisbursementDto, SalaryDisbursement>()
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore()) // computed dynamically
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(_ => 0)) // Pending
                .ForMember(dest => dest.TransactionTypeId, opt => opt.MapFrom(_ => 1)) // Debit
                .ForMember(dest => dest.Details, opt => opt.Ignore()); // handled manually

            // ✅ SalaryDisbursement → ReadSalaryDisbursementDto
            CreateMap<SalaryDisbursement, ReadSalaryDisbursementDto>()
                .ForMember(dest => dest.TransactionStatus,
                    opt => opt.MapFrom(src => src.TransactionStatus.StatusEnum.ToString()));

            // ✅ SalaryDisbursementDetail → SalaryDisbursementDetailReadDto
            CreateMap<SalaryDisbursementDetail, SalaryDisbursementDetailReadDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.EmployeeName));
        }
    }
}
