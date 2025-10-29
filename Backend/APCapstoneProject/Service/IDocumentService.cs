﻿using APCapstoneProject.DTO.Document;

namespace APCapstoneProject.Service
{
    public interface IDocumentService
    {
        Task<DocumentReadDto> UploadDocumentAsync(int clientUserId, int proofTypeId, IFormFile file);
        Task<IEnumerable<DocumentReadDto>> GetDocumentsForClientAsync(int clientUserId);
    }
}
