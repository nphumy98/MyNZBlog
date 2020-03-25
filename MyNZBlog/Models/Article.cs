 using System;
using System.Collections.Generic;
 using System.ComponentModel.DataAnnotations;
 using System.Linq;
using System.Threading.Tasks;

namespace MyNZBlog.Models
{
    public sealed class Article
    {
        public Article()
        {
            ContentTags = new HashSet<ContentTag>();
        }

        public int Id { get; set; }
        public string Title { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public string Content { get; set; }
        public ICollection<ContentTag> ContentTags { get; set; }
    }
}
