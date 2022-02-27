#nullable disable
using Microsoft.AspNetCore.Mvc;

namespace QTMusicStoreLight.AspMvc.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly Logic.Controllers.AlbumsController controller;

        public AlbumsController()
        {
            controller = new Logic.Controllers.AlbumsController();
        }

        private static Models.Album ToModel(Logic.Entities.Album entity)
        {
            var result = new Models.Album();

            result.CopyFrom(entity);
            return result;
        }
        private static Logic.Entities.Album ToEntity(Models.Album model)
        {
            var result = new Logic.Entities.Album();

            result.CopyFrom(model);
            return result;
        }

        // GET: Albums
        public async Task<IActionResult> Index()
        {
            using var genresCtrl = new Logic.Controllers.GenresController(controller);
            using var artistsCtrl = new Logic.Controllers.ArtistsController(controller);
            var genres = await genresCtrl.GetAllAsync();
            var artists = await artistsCtrl.GetAllAsync();
            var entities = await controller.GetAllAsync();
            var models = entities.Select(e =>
            {
                var model = ToModel(e);

                model.ArtistName = artists.FirstOrDefault(a => a.Id == model.ArtistId)?.Name;
                model.GenreName = genres.FirstOrDefault(g => g.Id == model.GenreId)?.Name;

                return model;
            });
            return View(models);
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

            var model = ToModel(entity);
            using var genresCtrl = new Logic.Controllers.GenresController(controller);
            using var artistsCtrl = new Logic.Controllers.ArtistsController(controller);
            var genre = await genresCtrl.GetByIdAsync(entity.GenreId);
            var artist = await artistsCtrl.GetByIdAsync(entity.ArtistId);

            model.GenreName = genre?.Name;
            model.ArtistName = artist?.Name;

            return View(model);
        }

        // GET: Albums/Create
        public async Task<IActionResult> Create()
        {
            var model = new Models.Album();
            using var genresCtrl = new Logic.Controllers.GenresController(controller);
            using var artistsCtrl = new Logic.Controllers.ArtistsController(controller);
            
            model.Genres = await genresCtrl.GetAllAsync();
            model.Artists = await artistsCtrl.GetAllAsync();

            return View(model);
        }

        // POST: Albums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,ArtistId,GenreId")] Models.Album model)
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

            using var genresCtrl = new Logic.Controllers.GenresController(controller);
            using var artistsCtrl = new Logic.Controllers.ArtistsController(controller);

            model.Genres = await genresCtrl.GetAllAsync();
            model.Artists = await artistsCtrl.GetAllAsync();
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

            var model = ToModel(entity);
            using var genresCtrl = new Logic.Controllers.GenresController(controller);
            using var artistsCtrl = new Logic.Controllers.ArtistsController(controller);

            model.Genres = await genresCtrl.GetAllAsync();
            model.Artists = await artistsCtrl.GetAllAsync();

            return View(model);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,ArtistId,GenreId")] Models.Album model)
        {
            var entity = await controller.GetByIdAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            entity.ArtistId = model.ArtistId;
            entity.GenreId = model.GenreId;
            entity.Title = model.Title;

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
            using var genresCtrl = new Logic.Controllers.GenresController(controller);
            using var artistsCtrl = new Logic.Controllers.ArtistsController(controller);

            model.Genres = await genresCtrl.GetAllAsync();
            model.Artists = await artistsCtrl.GetAllAsync();

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
            return View(ToModel(entity));
        }

        // POST: Albums/Delete/5
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
