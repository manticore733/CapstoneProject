using APCapstoneProject.DTO.Bank;
using APCapstoneProject.Model;
using APCapstoneProject.Service;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APCapstoneProject.Controllers
{
    [Authorize(Roles = "SUPER_ADMIN")]
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly IBankService _bankService;
        private readonly IMapper _mapper;

        public BankController(IBankService bankService, IMapper mapper)
        {
            _bankService = bankService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var banks = await _bankService.GetAllBanksAsync();
            var result = _mapper.Map<IEnumerable<BankReadDto>>(banks);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var bank = await _bankService.GetBankByIdAsync(id);
            if (bank == null) return NotFound();
            var result = _mapper.Map<BankReadDto>(bank);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BankCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var bank = _mapper.Map<Bank>(dto);
            var created = await _bankService.CreateBankAsync(bank);
            var readDto = _mapper.Map<BankReadDto>(created);
            return CreatedAtAction(nameof(GetById), new { id = readDto.BankId }, readDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BankUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var existingBank = await _bankService.GetBankByIdAsync(id);
            if (existingBank == null) return NotFound();
            _mapper.Map(dto, existingBank);
            await _bankService.UpdateBankAsync(id, existingBank);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _bankService.DeleteBankAsync(id);
            return NoContent();
        }
    }
}
