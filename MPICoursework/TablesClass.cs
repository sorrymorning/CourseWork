using MPICoursework.GenerateDb.GenerateEntities;

namespace MPICoursework
{
    // Класс для хранение списков таблиц
    public class TablesClass
    {
        // Список заявок
        public List<Application> ApplicationList { get; set; }
        // Список менеджеров
        public List<Manager> ManagerList { get; set; }
        // Список пользователей
        public List<User> UserList { get; set; }
        // Список статусов
        public List<Status> StatusList { get; set; }
    }
}
