//@CodeCopy
//MdStart
#nullable disable
using Microsoft.AspNetCore.Mvc;

namespace QTMusicStoreLight.AspMvc.Controllers
{
    public abstract class GenericController<TEntity, TModel> : Controller
        where TEntity : Logic.Entities.IdentityEntity, new()
        where TModel : class, new()
    {
        private readonly Logic.Controllers.GenericController<TEntity> controller;

        protected GenericController(Logic.Controllers.GenericController<TEntity> controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            this.controller = controller;
        }

        private static TModel ToModel(TEntity entity)
        {
            var result = new TModel();

            result.CopyFrom(entity);
            return result;
        }
        private static TEntity ToEntity(TModel model)
        {
            var result = new TEntity();

            result.CopyFrom(model);
            return result;
        }

        // GET: Item
        public virtual async Task<IActionResult> Index()
        {
            var entities = await controller.GetAllAsync();

            return View(entities.Select(e => ToModel(e)));
        }

        // GET: Item/Details/5
        public virtual async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await controller.GetByIdAsync(id.Value);
            if (genre == null)
            {
                return NotFound();
            }
            return View(ToModel(genre));
        }

        // GET: Item/Create
        public virtual IActionResult Create()
        {
            return View();
        }

        // POST: Item/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(TModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await controller.InsertAsync(ToEntity(model));
                    await controller.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;

                    if (ex.InnerException != null)
                    {
                        ViewBag.Error = ex.InnerException.Message;
                    }
                }
            }
            return View(model);
        }

        // GET: Item/Edit/5
        public virtual async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await controller.GetByIdAsync(id.Value);

            if (entity == null)
            {
                return NotFound();
            }
            return View(ToModel(entity));
        }

        // POST: Item/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(int id, TModel model)
        {
            var entity = await controller.GetByIdAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            entity.CopyFrom(model);

            if (ModelState.IsValid)
            {
                try
                {
                    entity = await controller.UpdateAsync(entity);
                    await controller.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;

                    if (ex.InnerException != null)
                    {
                        ViewBag.Error = ex.InnerException.Message;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ToModel(entity));
        }

        // GET: Item/Delete/5
        public virtual async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await controller.GetByIdAsync(id.Value);
            if (entity == null)
            {
                return NotFound();
            }
            return View(ToModel(entity));
        }

        // POST: Item/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await controller.GetByIdAsync(id);

            try
            {
                await controller.DeleteAsync(id);
                await controller.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;

                if (ex.InnerException != null)
                {
                    ViewBag.Error = ex.InnerException.Message;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        protected override void Dispose(bool disposing)
        {
            controller?.Dispose();
            base.Dispose(disposing);
        }
    }
}
//MdEnd
