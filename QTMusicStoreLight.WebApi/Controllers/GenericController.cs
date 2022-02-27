//@CodeCopy
//MdStart
using Microsoft.AspNetCore.Mvc;

namespace QTMusicStoreLight.WebApi.Controllers
{
    /// <summary>
    /// A generic one for the standard CRUD operations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity</typeparam>
    /// <typeparam name="TModel">The type of model</typeparam>
    public abstract partial class GenericController<TEntity, TEditModel, TModel> : ControllerBase, IDisposable
        where TEntity : Logic.Entities.IdentityEntity, new()
        where TEditModel : class, new()
        where TModel : class, new()
    {
        private bool disposedValue;

        protected Logic.Controllers.GenericController<TEntity> EntityController { get; init; }

        internal GenericController(Logic.Controllers.GenericController<TEntity> controller)
        {
            if (controller is null)
            {
                throw new ArgumentNullException(nameof(controller));
            }
            EntityController = controller;
        }
        /// <summary>
        /// Converts an entity to a model and copies all properties of the same name from the entity to the model.
        /// </summary>
        /// <param name="entity">The entity to be converted</param>
        /// <returns>The model with the property values of the same name</returns>
        protected virtual TModel ToModel(TEntity? entity)
        {
            var result = new TModel();

            if (entity != null)
            {
                result.CopyFrom(entity);
            }
            return result;
        }
        /// <summary>
        /// Converts all entities to models and copies all properties of the same name from an entity to the model.
        /// </summary>
        /// <param name="entities">The entities to be converted</param>
        /// <returns>The models</returns>
        protected virtual IEnumerable<TModel> ToModel(IEnumerable<TEntity> entities)
        {
            var result = new List<TModel>();

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
        public virtual async Task<ActionResult<IEnumerable<TModel>>> GetAsync()
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<TModel?>> GetAsync(int id)
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
        public virtual async Task<ActionResult<TModel>> PostAsync([FromBody] TEditModel model)
        {
            var entity = new TEntity();

            if (model != null)
            {
                entity.CopyFrom(model);
            }
            var insertEntity = await EntityController.InsertAsync(entity);

            await EntityController.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = entity.Id }, ToModel(insertEntity));
        }

        /// <summary>
        /// Updates a model
        /// </summary>
        /// <param name="id">Id of the model to update</param>
        /// <param name="model">Data to update</param>
        /// <returns>Data about the updated model</returns>
        /// <response code="200">Model updated</response>
        /// <response code="404">Model not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<TModel>> PutAsync(int id, [FromBody] TEditModel model)
        {
            var entity = await EntityController.GetByIdAsync(id);

            if (entity != null && model != null)
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
