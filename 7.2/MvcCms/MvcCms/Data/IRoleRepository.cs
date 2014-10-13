using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;

namespace MvcCms.Data
{
    public interface IRoleRepository : IDisposable
    {
        Task<IdentityRole> GetRoleByNameAsync(string name);
        Task<IEnumerable<IdentityRole>> GetAllRolesAsync();
        Task CreateAsync(IdentityRole role);
    }
}