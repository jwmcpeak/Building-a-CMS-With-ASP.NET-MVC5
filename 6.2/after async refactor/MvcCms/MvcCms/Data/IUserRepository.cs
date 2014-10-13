using System;
using System.Collections;
using System.Collections.Generic;
using MvcCms.Models;
using System.Threading.Tasks;

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
    }
}