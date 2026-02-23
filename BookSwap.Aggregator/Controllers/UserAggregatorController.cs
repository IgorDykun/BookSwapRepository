using BookSwap.Aggregator.ViewModels;
using BookSwap.Domain.Entities;
using BookSwap.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

namespace BookSwap.Aggregator.Controllers
{
    [ApiController]
    [Route("api/aggregator/[controller]")]
    public class UserAggregatorController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly Tracer _tracer;

        public UserAggregatorController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _tracer = TracerProvider.Default.GetTracer("UserAggregator");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            using var span = _tracer.StartActiveSpan("GetUserById");
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null) return NotFound();

                return Ok(MapToViewModel(user));
            }
            catch (Exception ex)
            {
                span.SetAttribute("error", true);
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            using var span = _tracer.StartActiveSpan("GetAllUsers");
            try
            {
                var users = await _userRepository.GetAllAsync();
                return Ok(users.Select(MapToViewModel).ToList());
            }
            catch (Exception ex)
            {
                span.SetAttribute("error", true);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserViewModel newUser)
        {
            using var span = _tracer.StartActiveSpan("CreateUser");
            try
            {
                var document = new UserDocument
                {
                    Name = newUser.Name,
                    Email = newUser.Email,
                    JoinedAt = DateTime.UtcNow
                };
                await _userRepository.CreateAsync(document);
                return CreatedAtAction(nameof(GetUserById), new { id = document.Id }, MapToViewModel(document));
            }
            catch (Exception ex)
            {
                span.SetAttribute("error", true);
                return StatusCode(500, ex.Message);
            }
        }

        private static UserViewModel MapToViewModel(UserDocument user) => new()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            JoinedAt = user.JoinedAt
        };
    }
}