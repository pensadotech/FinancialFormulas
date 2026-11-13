using Asp.Versioning;
using FinancialFormulas.Models;
using FinancialFormulas.Services;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace FinancialFormulas.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class FinancialController : ControllerBase
{
    private readonly IFinancialService _financialService;

    public FinancialController(IFinancialService financialService)
    {
        _financialService = financialService;
    }

    [HttpPost("formulas")]
    [Consumes("application/json", "application/xml")]
    [Produces("application/json", "application/xml")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult PostFormulas([FromBody] FormulaRequest request)
    {
        // Validate the is a request body
        if (request == null)
        {
            return BadRequest( new ProblemDetails
            {
                Title = "Invalid request",
                Detail = "The request body is null.",
                Status = StatusCodes.Status400BadRequest,
            });
        }

        // validate that the request contains at least one formula
        if (request.Formulas == null || !request.Formulas.Any())
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid request",
                Detail = "The request must contain at least one formula.",
                Status = StatusCodes.Status400BadRequest,
            });
        }

        try
        {
            // Execute the formulas using the financial service
            var responses = _financialService.ExecuteFormulas(request);

            // Create a batch response with statistics and results
            var bacthResponse = new FormulaBatchResponse
            {
                Statistics = new FormulaBatchStatistics
                {
                    Total = responses.Count(),
                    Succeeded = responses.Count(r => r.Error == null),
                    Failed = responses.Count(r => r.Error != null)
                },
                Responses = responses
            };

            return Ok(bacthResponse);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid request",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest,
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = ex.Message,
                Status = StatusCodes.Status500InternalServerError,
            });
        }
       
    }
}
