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
        private readonly IClientUserRepository _clientUserRepository; // To verify ClientUser exists
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary; // Cloudinary client

        public DocumentService(
            IDocumentRepository documentRepository,
            IClientUserRepository clientUserRepository,
            IMapper mapper,
            IOptions<CloudinarySettings> config) 
        {
            _documentRepository = documentRepository;
            _clientUserRepository = clientUserRepository;
            _mapper = mapper;

            // Configure Cloudinary Client 
            CloudinaryDotNet.Account account = new CloudinaryDotNet.Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret);

            _cloudinary = new Cloudinary(account);
     
        }

        public async Task<DocumentReadDto> UploadDocumentAsync(int clientUserId, int proofTypeId, IFormFile file)
        {
            // 1. Validate Client User
            var clientUser = await _clientUserRepository.GetClientUserByIdAsync(clientUserId);
            if (clientUser == null || !(clientUser is ClientUser))
            {
                throw new KeyNotFoundException($"Client user with ID {clientUserId} not found.");
            }

    

            // 2. Validate File
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or not provided.");
            }

            // 3. Upload to Cloudinary
           
            var uploadParams = new RawUploadParams() //  RawUploadParams for non-image files like PDF
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
           
                Folder = $"corporate_banking/{clientUserId}/documents",
           
                PublicId = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid().ToString(),
                Overwrite = true // false if you want to prevent overwriting

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


            // change user verification status back to pending if he is reuploading.
            if (clientUser.StatusId == (int)StatusEnum.REJECTED)
            {
                clientUser.StatusId = (int)StatusEnum.PENDING;
                await _clientUserRepository.UpdateClientUserAsync(clientUser);
                await _documentRepository.SaveChangesAsync();
            }



            // 6. Return DTO (Need to get ProofType name)
       
            var documentDto = _mapper.Map<DocumentReadDto>(document);

            documentDto.ProofTypeName = "Type_" + proofTypeId; // Placeholder

            return documentDto;
        }

        public async Task<IEnumerable<DocumentReadDto>> GetDocumentsForClientAsync(int bankUserId, int clientUserId)
        {
            // check if client with given Id exists
            var clientUser = await _clientUserRepository.GetClientUserByIdAsync(clientUserId);
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
            // 1. Validate Client User 
            var clientUser = await _clientUserRepository.GetClientUserByIdAsync(clientUserId);
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
