using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyNZBlog.Models;

namespace MyNZBlog.Data
{
    public class MyNZBlogContext : IdentityDbContext
    {
        //https://blog.stevensanderson.com/2011/01/28/mvcscaffolding-one-to-many-relationships/
        public MyNZBlogContext(DbContextOptions<MyNZBlogContext> options)
            : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<ContentTag> ContentTags { get; set; }
        public DbSet<ArticleHasTag> ArticleHasTags { get; set; }
        public DbSet<User> User { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);// fix the error The entity type 'IdentityUserLogin<string>' requires a primary key to be defined. If you intended to use a keyless entity type call 'HasNoKey()'.
            modelBuilder.Entity<ArticleHasTag>().HasKey(sc => new {articleId = sc.ArticleId, tagId = sc.ContentTagId});

            modelBuilder.Entity<ArticleHasTag>()
                .HasOne<Article>(sc => sc.Article)
                .WithMany(s => s.ArticleHasTags)
                .HasForeignKey(sc => sc.ArticleId);


            modelBuilder.Entity<ArticleHasTag>()
                .HasOne<ContentTag>(sc => sc.ContentTag)
                .WithMany(s => s.ArticleHasTags)
                .HasForeignKey(sc => sc.ContentTagId);
        }
    }
}
