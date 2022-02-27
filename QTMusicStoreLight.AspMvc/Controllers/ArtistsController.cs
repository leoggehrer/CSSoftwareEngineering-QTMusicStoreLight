#nullable disable
using Microsoft.AspNetCore.Mvc;

namespace QTMusicStoreLight.AspMvc.Controllers
{
    public class ArtistsController : Controller
    {
        private readonly Logic.Controllers.ArtistsController controller;

        public ArtistsController()
        {
            controller = new Logic.Controllers.ArtistsController();
        }

        private static Models.Artist ToModel(Logic.Entities.Artist entity)
        {
            var result = new Models.Artist();

            result.CopyFrom(entity);
            return result;
        }
        private static Logic.Entities.Artist ToEntity(Models.Artist model)
        {
            var result = new Logic.Entities.Artist();

            result.CopyFrom(model);
            return result;
        }

        // GET: Albums
        public async Task<IActionResult> Index()
        {
            var entities = await controller.GetAllAsync();

            return View(entities.Select(e => ToModel(e)));
        }

        // GET: Albums/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Albums/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Models.Artist model)
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

        // GET: Albums/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

        // POST: Albums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name")] Models.Artist model)
        {
            var entity = await controller.GetByIdAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            entity.Name = model.Name;

            if (ModelState.IsValid)
            {
                try
                {
                    await controller.UpdateAsync(entity);
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
            return View(model);
        }

        // GET: Albums/Delete/5
        public async Task<IActionResult> Delete(int? id)
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
            return View(entity);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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
