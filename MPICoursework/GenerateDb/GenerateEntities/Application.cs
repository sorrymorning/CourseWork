namespace MPICoursework.GenerateDb.GenerateEntities
{
    public class Application
    {
        // Id
        public int Id { get; set; }
        // Имя пользователя
        public string UserFirstName {  get; set; }
        // Фамилия пользователя
        public string UserLastName { get; set; }
        // FK на пользователя
        public int? UserId { get; set; }
        // FK навигационное свойство
        public User? User { get; set; }
        // Имя пользователя
        public string ManagerFirstName {  get; set; }
        // Фамилия пользователя
        public string ManagerLastName { get; set; }
        // FK на менеджера
        public int? ManagerId { get; set; }
        // FK навигационное свойство
        public Manager? Manager { get; set; }
    }
}
