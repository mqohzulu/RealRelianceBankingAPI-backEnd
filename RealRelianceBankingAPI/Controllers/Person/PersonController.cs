using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealRelianceBanking.Application.Person.Command.CreatePerson;
using RealRelianceBanking.Application.Person.Command.DeletePerson;
using RealRelianceBanking.Application.Person.Command.EditPerson;
using RealRelianceBanking.Application.Person.Queries.GetPersonByEmail;
using RealRelianceBanking.Application.Person.Queries.GetPersonById;
using RealRelianceBanking.Application.Person.Queries.GetPersonByIdNumber;
using RealRelianceBanking.Application.Person.Queries.GetPersons;

namespace RealRelianceBankingAPI.Controllers.Person
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PersonsController : ControllerBase
    {
        private readonly ISender _mediator;
        public PersonsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("AddNewPerson")]
        public async Task<IActionResult> AddNewPerson(CreatePersonCommand person)
        {
            var result = await _mediator.Send(person);
            return Ok(result);
        }

        [HttpGet("GetPersons")]
        public async Task<IActionResult> GetPersons(bool activeOnly)
        {
            var persons = await _mediator.Send(new GetPersonsQuery(activeOnly));
            return Ok(persons);
        }
        [HttpGet("GetPersonById")]
        public async Task<IActionResult> GetPersonById([FromQuery] GetPersonByIdQuery command)
        {
            var person = await _mediator.Send(new GetPersonByIdQuery(command.PersonId));
            return Ok(person);
        }

        [HttpDelete("DeletePerson")]
        public async Task<IActionResult> DeletePerson([FromQuery] DeletePersonCommand request)
        {
            await _mediator.Send(request);
            return NoContent();
        }
        [HttpGet("GetPersonByIdNumberAccountCount")]
        public async Task<IActionResult> GetPersonGetPersonByIdNumberAccountCountByIdNumber([FromQuery] GetPersonDtoByIdNumberQuery query)
        {
            var person = await _mediator.Send(query);
            return Ok(person);
        }
        
        [HttpGet("GetPersonByEmail")]
        public async Task<IActionResult> GetPersonByEmail([FromQuery] GetPersonByEmailQuery query)
        {
            var person = await _mediator.Send(query);
            return Ok(person);
        }

        [HttpPut]
        public async Task<IActionResult> EditPerson([FromBody] EditPersonCommand command)
        {
            var result = await _mediator.Send(command);
            if (result)
            {
                return Ok("Person updated successfully");
            }
            return BadRequest("Failed to update person");
        }

    }
}
