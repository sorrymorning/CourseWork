namespace MPICoursework.GenerateDb.GenerateEntities
{
    public class Customer
    {
        // Id клиента
        public int Id { get; set; }
        // Имя
        public string FirstName { get; set; }
        // Фамилия
        public string LastName { get; set; }
        // Номер телефона
        public string PhoneNumber { get; set; }
        // Адрес доставки
        public string DeliveryAddress { get; set; }
        // FK статус
        public int StatusId { get; set; }
        // FK навигационное свойство статуса
        public Status Status { get; set; }
    }
}
