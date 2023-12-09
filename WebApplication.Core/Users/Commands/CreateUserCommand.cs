using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using WebApplication.Core.Common.Exceptions;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Core.Users.Commands
{
    public class CreateUserCommand : IRequest<UserDto>
    {
        public string GivenNames { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;

        public class Validator : AbstractValidator<CreateUserCommand>
        {
            public Validator()
            {
                RuleFor(x => x.GivenNames)
                    .NotEmpty()
                    .WithMessage($"Given Names cannot be empty.");

                RuleFor(x => x.LastName)
                    .NotEmpty()
                    .WithMessage($"Last Name cannot be empty.");

                RuleFor(x => x.EmailAddress)
                    .NotEmpty()
                    .WithMessage($"Email Address cannot be empty.");

                RuleFor(x => x.MobileNumber)
                    .NotEmpty()
                    .WithMessage($"Mobile Number cannot be empty.");
            }
        }

        public class Handler : IRequestHandler<CreateUserCommand, UserDto>
        {
            private readonly IUserService _userService;
            private readonly IMapper _mapper;

            public Handler(IUserService userService, IMapper mapper)
            {
                _userService = userService;
                _mapper = mapper;
            }

            /// <inheritdoc />
            public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
            {
                Validator validator = new Validator();
                ValidationResult validationResult = validator.Validate(request);
                if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

                User user = new User
                {
                    GivenNames = request.GivenNames,
                    LastName = request.LastName,
                    ContactDetail = new ContactDetail
                    {
                        EmailAddress = request.EmailAddress,
                        MobileNumber = request.MobileNumber
                    }
                };

                User addedUser = await _userService.AddAsync(user, cancellationToken);
                UserDto result = _mapper.Map<UserDto>(addedUser);

                return result;
            }
        }
    }
}
