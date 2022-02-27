#nullable disable
using Microsoft.AspNetCore.Mvc;

namespace QTMusicStoreLight.AspMvc.Controllers
{
    public class GenresController : Controller
    {
        private readonly Logic.Controllers.GenresController controller;

        public GenresController()
        {
            controller = new Logic.Controllers.GenresController();
        }

        private static Models.Genre ToModel(Logic.Entities.Genre entity)
        {
            var result = new Models.Genre();

            result.CopyFrom(entity);
            return result;
        }
        private static Logic.Entities.Genre ToEntity(Models.Genre model)
        {
            var result = new Logic.Entities.Genre();

            result.CopyFrom(model);
            return result;
        }

        // GET: Genres
        public async Task<IActionResult> Index()
        {
            var entities = await controller.GetAllAsync();

            return View(entities.Select(e => ToModel(e)));
        }

        // GET: Genres/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Genres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Genres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Models.Genre model)
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

        // GET: Genres/Edit/5
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

        // POST: Genres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name")] Logic.Entities.Genre genre)
        {
            var entity = await controller.GetByIdAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            entity.Name = genre.Name;

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
            return View(ToModel(genre));
        }

        // GET: Genres/Delete/5
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
            return View(ToModel(entity));
        }

        // POST: Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genre = await controller.GetByIdAsync(id);

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
