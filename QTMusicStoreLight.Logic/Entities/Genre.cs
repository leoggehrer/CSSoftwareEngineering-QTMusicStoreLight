using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
