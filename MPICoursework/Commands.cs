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

                // Обновить изменения в таблице Products
                db.Products.UpdateRange(localDb.ProductList);

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


        
        // Увеличить возраст всех пользователей на 1 год
        public static int AddAge(TablesClass localDb)
        {
            // Перебрать каждого покупателя в списке
            foreach (var customer in localDb.CustomerList)
            {
                // Увеличить текущее значение возраста на 1
                customer.Age += 1;
            }
            // Перебрать каждого менеджера в списке
            foreach (var manager in localDb.ManagerList)
            {
                // Увеличить текущее значение возраста на 1
                manager.Age += 1;
            }
            return 1;
        }

        // Вывести количество пользователей с возрастом более 17 лет в статусе online
        public static int CountCustomersAndManagersOnline(TablesClass localDb)
        {
            int onlineCustomers = localDb.CustomerList.Where(c => c.StatusId == 1 && c.Age > 17).Count();
            int onlineManagers = localDb.ManagerList.Where(m => m.StatusId == 1 && m.Age > 17).Count();
            return onlineCustomers + onlineManagers;
        }

        // Вычислить количество заявок для каждого пользователя
        public static List<OrderCount> SumOrders(TablesClass localDb)
        {
            return localDb.OrderList.GroupBy(o => o.CustomerId).Select(g => new OrderCount // Новая таблица после группировки
            {
                // Id покупателя после группировки
                CustomerId = g.Key,
                // Имя покупателя
                Name = g.Select(o => o.CustomerFirstName).First(),
                // Фамилия покупателя
                Surname = g.Select(o => o.CustomerLastName).First(),
                // Количество заказов
                Count = g.Count()
            }).ToList();
        }

        // Найти минимальный возраст среди менеджеров
        public static int MinAge(TablesClass localDb)
        {
            // Если таблицы с покупателями и менеджерами не пусты, то вернуть максимальный возраст
            if (localDb.CustomerList.Any() || localDb.ManagerList.Any())
            {
                int maxCustomerAge = localDb.CustomerList.Any() ? localDb.CustomerList.Min(c => c.Age) : 0;
                int maxManagerAge = localDb.ManagerList.Any() ? localDb.ManagerList.Min(m => m.Age) : 0;
                return Math.Max(maxCustomerAge, maxManagerAge);
            }
            // Если обе таблицы пустые, вернуть 0
            else
                return 0;
        }

        // Найти максимальный возраст среди менеджеров
        public static int MaxAge(TablesClass localDb)
        {
            // Если таблицы с покупателями и менеджерами не пусты, то вернуть максимальный возраст
            if (localDb.CustomerList.Any() || localDb.ManagerList.Any())
            {
                int maxCustomerAge = localDb.CustomerList.Any() ? localDb.CustomerList.Max(c => c.Age) : 0;
                int maxManagerAge = localDb.ManagerList.Any() ? localDb.ManagerList.Max(m => m.Age) : 0;
                return Math.Max(maxCustomerAge, maxManagerAge);
            }
            // Если обе таблицы пустые, вернуть 0
            else
                return 0;
        }

        // Предзагрузка таблиц
        public static TablesClass PreLoading(int rank, int size)
        {
            using (var db = new AppDbContext())
            {
                // Количество строк в каждой таблице
                int countOrders = db.Orders.Count();
                int countManagers = db.Managers.Count();
                int countCustomers = db.Customers.Count();
                int countProducts = db.Products.Count();

                // Размер части для каждой таблицы
                int partOrders = (countOrders / size + 1);
                int partManagers = (countManagers / size + 1);
                int partCustomers = (countCustomers / size + 1);
                int partProducts = (countProducts / size + 1);

                // Смещение по каждой таблице
                int offsetOrders = rank * partOrders;
                int offsetManagers = rank * partManagers;
                int offsetCustomers = rank * partCustomers;
                int offsetProducts = rank * partProducts;

                return new TablesClass
                {
                    // Заполнение списка заказов из базы данных
                    OrderList = db.Orders
                        .Skip(offsetOrders)
                        .Take(partOrders)
                        .OrderBy(o => o.Id)
                        .ToList(),

                    // Заполнение списка менеджеров из базы данных
                    ManagerList = db.Managers
                        .Include(m => m.Status)
                        .Skip(offsetManagers)
                        .Take(partManagers)
                        .OrderBy(m => m.Id)
                        .ToList(),

                    // Заполнение списка покупателей из базы данных
                    CustomerList = db.Customers
                        .Include(c => c.Status)
                        .Skip(offsetCustomers)
                        .Take(partCustomers)
                        .OrderBy(c => c.Id)
                        .ToList(),

                    // Заполнение списка товаров из базы данных
                    ProductList = db.Products
                        .Include(p => p.Category)
                        .Skip(offsetProducts)
                        .Take(partProducts)
                        .OrderBy(p => p.Id)
                        .ToList(),

                    // Заполнение списка статусов из базы данных
                    StatusList = db.Statuses.ToList(),

                    // Заполнение списка категорий из базы данных
                    CategoryList = db.Categories.ToList()
                };
            }
        }

        // Сгенерировать базу данных с count строк
        public static void CreateDatabase(int count)
        {
            using (var db = new AppDbContext())
            {
                // Добавление статусов для заказов и клиентов
                var statuses = new List<Status>
                {
                    new Status { Id = 1, StatusName = "Активный" },
                    new Status { Id = 2, StatusName = "Заблокирован" },
                    new Status { Id = 3, StatusName = "Новый заказ" },
                    new Status { Id = 4, StatusName = "Оплачен" },
                    new Status { Id = 5, StatusName = "Доставлен" }
                };
                if (!db.Statuses.Any())
                {
                    db.Statuses.AddRange(statuses);
                    db.SaveChanges();
                }

                Random rand = new Random();

                // Списки для генерации данных
                List<Customer> customers = new List<Customer>();
                List<Manager> managers = new List<Manager>();
                List<Product> products = new List<Product>();
                List<Category> categories = new List<Category>
                {
                    new Category { CategoryName = "Электроника" },
                    new Category { CategoryName = "Одежда" },
                    new Category { CategoryName = "Бытовая техника" }
                };

                if (!db.Categories.Any())
                {
                    db.Categories.AddRange(categories);
                    db.SaveChanges();
                }

                // Создание случайных покупателей
                for (int i = 0; i < count; i++)
                {
                    var customer = new Customer
                    {
                        FirstName = Faker.Name.First(),
                        LastName = Faker.Name.Last(),
                        Age = rand.Next(18, 70),
                        StatusId = rand.Next(1, 3) // Активный или Заблокирован
                    };
                    customers.Add(customer);
                    db.Customers.Add(customer);
                }

                // Создание случайных менеджеров
                for (int i = 0; i < count / 2; i++)
                {
                    var manager = new Manager
                    {
                        FirstName = Faker.Name.First(),
                        LastName = Faker.Name.Last(),
                        Age = rand.Next(25, 60),
                        StatusId = 1 // Активный
                    };
                    managers.Add(manager);
                    db.Managers.Add(manager);
                }

                // Создание случайных товаров
                for (int i = 0; i < count * 2; i++)
                {
                    var product = new Product
                    {
                        ProductName = Faker.Name.First(),
                        Price = (decimal)Math.Round(rand.Next(10, 1000) + rand.NextDouble(), 2),
                        Stock = rand.Next(1, 100),
                        CategoryId = rand.Next(1, categories.Count + 1)
                    };
                    products.Add(product);
                    db.Products.Add(product);
                }

                db.SaveChanges();

                // Создание случайных заказов
                for (int i = 0; i < count; i++)
                {
                    // Случайный покупатель и менеджер
                    var randomCustomer = customers[rand.Next(customers.Count)];
                    var randomManager = managers[rand.Next(managers.Count)];

                    // Создание заказа
                    var order = new Order
                    {
                        CustomerFirstName = randomCustomer.FirstName,
                        CustomerLastName = randomCustomer.LastName,
                        CustomerId = randomCustomer.Id,
                        ManagerFirstName = randomManager.FirstName,
                        ManagerLastName = randomManager.LastName,
                        ManagerId = randomManager.Id,
                        StatusId = rand.Next(3, 6) // Новый заказ, Оплачен, Доставлен
                    };
                    db.Orders.Add(order);
                }

                db.SaveChanges();
            }
        }

    }
}