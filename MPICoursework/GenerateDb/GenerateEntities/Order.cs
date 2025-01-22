namespace MPICoursework.GenerateDb.GenerateEntities
{
    public class Order
    {
        // Id заказа
        public int Id { get; set; }
        // Имя клиента
        public string CustomerFirstName { get; set; }
        // Фамилия клиента
        public string CustomerLastName { get; set; }
        // FK на клиента
        public int? CustomerId { get; set; }
        // FK навигационное свойство клиента
        public Customer? Customer { get; set; }
        // Имя менеджера
        public string ManagerFirstName { get; set; }
        // Фамилия менеджера
        public string ManagerLastName { get; set; }
        // FK на менеджера
        public int? ManagerId { get; set; }
        // FK навигационное свойство менеджера
        public Manager? Manager { get; set; }
        // Статус заказа
        public int StatusId { get; set; }
        // Навигационное свойство статуса
        public Status Status { get; set; }
    }
}
