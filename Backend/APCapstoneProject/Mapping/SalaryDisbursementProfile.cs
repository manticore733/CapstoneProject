using APCapstoneProject.DTO.SalaryDisbursement;
using APCapstoneProject.Model;
using AutoMapper;

namespace APCapstoneProject.Mapping
{
    public class SalaryDisbursementProfile : Profile
    {
        public SalaryDisbursementProfile()
        {
            // 🔹 SalaryDisbursement → ReadSalaryDisbursementDto
            CreateMap<SalaryDisbursement, ReadSalaryDisbursementDto>()
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.ClientUserId, opt => opt.MapFrom(src => src.ClientUserId))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.BankName, opt => opt.MapFrom(src => src.BankName))
                .ForMember(dest => dest.IFSC, opt => opt.MapFrom(src => src.IFSC))
                .ForMember(dest => dest.DestinationAccountNumber, opt => opt.MapFrom(src => src.DestinationAccountNumber))
                .ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.Remarks))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.ProcessedAt, opt => opt.MapFrom(src => src.ProcessedAt))
                .ForMember(dest => dest.DisbursementDate, opt => opt.MapFrom(src => src.DisbursementDate))
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details));

            // 🔹 SalaryDisbursementDetail → SalaryDisbursementDetailReadDto
            CreateMap<SalaryDisbursementDetail, SalaryDisbursementDetailReadDto>()
                .ForMember(dest => dest.SalaryDisbursementDetailId, opt => opt.MapFrom(src => src.SalaryDisbursementDetailId))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.EmployeeName))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.Success))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark))
                .ForMember(dest => dest.ProcessedAt, opt => opt.MapFrom(src => src.ProcessedAt));

            // 🔹 CreateSalaryDisbursementDto → SalaryDisbursement
            CreateMap<CreateSalaryDisbursementDto, SalaryDisbursement>()
                .ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.Remarks))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => 0)) // PENDING
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
