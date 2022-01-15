using System.IO;
using System.Linq;

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
            var csvAlbums = File.ReadAllLines("CsvData/Album.csv")
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

                albums = (await albumsCtrl.InsertAsync(albums)).ToList();
                await albumsCtrl.SaveChangesAsync();
            }).Wait();
        }
    }
}