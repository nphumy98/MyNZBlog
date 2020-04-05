 using System;
using System.Collections.Generic;
 using System.ComponentModel.DataAnnotations;
 using System.Linq;
using System.Threading.Tasks;
 using Microsoft.AspNetCore.Mvc.Formatters;

 namespace MyNZBlog.Models
{
    public class Article
    {
        public Article()
        {
            ArticleHasTags = new List<ArticleHasTag>();
            ReleaseDate = DateTime.Now;
        }
        public int Id { get; set; }
        public string Title { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public string Content { get; set; }
        public IList<ArticleHasTag> ArticleHasTags { get; set; }
    }
}
