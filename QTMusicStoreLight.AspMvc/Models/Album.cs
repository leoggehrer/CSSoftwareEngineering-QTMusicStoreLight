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
