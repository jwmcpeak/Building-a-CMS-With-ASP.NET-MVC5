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

        public UserService(ModelStateDictionary modelState, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _modelState = modelState;
            _users = userRepository;
            _roles = roleRepository;
        }

        public async Task<UserViewModel> GetUserByNameAsync(string name)
        {
            var user = await _users.GetUserByNameAsync(name);

            if (user == null)
            {
                return null;
            }

            var viewModel = new UserViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                DisplayName = user.DisplayName
            };

            var userRoles = await _users.GetRolesForUserAsync(user);

            viewModel.SelectedRole = userRoles.Count() > 1 ?
                userRoles.FirstOrDefault() : userRoles.SingleOrDefault();

            viewModel.LoadUserRoles(await _roles.GetAllRolesAsync());

            return viewModel;
        }

        public async Task<bool> CreateAsync(UserViewModel model)
        {
            if (!_modelState.IsValid)
            {
                return false;
            }

            var existingUser = await _users.GetUserByNameAsync(model.UserName);

            if (existingUser != null)
            {
                _modelState.AddModelError(string.Empty, "The user already exists!");
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

            await _users.AddUserToRoleAsync(newUser, model.SelectedRole);

            return true;
        }

        public async Task<bool> UpdateUser(UserViewModel model)
        {
            var user = await _users.GetUserByNameAsync(model.UserName);

            if (user == null)
            {
                _modelState.AddModelError(string.Empty, "The specified user does not exist.");
                return false;
            }

            if (!_modelState.IsValid)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(model.CurrentPassword))
                {
                    _modelState.AddModelError(string.Empty, "The current password must be supplied");
                    return false;
                }

                var passwordVerified = _users.VerifyUserPassword(user.PasswordHash, model.CurrentPassword);

                if (!passwordVerified)
                {
                    _modelState.AddModelError(string.Empty, "The current password does match our records.");
                    return false;
                }

                var newHashedPassword = _users.HashPassword(model.NewPassword);

                user.PasswordHash = newHashedPassword;
            }

            user.Email = model.Email;
            user.DisplayName = model.DisplayName;

            await _users.UpdateAsync(user);

            var roles = await _users.GetRolesForUserAsync(user);

            await _users.RemoveUserFromRoleAsync(user, roles.ToArray());

            await _users.AddUserToRoleAsync(user, model.SelectedRole);

            return true;

        }

        public async Task DeleteAsync(string username)
        {
            var user = await _users.GetUserByNameAsync(username);

            if (user == null)
            {
                return;
            }

            await _users.DeleteAsync(user);
        }
    }
}