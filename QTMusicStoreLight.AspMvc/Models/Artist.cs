using System.ComponentModel.DataAnnotations;

namespace QTMusicStoreLight.AspMvc.Models
{
    public class Artist : VersionModel
    {
        [Required]
        [MaxLength(128)]
        public string Name { get; set; } = string.Empty;
    }
}
