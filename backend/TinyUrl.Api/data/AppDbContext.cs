using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TinyUrl.Api.models;

namespace TinyUrl.Api.data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<ShortUrl> ShortUrls => Set<ShortUrl>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShortUrl>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.OriginalUrl)
                    .IsRequired()
                    .HasMaxLength(2048);

                entity.Property(x => x.ShortCode)
                    .IsRequired()
                    .HasMaxLength(6);

                entity.HasIndex(x => x.ShortCode)
                    .IsUnique();
            });
        }
    }
}
