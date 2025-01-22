namespace MPICoursework.GenerateDb.GenerateEntities
{
    public class Track
    {
        // Id трека
        public int TrackId { get; set; }
        // Название трека
        public string Title { get; set; }
        // Длительность трека
        public TimeSpan Duration { get; set; }
        // Количество прослушиваний
        public int Plays { get; set; }
        // Дата выпуска трека
        public DateTime ReleaseDate { get; set; }
        // FK на альбом
        public int AlbumId { get; set; }
        //public int ArtistId { get; set; }
        // Навигационное свойство для альбома
        public Album? Album { get; set; }





    }
}
