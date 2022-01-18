using Microsoft.EntityFrameworkCore;
using QTMusicStoreLight.Logic.Entities;

namespace QTMusicStoreLight.Logic.DataContext
{
    partial class ProjectDbContext
    {
        public DbSet<Artist>? ArtistSet { get; set; }
        public DbSet<Album>? AlbumSet { get; set; }
        public DbSet<Genre>? GenreSet { get; set; }

        partial void GetDbSet<E>(ref DbSet<E>? dbSet, ref bool handled) where E : IdentityObject
        {
            if (typeof(E) == typeof(Artist))
            {
                handled = true;
                dbSet = ArtistSet as DbSet<E>;
            }
            else if (typeof(E) == typeof(Album))
            {
                handled = true;
                dbSet = AlbumSet as DbSet<E>;
            }
            else if ((typeof(E) == typeof(Genre)))
            {
                handled = true;
                dbSet = GenreSet as DbSet<E>;
            }
        }
    }
}
