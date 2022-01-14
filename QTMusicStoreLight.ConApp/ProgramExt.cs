using System.IO;
using System.Linq;

namespace QTMusicStoreLight.ConApp
{
    partial class Program
    {
        static partial void AfterRun()
        {
            var genres = File.ReadAllLines("CsvData/Genre.csv")
                             .Skip(1)
                             .Select(l => l.Split(";"))
                             .Select(d => new { id = d[0], Entity = new Logic.Entities.Genre { Name = d[1] }});
            var artists = File.ReadAllLines("CsvData/Artist.csv")
                             .Skip(1)
                             .Select(l => l.Split(";"))
                             .Select(d => new { id = d[0], Entity = new Logic.Entities.Artist { Name = d[1] }});
            Task.Run(async () =>
            {
                using var artistsCtrl = new Logic.Controllers.ArtistsController();
                using var genresCtrl = new Logic.Controllers.GenresController(artistsCtrl);
                using var albumsCtrl = new Logic.Controllers.AlbumsController(artistsCtrl);

                var arts = (await artistsCtrl.InsertAsync(artists.Select(e => e.Entity))).ToArray();
                var gens = (await genresCtrl.InsertAsync(genres.Select(e => e.Entity))).ToArray();
                await artistsCtrl.SaveChangesAsync();

                var albums = File.ReadAllLines("CsvData/Album.csv")
                                 .Skip(1)
                                 .Select(l => l.Split(";"))
                                 .Select(d => new {
                                     id = d[0],
                                     Entity = new Logic.Entities.Album
                                     {
                                         Title = d[1],
                                         Artist = artists.First(e => e.id == d[2]).Entity,
                                         Genre = genres.First(e => e.id == d[3]).Entity,
                                     }
                                 });

                await albumsCtrl.InsertAsync(albums.Select(e => e.Entity));
                await albumsCtrl.SaveChangesAsync();
            }).Wait();
        }
    }
}
