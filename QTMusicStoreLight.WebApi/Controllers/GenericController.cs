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

        /// <summary>
        /// Gets a list of models
        /// </summary>
        /// <returns>List of models</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<IEnumerable<M>>> GetAsync()
        {
            var entities = await EntityController.GetAllAsync();

            return Ok(ToModel(entities));
        }

        /// <summary>
        /// Get a single model by Id.
        /// </summary>
        /// <param name="id">Id of the model to get</param>
        /// <response code="200">Model found</response>
        /// <response code="404">Model not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<M?>> GetAsync(int id)
        {
            var entity = await EntityController.GetByIdAsync(id);

            return entity == null ? NotFound() : Ok(ToModel(entity));
        }

        /// <summary>
        /// Adds a model.
        /// </summary>
        /// <param name="model">Model to add</param>
        /// <returns>Data about the created model (including primary key)</returns>
        /// <response code="201">Model created</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public virtual async Task<ActionResult<M>> PostAsync([FromBody] M model)
        {
            var entity = new E();

            entity.CopyFrom(model);
            entity = await EntityController.InsertAsync(entity);
            await EntityController.SaveChangesAsync();

            return CreatedAtRoute(nameof(GetAsync), new { id = entity.Id }, ToModel(entity));
        }

        /// <summary>
        /// Updates a model
        /// </summary>
        /// <param name="id">Id of the model to update</param>
        /// <param name="model">Data to update</param>
        /// <returns>Data about the updated model</returns>
        /// <response code="404">Model not found</response>
        [HttpPut("{id}")]
        public virtual async Task<ActionResult<M>> PutAsync(int id, [FromBody] M model)
        {
            var entity = await EntityController.GetByIdAsync(id);

            if (entity != null)
            {
                entity.CopyFrom(model);
                await EntityController.UpdateAsync(entity);
                await EntityController.SaveChangesAsync();
            }
            return entity == null ? NotFound() : Ok(ToModel(entity));
        }

        /// <summary>
        /// Delete a single model by Id
        /// </summary>
        /// <param name="id">Id of the model to delete</param>
        /// <response code="204">Model deleted</response>
        /// <response code="404">Model not found</response>
        [HttpDelete("{id}")]
        public virtual async Task<ActionResult> DeleteAsync(int id)
        {
            var entity = await EntityController.GetByIdAsync(id);

            if (entity != null)
            {
                await EntityController.DeleteAsync(entity.Id);
                await EntityController.SaveChangesAsync();
            }
            return entity == null ? NotFound() : NoContent();
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
