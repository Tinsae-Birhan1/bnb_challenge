using Microsoft.AspNetCore.Mvc;
using Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using BLBServices;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
namespace Controllers.TokenSupplyController;

[Route("api/[controller]")]
public class TokenSupplyController : Controller
{

    private readonly AppDbContext _context;
    private readonly BLPService _blpService;
    public TokenSupplyController(AppDbContext context, BLPService blpService)
    {
        _context = context;
        _blpService = blpService;
    }


    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
        // the return is (decimal, decimal) 
        var tokenSupply = await _context.TokenSupplies.ToListAsync();
        return Ok(tokenSupply);
    }

    [Authorize]
    [HttpPost()]
    public async Task<IActionResult> Register()
    {

        (decimal circulating, decimal totalsupply) = await BLPService.GetTotalAndCirculatingSupply();

        var tokenSupply = new TokenSupply
        {
            TotalSupply = totalsupply,
            CirculatingSupply = circulating,
        };

        _context.TokenSupplies.Add(tokenSupply);
        await _context.SaveChangesAsync();
        return Ok(tokenSupply);
    }
}
