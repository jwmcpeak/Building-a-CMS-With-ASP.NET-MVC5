using Microsoft.AspNet.Identity;
using MvcCms.Areas.Admin.Services;
using MvcCms.Areas.Admin.ViewModels;
using MvcCms.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MvcCms.Areas.Admin.Controllers
{
    [RouteArea("admin")]
    [RoutePrefix("user")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly UserService _users;

        public UserController()
        {
            _userRepository = new UserRepository();
            _roleRepository = new RoleRepository();
            _users = new UserService(ModelState, _userRepository, _roleRepository);
        }

        // GET: Admin/User
        [Route("")]
        public ActionResult Index()
        {
            using (var manager = new CmsUserManager())
            {
                var users = manager.Users.ToList();

                return View(users);
            }
        }

        [Route("create")]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [Route("create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserViewModel model)
        {
            if (await _users.Create(model))
            {
                return RedirectToAction("index");
            }
            else
            {
                return View(model);
            }
        }

        [Route("edit/{username}")]
        [HttpGet]
        public ActionResult Edit(string username)
        {
            using (var userStore = new CmsUserStore())
            using (var userManager = new CmsUserManager(userStore))
            {
                var user = userStore.FindByNameAsync(username).Result;

                if (user == null)
                {
                    return HttpNotFound();
                }

                var viewModel = new UserViewModel
                {
                    UserName = user.UserName,
                    Email = user.Email
                };

                return View(viewModel);
            }
        }

        [Route("edit/{username}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserViewModel model)
        {
            using (var userStore = new CmsUserStore())
            using (var userManager = new CmsUserManager(userStore))
            {
                var user = userStore.FindByNameAsync(model.UserName).Result;

                if (user == null)
                {
                    return HttpNotFound();
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (!string.IsNullOrWhiteSpace(model.NewPassword))
                {
                    if (string.IsNullOrWhiteSpace(model.CurrentPassword))
                    {
                        ModelState.AddModelError(string.Empty, "The current password must be supplied");
                        return View(model);
                    }

                    var passwordVerified = userManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, model.CurrentPassword);

                    if (passwordVerified != PasswordVerificationResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "The current password does match our records.");
                        return View(model);
                    }

                    var newHashedPassword = userManager.PasswordHasher.HashPassword(model.NewPassword);

                    user.PasswordHash = newHashedPassword;
                }

                user.Email = model.Email;
                user.DisplayName = model.DisplayName;

                var updateResult = userManager.UpdateAsync(user).Result;

                if (updateResult.Succeeded)
                {
                    return RedirectToAction("index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred. Please try again.");
                    return View(model);
                }

                
            }
        }

        [Route("delete/{username}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string username)
        {
            using (var userStore = new CmsUserStore())
            using (var userManager = new CmsUserManager(userStore))
            {
                var user = userStore.FindByNameAsync(username).Result;

                if (user == null)
                {
                    return HttpNotFound();
                }

                var deleteResult = userManager.DeleteAsync(user).Result;

                return RedirectToAction("index");


            }
        }

        private bool _isDisposed;
        protected override void Dispose(bool disposing)
        {

            if (!_isDisposed)
            {
                _userRepository.Dispose();
                _roleRepository.Dispose();
            }
            _isDisposed = true;

            base.Dispose(disposing);
        }
    }
}