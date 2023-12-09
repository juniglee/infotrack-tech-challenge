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
using WebApplication.Infrastructure.Services;

namespace WebApplication.Core.Users.Commands
{
    public class UpdateUserCommand : IRequest<UserDto>
    {
        public int Id { get; set; }
        public string GivenNames { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;

        public class Validator : AbstractValidator<UpdateUserCommand>
        {
            public Validator()
            {
                // TODO: Create validation rules for UpdateUserCommand so that all properties are required.
                // If you are feeling ambitious, also create a validation rule that ensures the user exists in the database.
                RuleFor(x => x.Id)
                    .GreaterThan(0);

                RuleFor(x => x.GivenNames)
                    .NotEmpty();

                RuleFor(x => x.LastName)
                    .NotEmpty();

                RuleFor(x => x.EmailAddress)
                    .NotEmpty();

                RuleFor(x => x.MobileNumber)
                    .NotEmpty();
            }
        }

        public class Handler : IRequestHandler<UpdateUserCommand, UserDto>
        {
            private readonly IUserService _userService;
            private readonly IMapper _mapper;

            public Handler(IUserService userService, IMapper mapper)
            {
                _userService = userService;
                _mapper = mapper;
            }

            /// <inheritdoc />
            public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
            {
                User? user = await _userService.GetAsync(request.Id, cancellationToken);
                if (user is default(User)) { 
                    throw new NotFoundException($"The user '{request.Id}' could not be found."); 
                }
                else
                {
                    user.GivenNames = request.GivenNames;
                    user.LastName = request.LastName;
                    user.ContactDetail = new ContactDetail
                    {
                        EmailAddress = request.EmailAddress,
                        MobileNumber = request.MobileNumber
                    };

                    await _userService.UpdateAsync(user);

                    UserDto result = _mapper.Map<UserDto>(user);

                    return result;
                }
            }
        }
    }
}
