using MvcCms.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MvcCms.Controllers
{
    [RoutePrefix("")]
    public class HomeController : Controller
    {
        private readonly IPostRepository _posts;

        public HomeController() : this(new PostRepository()) { }

        public HomeController(IPostRepository postRepository)
        {
            _posts = postRepository;
        }

        // GET: Default
        // root/
        [Route("")]
        public async Task<ActionResult> Index()
        {
            var posts = await _posts.GetPublishedPostsAsync();
            return View(posts);
        }

        // root/posts/post-id
        [Route("posts/{postId}")]
        public async Task<ActionResult> Post(string postId)
        {
            var post = _posts.Get(postId);

            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        // root/tags/tag-id
        [Route("tags/{tagId}")]
        public async Task<ActionResult> Tag(string tagId)
        {
            var posts = await _posts.GetPostsByTagAsync(tagId);

            if (!posts.Any())
            {
                return HttpNotFound();
            }

            ViewBag.Tag = tagId;

            return View(posts);
        }
    }
}