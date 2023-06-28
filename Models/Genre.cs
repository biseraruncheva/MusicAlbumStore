using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MusicAlbumStore.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Genre")]
        public string GenreName { get; set; }

        public ICollection<MusicAlbumGenre>? MusicAlbumGenres { get; set; }
    }
}
