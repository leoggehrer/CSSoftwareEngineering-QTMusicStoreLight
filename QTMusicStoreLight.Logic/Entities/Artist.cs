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
