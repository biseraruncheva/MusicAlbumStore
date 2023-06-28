using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MusicAlbumStore.Models
{
    public class MusicAlbum
    {
        public int Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Music Album Name")]
        [Required] 
        public string MusicAlbumName { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? ReleaseDate { get; set; }

        [StringLength(int.MaxValue)]
        public string? Description { get; set; }

        [StringLength(int.MaxValue)]
        public string? Language { get; set; }

        [Display(Name = "Length")]
        public int? LengthInMinutes { get; set; }


        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Display(Name = "Number of songs")]
        public int NumSongs { get; set; }

        [Display(Name = "Cover Image")]
        [StringLength(int.MaxValue)]

        public string? CoverImage { get; set; }

        [Display(Name = "PDF URL")]
        [StringLength(int.MaxValue)]

        public string? PdfUrl { get; set; }

        [Display(Name = "Artist")]
        public int ArtistId { get; set; }
        public Artist? Artist { get; set; }

        public ICollection<MusicAlbumGenre>? Genres { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}
