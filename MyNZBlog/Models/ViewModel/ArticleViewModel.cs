using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNZBlog.Models.ViewModel
{
    public class ArticleViewModel
    {
        public List<Article> articles { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public int size { get; set; }
    }
}
