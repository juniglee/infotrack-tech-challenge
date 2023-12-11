using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using NLog;
using WebApplication.Core.Common.Exceptions;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Core.Users.Commands
{
    public class DeleteUserCommand : IRequest<UserDto>
    {
        public int Id { get; set; }

        public class Validator : AbstractValidator<DeleteUserCommand>
        {
            public Validator()
            {
                RuleFor(x => x.Id)
                    .GreaterThan(0);
            }
        }

        public class Handler : IRequestHandler<DeleteUserCommand, UserDto>
        {
            private Logger logger = LogManager.GetCurrentClassLogger();
            private readonly IUserService _userService;
            private readonly IMapper _mapper;

            public Handler(IUserService userService, IMapper mapper)
            {
                _userService = userService;
                _mapper = mapper;
            }

            /// <inheritdoc />
            public async Task<UserDto> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
            {
                string userIdCouldNotBeFound = $"The user '{request.Id}' could not be found.";
                User? deletedUser = await _userService.DeleteAsync(request.Id, cancellationToken);

                if (deletedUser is default(User))
                {
                    logger.Error(userIdCouldNotBeFound);
                    throw new NotFoundException(userIdCouldNotBeFound);
                }

                return _mapper.Map<UserDto>(deletedUser);
            }
        }
    }
}
