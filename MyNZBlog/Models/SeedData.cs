using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyNZBlog.Data;

namespace MyNZBlog.Models
{
    public class SeedData
    {
        public static void InitializeContentTag(IServiceProvider serviceProvider)
        {
            Console.WriteLine("method called 1");
            using (var context = new MyNZBlogContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MyNZBlogContext>>()))
            {
                // Look for any movies.
                if (context.ContentTags.Any())
                {
                    return;   // DB has been seeded
                }

                context.ContentTags.AddRange(
                    new ContentTag
                    {
                        Tag = "NewZealand Life",
                    },

                    new ContentTag
                    {
                        Tag = "IT career",
                    },

                    new ContentTag
                    {
                        Tag = "Love",
                    },

                    new ContentTag
                    {
                        Tag = "Politics",
                    }
                );
                context.SaveChanges();
            }
        }

        public static void InitializeArticle(IServiceProvider serviceProvider)
        {
            Console.WriteLine("method called 2");
            using (var context = new MyNZBlogContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MyNZBlogContext>>()))
            {
                // Look for any movies.
                if (context.Articles.Any())
                {
                    return;   // DB has been seeded
                }

                context.Articles.AddRange(
                    new Article
                    {
                        Title = "When Harry Met Sally",
                        ReleaseDate = DateTime.Parse("1989-2-12"),
                        Content = "Just test When Harry Met Sally"
                    },

                    new Article
                    {
                        Title = "Ghostbusters ",
                        ReleaseDate = DateTime.Parse("1984-3-13"),
                        Content = "Just test Ghostbusters"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
