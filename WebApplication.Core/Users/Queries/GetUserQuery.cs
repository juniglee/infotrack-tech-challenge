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

namespace WebApplication.Core.Users.Queries
{
    public class GetUserQuery : IRequest<UserDto>
    {
        public int Id { get; set; }

        public class Validator : AbstractValidator<GetUserQuery>
        {
            public Validator(IUserService userService)
            {
                RuleFor(x => x.Id)
                .GreaterThan(0);
            }
        }

        public class Handler : IRequestHandler<GetUserQuery, UserDto>
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
            public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
            {
                string userIdCouldNotBeFound = $"The user '{request.Id}' could not be found.";
                User? user = await _userService.GetAsync(request.Id, cancellationToken);

                if (user is default(User)) 
                {
                    logger.Error(userIdCouldNotBeFound);
                    throw new NotFoundException(userIdCouldNotBeFound); 
                }

                UserDto result = _mapper.Map<UserDto>(user);

                return result;
            }
        }
    }
}
