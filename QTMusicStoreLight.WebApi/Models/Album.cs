namespace QTMusicStoreLight.WebApi.Models
{
    public class Album : VersionModel
    {
        public int ArtistId { get; set; }
        public int GenreId { get; set; }
        public string? Title { get; set; }
    }
}
