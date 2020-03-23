using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNZBlog.Models
{
    public class ArticleHasTag
    {
        public int ArticleId { get; set; }
        public Article Article { get; set; }
        public int ContentTagId { get; set; }
        public ContentTag ContentTag { get; set; }
    }
}
