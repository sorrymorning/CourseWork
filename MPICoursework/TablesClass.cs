using MPICoursework.GenerateDb.GenerateEntities;

namespace MPICoursework
{
    // Класс для хранение списков таблиц
    public class TablesClass
    {
        // Список заявок
        public List<Album> AlbumList { get; set; }
        // Список менеджеров
        public List<Artist> ArtistList { get; set; }
        // Список пользователей
        public List<Playlist> PlaylistList { get; set; }
        // Список статусов
        public List<Track> TrackList { get; set; }
    }
}
