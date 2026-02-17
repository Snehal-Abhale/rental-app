using Microsoft.AspNetCore.Mvc;
using RentalMarket.Application.Listings;
using RentalMarket.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
namespace RentalMarket.Api.Controllers;
using MediatR;
using RentalMarket.Application.Listings.Commands.CreateListing; // <--- Add this
using RentalMarket.Application.Listings.Queries.GetListings;

[ApiController]
[Route("api/[controller]")]
public class ListingsController : ControllerBase
{
    private readonly IListingRepository _repository;
    private readonly IMediator _mediator; // <--- Add this

     public ListingsController(IListingRepository repository, IMediator mediator) // <--- Update constructor
    {
        _repository = repository;
        _mediator = mediator; // <--- Initialize mediator
    }

       [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        // OLD: var listings = await _repository.GetAllAsync(ct);
        
        // NEW: CQRS + Dapper
        var listings = await _mediator.Send(new GetListingsQuery(), ct);
        
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
public async Task<IActionResult> Create(CreateListingCommand command, CancellationToken ct)
{
    // The Controller just "Sends" the command. It doesn't know HOW it's saved.
    var listingId = await _mediator.Send(command, ct);
    
    // We return the Id (or you could fetch the full object if you want)
    return CreatedAtAction(nameof(GetById), new { id = listingId }, new { id = listingId });
}

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<SearchListingDto>>> Search([FromQuery] decimal? maxPrice, [FromQuery] string? q, CancellationToken ct)
    {
        var listings = await _repository.SearchAsync(maxPrice, q, ct);
        return Ok(listings);                     
    }
}