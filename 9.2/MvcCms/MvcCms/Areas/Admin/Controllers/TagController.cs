using MvcCms.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcCms.Areas.Admin.Controllers
{
    [RouteArea("admin")]
    [RoutePrefix("tag")]
    [Authorize]
    public class TagController : Controller
    {
        private readonly ITagRepository _repository;

        public TagController() : this(new TagRepository()) { }

        public TagController(ITagRepository repository)
        {
            _repository = repository;
        }
        
        // GET: Admin/Tag
        [Route("")]
        public ActionResult Index()
        {
            var tags = _repository.GetAll();

            if (Request.AcceptTypes.Contains("application/json"))
            {
                return Json(tags, JsonRequestBehavior.AllowGet);
            }

            if (User.IsInRole("author"))
            {
                return new HttpUnauthorizedResult();
            }

            return View(tags);
        }

        [HttpGet]
        [Route("edit/{tag}")]
        [Authorize(Roles = "admin, editor")]
        public ActionResult Edit(string tag)
        {
            try
            {
                var model = _repository.Get(tag);
                return View(model: model);
            }
            catch (KeyNotFoundException e)
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit/{tag}")]
        [Authorize(Roles = "admin, editor")]
        public ActionResult Edit(string tag, string newTag)
        {
            var tags = _repository.GetAll();

            if (!tags.Contains(tag))
            {
                return HttpNotFound();
            }

            if (tags.Contains(newTag))
            {
                return RedirectToAction("index");
            }

            if (string.IsNullOrWhiteSpace(newTag))
            {
                ModelState.AddModelError("key", "New tag value cannot be empty.");

                return View(model: tag);
            }

            _repository.Edit(tag, newTag);

            return RedirectToAction("index");
        }

        [HttpGet]
        [Route("delete/{tag}")]
        [Authorize(Roles = "admin, editor")]
        public ActionResult Delete(string tag)
        {
            try
            {
                var model = _repository.Get(tag);
                return View(model: model);
            }
            catch (KeyNotFoundException e)
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete/{tag}")]
        [Authorize(Roles = "admin, editor")]
        public ActionResult Delete(string tag, string foo)
        {
            try
            {
                _repository.Delete(tag);

                return RedirectToAction("index");
            }
            catch (KeyNotFoundException e)
            {
                return HttpNotFound();
            }
        }
    }
}