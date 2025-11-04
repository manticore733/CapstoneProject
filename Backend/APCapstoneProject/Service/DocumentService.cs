using APCapstoneProject.DTO.Document;
using APCapstoneProject.Model;
using APCapstoneProject.Repository;
using APCapstoneProject.Settings;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace APCapstoneProject.Service
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IUserRepository _userRepository; // To verify ClientUser exists
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary; // Cloudinary client

        public DocumentService(
            IDocumentRepository documentRepository,
            IUserRepository userRepository,
            IMapper mapper,
            IOptions<CloudinarySettings> config) // Inject settings via IOptions
        {
            _documentRepository = documentRepository;
            _userRepository = userRepository;
            _mapper = mapper;

            // --- Configure Cloudinary Client ---
            CloudinaryDotNet.Account account = new CloudinaryDotNet.Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret);

            _cloudinary = new Cloudinary(account);
            // --- End Cloudinary Config ---
        }

        public async Task<DocumentReadDto> UploadDocumentAsync(int clientUserId, int proofTypeId, IFormFile file)
        {
            // 1. Validate Client User
            var clientUser = await _userRepository.GetByIdAsync(clientUserId);
            if (clientUser == null || !(clientUser is ClientUser))
            {
                throw new KeyNotFoundException($"Client user with ID {clientUserId} not found.");
            }

            // Optional: Validate ProofTypeId exists (you'd need a ProofTypeRepository)

            // 2. Validate File
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or not provided.");
            }

            // 3. Upload to Cloudinary
            // Note: Cloudinary calls it ImageUploadParams, but it works for any file type (PDFs too)
            var uploadParams = new RawUploadParams() // Use RawUploadParams for non-image files like PDF
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                // Optional: Define a folder structure in Cloudinary
                Folder = $"corporate_banking/{clientUserId}/documents",
                // Optional: Use original filename, or generate a unique one
                PublicId = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid().ToString(),
                Overwrite = true // Or false if you want to prevent overwriting

            };

            RawUploadResult uploadResult; // Use RawUploadResult
            try
            {
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            catch (Exception ex)
            {
                // Log the exception details
                throw new Exception($"Cloudinary upload failed: {ex.Message}");
            }


            if (uploadResult.Error != null)
            {
                throw new Exception($"Cloudinary upload error: {uploadResult.Error.Message}");
            }

            // 4. Create Document Entity
            var document = new Document
            {
                ClientUserId = clientUserId,
                ProofTypeId = proofTypeId,
                DocumentName = file.FileName, // Store original filename
                DocumentURL = uploadResult.SecureUrl.ToString() // Store the secure URL from Cloudinary
            };

            // 5. Save to Database
            await _documentRepository.AddAsync(document);
            await _documentRepository.SaveChangesAsync();

            // 6. Return DTO (Need to get ProofType name)
            // Re-fetch or manually construct DTO if AddAsync doesn't return relations
            // For simplicity, let's assume AddAsync updates the ID and we map manually for now
            var documentDto = _mapper.Map<DocumentReadDto>(document);

            // We need the ProofType Name, but the 'document' object doesn't have it loaded.
            // A better way would be to fetch the ProofType separately or load it.
            // For now, let's skip the name, or you can add logic to fetch it.
            documentDto.ProofTypeName = "Type_" + proofTypeId; // Placeholder

            return documentDto;
        }

        public async Task<IEnumerable<DocumentReadDto>> GetDocumentsForClientAsync(int bankUserId, int clientUserId)
        {
            // check if client with given Id exists
            var clientUser = await _userRepository.GetByIdAsync(clientUserId);
            if (clientUser == null || clientUser is not ClientUser)
                throw new KeyNotFoundException($"Client user with ID {clientUserId} not found.");

            // Checking if clientUser belonmgs to BankUser
            var client = clientUser as ClientUser;
            if (client.BankUserId != bankUserId)
                throw new UnauthorizedAccessException("This client does not belong to you");

            var documents = await _documentRepository.GetDocumentsByClientIdAsync(clientUserId);

            return _mapper.Map<IEnumerable<DocumentReadDto>>(documents);
        }

        public async Task<IEnumerable<DocumentReadDto>> GetMyDocumentsAsync(int clientUserId)
        {
            // 1. Validate Client User (optional, but good practice)
            var clientUser = await _userRepository.GetByIdAsync(clientUserId);
            if (clientUser == null || !(clientUser is ClientUser))
            {
                throw new KeyNotFoundException($"Client user with ID {clientUserId} not found.");
            }

            // 2. Get documents
            var documents = await _documentRepository.GetDocumentsByClientIdAsync(clientUserId);

            // 3. Map and return
            return _mapper.Map<IEnumerable<DocumentReadDto>>(documents);
        }







    }
}
