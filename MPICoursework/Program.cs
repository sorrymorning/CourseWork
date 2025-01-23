using MPI;
using System.Diagnostics;
using System.Text.Json;

namespace MPICoursework
{
    class Program
    {
        static void Main(string[] args)
        {
            MPI.Environment.Run(ref args, comm =>
            {
                // Создаю объект класса, чтобы он был доступени из всех контекстов
                TablesClass localDataBase = new TablesClass();
                // Предзагрузка базы данных
                // Первым загружает базу 0 процесс, чтобы не было проблемы с одновременным созданием базы данных
                if (comm.Rank == 0)
                    // База данных для 0 процесса
                    localDataBase = Commands.PreLoading(comm.Rank, comm.Size);
                // После того как 0 процесс загрузит базу, то все процессы выйдут из барьера
                comm.Barrier();
                // Все !0 процессы загрузят базу
                if (comm.Rank != 0)
                {
                    // База данных для остальных(не 0) процессов
                    localDataBase = Commands.PreLoading(comm.Rank, comm.Size);
                }
                // Замер времени работы
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                // Команда от пользователя
                string? command = null;
                while (command != "quit")
                {
                    // Получение команды от пользователя
                    if (comm.Rank == 0)
                    {
                        Console.Write("Введите команду \nsave - сохранить базу данных;" +
                                                      "\nscl - установить жанр на классику;" + 
                                                      "\nplays - увеличить прослушивания на 1000;" +
                                                      "\ncount - посчитать количество песен с больше 10000 прослушиваний;" +
                                                      "\nmax - найти максимальное количество прослушиваний;" +
                                                      "\nmin - найти минимальное количество прослушиваний;" +
                                                      "\nsum - вывести альбомы и количество прослушиваний;" +
                                                      "\ncreate - генерация базы данных: ");
                        command = Console.ReadLine();
                    }

                    // Рассылка команды по всем процессам
                    comm.Broadcast(ref command, 0);

                    switch (command)
                    {
                        case "save":
                            // Запуск замера времени
                            stopWatch.Restart();
                            stopWatch.Start();
                            if (comm.Rank == 0)
                            {
                                // Сборка всех данных в 0 процессе с суммированием
                                int count = comm.Reduce(Commands.SaveLocalDb(localDataBase), Operation<int>.Add, 0);
                                // Если число успешных завершений совпало с числом процессов
                                if (count == comm.Size)
                                    Console.WriteLine("База данных выгружена");
                                else
                                    Console.WriteLine("Возникла ошибка");
                            }
                            else
                            {
                                // Сборка всех данных в 0 процессе с суммированием
                                comm.Reduce(Commands.SaveLocalDb(localDataBase), Operation<int>.Add, 0);
                            }
                            stopWatch.Stop();
                            break;
                        case "scl":
                            // Запуск замера времени
                            stopWatch.Restart();
                            stopWatch.Start();
                            if (comm.Rank == 0)
                            {
                                // Сборка всех данных в 0 процессе с суммированием
                                int count = comm.Reduce(Commands.SetOffline(localDataBase), Operation<int>.Add, 0);
                                // Если число успешных выполнений совпало с числом процессов
                                if (count == comm.Size)
                                    Console.WriteLine("Всем артистам поменяли жанр на классику");
                                else
                                    Console.WriteLine("Не получилось поменять жанр на классику");
                            }
                            else
                            {
                                // Сборка всех данных в 0 процессе с суммированием
                                comm.Reduce(Commands.SetOffline(localDataBase), Operation<int>.Add, 0);
                            }
                            stopWatch.Stop();
                            break;
                        case "plays":
                            // Запуск замера времени
                            stopWatch.Restart();
                            stopWatch.Start();
                            if (comm.Rank == 0)
                            {
                                Console.WriteLine("Увеличиваем количество прослушиваний на 1000");
                                // Сборка всех данных в 0 процессе с суммированием
                                int count = comm.Reduce(Commands.AddPlays(localDataBase), Operation<int>.Add, 0);
                                // Если число успешных выполнений совпало с числом процессов
                                if (count == comm.Size)
                                    Console.WriteLine("Прослушивания успешно увеличены на 1000");
                                else
                                    Console.WriteLine("Прослушивания не были увеличены на 1000(((");
                            }
                            else
                            {
                                // Сборка всех данных в 0 процессе с суммированием
                                comm.Reduce(Commands.AddPlays(localDataBase), Operation<int>.Add, 0);
                            }
                            stopWatch.Stop();
                            break;
                        case "count":
                            
                            stopWatch.Restart();
                            stopWatch.Start();
                            if (comm.Rank == 0)
                            {
                                Console.WriteLine("Выбираем треки, у которых больше 10000 прослушиваний");
                               
                                int count = comm.Reduce(Commands.CountPlaysTenThousand(localDataBase), Operation<int>.Add, 0);
                                Console.WriteLine($"Вот их количество: {count}");
                            }
                            else
                            {
                               
                                comm.Reduce(Commands.CountPlaysTenThousand(localDataBase), Operation<int>.Add, 0);
                            }
                            stopWatch.Stop();
                            break;
                        case "sum":
                            // Запуск замера времени
                            stopWatch.Restart();
                            stopWatch.Start();
                            if (comm.Rank == 0)
                            {
                                Console.WriteLine("Считаем количество прослушиваний по альбомам");

                                // Сборка результата выполнения в 0 процессе
                                string[] serializedResultsGath = comm.Gather(JsonSerializer.Serialize(Commands.SumPlaysByAlbum(localDataBase)), 0);

                                // Список для хранения десериализованного результата
                                List<AlbumPlays> albumPlaysList = new List<AlbumPlays> { };
                                if (serializedResultsGath.Any())
                                {
                                    albumPlaysList = serializedResultsGath
                                        .Select(x => JsonSerializer.Deserialize<List<AlbumPlays>>(x)!)
                                        .Where(p => p != null)
                                        .Aggregate((a, b) => a.Concat(b).ToList()); // Объединение всех результатов
                                }

                                if (!albumPlaysList.Any())
                                {
                                    Console.WriteLine("Список пуст");
                                    stopWatch.Stop();
                                    break;
                                }

                                // Вывод на экран первых 100 альбомов и суммы прослушиваний
                                foreach (var album in albumPlaysList.OrderByDescending(p => p.TotalPlays).Take(100))
                                {
                                    Console.WriteLine($"{album.AlbumName} | Total Plays: {album.TotalPlays}");
                                }
                            }
                            else
                            {
                                // Все !0 процессы выполняют сериализацию и отправляют данные в 0 процесс
                                comm.Gather(JsonSerializer.Serialize(Commands.SumPlaysByAlbum(localDataBase)), 0);
                            }
                            stopWatch.Stop();
                            break;

                        case "max":
                            stopWatch.Restart();
                            stopWatch.Start();
                            if (comm.Rank == 0)
                            {
                                Console.WriteLine("Высчитываем максимальное количество прослушиваний");
                                int max = comm.Reduce(Commands.MaxAge(localDataBase), Operation<int>.Max, 0);
                                Console.WriteLine($"Максимальное количество прослушивание: {max}");
                            }
                            else
                            {

                                comm.Reduce(Commands.MaxAge(localDataBase), Operation<int>.Max, 0);
                            }
                            stopWatch.Stop();
                            break;
                        case "min":
                            // Запуск замера времени
                            stopWatch.Restart();
                            stopWatch.Start();
                            if (comm.Rank == 0)
                            {
                                Console.WriteLine("Высчитываем минимальное количество прослушиваний");
                                // Собираем все данные, определяем минимальное значение и передаем 0 процессу
                                int min = comm.Reduce(Commands.MinAge(localDataBase), Operation<int>.Min, 0);
                                Console.WriteLine($"Минимальное количество: {min}");
                            }
                            else
                            {
                                // Собираем все данные, определяем минимальное значение и передаем 0 процессу
                                comm.Reduce(Commands.MinAge(localDataBase), Operation<int>.Min, 0);
                            }
                            stopWatch.Stop();
                            break;
                        case "create":
                            // Запуск замера времени
                            stopWatch.Restart();
                            stopWatch.Start();
                            // Количество строк
                            int countStrings = 0;
                            if (comm.Rank == 0)
                            {
                                Console.Write("Введите число строк для генерании: ");
                                countStrings = Convert.ToInt32(Console.ReadLine());
                                Console.WriteLine("Генерируем строки");
                                // Генерируем данные
                                Commands.CreateDatabase(countStrings);
                                Console.WriteLine("Строки данных сгенерированы");
                            }       
                            // Барьер, чтобы не терялись данные и процессы не начали загрузку базы раньше окончания генерации
                            comm.Barrier();
                            // Локальная база данных
                            localDataBase = Commands.PreLoading(comm.Rank, comm.Size);
                            stopWatch.Stop();
                            break;
                        case "counting":
                            Commands.Count();
                            break;
                        default:
                            if (comm.Rank == 0 && command != "quit")
                                Console.WriteLine("Неизвестная команда");
                            break;
                    }
                    // Барьер, чтобы единовременно начать вывод данных
                    comm.Barrier();
                    // Вывод времени выполнения
                    TimeSpan ts = stopWatch.Elapsed;
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                  
                    Console.WriteLine($"RunTime {comm.Rank} " + elapsedTime);
                    
                    // Барьер, чтобы единовременно закончить вывод данных
                    comm.Barrier();
                }
            });
        }
    }
}