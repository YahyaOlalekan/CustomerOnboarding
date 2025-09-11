using CustomerOnboarding.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerOnboarding.Api.Controllers
{
    [ApiController]
    [Route("api/banks")]

    public sealed class BanksController : ControllerBase
    {
        private readonly BankService _svc;


        public BanksController(BankService svc) => _svc = svc;


        [HttpGet]
        public async Task<IActionResult> GetBanks(CancellationToken ct)
        {
            var json = await _svc.GetBanksRawAsync(ct);
            return Content(json, "application/json");
        }
    }
}
