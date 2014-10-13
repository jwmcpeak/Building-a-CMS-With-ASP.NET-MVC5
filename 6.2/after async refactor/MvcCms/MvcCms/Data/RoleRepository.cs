using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MvcCms.Models;
using System.Threading.Tasks;

namespace MvcCms.Data
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleStore<IdentityRole> _store;
        private readonly RoleManager<IdentityRole> _manager;

        public RoleRepository()
        {
            _store = new RoleStore<IdentityRole>();
            _manager = new RoleManager<IdentityRole>(_store);
        }

        public async Task<IdentityRole> GetRoleByNameAsync(string name)
        {
            return await _store.FindByNameAsync(name);
        }

        public async Task<IEnumerable<IdentityRole>> GetAllRolesAsync()
        {
            return await _store.Roles.ToArrayAsync();
        }

        public async Task CreateAsync(IdentityRole role)
        {
            await _manager.CreateAsync(role);
        }

        private bool _disposed = false;
        public void Dispose()
        {
            if (!_disposed)
            {
                _store.Dispose();
                _manager.Dispose();
            }

            _disposed = true;
        }
    }
}