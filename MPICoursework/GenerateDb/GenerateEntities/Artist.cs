namespace MPICoursework.GenerateDb.GenerateEntities
{
    public class Artist
    {
        // Id исполнителя
        public int ArtistId { get; set; }
        // Имя исполнителя
        public string Name { get; set; }
        // Жанр исполнителя
        public string Genre { get; set; }
        // Страна исполнителя
        public string Country { get; set; }
        // Список треков исполнителя
        public ICollection<Track>? Tracks { get; set; }
    }
}
