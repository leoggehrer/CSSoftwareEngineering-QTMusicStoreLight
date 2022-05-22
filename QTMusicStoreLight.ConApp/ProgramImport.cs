namespace QTMusicStoreLight.ConApp
{
    partial class Program
    {
        static partial void CreateImport()
        {
            var csvGenres = File.ReadAllLines("Data/Genre.csv")
                             .Skip(1)
                             .Select(l => l.Split(";"))
                             .Select(d => new { id = d[0], Genre = d[1] });
            var csvArtists = File.ReadAllLines("Data/Artist.csv")
                             .Skip(1)
                             .Select(l => l.Split(";"))
                             .Select(d => new { id = d[0], Artist = d[1] });
            var csvAlbums = File.ReadAllLines("Data/Album.csv")
                             .Skip(1)
                             .Select(l => l.Split(";"))
                             .Select(d => new { id = d[0], Album = d[1], ArtistId = d[2] });

            var songs = File.ReadAllLines("Data/Track.csv")
                             .Skip(1)
                             .Select(l => l.Split(";"))
                             .Select(d => new Logic.Entities.App.Song
                             { 
                                 Album = csvAlbums.First(e => e.id == d[2]).Album,
                                 Artist = csvArtists.First(e => e.id == csvAlbums.First(e => e.id == d[2]).ArtistId).Artist,
                                 Genre = csvGenres.First(e => e.id == d[3]).Genre,
                                 Title = d[1],
                                 Composer = d[4].Equals("<NULL>") ? null : d[4],
                                 Millisconds = Convert.ToInt64(d[5]),
                                 Bytes = Convert.ToInt64(d[6]),
                                 UnitPrice = decimal.Parse(d[7], System.Globalization.CultureInfo.CurrentCulture),
                             }).OrderBy(e => e.Album)
                               .ToArray();

            Task.Run(async () =>
            {
                using var songsCtrl = new Logic.Controllers.SongsController();

                await songsCtrl.InsertAsync(songs);
                await songsCtrl.SaveChangesAsync();
            }).Wait();
        }
    }
}