using System.ComponentModel.DataAnnotations;

namespace QTMusicStoreLight.AspMvc.Models
{
    public class Genre : VersionModel
    {
        [Required]
        [MaxLength(128)]
        public string Name { get; set; } = String.Empty;
    }
}
