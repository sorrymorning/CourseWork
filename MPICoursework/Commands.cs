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
                // Обновить изменения в таблице Users
                db.Users.UpdateRange(localDb.UserList);
                // Обновить изменения в таблице Managers
                db.Managers.UpdateRange(localDb.ManagerList);
                // Обновить изменения в таблице Applications
                db.Applications.UpdateRange(localDb.ApplicationList);
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

        // Всех онлайн пользователй перевести в оффлайн
        public static int SetOffline(TablesClass localDb)
        {
            {
                // status с полем offline
                var offline = localDb.StatusList.Where(p => p.Id == 2).FirstOrDefault();
                // Список пользователей
                List<User> users = localDb.UserList.ToList();
                // Если список пользователей пустой
                if (!users.Any() || offline is null)
                    return 0;
                // Перебрать список пользователей
                foreach (var user in users)
                {
                    // Установить статус в offline
                    user.StatusId = 2;
                    // Полю status с навигационным свойством присваивается offline
                    user.Status = offline;
                }
                return 1;
            }      
        }
        // Увеличить возраст всех пользователей на 1 год
        public static int AddAge(TablesClass localDb)
        {
            // Перебрать каждого пользователя в списке
            foreach (var user in localDb.UserList)
            {
                // Увеличить текущее значение возраста на 1
                user.Age = user.Age + 1;
            }
            // Перебрать каждого менеджера в списке
            foreach (var manager in localDb.ManagerList)
            {
                // Увеличить текущее значение возраста на 1
                manager.Age = manager.Age + 1;
            }
            return 1;
        }
        // Вывести количество пользователей с возрастом более 17 лет в статусе online
        public static int CountUsersOnline(TablesClass localDb)
        {
            return localDb.UserList.Where(p => p.StatusId == 1 && p.Age > 17).Count();
        }
        // Вычислить количество заявок для каждого пользователя
        public static List<AppsCount> SumApps(TablesClass localDb)
        {
            return localDb.ApplicationList.GroupBy(p => p.UserId).Select(g => new AppsCount // Новая таблица после группировки
            {
                // Id после группировки
                AppsCountId = g.Select(p => p.UserId).First(),
                // Имя
                Name = g.Select(p => p.UserFirstName).First(),
                // Фамилия
                Surname = g.Select(p => p.UserLastName).First(),
                // Количество заявок
                Count = g.Count()
            }).ToList();
        }
        // Найти минимальный возраст среди менеджеров
        public static int MinAge(TablesClass localDb)
        {
            // Если таблица с менеджерами не пустая, то вернуть минимальный возраст
            if (localDb.ManagerList.Any())
                return localDb.ManagerList.Min(p => p.Age);
            // Если таблица пустая, то вернуть 0
            else
                return 0;
        }
        // Найти максимальный возраст среди менеджеров
        public static int MaxAge(TablesClass localDb)
        {
            // Если таблица с менеджерами не пустая, то вернуть максимальный возраст
            if (localDb.ManagerList.Any())
                return localDb.ManagerList.Max(p => p.Age);
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
                int countApplication = db.Applications.Count();
                int countManager = db.Managers.Count();
                int countUser = db.Users.Count();
                // Размер части для каждой таблицы
                int partApplication = (countApplication / size + 1);
                int partManager = (countManager / size + 1);
                int partUser = (countUser / size + 1);
                // Смещение по каждой таблице
                int offsetApplication = rank * partApplication;
                int offsetManager = rank * partManager;
                int offsetUser = rank * partUser;

                return new TablesClass
                {
                    // Заполнение списка заявок из базы данных
                    ApplicationList = db.Applications.Skip(offsetApplication).Take(partApplication).OrderBy(p => p.Id).ToList(),
                    // Заполнение списка менеджеров из базы данных
                    ManagerList = db.Managers.Include(p => p.Status).Skip(offsetManager).Take(partManager).OrderBy(p => p.Id).ToList(),
                    // Заполнение списка пользователей из базы данных
                    UserList = db.Users.Include(p => p.Status).Skip(offsetUser).Take(partUser).OrderBy(p => p.Id).ToList(),
                    // Заполнение списка статусов из базы данных
                    StatusList = db.Statuses.ToList()
                };
            }
        }
        // Сгенерировать базу данных с count строк
        public static void CreateDatabase(int count)
        {
            using (var db = new AppDbContext())
            {
                // Добавление онлайн статуса
                var onlineStatus = new Status { Id = 1, StatusName = "online" };
                // Добавление оффлайн статуса
                var offlineStatus = new Status { Id = 2, StatusName = "offline" };
                if (!db.Statuses.Any())
                {
                    // Добавить статусы в базу данных
                    db.Statuses.AddRange(onlineStatus, offlineStatus);
                    // Зафиксировать изменения
                    db.SaveChanges();
                }

                Random rand = new Random();
                // Список пользователей
                List<User> users = new List<User>{ };
                // Список менеджеров
                List<Manager> managers = new List<Manager> { };
                for (int i = 0; i < count; i++)
                {
                    // Случайный пользователь
                    User user = new User
                    {
                        FirstName = Faker.Name.First(),
                        LastName = Faker.Name.Last(),
                        Age = rand.Next(14, 66),
                        StatusId = rand.Next(1, 3)
                    };
                    // Случайный пользователь добавляется в список users
                    users.Add(user);
                    // Случайный пользователь добавляется в базу данных
                    db.Users.Add(user);

                    // Случайный менеджер
                    var manager = new Manager
                    {
                        FirstName = Faker.Name.First(),
                        LastName = Faker.Name.Last(),
                        Age = rand.Next(14, 66),
                        StatusId = rand.Next(1, 3)
                    };
                    // Случайный менеджер добавляется в список managers
                    managers.Add(manager);
                    // Случайный менеджер добавляется в базу данных
                    db.Managers.Add(manager);
                }
                // Сохранить изменения в базе данных
                db.SaveChanges();

                for (int i = 0; i < count ; i++)
                {
                    // Случайный пользователь из списка users
                    User randUser = users[rand.Next(users.Count)];
                    // Случайный менеджер из списка managers
                    Manager randManager = managers[rand.Next(managers.Count)];
                    // Формирование случайной заявки
                    db.Applications.Add(new Application
                    {
                        UserFirstName = randUser.FirstName,
                        UserLastName = randUser.LastName,
                        UserId = randUser.Id,
                        ManagerFirstName = randManager.FirstName,
                        ManagerLastName = randManager.LastName,
                        ManagerId = randManager.Id
                    });
                }
                // Сохранить изменения в базе данных
                db.SaveChanges();
            }
        }
    }
}