
namespace Domain.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base()
        {
        }
        public ApplicationRole(string roleName)
        {
            this.Name = roleName;
        }
    }
}
