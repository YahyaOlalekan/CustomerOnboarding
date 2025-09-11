using CustomerOnboarding.Api.Application.Services;
using CustomerOnboarding.Api.DTOs;
using CustomerOnboarding.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerOnboarding.Api.Controllers
{
    [ApiController]
    //[Route("api/[controller]")]
    [Route("api/customers")]
    public sealed class CustomersController : ControllerBase
    {
        private readonly CustomerService _svc;
        private readonly AppDbContext _db;


        public CustomersController(CustomerService svc, AppDbContext db)
        {
            _svc = svc;
            _db = db;
        }


        [HttpPost]
        public async Task<IActionResult> StartOnboarding([FromBody] CreateCustomerRequest request, CancellationToken ct)
        {
            // validate state-lga mapping
            var lga = await _db.Lgas.AsNoTracking().FirstOrDefaultAsync(l => l.Id == request.LgaId && l.StateId == request.StateId, ct);
            if (lga is null) return BadRequest(new { error = "LGA does not belong to the selected State" });


            await _svc.StartOnboardingAsync(request, ct);
            return Accepted(new { message = "OTP sent to phone. Verify to complete onboarding." });
        }


        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request, CancellationToken ct)
        {
            var dto = await _svc.VerifyOtpAndActivateAsync(request, ct);
            return Ok(dto);
        }


        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var list = await _svc.GetAllAsync(ct);
            return Ok(list);
        }
    }
}
