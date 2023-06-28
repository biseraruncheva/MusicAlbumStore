using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace MusicAlbumStore.Models
{
    public class Artist
    {
        public int Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Artist Name")]
        [Required] public string ArtistName { get; set; }

        [StringLength(int.MaxValue)]
        public string? Biography { get; set; }

        [StringLength(100)]
        public string? Website { get; set; }

        [StringLength(50)]
        public string? Label { get; set; }

        [Display(Name = "Formation Year")]
        public int? FormationYear { get; set; }

        [Display(Name = "Disbandment Year")]
        public int? DisbandmentYear { get; set; }

        [Display(Name = "Artist Image")]
        [StringLength(int.MaxValue)]

        public string? ArtistImage { get; set; }

        [NotMapped]
        [Display(Name = "Years Active")]
        public string YearsActive
        {
            get
            {
                if (DisbandmentYear == null)
                {
                    return String.Format("{0} - Present", FormationYear);
                }
                else
                {
                    return String.Format("{0} - {1}", FormationYear, DisbandmentYear);
                }
            }
        }

        public ICollection<MusicAlbum>? MusicAlbums { get; set; }



    }
}
