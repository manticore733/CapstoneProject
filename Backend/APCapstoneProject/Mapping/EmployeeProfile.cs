using APCapstoneProject.DTO.Employee;
using APCapstoneProject.Model;
using AutoMapper;

namespace APCapstoneProject.Mapping
{
    public class EmployeeProfile: Profile
    {
        public EmployeeProfile() 
        {
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
