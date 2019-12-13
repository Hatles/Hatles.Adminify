using Abp.Authorization;
using Hatles.Adminify.Authorization.Roles;
using Hatles.Adminify.Authorization.Users;

namespace Hatles.Adminify.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
