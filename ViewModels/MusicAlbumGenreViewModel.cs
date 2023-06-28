using Microsoft.AspNetCore.Mvc.Rendering;
using MusicAlbumStore.Models;

namespace MusicAlbumStore.ViewModels
{
    public class MusicAlbumGenreViewModel
    {
        public IList<MusicAlbum> MusicAlbums { get; set; }
        public SelectList Genres { get; set; }

        public string MusicAlbumGenre { get; set; }

        public string SearchMusicAlbum { get; set; }

        public string SearchReleaseYear { get; set; }
    }
}
