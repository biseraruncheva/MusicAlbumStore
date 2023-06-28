namespace MusicAlbumStore.Interfaces
{
    public interface IBufferedFileUploadService
    {
        Task<string> UploadFile(IFormFile file);

    }
}
