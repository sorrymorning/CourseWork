namespace MPICoursework.GenerateDb.GenerateEntities
{
    public class Manager
    {
        // Id менеджера
        public int Id { get; set; }
        // Имя
        public string FirstName { get; set; }
        // Фамилия
        public string LastName { get; set; }
        // Возраст
        public int Age { get; set; }
        // FK статус
        public int StatusId { get; set; }
        // FK навигационное свойство статуса
        public Status Status { get; set; }
    }
}
