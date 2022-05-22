namespace QTMusicStoreLight.Logic.Entities.App
{
    [Table("Songs", Schema = "App")]
    [Index(nameof(Album), nameof(Artist), nameof(Genre), nameof(Title), IsUnique = true)]
    public class Song : VersionEntity
    {
        [Required]
        [MaxLength(256)]
        public string Album { get; set; } = string.Empty;
        [Required]
        [MaxLength(128)]
        public string Artist { get; set; } = string.Empty;
        [Required]
        [MaxLength(128)]
        public string Genre { get; set; } = string.Empty;
        [Required]
        [MaxLength(1024)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(512)]
        public string? Composer { get; set; }
        public long Millisconds { get; set; }
        public long Bytes { get; set; }
        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }
    }
}
