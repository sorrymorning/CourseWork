using MPICoursework.GenerateDb.GenerateEntities;

namespace MPICoursework
{
    // Класс для хранение списков таблиц
    public class TablesClass
    {
        // Список заявок
        public List<Order> OrderList { get; set; }
        // Список менеджеров
        public List<Manager> ManagerList { get; set; }
        // Список пользователей
        public List<Customer> CustomerList { get; set; }
        // Список статусов
        public List<Status> StatusList { get; set; }
        public List<Product> ProductList { get; set; }
        public List<Category> CategoryList { get; set; }

    }
}
