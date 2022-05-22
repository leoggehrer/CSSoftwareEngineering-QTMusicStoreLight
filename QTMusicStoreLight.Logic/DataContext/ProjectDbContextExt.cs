namespace QTMusicStoreLight.Logic.DataContext
{
    partial class ProjectDbContext
    {
        public DbSet<Entities.App.Song>? SongSet { get; set; }

        partial void GetDbSet<E>(ref DbSet<E>? dbSet, ref bool handled) where E : Entities.IdentityEntity
        {
            if (typeof(E) == typeof(Entities.App.Song))
            {
                handled = true;
                dbSet = SongSet as DbSet<E>;
            }
        }
    }
}
