using Microsoft.AspNetCore.Mvc;
using System;
using VendorHub.Models.Repositories;

namespace VendorHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly AdminDashboardRepository _repo;

        public AdminDashboardController(AdminDashboardRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("{companyId}")]
        public IActionResult GetDashboardStats(int companyId)
        {
            try
            {
                var stats = _repo.GetAdminDashboardStats(companyId);
                var topVendors = _repo.GetTopVendors(companyId);
                return Ok(new { stats, topVendors });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ADMIN API ERROR] Type={ex.GetType().Name} Message={ex.Message}");
                if (ex.InnerException != null)
                    Console.Error.WriteLine($"[ADMIN API ERROR] Inner={ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                Console.Error.WriteLine($"[ADMIN API ERROR] StackTrace={ex.StackTrace}");
                return StatusCode(500, new { message = "An error occurred while fetching admin stats.", error = ex.Message, errorType = ex.GetType().Name });
            }
        }
    }
}
