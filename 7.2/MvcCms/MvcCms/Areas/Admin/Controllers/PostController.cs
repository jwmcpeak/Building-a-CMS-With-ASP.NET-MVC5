using MvcCms.Data;
using MvcCms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MvcCms.Areas.Admin.Controllers
{
    // /admin/post

    [RouteArea("Admin")]
    [RoutePrefix("post")]
    [Authorize]
    public class PostController : Controller
    {
        private readonly IPostRepository _repository;
        private readonly IUserRepository _users;


        public PostController()
            : this(new PostRepository(), new UserRepository()) { }

        public PostController(IPostRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _users = userRepository;
        }

        // GET: Admin/Post
        [Route("")]
        public async Task<ActionResult> Index()
        {
            if (!User.IsInRole("author"))
            {
                return View(await _repository.GetAllAsync());
            }

            var user = await GetLoggedInUser();
            var posts = await _repository.GetPostsByAuthorAsync(user.Id);

            return View(posts);
        }

        // /admin/post/create/
        [HttpGet]
        [Route("create")]
        public ActionResult Create()
        {
            return View(new Post());
        }

        // /admin/post/create/
        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Post model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetLoggedInUser();

            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = model.Title;
            }

            model.Id = model.Id.MakeUrlFriendly();
            model.Tags = model.Tags.Select(tag => tag.MakeUrlFriendly()).ToList();

            model.Created = DateTime.Now;

            model.AuthorId = user.Id;

            try
            {
                _repository.Create(model);

                return RedirectToAction("index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("key", e);
                return View(model);
            }
        }

        // /admin/post/edit/post-to-edit
        [HttpGet]
        [Route("edit/{postId}")]
        public async Task<ActionResult> Edit(string postId)
        {
            var post = _repository.Get(postId);

            if (post == null)
            {
                return HttpNotFound();
            }

            if (User.IsInRole("author"))
            {
                var user = await GetLoggedInUser();

                if (post.AuthorId != user.Id)
                {
                    return new HttpUnauthorizedResult();
                }
            }

            return View(post);
        }

        // /admin/post/edit/post-to-edit
        [HttpPost]
        [Route("edit/{postId}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string postId, Post model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (User.IsInRole("author"))
            {
                var user = await GetLoggedInUser();
                var post = _repository.Get(postId);

                try
                {
                    if (post.AuthorId != user.Id)
                    {
                        return new HttpUnauthorizedResult();
                    }
                }
                catch { }
            }

            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = model.Title;
            }

            model.Id = model.Id.MakeUrlFriendly();
            model.Tags = model.Tags.Select(tag => tag.MakeUrlFriendly()).ToList();

            try
            {
                _repository.Edit(postId, model);

                return RedirectToAction("index");
            }
            catch (KeyNotFoundException e)
            {
                return HttpNotFound();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }

        }

        // /admin/post/delete/post-to-edit
        [HttpGet]
        [Route("delete/{postId}")]
        [Authorize(Roles="admin, editor")]
        public ActionResult Delete(string postId)
        {
            var post = _repository.Get(postId);

            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        // /admin/post/delete/post-to-edit
        [HttpPost]
        [Route("delete/{postId}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, editor")]
        public ActionResult Delete(string postId, string foo)
        {
            try
            {
                _repository.Delete(postId);

                return RedirectToAction("index");
            }
            catch (KeyNotFoundException e)
            {
                return HttpNotFound();
            }
        }

        private async Task<CmsUser> GetLoggedInUser()
        {
            return await _users.GetUserByNameAsync(User.Identity.Name);
        }

        private bool _isDisposed;
        protected override void Dispose(bool disposing)
        {

            if (!_isDisposed)
            {
                _users.Dispose();
            }
            _isDisposed = true;

            base.Dispose(disposing);
        }
    }
}