namespace MusicAlbumStore.Models
{
    public class MusicAlbumGenre
    {
        public int Id { get; set; }
        public int MusicAlbumId { get; set; }
        public MusicAlbum? MusicAlbum { get; set; }
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }
    }
}
