using Application.Features.User.Command.UpdateUser;
using Application.Features.User.Commands.CreateUser;
using Application.Interfaces.Repositories;
using Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.User.Commands.UpdateUser
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        private IUserRepository _userRepository;
        public UpdateUserCommandValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            RuleFor(v => v.ForeName).NotEmpty().WithMessage("ForName is Required");

            RuleFor(v => v.SurName).NotEmpty().WithMessage("ForName is Required");

            RuleFor(v => v.Email).NotEmpty().WithMessage("ForName is Required")
                .EmailAddress().WithMessage("Invalid Email Address")
                 .MustAsync(
                async (model, UserId, cancellation) =>
                 {
                     return await HasUniqueEmailAsync(model.Email, model.UserId, cancellation);
                 }).WithMessage("Email already exists.");

            RuleFor(v => v.DateOfBirth)
            .NotEmpty().WithMessage("Date of Birth is required.")
            .MustAsync(ValidDateAsync).WithMessage($"Birthdate must be between {DateTime.Today.AddYears(-100):yyyy-MM-dd} and {DateTime.Today:yyyy-MM-dd}.");
        }
        /// <summary>
        /// This method checks if the email is unique in the database.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<bool> HasUniqueEmailAsync(string email, int id, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Queryable
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email.ToUpper() == email.ToUpper())
                .ConfigureAwait(false);

            if (user is null) return true;

            if (user.Id == id && string.Equals(email, user.Email, StringComparison.InvariantCultureIgnoreCase))
                return true;

            return false;
        }
        /// <summary>
        /// This method checks if the date is valid.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<bool> ValidDateAsync(DateOnly date, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var hundredYearsAgo = today.AddYears(-100);

            return await Task.FromResult(date >= hundredYearsAgo && date <= today);
        }
    }
}
