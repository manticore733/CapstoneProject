using APCapstoneProject.DTO.Document;
using APCapstoneProject.Service;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "CLIENT_USER")]
        [HttpPost]
        
        public async Task<ActionResult<DocumentReadDto>> UploadDocument([FromForm] int proofTypeId, IFormFile file)
        {
            var clientUserId = int.Parse(User.FindFirst("UserId")!.Value);

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded or file is empty.");
            }
            if (proofTypeId < 0)
            {
                return BadRequest("Invalid ProofTypeId provided.");
            }

            try
            {
                var createdDocument = await _documentService.UploadDocumentAsync(clientUserId, proofTypeId, file);
                return StatusCode(StatusCodes.Status201Created, createdDocument);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); 
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred during upload: {ex.Message}");
            }
        }

        [Authorize(Roles = "BANK_USER")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentReadDto>>> GetDocumentsForClient(int clientUserId)
        {
            var bankUserId = int.Parse(User.FindFirst("UserId")!.Value);

            try
            {
                var documents = await _documentService.GetDocumentsForClientAsync(bankUserId, clientUserId);
                return Ok(documents);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred retrieving documents: {ex.Message}");
            }
        }

        [Authorize(Roles = "CLIENT_USER")]
        [HttpGet("mydocuments")]
        public async Task<ActionResult<IEnumerable<DocumentReadDto>>> GetMyDocuments()
        {
            var clientUserId = int.Parse(User.FindFirst("UserId")!.Value);
            try
            {
                var documents = await _documentService.GetMyDocumentsAsync(clientUserId);
                return Ok(documents);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }







    }
}
