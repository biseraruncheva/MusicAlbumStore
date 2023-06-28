using MusicAlbumStore.Models;

namespace MusicAlbumStore.ViewModels
{
    public class ArtistsViewModel
    {
        public IList<Artist> Artists { get; set; }
        public string ArtistNameSearch { get; set; }

        public string FormationYearSearch { get; set; }
    }
}
