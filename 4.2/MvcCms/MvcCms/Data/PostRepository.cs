using MvcCms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcCms.Data
{
    public class PostRepository : IPostRepository
    {
        public Post Get(string id)
        {
            using (var db = new CmsContext())
            {
                return db.Posts.Include("Author")
                    .SingleOrDefault(post => post.Id == id);
            }
        }

        public void Edit(string id, Models.Post updatedItem)
        {
            using (var db = new CmsContext())
            {
                var post = db.Posts.SingleOrDefault(p => p.Id == id);

                if (post == null)
                {
                    throw new KeyNotFoundException("A post with the id of " 
                        + id + " does not exist in the data store.");
                }

                post.Id = updatedItem.Id;
                post.Title = updatedItem.Title;
                post.Content = updatedItem.Content;
                post.Published = updatedItem.Published;
                post.Tags = updatedItem.Tags;

                db.SaveChanges();
            }
        }

        public void Create(Post model)
        {
            using (var db = new CmsContext())
            {
                var post = db.Posts.SingleOrDefault(p => p.Id == model.Id);

                if (post != null)
                {
                    throw new ArgumentException("A post with the id of " + model.Id + " already exists.");
                }

                db.Posts.Attach(model);
                db.SaveChanges();
            }
        }

        public IEnumerable<Post> GetAll()
        {
            using (var db = new CmsContext())
            {
                return db.Posts.Include("Author")
                    .OrderByDescending(post => post.Created).ToArray();
            }
        }
    }
}