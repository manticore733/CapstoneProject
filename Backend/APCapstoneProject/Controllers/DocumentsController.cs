using APCapstoneProject.DTO.Document;
using APCapstoneProject.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APCapstoneProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        // POST /api/documents/uploadby/{clientUserId}
        // [Authorize(Roles = "CLIENT_USER")] // <-- Add this later
        [HttpPost("uploadby/{clientUserId}")]
        // We use [FromForm] because the request includes a file
        public async Task<ActionResult<DocumentReadDto>> UploadDocument(int clientUserId,[FromForm] int proofTypeId,IFormFile file)
        {
            // LATER: clientUserId will come from the JWT token, not the URL

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded or file is empty.");
            }
            if (proofTypeId < 0) // Basic validation for proofTypeId
            {
                return BadRequest("Invalid ProofTypeId provided.");
            }

            try
            {
                // Pass the data to the service
                var createdDocument = await _documentService.UploadDocumentAsync(clientUserId, proofTypeId, file);

                // Return 201 Created with the details of the created document
                // We don't have a specific "GetById" for documents, so we return the object directly
                return StatusCode(StatusCodes.Status201Created, createdDocument);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // e.g., "Client user not found"
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message }); // e.g., "File is empty"
            }
            catch (Exception ex)
            {
                // Log the full exception ex
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred during upload: {ex.Message}");
            }
        }

        // GET /api/documents/forclient/{clientUserId}
        // [Authorize(Roles = "BANK_USER")] // <-- Add this later for Bank Users reviewing
        [HttpGet("forclient/{clientUserId}")]
        public async Task<ActionResult<IEnumerable<DocumentReadDto>>> GetDocumentsForClient(int clientUserId)
        {
            // LATER: A Bank User would call this. We might need extra checks
            // to ensure this Bank User manages this Client User.

            try
            {
                var documents = await _documentService.GetDocumentsForClientAsync(clientUserId);

                // If the service returns an empty list because the client wasn't found, 
                // we might still return OK with an empty list, or explicitly return NotFound.
                // Let's assume the service handles the check and returns empty if client not found.
                return Ok(documents);
            }
            catch (Exception ex)
            {
                // Log the full exception ex
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred retrieving documents: {ex.Message}");
            }
        }
    }
}
