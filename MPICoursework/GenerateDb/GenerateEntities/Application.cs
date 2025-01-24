namespace MPICoursework.GenerateDb.GenerateEntities
{
    public class Movie
    {
        // Id фильма
        public int Id { get; set; }
        // Название фильма
        public string Title { get; set; }
        // Год выпуска
        public int ReleaseYear { get; set; }
        // Продолжительность (в минутах)
        public int Duration { get; set; }
        // FK жанр
        public int GenreId { get; set; }
        // FK навигационное свойство жанра
        public Genre Genre { get; set; }
        // FK режиссер
        public int DirectorId { get; set; }
        // FK навигационное свойство режиссера
        public Director Director { get; set; }

        public ICollection<MovieActor> MovieActors { get; set; }
    }



    public class Director
    {
        // Id режиссера
        public int Id { get; set; }
        // Имя режиссера
        public string FirstName { get; set; }
        // Фамилия режиссера
        public string LastName { get; set; }
        // Возраст
        public int Age { get; set; }

        public DateTime BirthDate { get; set; }
    }

    public class Genre
    {
        // Id жанра
        public int Id { get; set; }
        // Название жанра
        public string Name { get; set; }
    }

    public class Actor
    {
        // Id актера
        public int Id { get; set; }
        // Имя актера
        public string FirstName { get; set; }
        // Фамилия актера
        public string LastName { get; set; }
        // Возраст
        public int Age { get; set; }

        public DateTime BirthDate { get; set; }
        public ICollection<MovieActor> MovieActors { get; set; }

    }

    public class MovieActor
    {
        // Id фильма
        public int MovieId { get; set; }
        // FK навигационное свойство фильма
        public Movie Movie { get; set; }
        // Id актера
        public int ActorId { get; set; }
        // FK навигационное свойство актера
        public Actor Actor { get; set; }
    }
}

