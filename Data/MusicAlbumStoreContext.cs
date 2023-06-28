using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicAlbumStore.Areas.Identity.Data;
using MusicAlbumStore.Models;

namespace MusicAlbumStore.Data
{
    public class MusicAlbumStoreContext : IdentityDbContext<MusicAlbumStoreUser>/*DbContext*/
    {
        public MusicAlbumStoreContext (DbContextOptions<MusicAlbumStoreContext> options)
            : base(options)
        {
        }

        public DbSet<MusicAlbumStore.Models.Artist> Artist { get; set; } = default!;

        public DbSet<MusicAlbumStore.Models.Genre>? Genre { get; set; }

        public DbSet<MusicAlbumStore.Models.MusicAlbum>? MusicAlbum { get; set; }

        public DbSet<MusicAlbumStore.Models.Review>? Review { get; set; }

        public DbSet<MusicAlbumStore.Models.MusicAlbumGenre>? MusicAlbumGenre { get; set; }

        public DbSet<MusicAlbumStore.Models.MusicAlbumUser> MusicAlbumUser { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
