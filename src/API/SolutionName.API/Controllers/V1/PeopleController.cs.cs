namespace SolutionName.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/People")]
public class PeopleController(IMediator mediator) : ControllerBase
{
    //Template
    //[HttpGet("{PersonId}")]
    //[ApiResponse(StatusCodes.Status200OK, typeof(PersonQueryResponse))]
    //[ApiResponse(StatusCodes.Status400BadRequest)]
    //public async Task<IActionResult> Get([FromRoute] GetPersonQuery query) =>
    //(await mediator.Send(query)).ToActionResult();

}


