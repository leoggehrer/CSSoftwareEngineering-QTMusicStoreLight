
# QTMusicStoreLight

Dieses Projekt ist mit der Vorlage ***QuickTemplate*** erstellt worden und wird nun Schritt f�r Schritt vom ***'Backend'*** bis zum ***'Frontend'*** entwickelt - also eine ***'Full Stack Application'***. Die Beschreibung f�r die Erstellung diese Projektbasis ist auf [Github-QuickTemplate](https://github.com/leoggehrer/CSSoftwareEngineering-QuickTemplate) dokumentiert.   

# Inhaltsverzeichnis
1. [Projektbeschreibung](#Projektbeschreibung)
   1. [Definition von Artist](#definition-von-artist)
   2. [Definition von Album](#definition-von-album)
   3. [Definition von Genre](#definition-von-genre)
2. [Erstellen des Backends](#erstellen-des-backends)
   1. [Erstellen der Entit�ten](#erstellen-der-entitaeten)
   2. [Erstellen der Kontroller](#erstellen-der-kontroller)
   3. [Erstellen der Datenbank](#erstellen-der-datenbank)
   4. [Importieren von Daten](#importieren-von-daten)
3. [Erstellen der AspMvc-Anwendung](#erstellen-der-aspmvc-anwendung)
   1. [Erstellen der Models](#erstellen-der-models)
   2. [Erstellen der Kontrollers](#erstellen-der-kontrollers)
   3. [Erstellen der Ansichten](#erstellen-der-ansichten)

# Projektbeschreibung  

***QTMusicStoreLight*** ist eine einfache Anwendung zur Verwaltung von Musik-Daten. Damit sollen K�nstler (Artists) und deren Alben in einer Datenbank gespeichert werden und bei Bedarf wieder abgerufen werden k�nnen. Die Bearbeitung der Daten soll �ber eine Web-Anwendung als auch �ber eine Desktop Anwendung erm�glicht werden. Das Datenmodell ist in der folgenden Abbildung skizziert.

```txt
                +-------------+                 +-------------+
                |             |                 |             |
       +----- n +    Album    + n ----------- 1 +    Genre    |
       |        |             |                 |             | 
       |        +-------------+                 +-------------+
       |
       1
+------+------+
|             |
|   Artist    |
|             |
+------+------+

```

Ein K�nstler kann beliebig viele Alben zugeordnet haben und das Album ist mit einer Musikrichtung (Genre) verbunden. Das Datenmodell f�r den **MusicStoreLight** ist wie folgt definiert:


## Definition von ***Artist***

| Name | Type | MaxLength | Nullable |Unique|
|------|------|-----------|----------|------|
| Id | int |---|---|---|
| RowVersion | byte[] |---|No|---|
| Name | String | 128 | No |Yes|

## Definition von ***Album***  

| Name | Type | MaxLength | Nullable |Unique|
|------|------|-----------|----------|------|
| Id | int |---|---|---|
| RowVersion | byte[] |---|No|---|
| ArtistId | int |---|---|---|
| GenreId | int |---|---|---|
| Title | String | 256 | No |Yes|

## Definition von ***Genre***

| Name | Type | MaxLength | Nullable |Unique|
|------|------|-----------|----------|------|
| Id | int |---|---|---|
| RowVersion | byte[] |---|No|---|
| Name | String | 128 | No |Yes|

# Erstellen des Backends 

Das Backend-System bildet das Grundsystem der Anwendung und beinhaltet die Daten- und die Logik-Schicht. 
 
## Erstellen der Entit�ten<a name="erstellen-der-entitaeten"/>  

Die Entit�ten werden im Projekt ***QTMusicStoreLight.Logic*** im Ordner ***Entities*** definiert. Nachdem f�r die Entit�ten als Zugriffsstrategie ***Cuncurrency Optimistic*** verwendet wird (RowVersion ist definiert), werden die Entit�ten vom bereits definierten ***VersionObject*** abgeleitet.

Die Implementierung der Entit�t ***Artist***:  

```csharp
namespace QTMusicStoreLight.Logic.Entities
{
    [Table("Artists", Schema = "App")]
    [Index(nameof(Name), IsUnique = true)]
    public class Artist : VersionEntity
    {
        [Required]
        [MaxLength(128)]
        public string Name { get; set; } = String.Empty;

        // Navigation properties
        public List<Album> Albums { get; set; } = new();
    }
}
```

Die Implementierung der Entit�t ***Genre***:  

```csharp
namespace QTMusicStoreLight.Logic.Entities
{
    [Table("Genres", Schema = "App")]
    [Index(nameof(Name), IsUnique = true)]
    public class Genre : VersionEntity
    {
        [Required]
        [MaxLength(128)]
        public string Name { get; set; } = String.Empty;

        // Navigation properties
        public List<Album> Albums { get; set; } = new();
    }
}
```

Die Implementierung der Entit�t ***Album***:  

```csharp
namespace QTMusicStoreLight.Logic.Entities
{
    [Table("Albums", Schema = "App")]
    [Index(nameof(Title), IsUnique = true)]
    public class Album : VersionEntity
    {
        public int ArtistId { get; set; }
        public int GenreId { get; set; }
        [Required]
        [MaxLength(256)]
        public string Title { get; set; } = String.Empty;

        // Navigation properties
        public Artist? Artist { get; set; }
        public Genre? Genre { get; set; }
    }
}
```  

Das Datenmodell ist wie nachfolgend dargestellt in ein Objektmodell transformiert worden:

![QTMusicStoreLight-Entities](Entities.png) 

## Definition vom Datenbank-Kontext  

Nachdem die Entit�ten definiert sind, wird nun der Datenbank-Kontext f�r die Anwendung fertiggesetllt. Aus der Vorlage ***QuickTemplate*** ist der vordefinierte Datenbank-Kontext ***'ProjectDbContext'*** kopiert worden. Diese beinhaltet bereits eine Standard-Implementierung f�r den generischen Kontroller ***'GenericController&lt;E&gt;'*** und ist als eine **'partielle Klasse'** ausgef�hrt. Damit diese Klasse angepasst werden kann, wird eine weitere **'partielle Klasse'** zur Klasse ***'ProjectDbContext'*** erstellt. Dazu wird eine Datei mit dem Namen ***'ProjectDbContextExt.cs'*** erstellt. Die Erweiterung ist nachfolgend definiert:

```csharp
using QTMusicStoreLight.Logic.Entities;

namespace QTMusicStoreLight.Logic.DataContext
{
    partial class ProjectDbContext
    {
        public DbSet<Artist>? ArtistSet { get; set; }
        public DbSet<Album>? AlbumSet { get; set; }
        public DbSet<Genre>? GenreSet { get; set; }

        partial void GetDbSet<E>(ref DbSet<E>? dbSet, ref bool handled) where E : IdentityEntity
        {
            if (typeof(E) == typeof(Artist))
            {
                handled = true;
                dbSet = ArtistSet as DbSet<E>;
            }
            else if (typeof(E) == typeof(Album))
            {
                handled = true;
                dbSet = AlbumSet as DbSet<E>;
            }
            else if ((typeof(E) == typeof(Genre)))
            {
                handled = true;
                dbSet = GenreSet as DbSet<E>;
            }
        }
    }
}
```

Wie aus der obigen Implementierung ersichtlich ist, werden f�r die entsprechenden Entit�ten die ***'DbSet&lt;E&gt;'*** definiert. Zus�tzlich wird die partielle Methode ***'GetDbSet&lt;E&gt;(...)'*** definiert. �ber diese Methode greift der ***'GenericController&lt;E&gt;'*** auf den konkreten ***'DbSet&lt;E&gt;'*** zu.

## Erstellen der Kontroller  

Der generische Kontroller ***'GenericController&lt;E&gt;'*** implementiert bereits die ***'CRUD'*** Funktionen f�r eine Entit�t. Um diese Funktionen f�r jede Entit�t zur Verf�gung zu stellen, muss f�r jede Entit�t ein eigener Kontroller angelegt werden. Die Kontroller werden im Projekt ***QTMusicStoreLight.Logic*** im Ordner ***Controllers*** definiert.  

### Kontroller f�r die Entit�t *Artist* erstellen

```csharp
namespace QTMusicStoreLight.Logic.Controllers
{
    public sealed partial class ArtistsController : GenericController<Entities.Artist>
    {
        public ArtistsController() : base()
        {
        }

        public ArtistsController(ControllerObject other) : base(other)
        {
        }
    }
}
```

### Kontroller f�r die Entit�t *Album* erstellen

```csharp
namespace QTMusicStoreLight.Logic.Controllers
{
    public sealed partial class AlbumsController : GenericController<Entities.Album>
    {
        public AlbumsController() : base()
        {
        }

        public AlbumsController(ControllerObject other) : base(other)
        {
        }
    }
}
```

### Kontroller f�r die Entit�t *Genre* erstellen

```csharp
namespace QTMusicStoreLight.Logic.Controllers
{
    public sealed partial class GenresController : GenericController<Entities.Genre>
    {
        public GenresController() : base()
        {
        }

        public GenresController(ControllerObject other) : base(other)
        {
        }
    }
}
```

> **ACHTUNG:**  Die konkreten Kontroller werden mit dem Schl�sselwort ***'sealed'*** spezifiziert. Die Erkl�rung folgt zu einem sp�teren Zeitpunkt.  

## Erstellen der Datenbank

Dieser Abschnitt erl�utert die Erstellung der Datenbank.

**Notwendige Voraussetzungen:**  
* Die gesamte Projektmappe muss fehlerfrei �bersetzt werden k�nnen.
Die Verbindungszeichenfolge kann im Projekt **QTMusicStoreLight.ConApp** in der Datei **appsettings.Development.json** angepasst werden.
* Als Startprojekt muss das Projekt **QTMusicStoreLight.ConApp** eingestellt sein.
* In der **Package Manager Console** muss das **Default projekt: QTMusicStoreLight.Logic** ausgew�hlt sein.

**Wichtige Kommandos:**

> PM> add-migration InitDb

...diese Kommando erzeugt eine Migration mit dem Namen 'InitDb'.

> PM> update-database

...diese Kommando f�hrt die Migration aus und erzeugt die Datenbank.

## Importieren von Daten  

Der Import wird im Projekt **QTMusicStoreLight.ConApp** in der Datei **Program.cs** implementiert. Zu diesem Zweck werden die Daten, im csv-Format, vom [GitHub-MusicStoreDaten](https://github.com/leoggehrer/Data-MusicStore) heruntergeladen und im Ordner **CsvData** abgelegt. In den Eigenschaften der csv-Dateien wird die Eigenschaft **Copy to Output Directory** auf **Copy if newer** eingestellt. Mit dieser Einstellung m�ssen keine Pfade angegeben werden weil die Dateien in das Ausf�hrungsverzeichnis kopiert werden. Nachfolgend ist der Programm-Code f�r den Import angegeben:  

```csharp
namespace QTMusicStoreLight.ConApp
{
    partial class Program
    {
        static partial void AfterRun()
        {
            var csvGenres = File.ReadAllLines("CsvData/Genre.csv")
                             .Skip(1)
                             .Select(l => l.Split(";"))
                             .Select(d => new { id = d[0], Entity = new Logic.Entities.Genre { Name = d[1] } });
            var csvArtists = File.ReadAllLines("CsvData/Artist.csv")
                             .Skip(1)
                             .Select(l => l.Split(";"))
                             .Select(d => new
                             {
                                 id = d[0],
                                 Entity = new Logic.Entities.Artist
                                 {
                                     Name = d[1],
                                 }
                             });
            var csvAlbums = File.ReadAllLines("CsvData/AlbumWithGenre.csv")
                             .Skip(1)
                             .Select(l => l.Split(";"))
                             .Select(d => new
                             {
                                 id = d[0],
                                 ArtistId = d[2],
                                 GenreId = d[3],
                                 Entity = new Logic.Entities.Album
                                 {
                                     Title = d[1],
                                 }
                             });
                var artists = csvArtists.Select(e => e.Entity).ToArray();
                var genres = csvGenres.Select(e => e.Entity).ToArray();
                var albums = new List<Logic.Entities.Album>();

                foreach (var item in csvAlbums)
                {
                    var genIdx = csvGenres.IndexOf(e => e.id == item.GenreId);
                    var artIdx = csvArtists.IndexOf(e => e.id == item.ArtistId);

                    item.Entity.Genre = genres[genIdx];
                    item.Entity.Artist = artists[artIdx];
                    albums.Add(item.Entity);
                }

            Task.Run(async () =>
            {
                using var albumsCtrl = new Logic.Controllers.AlbumsController();

                await albumsCtrl.InsertAsync(albums);
                await albumsCtrl.SaveChangesAsync();
            }).Wait();
        }
    }
}
```

In der Zeile ***await albumsCtrl.InsertAsync(albums);*** werden die Objekte in den Datenbank-Kontext �bertragen. Mit der Anweisung ***await albumsCtrl.SaveChangesAsync();*** werden die Daten in die Datenbank eingetragen. Alle referenzierten Objekte (z.B.: Album -> Artist) werden ebenfalls vom Kontroller in die Datenbank �bermittelt.  

## Erstellen der AspMvc-Anwendung

### Erstellen der Models

F�r die Umsetzung der AspMvc-Anwendung ist die Erstellung von Models erforderlich. Diese Models sind die Daten-Transfer-Objekte (DTO's) f�r die AspMvc-Anwendung vom Kontroller zur Ansicht und umgekehrt. Diese DTO's befinden sich im Ordner **Models** der Amwendung.

Die Implementierung des Models ***Artist***:  

```csharp
using System.ComponentModel.DataAnnotations;

namespace QTMusicStoreLight.AspMvc.Models
{
    public class Artist : VersionModel
    {
        [Required]
        [MaxLength(128)]
        public string Name { get; set; } = string.Empty;
    }
}
```

Die Implementierung des Models ***Genre***:  

```csharp
using System.ComponentModel.DataAnnotations;

namespace QTMusicStoreLight.AspMvc.Models
{
    public class Genre : VersionModel
    {
        [Required]
        [MaxLength(128)]
        public string Name { get; set; } = String.Empty;
    }
}
```

Die Implementierung des Models ***Album***:  

```csharp
using System.ComponentModel.DataAnnotations;

namespace QTMusicStoreLight.AspMvc.Models
{
    public class Album : VersionModel
    {
        public int ArtistId { get; set; }
        public int GenreId { get; set; }
        [Required]
        [MaxLength(256)]
        public string Title { get; set; } = string.Empty;

        public string ArtistName { get; set; } = string.Empty;
        public string GenreName { get; set; } = string.Empty;

        public Logic.Entities.Genre[]? Genres { get; set; }
        public Logic.Entities.Artist[]? Artists { get; set; }
    }
}
```

> **HINWEIS:** Die zus�tzlichen Eigenschaften *ArtistName* und *GenreName* dienen zur Anzeige in der �bersicht (Liste). Die Eigenschaften *Genres* und *Artist* werden in der Ansicht *Create* und *Edit* f�r die Zuordnung ben�tigt.  

### Erstellen der Kontrollers

### Kontroller *Artist* erstellen

```csharp
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
```

### Kontroller f�r *Genre* erstellen

```csharp
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
```

### Kontroller f�r *Album* erstellen

```csharp
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
```

### Erstellen der Ansichten

### Ansichten f�r die �bersicht von *Artist* (Index.cshtml)

```csharp
@model IEnumerable<QTMusicStoreLight.AspMvc.Models.Artist>

@{
    ViewData["Title"] = "Index";
}

<h1>Index</h1>

<p>
    <a asp-action="Create">Create New</a>
</p>
<table class="table">
    <thead>
        <tr>
            <th>
                @Html.DisplayNameFor(model => model.Name)
            </th>
            <th></th>
        </tr>
    </thead>
    <tbody>
@foreach (var item in Model) {
        <tr>
            <td>
                @Html.DisplayFor(modelItem => item.Name)
            </td>
            <td>
                <a asp-action="Edit" asp-route-id="@item.Id">Edit</a> |
                <a asp-action="Details" asp-route-id="@item.Id">Details</a> |
                <a asp-action="Delete" asp-route-id="@item.Id">Delete</a>
            </td>
        </tr>
}
    </tbody>
</table>
```

### Ansichten f�r das Erstellen eines *Artist* (Create.cshtml)

```csharp
@model QTMusicStoreLight.AspMvc.Models.Artist

@{
    ViewData["Title"] = "Create";
}

<h1>Create</h1>

<h4>Artist</h4>
<hr />

@if (string.IsNullOrEmpty(ViewBag.Error) == false)
{
    <div class="alert alert-danger" role="alert">
        @ViewBag.Error
    </div>
}

<div class="row">
    <div class="col-md-4">
        <form asp-action="Create">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <div class="form-group">
                <label asp-for="Name" class="control-label"></label>
                <input asp-for="Name" class="form-control" />
                <span asp-validation-for="Name" class="text-danger"></span>
            </div>
            <div class="form-group">
                <input type="submit" value="Create" class="btn btn-primary" />
            </div>
        </form>
    </div>
</div>

<div>
    <a asp-action="Index">Back to List</a>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

### Ansichten f�r das Bearbeiten eines *Artist* (Edit.cshtml)

```csharp
@model QTMusicStoreLight.AspMvc.Models.Artist

@{
    ViewData["Title"] = "Edit";
}

<h1>Edit</h1>

@if (string.IsNullOrEmpty(ViewBag.Error) == false)
{
    <div class="alert alert-danger" role="alert">
        @ViewBag.Error
    </div>
}

<h4>Artist</h4>
<hr />
<div class="row">
    <div class="col-md-4">
        <form asp-action="Edit">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <div class="form-group">
                <label asp-for="Name" class="control-label"></label>
                <input asp-for="Name" class="form-control" />
                <span asp-validation-for="Name" class="text-danger"></span>
            </div>
            <input type="hidden" asp-for="Id" />
            <div class="form-group">
                <input type="submit" value="Save" class="btn btn-primary" />
            </div>
        </form>
    </div>
</div>

<div>
    <a asp-action="Index">Back to List</a>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

### Ansichten f�r das L�schen eines *Artist* (Delete.cshtml)

```csharp
@model QTMusicStoreLight.AspMvc.Models.Artist

@{
    ViewData["Title"] = "Delete";
}

<h1>Delete</h1>

@if (string.IsNullOrEmpty(ViewBag.Error) == false)
{
    <div class="alert alert-danger" role="alert">
        @ViewBag.Error
    </div>
}

<h3>Are you sure you want to delete this?</h3>
<div>
    <h4>Artist</h4>
    <hr />
    <dl class="row">
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.Name)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.Name)
        </dd>
    </dl>
    
    <form asp-action="Delete">
        <input type="hidden" asp-for="Id" />
        <input type="submit" value="Delete" class="btn btn-danger" /> |
        <a asp-action="Index">Back to List</a>
    </form>
</div>
```

### Ansichten f�r die �bersicht von *Genre* (Index.cshtml)

```csharp
@model IEnumerable<QTMusicStoreLight.AspMvc.Models.Genre>

@{
    ViewData["Title"] = "Index";
}

<h1>Index</h1>

<p>
    <a asp-action="Create">Create New</a>
</p>
<table class="table">
    <thead>
        <tr>
            <th>
                @Html.DisplayNameFor(model => model.Name)
            </th>
            <th></th>
        </tr>
    </thead>
    <tbody>
@foreach (var item in Model) {
        <tr>
            <td>
                @Html.DisplayFor(modelItem => item.Name)
            </td>
            <td>
                <a asp-action="Edit" asp-route-id="@item.Id">Edit</a> |
                <a asp-action="Details" asp-route-id="@item.Id">Details</a> |
                <a asp-action="Delete" asp-route-id="@item.Id">Delete</a>
            </td>
        </tr>
}
    </tbody>
</table>
```

### Ansichten f�r das Erstellen eines *Genre* (Create.cshtml)

```csharp
@model QTMusicStoreLight.AspMvc.Models.Genre

@{
    ViewData["Title"] = "Create";
}

<h1>Create</h1>

<h4>Genre</h4>
<hr />

@if (string.IsNullOrEmpty(ViewBag.Error) == false)
{
    <div class="alert alert-danger" role="alert">
        @ViewBag.Error
    </div>
}

<div class="row">
    <div class="col-md-4">
        <form asp-action="Create">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <div class="form-group">
                <label asp-for="Name" class="control-label"></label>
                <input asp-for="Name" class="form-control" />
                <span asp-validation-for="Name" class="text-danger"></span>
            </div>
            <div class="form-group">
                <input type="submit" value="Create" class="btn btn-primary" />
            </div>
        </form>
    </div>
</div>

<div>
    <a asp-action="Index">Back to List</a>
</div>

@section Scripts {
    @{
    await Html.RenderPartialAsync("_ValidationScriptsPartial");
}
}
```

### Ansichten f�r das Bearbeiten eines *Genre* (Edit.cshtml)

```csharp
@model QTMusicStoreLight.AspMvc.Models.Genre

@{
    ViewData["Title"] = "Edit";
}

<h1>Edit</h1>

@if (string.IsNullOrEmpty(ViewBag.Error) == false)
{
    <div class="alert alert-danger" role="alert">
        @ViewBag.Error
    </div>
}

<h4>Genre</h4>
<hr />
<div class="row">
    <div class="col-md-4">
        <form asp-action="Edit">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <div class="form-group">
                <label asp-for="Name" class="control-label"></label>
                <input asp-for="Name" class="form-control" />
                <span asp-validation-for="Name" class="text-danger"></span>
            </div>
            <input type="hidden" asp-for="Id" />
            <div class="form-group">
                <input type="submit" value="Save" class="btn btn-primary" />
            </div>
        </form>
    </div>
</div>

<div>
    <a asp-action="Index">Back to List</a>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

### Ansichten f�r das L�schen eines *Genre* (Delete.cshtml)

```csharp
@model QTMusicStoreLight.AspMvc.Models.Genre

@{
    ViewData["Title"] = "Delete";
}

<h1>Delete</h1>

@if (string.IsNullOrEmpty(ViewBag.Error) == false)
{
    <div class="alert alert-danger" role="alert">
        @ViewBag.Error
    </div>
}

<h3>Are you sure you want to delete this?</h3>
<div>
    <h4>Genre</h4>
    <hr />
    <dl class="row">
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.Name)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.Name)
        </dd>
    </dl>
    
    <form asp-action="Delete">
        <input type="hidden" asp-for="Id" />
        <input type="submit" value="Delete" class="btn btn-danger" /> |
        <a asp-action="Index">Back to List</a>
    </form>
</div>
```

### Ansichten f�r die �bersicht von *Album* (Index.cshtml)

```csharp
@model IEnumerable<QTMusicStoreLight.AspMvc.Models.Album>

@{
    ViewData["Title"] = "Index";
}

<h1>Index</h1>

<p>
    <a asp-action="Create">Create New</a>
</p>
<table class="table">
    <thead>
        <tr>
            <th>
                @Html.DisplayNameFor(model => model.Title)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.ArtistName)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.GenreName)
            </th>
            <th></th>
        </tr>
    </thead>
    <tbody>
@foreach (var item in Model) {
        <tr>
            <td>
                @Html.DisplayFor(modelItem => item.Title)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.ArtistName)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.GenreName)
            </td>
            <td>
                <a asp-action="Edit" asp-route-id="@item.Id">Edit</a> |
                <a asp-action="Details" asp-route-id="@item.Id">Details</a> |
                <a asp-action="Delete" asp-route-id="@item.Id">Delete</a>
            </td>
        </tr>
}
    </tbody>
</table>
```

### Ansichten f�r das Erstellen eines *Album* (Create.cshtml)

```csharp
@model QTMusicStoreLight.AspMvc.Models.Album

@{
    var artistList = new SelectList(Model.Artists, "Id", "Name", Model.ArtistId);
    var genreList = new SelectList(Model.Genres, "Id", "Name", Model.GenreId);

    ViewData["Title"] = "Create";

}

<h1>Create</h1>

<h4>Album</h4>
<hr />

@if (string.IsNullOrEmpty(ViewBag.Error) == false)
{
    <div class="alert alert-danger" role="alert">
        @ViewBag.Error
    </div>
}

<div class="row">
    <div class="col-md-4">
        <form asp-action="Create">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <div class="form-group">
                <label asp-for="Title" class="control-label"></label>
                <input asp-for="Title" class="form-control" />
                <span asp-validation-for="Title" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Artists" class="control-label"></label>
                @Html.DropDownListFor(m => m.ArtistId, artistList, null, new { @class = "form-select" })
            </div>
            <div class="form-group">
                <label asp-for="Genres" class="control-label"></label>
                @Html.DropDownListFor(m => m.GenreId, genreList, null, new { @class = "form-select" })
            </div>
            <br />
            <div class="form-group">
                <input type="submit" value="Create" class="btn btn-primary" />
            </div>
        </form>
    </div>
</div>

<div>
    <a asp-action="Index">Back to List</a>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

### Ansichten f�r das Bearbeiten eines *Album* (Edit.cshtml)

```csharp
@model QTMusicStoreLight.AspMvc.Models.Album

@{
    var artistList = new SelectList(Model.Artists, "Id", "Name", Model.ArtistId);
    var genreList = new SelectList(Model.Genres, "Id", "Name", Model.GenreId);

    ViewData["Title"] = "Edit";
}

<h1>Edit</h1>

@if (string.IsNullOrEmpty(ViewBag.Error) == false)
{
    <div class="alert alert-danger" role="alert">
        @ViewBag.Error
    </div>
}

<h4>Artist</h4>
<hr />
<div class="row">
    <div class="col-md-4">
        <form asp-action="Edit">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <div class="form-group">
                <label asp-for="Title" class="control-label"></label>
                <input asp-for="Title" class="form-control" />
                <span asp-validation-for="Title" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Artists" class="control-label"></label>
                @Html.DropDownListFor(m => m.ArtistId, artistList, null, new { @class = "form-select" })
            </div>
            <div class="form-group">
                <label asp-for="Genres" class="control-label"></label>
                @Html.DropDownListFor(m => m.GenreId, genreList, null, new { @class = "form-select" })
            </div>
            <br />
            <input type="hidden" asp-for="Id" />
            <div class="form-group">
                <input type="submit" value="Save" class="btn btn-primary" />
            </div>
        </form>
    </div>
</div>

<div>
    <a asp-action="Index">Back to List</a>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

### Ansichten f�r das L�schen eines *Album* (Delete.cshtml)

```csharp
@model QTMusicStoreLight.AspMvc.Models.Album

@{
    ViewData["Title"] = "Delete";
}

<h1>Delete</h1>

@if (string.IsNullOrEmpty(ViewBag.Error) == false)
{
    <div class="alert alert-danger" role="alert">
        @ViewBag.Error
    </div>
}

<h3>Are you sure you want to delete this?</h3>
<div>
    <h4>Album</h4>
    <hr />
    <dl class="row">
        <dt class = "col-sm-2">
            @Html.DisplayNameFor(model => model.Title)
        </dt>
        <dd class = "col-sm-10">
            @Html.DisplayFor(model => model.Title)
        </dd>
    </dl>
    
    <form asp-action="Delete">
        <input type="hidden" asp-for="Id" />
        <input type="submit" value="Delete" class="btn btn-danger" /> |
        <a asp-action="Index">Back to List</a>
    </form>
</div>
```

**Viel Spa� beim Testen!**
