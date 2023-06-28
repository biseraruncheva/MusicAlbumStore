using MusicAlbumStore.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MusicAlbumStore.Models
{
    public class MusicAlbumUser
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public MusicAlbumStoreUser? User { get; set; }

        [Display(Name = "Music Album")]
        public int MusicAlbumId { get; set; }
        public MusicAlbum? MusicAlbum { get; set; }
    }
}
