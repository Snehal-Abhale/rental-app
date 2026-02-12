using Microsoft.AspNetCore.Mvc;
using RentalMarket.Application.Listings;
using RentalMarket.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
namespace RentalMarket.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ListingsController : ControllerBase
{
    private readonly IListingRepository _repository;

    public ListingsController(IListingRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var listings = await _repository.GetAllAsync(ct);
        return Ok(listings);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var listing = await _repository.GetByIdAsync(id, ct);
        if (listing == null) return NotFound();
        return Ok(listing);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(Listing listing, CancellationToken ct)
    {
        // Simple create logic
        listing.Id = Guid.NewGuid();
        await _repository.AddAsync(listing, ct);
        return CreatedAtAction(nameof(GetById), new { id = listing.Id }, listing);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] decimal? maxPrice, CancellationToken ct)
    {
        var listings = await _repository.SearchAsync(maxPrice, ct);
        return Ok(listings);                     
    }
}