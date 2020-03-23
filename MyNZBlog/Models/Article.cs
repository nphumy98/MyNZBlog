 using System;
using System.Collections.Generic;
 using System.ComponentModel.DataAnnotations;
 using System.Linq;
using System.Threading.Tasks;

namespace MyNZBlog.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public string Content { get; set; }
        public IList<ArticleHasTag> ArticleHasTags { get; set; }
    }
}
