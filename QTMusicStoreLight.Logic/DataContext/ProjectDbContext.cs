//@CodeCopy
//MdStart
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace QTMusicStoreLight.Logic.DataContext
{
    internal partial class ProjectDbContext : DbContext
    {
        private static readonly string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Database=QTMusicStoreLightDb;Integrated Security=True";
        static ProjectDbContext()
        {
            BeforeClassInitialize();
            try
            {
                var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                var connectionString = configuration["ConnectionStrings:DefaultConnection"];

                if (string.IsNullOrEmpty(connectionString) == false)
                {
                    ConnectionString = connectionString;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(message: $"Error in {System.Reflection.MethodBase.GetCurrentMethod()?.Name}: {ex.Message}");
            }
            AfterClassInitialize();
        }
        static partial void BeforeClassInitialize();
        static partial void AfterClassInitialize();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);

            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<E> GetDbSet<E>() where E : Entities.IdentityObject
        {
            var handled = false;
            var result = default(DbSet<E>);

            GetDbSet(ref result, ref handled);
            if (handled == false || result == null)
            {
                result = Set<E>();
            }
            return result;
        }
        partial void GetDbSet<E>(ref DbSet<E>? dbSet, ref bool handled) where E : Entities.IdentityObject;
        public IQueryable<E> QueryableSet<E>() where E : Entities.IdentityObject
        {
            return GetDbSet<E>();
        }
    }
}
//MdEnd
