using System;
using System.Collections;
using System.Collections.Generic;
using MvcCms.Models;
using System.Threading.Tasks;
using System.Security.Claims;

namespace MvcCms.Data
{
    public interface IUserRepository : IDisposable
    {

        Task<CmsUser> GetUserByNameAsync(string username);
        Task<IEnumerable<CmsUser>> GetAllUsersAsync();
        Task CreateAsync(CmsUser user, string password);
        Task DeleteAsync(CmsUser user);
        Task UpdateAsync(CmsUser user);
        bool VerifyUserPassword(string hashedPassword, string providedPassword);
        string HashPassword(string password);

        Task AddUserToRoleAsync(CmsUser newUser, string p);

        Task<IEnumerable<string>> GetRolesForUserAsync(CmsUser user);

        Task RemoveUserFromRoleAsync(CmsUser user, params string[] roleNames);

        Task<CmsUser> GetLoginUserAsync(string username, string password);

        Task<ClaimsIdentity> CreateIdentityAsync(CmsUser user);
    }
}