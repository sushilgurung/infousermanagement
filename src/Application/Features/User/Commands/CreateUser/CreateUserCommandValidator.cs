using Application.Interfaces.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.User.Commands.CreateUser
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        private IUserRepository _userRepository;
        public CreateUserCommandValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            RuleFor(v => v.ForeName).NotEmpty().WithMessage("ForName is Required");

            RuleFor(v => v.SurName).NotEmpty().WithMessage("ForName is Required");

            RuleFor(v => v.Email).NotEmpty().WithMessage("ForName is Required")
                .EmailAddress().WithMessage("Invalid Email Address")
                 .MustAsync(HasUniqueEmailAsync).WithMessage("Email already exists.");

            RuleFor(v => v.DateOfBirth)
            .NotEmpty().WithMessage("Date of Birth is required.")
            .MustAsync(ValidDateAsync).WithMessage($"Birthdate must be between {DateTime.Today.AddYears(-100):yyyy-MM-dd} and {DateTime.Today:yyyy-MM-dd}.");

        }

        private async Task<bool> HasUniqueEmailAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Queryable.AsNoTracking().AnyAsync(x => x.Email.ToUpper() == email.ToUpper()).ConfigureAwait(false);
            return !user;
        }

        private async Task<bool> ValidDateAsync(DateOnly date, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var hundredYearsAgo = today.AddYears(-100);

            return await Task.FromResult(date >= hundredYearsAgo && date <= today);
        }
    }
}
