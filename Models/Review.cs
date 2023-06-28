using MusicAlbumStore.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MusicAlbumStore.Models
{
    public class Review
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public MusicAlbumStoreUser? User { get; set; }

        [Required]
        [StringLength(500)]
        public string Comment { get; set; }

        [Display(Name = "Submission Date")]
        public DateTime? SubmissionDate { get; set; }
        public int? Rating { get; set; }

        [Display(Name = "MusicAlbum")]
        public int MusicAlbumId { get; set; }
        public MusicAlbum? MusicAlbum { get; set; }
    }
}
