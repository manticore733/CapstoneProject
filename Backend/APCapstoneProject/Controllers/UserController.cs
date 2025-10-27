//using APCapstoneProject.Data;
//using APCapstoneProject.DTO.User;
//using APCapstoneProject.Model;
//using AutoMapper;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System;

//namespace APCapstoneProject.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UserController : ControllerBase
//    {
//        private readonly BankingAppDbContext _context;
//        private readonly IMapper _mapper;

//        public UserController(BankingAppDbContext context, IMapper mapper)
//        {
//            _context = context;
//            _mapper = mapper;
//        }

//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetUsers()
//        {
//            var users = await _context.Users
//                .Include(u => u.Role)
//                .ToListAsync();

//            return Ok(_mapper.Map<IEnumerable<UserReadDto>>(users));
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<UserReadDto>> GetUser(int id)
//        {
//            var user = await _context.Users
//                .Include(u => u.Role)
//                .FirstOrDefaultAsync(u => u.UserId == id);

//            if (user == null) return NotFound();

//            return Ok(_mapper.Map<UserReadDto>(user));
//        }

//        [HttpPost]
//        public async Task<ActionResult<UserReadDto>> CreateUser(UserCreateDto userCreateDto)
//        {
//            var user = _mapper.Map<User>(userCreateDto);
//            user.CreatedAt = DateTime.UtcNow;

//            _context.Users.Add(user);
//            await _context.SaveChangesAsync();

//            var readDto = _mapper.Map<UserReadDto>(user);
//            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, readDto);
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateUser(int id, UserUpdateDto userUpdateDto)
//        {
//            if (id != userUpdateDto.UserId) return BadRequest();

//            var existingUser = await _context.Users.FindAsync(id);
//            if (existingUser == null) return NotFound();

//            _mapper.Map(userUpdateDto, existingUser);
//            existingUser.UpdatedAt = DateTime.UtcNow;

//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteUser(int id)
//        {
//            var user = await _context.Users.FindAsync(id);
//            if (user == null) return NotFound();

//            _context.Users.Remove(user);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }
//    }
//}








using APCapstoneProject.DTO.User;
using APCapstoneProject.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APCapstoneProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetUsers()
        {
            var users = await _service.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserReadDto>> GetUser(int id)
        {
            var user = await _service.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // --- GENERIC POST IS REMOVED ---

        // This endpoint is ONLY for Super Admins to create other Super Admins
        [HttpPost("superadmin")]
        public async Task<ActionResult<UserReadDto>> CreateSuperAdmin(UserCreateDto userCreateDto)
        {
            try
            {
                var created = await _service.CreateAsync(userCreateDto);
                return CreatedAtAction(nameof(GetUser), new { id = created.UserId }, created);
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDto userUpdateDto)
        {
            if (id != userUpdateDto.UserId) return BadRequest();
            var result = await _service.UpdateAsync(id, userUpdateDto);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteUser(int id)
        {
            var success = await _service.SoftDeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}








