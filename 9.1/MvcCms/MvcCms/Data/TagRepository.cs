using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcCms.Data
{
    public class TagRepository : ITagRepository
    {
        public IEnumerable<string> GetAll()
        {
            using (var db = new CmsContext())
            {
                var tagsCollection = db.Posts.Select(p => p.CombinedTags).ToList();
                return string.Join(",", tagsCollection).Split(',').Distinct();
                
                //return db.Posts.ToList().SelectMany(post => post.Tags).Distinct();
            }
        }

        public string Get(string tag)
        {
            using (var db = new CmsContext())
            {
                var posts = db.Posts.Where(post => post.CombinedTags.Contains(tag)).ToList();

                posts = posts.Where(post =>
                    post.Tags.Contains(tag, StringComparer.CurrentCultureIgnoreCase))
                    .ToList();

                if (!posts.Any())
                {
                    throw new KeyNotFoundException("The tag " + tag + " does not exist.");
                }

                return tag.ToLower();
            }
        }

        public void Edit(string existingTag, string newTag)
        {
            using (var db = new CmsContext())
            {
                var posts = db.Posts.Where(post => post.CombinedTags.Contains(existingTag))
                    .ToList();

                posts = posts.Where(post => 
                    post.Tags.Contains(existingTag, StringComparer.CurrentCultureIgnoreCase))
                    .ToList();

                if (!posts.Any())
                {
                    throw new KeyNotFoundException("The tag " + existingTag + " does not exist.");
                }

                foreach (var post in posts)
                {
                    post.Tags.Remove(existingTag);
                    post.Tags.Add(newTag);
                }

                db.SaveChanges();
            }
        }

        public void Delete(string tag)
        {
            using (var db = new CmsContext())
            {
                var posts = db.Posts.Where(post => post.CombinedTags.Contains(tag))
                    .ToList();

                posts = posts.Where(post =>
                    post.Tags.Contains(tag, StringComparer.CurrentCultureIgnoreCase))
                    .ToList();

                if (!posts.Any())
                {
                    throw new KeyNotFoundException("The tag " + tag + " does not exist.");
                }

                foreach (var post in posts)
                {
                    post.Tags.Remove(tag);
                }

                db.SaveChanges();
            }
        }
    }
}