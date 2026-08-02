using Catalog.API.Features.GetProductById;
using Catalog.API.Features.GetProductsByIds;
using Catalog.API.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CatalogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{productId:guid}")]
        public async Task<ActionResult<ProductDto>> GetById([FromRoute] GetProductByIdQuery query)
        {
            var result = await _mediator.Send(query);

            if (result.IsFailed)
                return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> GetAllByIds([FromQuery] GetProductsByIdsQuery query)
        {
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }
        
    }
}
