using APCapstoneProject.DTO.Document;
using APCapstoneProject.Model;
using AutoMapper;

namespace APCapstoneProject.Mapping
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            // --- ADD THESE DOCUMENT MAPS ---
            CreateMap<Document, DocumentReadDto>()
                .ForMember(dest => dest.ProofTypeName, opt => opt.MapFrom(src => src.ProofType.Type.ToString())); // Get the enum name

            // No map needed from CreateDocumentDto -> Document, as the service handles that manually
        }
    }
}





