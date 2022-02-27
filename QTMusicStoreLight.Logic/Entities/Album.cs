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
