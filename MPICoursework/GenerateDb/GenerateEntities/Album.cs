namespace MPICoursework.GenerateDb.GenerateEntities
{
    public class Album
    {
        // Id альбома
        public int AlbumId { get; set; }
        // Название альбома
        public string Title { get; set; }
        // Дата выпуска альбома
        public DateTime ReleaseDate { get; set; }
        // FK на исполнителя
        public int ArtistId { get; set; }
        // Навигационное свойство для исполнителя
        public Artist? Artist { get; set; }
        // Список треков альбома
        public ICollection<Track>? Tracks { get; set; }
    }
}
