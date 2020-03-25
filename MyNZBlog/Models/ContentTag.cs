using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNZBlog.Models
{
    public sealed class ContentTag
    {
        public ContentTag()
        {
            Articles = new HashSet<Article>();
        }
        public int Id { get; set; }
        public string Tag { get; set; }
        public ICollection<Article> Articles { get; set; }
    }
}
