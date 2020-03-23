using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNZBlog.Models
{
    public class ContentTag
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public IList<ArticleHasTag> ArticleHasTags { get; set; }
    }
}
