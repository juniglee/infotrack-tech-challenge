using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Core.Common.Models;
using WebApplication.Core.Users.Commands;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Core.Users.Queries;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserAsync(
            [FromQuery] GetUserQuery query,
            CancellationToken cancellationToken)
        {
            UserDto result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        // TODO: create a route (at /Find) that can retrieve a list of matching users using the `FindUsersQuery`

        [HttpGet]
        [Route("Find/{id?}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> FindUsersAsync(
            [FromQuery] FindUsersQuery query,
            CancellationToken cancellationToken)
        {
            IEnumerable<UserDto> result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        // TODO: create a route (at /List) that can retrieve a paginated list of users using the `ListUsersQuery`

        [HttpGet]
        [Route("/List")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListUsersAsync(
            [FromQuery] ListUsersQuery query,
            CancellationToken cancellationToken)
        {
            PaginatedDto<IEnumerable<UserDto>> result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        // TODO: create a route that can create a user using the `CreateUserCommand`
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateUserAsync(
            [FromBody] CreateUserCommand query,
            CancellationToken cancellationToken)
        {
            UserDto result = await _mediator.Send(query, cancellationToken);
            var parameters = new { query = new GetUserQuery { Id = result.UserId }, cancellationToken };

            return CreatedAtAction(nameof(GetUserAsync), new {id = result.UserId}, result);
        }

        // TODO: create a route that can update an existing user using the `UpdateUserCommand`
        [HttpPut]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> PutUserAsync(
            [FromBody] UpdateUserCommand query,
            CancellationToken cancellationToken)
        {
            UserDto result = await _mediator.Send(query, cancellationToken);
            //return Ok(result);
            return CreatedAtAction(nameof(GetUserAsync), new { id = result.UserId }, result);
        }

        // TODO: create a route that can delete an existing user using the `DeleteUserCommand`
        [HttpDelete]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUserAsync(
            [FromQuery] DeleteUserCommand query,
            CancellationToken cancellationToken)
        {
            UserDto result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
