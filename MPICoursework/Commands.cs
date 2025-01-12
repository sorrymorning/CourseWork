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
                db.Albums.UpdateRange(localDb.AlbumList);
                db.Artists.UpdateRange(localDb.ArtistList);
                db.Playlists.UpdateRange(localDb.PlaylistList);
                db.Tracks.UpdateRange(localDb.TrackList);
                // Если при сохранении базы данных возникнет ошибка, вернется 0, иначе 1
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    return 0;
                }
                return 1;
            }

        }

        //// Всех онлайн пользователй перевести в оффлайн
        public static int SetOffline(TablesClass localDb)
        {
            // Поиск записи с полем Country, равным "Greece"
            List<Artist> artists = localDb.ArtistList.Where(p => p.Country == "Greece").ToList();
            //Console.WriteLine($"Found {greeceArtists.Count} artists from Greece.");
           
            if (!artists.Any())
                return 0;

            // Перебрать найденных артистов из Греции
            foreach (var artist in artists)
            {
                artist.Genre = "Classic";
            }

            return 1;
        }

        //// Увеличить возраст всех пользователей на 1 год
        public static int AddPlays(TablesClass localDb)
        {
            // Перебрать каждого пользователя в списке
            foreach (var user in localDb.TrackList)
            {
                // Увеличить текущее значение возраста на 1
                user.Plays = user.Plays + 1000;
            }
            return 1;
        }
        //// Вывести количество пользователей с возрастом более 17 лет в статусе online
        public static int CountPlaysTenThousand(TablesClass localDb)
        {
            return localDb.TrackList.Where(p => p.Plays > 10000).Count();
        }
        // Вычислить количество заявок для каждого пользователя
        public static List<AlbumPlays> SumPlaysByAlbum(TablesClass localDb)
        {
            return localDb.TrackList.GroupBy(p => p.AlbumId).Select(g => new AlbumPlays
            {
                // Название альбома
                AlbumName = g.Select(p => p.Album.Title).First(),
                // Сумма прослушиваний всех треков альбома
                TotalPlays = g.Sum(p => p.Plays)
            }).ToList();
        }

        //// Найти минимальный возраст среди менеджеров
        public static int MinAge(TablesClass localDb)
        {
            // Если таблица с менеджерами не пустая, то вернуть минимальный возраст
            if (localDb.TrackList.Any())
                return localDb.TrackList.Min(p => p.Plays);
            // Если таблица пустая, то вернуть 0
            else
                return 0;
        }
        //// Найти максимальный возраст среди менеджеров
        public static int MaxAge(TablesClass localDb)
        {
            // Если таблица с менеджерами не пустая, то вернуть максимальный возраст
            if (localDb.TrackList.Any())
                return localDb.TrackList.Max(p => p.Plays);
            // Если таблица пустая, то вернуть 0
            else
                return 0;
        }
        //// Предзагрузка таблиц
        public static TablesClass PreLoading(int rank, int size)
        {
            using (var db = new AppDbContext())
            {
                // Количество строк в каждой таблице
                int countTracks = db.Tracks.Count();
                int countArtists = db.Artists.Count();
                int countAlbums = db.Albums.Count();
                int countPlaylists = db.Playlists.Count();

                // Размер части для каждой таблицы
                int partTracks = (countTracks / size + 1);
                int partArtists = (countArtists / size + 1);
                int partAlbums = (countAlbums / size + 1);
                int partPlaylists = (countPlaylists / size + 1);

                // Смещение по каждой таблице
                int offsetTracks = rank * partTracks;
                int offsetArtists = rank * partArtists;
                int offsetAlbums = rank * partAlbums;
                int offsetPlaylists = rank * partPlaylists;

                return new TablesClass
                {
                    // Заполнение списка треков из базы данных
                    TrackList = db.Tracks
                        .Include(t => t.Album)
                        //.Include(t => t.Artist)
                        .Skip(offsetTracks)
                        .Take(partTracks)
                        .OrderBy(t => t.TrackId)
                        .ToList(),

                    // Заполнение списка исполнителей из базы данных
                    ArtistList = db.Artists
                        //.Include(a => a.Albums)
                        .Include(a => a.Tracks)
                        .Skip(offsetArtists)
                        .Take(partArtists)
                        .OrderBy(a => a.ArtistId)
                        .ToList(),

                    // Заполнение списка альбомов из базы данных
                    AlbumList = db.Albums
                        .Include(a => a.Artist)
                        .Include(a => a.Tracks)
                        .Skip(offsetAlbums)
                        .Take(partAlbums)
                        .OrderBy(a => a.AlbumId)
                        .ToList(),

                    // Заполнение списка плейлистов из базы данных
                    PlaylistList = db.Playlists
                        .Include(p => p.Tracks)
                            //.ThenInclude(pt => pt.Track)
                        .Skip(offsetPlaylists)
                        .Take(partPlaylists)
                        .OrderBy(p => p.PlaylistId)
                        .ToList()
                };
            }
        }

        // Сгенерировать базу данных с count строк
        public static void CreateDatabase(int count)
        {
            using (var db = new AppDbContext())
            {
                Random rand = new Random();

                // Создание и добавление исполнителей
                var artists = new List<Artist>();
                for (int i = 0; i < count / 2; i++)
                {
                    var artist = new Artist
                    {
                        Name = Faker.Name.FullName(),
                        Genre = Faker.Lorem.Words(1).First(),
                        Country = Faker.Address.Country()
                    };
                    artists.Add(artist);
                    db.Artists.Add(artist);
                }

                // Сохранение изменений в базе данных
                db.SaveChanges();

                // Создание и добавление альбомов

                var albums = new List<Album>();
                foreach (var artist in artists)
                {
                    for (int i = 0; i < rand.Next(1, 5); i++)
                    {
                        var album = new Album
                        {
                            Title = Faker.Lorem.Sentence(3),
                            ReleaseDate = Faker.Identification.DateOfBirth(),
                            ArtistId = artist.ArtistId
                        };
                        albums.Add(album);
                        db.Albums.Add(album);
                    }
                }

                // Сохранение изменений в базе данных
                db.SaveChanges();

                // Создание и добавление треков
                var tracks = new List<Track>();
                foreach (var album in albums)
                {
                    for (int i = 0; i < rand.Next(5, 15); i++)
                    {
                        var track = new Track
                        {
                            Title = Faker.Lorem.Sentence(2),
                            Duration = TimeSpan.FromSeconds(rand.Next(120, 420)),
                            Plays = rand.Next(52, 1000000),
                            ReleaseDate = album.ReleaseDate.AddDays(rand.Next(1, 365)),
                            AlbumId = album.AlbumId
                        };
                        tracks.Add(track);
                        db.Tracks.Add(track);
                    }
                }

                // Сохранение изменений в базе данных
                db.SaveChanges();

                // Создание и добавление плейлистов
                for (int i = 0; i < count / 4; i++)
                {
                    var playlist = new Playlist
                    {
                        Name = Faker.Lorem.Sentence(2),
                        CreationDate = DateTime.Now.AddDays(-rand.Next(1, 1000)),
                        Tracks = tracks.OrderBy(t => rand.Next()).Take(rand.Next(5, 20)).ToList()
                    };
                    db.Playlists.Add(playlist);
                }

                // Сохранение изменений в базе данных
                db.SaveChanges();
            }
        }
    }
}