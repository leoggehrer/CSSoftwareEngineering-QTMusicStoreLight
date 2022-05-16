//@CodeCopy
//MdStart
#if ACCOUNT_ON
using QTMusicStoreLight.Logic.Entities.Account;

namespace QTMusicStoreLight.Logic.Controllers.Account
{
    [Modules.Security.Authorize("SysAdmin", "AppAdmin")]
    internal sealed partial class IdentityXRolesController : GenericController<Entities.Account.IdentityXRole>
    {
        public IdentityXRolesController()
        {
        }

        public IdentityXRolesController(ControllerObject other) : base(other)
        {
        }
        public async Task<Role[]> QueryIdentityRolesAsync(int identityId)
        {
            var result = new List<Role>();
            var roles = await EntitySet.Where(e => e.IdentityId == identityId)
                                       .Include(e => e.Role)
                                       .Select(e => e.Role)
                                       .ToArrayAsync();
            foreach (var role in roles)
            {
                if (role != null)
                    result.Add(role);
            }
            return result.ToArray();
        }
    }
}
#endif
//MdEnd
