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
                // Обновить изменения в таблице Customers
                db.Customers.UpdateRange(localDb.CustomerList);
                // Обновить изменения в таблице Managers
                db.Managers.UpdateRange(localDb.ManagerList);
                // Обновить изменения в таблице Orders
                db.Orders.UpdateRange(localDb.OrderList);
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
            // Найти статус "В ожидании"
            var pendingStatus = localDb.StatusList.FirstOrDefault(p => p.StatusName == "В ожидании");
            // Список клиентов
            var customers = localDb.CustomerList.ToList();
            // Если список клиентов пуст или статус "В ожидании" не найден
            if (!customers.Any() || pendingStatus is null)
                return 0;

            // Установить статус "В ожидании" для всех клиентов
            foreach (var customer in customers)
            {
                customer.StatusId = pendingStatus.Id;
                customer.Status = pendingStatus;
            }
            return 1;
        }

        //// Вывести количество пользователей с возрастом более 17 лет в статусе online
        public static int CountUsersOnline(TablesClass localDb)
        {
            return localDb.CustomerList.Where(p => p.StatusId == 1).Count();
        }
        //// Вычислить количество заявок для каждого пользователя
        public static List<OrderCount> SumOrders(TablesClass localDb)
        {
            return localDb.OrderList.GroupBy(p => p.CustomerId).Select(g => new OrderCount // Новая таблица после группировки
            {
                // Id после группировки
                CustomerId = g.Select(p => p.CustomerId).First(),
                // Имя клиента
                CustomerFirstName = g.Select(p => p.CustomerFirstName).First(),
                // Фамилия клиента
                CustomerLastName = g.Select(p => p.CustomerLastName).First(),
                // Количество заказов
                OrderCountValue = g.Count()
            }).ToList();
        }
        //// Найти минимальный возраст среди менеджеров
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
                int countOrder = db.Orders.Count();
                int countManager = db.Managers.Count();
                int countCustomer = db.Customers.Count();
                // Размер части для каждой таблицы
                int partOrder = (countOrder / size + 1);
                int partManager = (countManager / size + 1);
                int partCustomer = (countCustomer / size + 1);
                // Смещение по каждой таблице
                int offsetOrder = rank * partOrder;
                int offsetManager = rank * partManager;
                int offsetCustomer = rank * partCustomer;

                return new TablesClass
                {
                    // Заполнение списка заказов из базы данных
                    OrderList = db.Orders.Skip(offsetOrder).Take(partOrder).OrderBy(p => p.Id).ToList(),
                    // Заполнение списка менеджеров из базы данных
                    ManagerList = db.Managers.Include(p => p.Status).Skip(offsetManager).Take(partManager).OrderBy(p => p.Id).ToList(),
                    // Заполнение списка клиентов из базы данных
                    CustomerList = db.Customers.Include(p => p.Status).Skip(offsetCustomer).Take(partCustomer).OrderBy(p => p.Id).ToList(),
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
                // Добавление статусов заказов
                var statuses = new List<Status>
                {
                    new Status { Id = 1, StatusName = "В ожидании" },
                    new Status { Id = 2, StatusName = "Готовится" },
                    new Status { Id = 3, StatusName = "Доставляется" },
                    new Status { Id = 4, StatusName = "Доставлено" }
                };

                if (!db.Statuses.Any())
                {
                    // Добавить статусы в базу данных
                    db.Statuses.AddRange(statuses);
                    db.SaveChanges();
                }

                Random rand = new Random();
                // Список клиентов
                List<Customer> customers = new List<Customer>();
                // Список менеджеров
                List<Manager> managers = new List<Manager>();

                for (int i = 0; i < count; i++)
                {
                    // Случайный клиент
                    Customer customer = new Customer
                    {
                        FirstName = Faker.Name.First(),
                        LastName = Faker.Name.Last(),
                        PhoneNumber = Faker.Phone.Number(),
                        DeliveryAddress = Faker.Address.StreetAddress(),
                        StatusId = rand.Next(1, 5)
                    };
                    customers.Add(customer);
                    db.Customers.Add(customer);

                    // Случайный менеджер
                    Manager manager = new Manager
                    {
                        FirstName = Faker.Name.First(),
                        LastName = Faker.Name.Last(),
                        Age = rand.Next(20, 65),
                        StatusId = rand.Next(1, 5)
                    };
                    managers.Add(manager);
                    db.Managers.Add(manager);
                }

                // Сохранить клиентов и менеджеров в базе данных
                db.SaveChanges();

                for (int i = 0; i < count; i++)
                {
                    // Случайный клиент из списка
                    Customer randCustomer = customers[rand.Next(customers.Count)];
                    // Случайный менеджер из списка
                    Manager randManager = managers[rand.Next(managers.Count)];

                    // Создание случайного заказа
                    db.Orders.Add(new Order
                    {
                        CustomerFirstName = randCustomer.FirstName,
                        CustomerLastName = randCustomer.LastName,
                        CustomerId = randCustomer.Id,
                        ManagerFirstName = randManager.FirstName,
                        ManagerLastName = randManager.LastName,
                        ManagerId = randManager.Id,
                        StatusId = rand.Next(1, 5) // Случайный статус заказа
                    });
                }

                // Сохранить заказы в базе данных
                db.SaveChanges();
            }
        }
    }
}