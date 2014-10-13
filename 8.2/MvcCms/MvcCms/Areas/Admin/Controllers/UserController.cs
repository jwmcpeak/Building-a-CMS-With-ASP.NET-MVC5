using MvcCms.Areas.Admin.Services;
using MvcCms.Areas.Admin.ViewModels;
using MvcCms.Data;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MvcCms.Areas.Admin.Controllers
{
    [RouteArea("admin")]
    [RoutePrefix("user")]
    [Authorize]
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
        [Authorize(Roles="admin")]
        public async Task<ActionResult> Index()
        {
            var users = await _userRepository.GetAllUsersAsync();

            return View(users);
        }

        [Route("create")]
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Create()
        {
            var model = new UserViewModel();
            model.LoadUserRoles(await _roleRepository.GetAllRolesAsync());

            return View(model);
        }

        [Route("create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Create(UserViewModel model)
        {
            var completed = await _users.CreateAsync(model);

            if (completed)
            {
                return RedirectToAction("index");
            }

            return View(model);
        }

        [Route("edit/{username}")]
        [HttpGet]
        [Authorize(Roles = "admin, editor, author")]
        public async Task<ActionResult> Edit(string username)
        {
            var currentUser = User.Identity.Name;

            if (!User.IsInRole("admin") &&
                !string.Equals(currentUser, username, StringComparison.CurrentCultureIgnoreCase))
            {
                return new HttpUnauthorizedResult();
            }

            var user = await _users.GetUserByNameAsync(username);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [Route("edit/{username}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, editor, author")]
        public async Task<ActionResult> Edit(UserViewModel model, string username)
        {
            var currentUser = User.Identity.Name;
            var isAdmin = User.IsInRole("admin");

            if (!isAdmin &&
                !string.Equals(currentUser, username, StringComparison.CurrentCultureIgnoreCase))
            {
                return new HttpUnauthorizedResult();
            }

            var userUpdated = await _users.UpdateUser(model);

            if (userUpdated)
            {
                if (isAdmin)
                {
                    return RedirectToAction("index");
                }

                return RedirectToAction("index", "admin");
            }

            return View(model);
        }

        [Route("delete/{username}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Delete(string username)
        {
            await _users.DeleteAsync(username);

            return RedirectToAction("index");
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