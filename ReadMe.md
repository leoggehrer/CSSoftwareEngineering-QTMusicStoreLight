
# QTMusicStoreLight

Dieses Projekt ist mit der Vorlage ***QuickTemplate*** erstellt worden und wird nun Schritt für Schritt vom ***'Backend'*** bis zum ***'Frontend'*** entwickelt - also eine ***'Full Stack Application'***. Die Beschreibung für die Erstellung diese Projektbasis ist auf [Github-QuickTemplate](https://github.com/leoggehrer/CSSoftwareEngineering-QuickTemplate) dokumentiert.   

# Inhaltsverzeichnis
1. [Projektbeschreibung](#Projektbeschreibung)
   1. [Definition von Artist](#definition-von-artist)
   2. [Definition von Album](#definition-von-album)
   3. [Definition von Genre](#definition-von-genre)
2. [Erstellen der Entitäten](#erstellen-der-entitaeten)
3. [Erstellen der Kontroller](#erstellen-der-kontroller)
4. [Erstellen der Datenbank](#erstellen-der-datenbank)
5. [Importieren von Daten](#importieren-von-daten)

# Projektbeschreibung  

***QTMusicStoreLight*** ist eine einfache Anwendung zur Verwaltung von Musik-Daten. Damit sollen Künstler (Artists) und deren Alben in einer Datenbank gespeichert werden und bei Bedarf wieder abgerufen werden können. Die Bearbeitung der Daten soll über eine Web-Anwendung als auch über eine Desktop Anwendung ermöglicht werden. Das Datenmodell ist in der folgenden Abbildung skizziert.

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

Ein Künstler kann beliebig viele Alben zugeordnet haben und das Album ist mit einer Musikrichtung (Genre) verbunden. Das Datenmodell für den **MusicStoreLight** ist wie folgt definiert:


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


# Erstellen der Entitäten<a name="erstellen-der-entitaeten"/>  

Die Entitäten werden im Projekt ***QTMusicStoreLight.Logic*** im Ordner ***Entities*** definiert. Nachdem für die Entitäten als Zugriffsstrategie ***Cuncurrency Optimistic*** verwendet wird (RowVersion ist definiert), werden die Entitäten vom bereits definierten ***VersionObject*** abgeleitet.

Die Implementierung der Entität ***Artist***:  

```csharp
namespace QTMusicStoreLight.Logic.Entities
{
    [Table("Artists", Schema = "App")]
    [Index(nameof(Name), IsUnique = true)]
    public class Artist : VersionObject
    {
        [Required]
        [MaxLength(128)]
        public string Name { get; set; } = String.Empty;

        // Navigation properties
        public List<Album> Albums { get; set; } = new();
    }
}
```

Die Implementierung der Entität ***Genre***:  

```csharp
namespace QTMusicStoreLight.Logic.Entities
{
    [Table("Genres", Schema = "App")]
    [Index(nameof(Name), IsUnique = true)]
    public class Genre : VersionObject
    {
        [Required]
        [MaxLength(128)]
        public string Name { get; set; } = String.Empty;

        // Navigation properties
        public List<Album> Albums { get; set; } = new();
    }
}
```

Die Implementierung der Entität ***Album***:  

```csharp
namespace QTMusicStoreLight.Logic.Entities
{
    [Table("Albums", Schema = "App")]
    [Index(nameof(Title), IsUnique = true)]
    public class Album : VersionObject
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

# Definition vom Datenbank-Kontext  

Nachdem die Entitäten definiert sind, wird nun der Datenbank-Kontext für die Anwendung fertiggesetllt. Aus der Vorlage ***QuickTemplate*** ist der vordefinierte Datenbank-Kontext ***'ProjectDbContext'*** kopiert worden. Diese beinhaltet bereits eine Standard-Implementierung für den generischen Kontroller ***'GenericController&lt;E&gt;'*** und ist als eine **'partielle Klasse'** ausgeführt. Damit diese Klasse angepasst werden kann, wird eine weitere **'partielle Klasse'** zur Klasse ***'ProjectDbContext'*** erstellt. Dazu wird eine Datei mit dem Namen ***'ProjectDbContextExt.cs'*** erstellt. Die Erweiterung ist nachfolgend definiert:

```csharp
using QTMusicStoreLight.Logic.Entities;

namespace QTMusicStoreLight.Logic.DataContext
{
    partial class ProjectDbContext
    {
        public DbSet<Artist>? ArtistSet { get; set; }
        public DbSet<Album>? AlbumSet { get; set; }
        public DbSet<Genre>? GenreSet { get; set; }

        partial void GetDbSet<E>(ref DbSet<E>? dbSet, ref bool handled) where E : IdentityObject
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

Wie aus der obigen Implementierung ersichtlich ist, werden für die entsprechenden Entitäten die ***'DbSet&lt;E&gt;'*** definiert. Zusätzlich wird die partielle Methode ***'GetDbSet&lt;E&gt;(...)'*** definiert. Über diese Methode greift der ***'GenericController&lt;E&gt;'*** auf den konkreten ***'DbSet&lt;E&gt;'*** zu.

# Erstellen der Kontroller  

Der generische Kontroller ***'GenericController&lt;E&gt;'*** implementiert bereits die ***'CRUD'*** Funktionen für eine Entität. Um diese Funktionen für jede Entität zur Verfügung zu stellen, muss für jede Entität ein eigener Kontroller angelegt werden. Die Kontroller werden im Projekt ***QTMusicStoreLight.Logic*** im Ordner ***Controllers*** definiert.  

## Kontroller für die Entität *Artist* erstellen

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

## Kontroller für die Entität *Album* erstellen

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

## Kontroller für die Entität *Genre* erstellen

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

> **ACHTUNG:**  Die konkreten Kontroller werden mit dem Schlüsselwort ***'sealed'*** spezifiziert. Die Erklärung folgt zu einem späteren Zeitpunkt.  

# Erstellen der Datenbank

Dieser Abschnitt erläutert die Erstellung der Datenbank.

**Notwendige Voraussetzungen:**  
* Die gesamte Projektmappe muss fehlerfrei Übersetzt werden können.
Die Verbindungszeichenfolge kann im Projekt **QTMusicStoreLight.ConApp** in der Datei **appsettings.Development.json** angepasst werden.
* Als Startprojekt muss das Projekt **QTMusicStoreLight.ConApp** eingestellt sein.
* In der **Package Manager Console** muss das **Default projekt: QTMusicStoreLight.Logic** ausgewählt sein.

**Wichtige Kommandos:**

> PM> add-migration InitDb

...diese Kommando erzeugt eine Migration mit dem Namen 'InitDb'.

> PM> update-database

...diese Kommando führt die Migration aus und erzeugt die Datenbank.

# Importieren von Daten  

Der Import wird im Projekt **QTMusicStoreLight.ConApp** in der Datei **Program.cs** implementiert. Zu diesem Zweck werden die Daten, im csv-Format, vom [GitHub-MusicStoreDaten](https://github.com/leoggehrer/Data-MusicStore) heruntergeladen und im Ordner **CsvData** abgelegt. In den Eigenschaften der csv-Dateien wird die Eigenschaft **Copy to Output Directory** auf **Copy if newer** eingestellt. Mit dieser Einstellung müssen keine Pfade angegeben werden weil die Dateien in das Ausführungsverzeichnis kopiert werden. Nachfolgend ist der Programm-Code für den Import angegeben:  

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

In der Zeile ***await albumsCtrl.InsertAsync(albums);*** werden die Objekte in den Datenbank-Kontext übertragen. Mit der Anweisung ***await albumsCtrl.SaveChangesAsync();*** werden die Daten in die Datenbank eingetragen. Alle referenzierten Objekte (z.B.: Album -> Artist) werden ebenfalls vom Kontroller in die Datenbank übermittelt.  

**Viel Spaß beim Testen!**
