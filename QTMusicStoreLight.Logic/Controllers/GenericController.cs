//@CodeCopy
//MdStart

namespace QTMusicStoreLight.Logic.Controllers
{
    /// <summary>
    /// This class provides the CRUD operations for an entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type for which the operations are available.</typeparam>
    public abstract partial class GenericController<TEntity> : ControllerObject, IDataAccess<TEntity> 
        where TEntity : Entities.IdentityEntity, new()
    {
        static GenericController()
        {
            BeforeClassInitialize();

            AfterClassInitialize();
        }
        static partial void BeforeClassInitialize();
        static partial void AfterClassInitialize();

        private DbSet<TEntity>? dbSet = null;
        public GenericController()
            : base(new DataContext.ProjectDbContext())
        {

        }
        public GenericController(ControllerObject other)
            : base(other)
        {

        }

        internal DbSet<TEntity> EntitySet
        {
            get
            {
                if (dbSet == null)
                {
                    if (Context != null)
                    {
                        dbSet = Context.GetDbSet<TEntity>();
                    }
                    else
                    {
                        using var ctx = new DataContext.ProjectDbContext();

                        dbSet = ctx.GetDbSet<TEntity>();

                    }
                }
                return dbSet;
            }
        }

        #region Queries
        public virtual Task<TEntity[]> GetAllAsync()
        {
            return EntitySet.AsNoTracking().ToArrayAsync();
        }
        public virtual ValueTask<TEntity?> GetByIdAsync(int id)
        {
            return EntitySet.FindAsync(id);
        }
        #endregion Queries

        #region Insert
        public virtual async Task<TEntity> InsertAsync(TEntity entity)
        {
            await EntitySet.AddAsync(entity).ConfigureAwait(false);
            return entity;
        }
        public virtual async Task<IEnumerable<TEntity>> InsertAsync(IEnumerable<TEntity> entities)
        {
            await EntitySet.AddRangeAsync(entities).ConfigureAwait(false);
            return entities;
        }
        #endregion Insert

        #region Update
        public virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            return Task.Run(() =>
            {
                EntitySet.Update(entity);
                return entity;
            });
        }
        public virtual Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities)
        {
            return Task.Run(() =>
            {
                EntitySet.UpdateRange(entities);
                return entities;
            });
        }
        #endregion Update

        #region Delete
        public virtual Task DeleteAsync(int id)
        {
            return Task.Run(() =>
            {
                TEntity? result = EntitySet.Find(id);

                if (result != null)
                {
                    EntitySet.Remove(result);
                }
            });
        }
        #endregion Delete

        #region SaveChanges
        public async Task<int> SaveChangesAsync()
        {
            var result = 0;

            if (Context != null)
            {
                result = await Context.SaveChangesAsync().ConfigureAwait(false);
            }
            return result;
        }
        #endregion SaveChanges
    }
}
//MdEnd
