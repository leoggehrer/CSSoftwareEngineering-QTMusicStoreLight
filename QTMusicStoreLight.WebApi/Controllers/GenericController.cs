//@CodeCopy
//MdStart
using Microsoft.AspNetCore.Mvc;

namespace QTMusicStoreLight.WebApi.Controllers
{
    public abstract partial class GenericController<E, M> : ControllerBase, IDisposable
        where E : Logic.Entities.IdentityObject, new()
        where M : class, new()
    {
        private bool disposedValue;

        protected Logic.Controllers.GenericController<E> EntityController { get; init; }

        internal GenericController(Logic.Controllers.GenericController<E> controller)
        {
            if (controller is null)
            {
                throw new ArgumentNullException(nameof(controller));
            }
            EntityController = controller;
        }

        protected virtual M ToModel(E? entity)
        {
            var result = new M();

            if (entity != null)
            {
                result.CopyFrom(entity);
            }
            return result;
        }
        protected virtual IEnumerable<M> ToModel(IEnumerable<E> entities)
        {
            var result = new List<M>();

            foreach (var entity in entities)
            {
                result.Add(ToModel(entity));
            }
            return result;
        }
        // GET: api/<Controller>
        [HttpGet]
        public async Task<IEnumerable<M>> GetAsync()
        {
            var entities = await EntityController.GetAllAsync();

            return ToModel(entities);
        }

        // GET api/<Controller>/5
        [HttpGet("{id}")]
        public async Task<M?> GetAsync(int id)
        {
            var entity = await EntityController.GetByIdAsync(id);

            return ToModel(entity);
        }

        #region Dispose pattern
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    EntityController.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~GenericController()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion Dispose pattern
    }
}
//MdEnd
