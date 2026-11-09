using Microsoft.EntityFrameworkCore;
using PostalSystem.Api.Data;
using PostalSystem.Api.Dtos;

namespace PostalSystem.Api.Services
{
    public class PostmanService : IPostmanService
    {
        private readonly PostalSystemDbContext _context;
        private readonly ILogger<PostmanService> _logger;

        public PostmanService(PostalSystemDbContext context, ILogger<PostmanService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult> LoginAsync(LoginRequest request)
        {
            var postman = await _context.Postmen
                .FirstOrDefaultAsync(p => p.Name == request.Name && p.Phone == request.Phone && p.IsActive);

            if (postman == null)
            {
                _logger.LogWarning("Failed login attempt for {Name} with phone {Phone}", request.Name, request.Phone);
                return new ServiceResult(false, "Invalid name or phone number.");
            }

            return new ServiceResult(true, "Login successful. Welcome!",
                   new LoginResponse(postman.PostmanId));
        }
    }
}