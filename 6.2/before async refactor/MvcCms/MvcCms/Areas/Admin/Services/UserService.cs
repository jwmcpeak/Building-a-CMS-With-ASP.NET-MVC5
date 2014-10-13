using MvcCms.Areas.Admin.ViewModels;
using MvcCms.Data;
using MvcCms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MvcCms.Areas.Admin.Services
{
    public class UserService
    {
        private readonly IUserRepository _users;
        private readonly IRoleRepository _roles;
        private readonly ModelStateDictionary _modelState;

        public UserService(ModelStateDictionary modelState, 
            IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _modelState = modelState;
            _users = userRepository;
            _roles = roleRepository;
        }

        public async Task<bool> Create(UserViewModel model)
        {
            if (!_modelState.IsValid)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                _modelState.AddModelError(string.Empty, "You must type a password.");
                return false;
            }

            var newUser = new CmsUser
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.UserName
            };

            await _users.CreateAsync(newUser, model.NewPassword);

            return true;
        }
    }
}