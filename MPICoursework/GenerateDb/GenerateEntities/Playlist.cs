namespace MPICoursework.GenerateDb.GenerateEntities
{
    public class Playlist
    {
        // Id плейлиста
        public int PlaylistId { get; set; }
        // Название плейлиста
        public string Name { get; set; }
        // Дата создания плейлиста
        public DateTime CreationDate { get; set; }
        // Список треков в плейлисте
        public ICollection<Track>? Tracks { get; set; }
    }
}
