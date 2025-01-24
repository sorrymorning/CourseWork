using MPICoursework.GenerateDb.GenerateEntities;

namespace MPICoursework
{
    // Класс для хранение списков таблиц
    public class TablesClass
    {
        // Список заявок
        public List<Movie> MovieList { get; set; }
        // Список менеджеров
        public List<Director> DirectorList { get; set; }
        // Список пользователей
        public List<Genre> GenreList { get; set; }
        // Список статусов
        public List<Actor> ActorList { get; set; }

        public List<MovieActor> MovieActorList { get; set; }
    }
}
