using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcCms.Data
{
    public class PostRepository : IPostRepository
    {
        public Models.Post Get(string id)
        {
            throw new NotImplementedException();
        }

        public void Edit(string id, Models.Post updatedItem)
        {
            throw new NotImplementedException();
        }

        public void Create(Models.Post model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Models.Post> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}