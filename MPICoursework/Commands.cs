using Microsoft.EntityFrameworkCore;
using MPICoursework.GenerateDb.GenerateEntities;

namespace MPICoursework
{
    public static class Commands
    {
        // Сохранить все изменения в локальной коллекции в базу
        public static int SaveLocalDb(TablesClass localDb)
        {
            using (var db = new AppDbContext())
            {
                // Обновить изменения в таблице Actors
                db.Actors.UpdateRange(localDb.ActorList);
                // Обновить изменения в таблице Directors
                db.Directors.UpdateRange(localDb.DirectorList);
                // Обновить изменения в таблице Movies
                db.Movies.UpdateRange(localDb.MovieList);

                // Если при сохранении базы данных возникнет ошибка, вернется 0, иначе 1
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    // Логирование ошибки (если нужно)
                    Console.WriteLine($"Ошибка при сохранении данных: {ex.Message}");
                    return 0;
                }
                return 1;
            }
        }


        // Всех онлайн пользователй перевести в оффлайн
        public static int SetDefaultBirthYear(TablesClass localDb)
        {
            // Список актеров
            List<Actor> actors = localDb.ActorList.ToList();
            // Список режиссеров
            List<Director> directors = localDb.DirectorList.ToList();

            // Если списки актеров и режиссеров пустые
            if (!actors.Any() && !directors.Any())
                return 0;

            // Установим дату рождения на 1 января 1900 года как условный показатель
            DateTime defaultDate = new DateTime(1900, 1, 1);

            // Перебираем список актеров
            foreach (var actor in actors)
            {
                // Изменяем дату рождения
                actor.BirthDate = defaultDate;
            }

            // Перебираем список режиссеров
            foreach (var director in directors)
            {
                // Изменяем дату рождения
                director.BirthDate = defaultDate;
            }

            return 1;
        }


        // Увеличить возраст всех пользователей на 1 год
        public static int AddAge(TablesClass localDb)
        {
            // Перебрать каждого актера в списке
            foreach (var actor in localDb.ActorList)
            {
                // Увеличить текущий возраст на 1
                actor.Age = actor.Age + 1;
            }

            // Перебрать каждого режиссера в списке
            foreach (var director in localDb.DirectorList)
            {
                // Увеличить текущий возраст на 1
                director.Age = director.Age + 1;
            }

            // Вернуть успешный код (1)
            return 1;
        }

        // Подсчет количества актеров, участвующих в фильмах (старше 17 лет):
        public static int CountActorsInMovies(TablesClass localDb)
        {
            return localDb.ActorList
                .Where(actor => actor.Age > 17 && localDb.MovieActorList.Any(ma => ma.ActorId == actor.Id))
                .Count();
        }


        // Вычислить количество заявок для каждого пользователя
        public static List<MoviesCount> SumMovies(TablesClass localDb)
        {
            return localDb.MovieList.GroupBy(p => p.DirectorId).Select(g => new MoviesCount // Новая таблица после группировки
            {
                // Id режиссера после группировки
                DirectorId = g.Select(p => p.DirectorId).First(),
                // Имя режиссера
                Name = g.Select(p => p.Director.FirstName).First(),
                // Фамилия режиссера
                Surname = g.Select(p => p.Director.LastName).First(),
                // Количество фильмов
                Count = g.Count()
            }).ToList();
        }
        // Найти минимальный возраст среди менеджеров
        // Найти минимальный возраст среди актеров
        public static int MinAge(TablesClass localDb)
        {
            // Если таблица с актерами не пустая, то вернуть минимальный возраст
            if (localDb.ActorList.Any())
                return localDb.ActorList.Min(p => p.Age);
            // Если таблица пустая, то вернуть 0
            else
                return 0;
        }

        // Найти максимальный возраст среди актеров
        public static int MaxAge(TablesClass localDb)
        {
            // Если таблица с актерами не пустая, то вернуть максимальный возраст
            if (localDb.ActorList.Any())
                return localDb.ActorList.Max(p => p.Age);
            // Если таблица пустая, то вернуть 0
            else
                return 0;
        }

        // Предзагрузка таблиц
        public static TablesClass PreLoading(int rank, int size)
        {
            using (var db = new AppDbContext())
            {
                // Количество строк в каждой таблице
                int countMovies = db.Movies.Count();
                int countDirectors = db.Directors.Count();
                int countActors = db.Actors.Count();
                int countGenres = db.Genres.Count();
                int countMovieActors = db.MovieActors.Count();

                // Размер части для каждой таблицы
                int partMovies = (countMovies / size + 1);
                int partDirectors = (countDirectors / size + 1);
                int partActors = (countActors / size + 1);
                int partGenres = (countGenres / size + 1);
                int partMovieActors = (countMovieActors / size + 1);

                // Смещение по каждой таблице
                int offsetMovies = rank * partMovies;
                int offsetDirectors = rank * partDirectors;
                int offsetActors = rank * partActors;
                int offsetGenres = rank * partGenres;
                int offsetMovieActors = rank * partMovieActors;

                return new TablesClass
                {
                    // Заполнение списка фильмов из базы данных
                    MovieList = db.Movies.Include(m => m.Director)
                                         .Include(m => m.Genre)
                                         .Skip(offsetMovies)
                                         .Take(partMovies)
                                         .OrderBy(m => m.Id)
                                         .ToList(),
                    // Заполнение списка режиссеров из базы данных
                    DirectorList = db.Directors.Skip(offsetDirectors)
                                               .Take(partDirectors)
                                               .OrderBy(d => d.Id)
                                               .ToList(),
                    // Заполнение списка актеров из базы данных
                    ActorList = db.Actors.Skip(offsetActors)
                                         .Take(partActors)
                                         .OrderBy(a => a.Id)
                                         .ToList(),
                    // Заполнение списка жанров из базы данных
                    GenreList = db.Genres.Skip(offsetGenres)
                                         .Take(partGenres)
                                         .OrderBy(g => g.Id)
                                         .ToList(),
                    // Заполнение списка связей фильмов и актеров
                    MovieActorList = db.MovieActors.Include(ma => ma.Actor)
                                                   .Include(ma => ma.Movie)
                                                   .Skip(offsetMovieActors)
                                                   .Take(partMovieActors)
                                                   .OrderBy(ma => ma.MovieId)
                                                   .ToList()
                };
            }
        }

        // Сгенерировать базу данных с count строк
        public static void CreateDatabase(int movieCount)
        {
            using (var db = new AppDbContext())
            {
                // Добавление жанров
                if (!db.Genres.Any())
                {
                    var genres = new List<Genre>
            {
                new Genre { Id = 1, Name = "Action" },
                new Genre { Id = 2, Name = "Comedy" },
                new Genre { Id = 3, Name = "Drama" },
                new Genre { Id = 4, Name = "Horror" },
                new Genre { Id = 5, Name = "Sci-Fi" }
            };
                    db.Genres.AddRange(genres);
                    db.SaveChanges();
                }

                Random rand = new Random();

                // Списки режиссеров и актеров
                List<Director> directors = new List<Director>();
                List<Actor> actors = new List<Actor>();

                // Генерация данных для режиссеров
                if (!db.Directors.Any())
                {
                    for (int i = 0; i < 10; i++) // Создаем 10 режиссеров
                    {
                        var director = new Director
                        {
                            FirstName = Faker.Name.First(),
                            LastName = Faker.Name.Last(),
                            Age = rand.Next(30, 70)
                        };
                        directors.Add(director);
                        db.Directors.Add(director);
                    }
                    db.SaveChanges();
                }
                else
                {
                    directors = db.Directors.ToList();
                }

                // Генерация данных для актеров
                if (!db.Actors.Any())
                {
                    for (int i = 0; i < 50; i++) // Создаем 50 актеров
                    {
                        var actor = new Actor
                        {
                            FirstName = Faker.Name.First(),
                            LastName = Faker.Name.Last(),
                            Age = rand.Next(20, 60)
                        };
                        actors.Add(actor);
                        db.Actors.Add(actor);
                    }
                    db.SaveChanges();
                }
                else
                {
                    actors = db.Actors.ToList();
                }

                // Генерация данных для фильмов
                for (int i = 0; i < movieCount; i++)
                {
                    var movie = new Movie
                    {
                        Title = Faker.Lorem.Sentence(3), // Случайное название фильма
                        ReleaseYear = rand.Next(1980, 2025),
                        Duration = rand.Next(90, 180), // Продолжительность от 90 до 180 минут
                        GenreId = rand.Next(1, 6), // Id жанра (1-5)
                        DirectorId = directors[rand.Next(directors.Count)].Id
                    };
                    db.Movies.Add(movie);
                    db.SaveChanges();

                    // Связываем фильм с актерами
                    int actorCount = rand.Next(3, 7); // В фильме от 3 до 6 актеров
                    var existingActors = new HashSet<int>(); // Для проверки уникальности актеров

                    for (int j = 0; j < actorCount; j++)
                    {
                        int actorId;
                        do
                        {
                            actorId = actors[rand.Next(actors.Count)].Id;
                        } while (existingActors.Contains(actorId)); // Убедиться, что актер не был добавлен ранее

                        existingActors.Add(actorId);

                        db.MovieActors.Add(new MovieActor
                        {
                            MovieId = movie.Id,
                            ActorId = actorId
                        });
                    }
                }

                // Сохраняем изменения
                db.SaveChanges();
            }
        }


    }
}