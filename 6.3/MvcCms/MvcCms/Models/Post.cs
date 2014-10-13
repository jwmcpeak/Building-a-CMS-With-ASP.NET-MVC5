using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MvcCms.Models
{
    public class Post
    {
        private IList<string> _tags = new List<string>();

        [Display(Name = "Slug")]
        public string Id { get; set; }

        [Display(Name = "Title")]
        [Required]
        public string Title { get; set; }

        [Display(Name = "Post Content")]
        [Required]
        public string Content { get; set; }

        [Display(Name = "Date Created")]
        public DateTime Created { get; set; }

        [Display(Name = "Date Published")]
        public DateTime? Published { get; set; }

        public IList<string> Tags
        {
            get { return _tags; }
            set { _tags = value; }
        }

        public string CombinedTags
        {
            get { return string.Join(",", _tags); }
            set
            {
                _tags = value.Split(',').Select(s => s.Trim()).ToList();
            }
        }

        public string AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public virtual CmsUser Author { get; set; }
    }
}