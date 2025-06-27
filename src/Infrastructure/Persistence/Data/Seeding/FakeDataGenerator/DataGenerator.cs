using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Data.Seeding.FakeDataGenerator
{
    public class DataGenerator
    {
        /// <summary>
        /// This method generates fake data for UserManagement entities using Bogus library.
        /// </summary>
        /// <returns></returns>
        public static Faker<User> GenerateUserManagements()
        {
            return new Faker<User>()
         .RuleFor(u => u.Id, f => f.IndexFaker + 1)
         .RuleFor(u => u.ForeName, f => f.Name.FirstName())
         .RuleFor(u => u.SurName, f => f.Name.LastName())
         .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.ForeName, u.SurName))
         .RuleFor(u => u.DateOfBirth, f =>
         {
             var dob = f.Date.Past(40, DateTime.Today.AddYears(-18)); // Between 18-58 years old
             return DateOnly.FromDateTime(dob);
         })
         .RuleFor(u => u.IsActive, f => f.Random.Bool());
        }

    }
}
