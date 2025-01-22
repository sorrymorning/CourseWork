namespace MPICoursework
{
    // Класс для группировки пользователей по Id в таблице Applications(применяется в Commands.SumApps)
    public class OrderCount
    {
        // Id клиента
        public int? CustomerId { get; set; }

        // Имя клиента
        public string CustomerFirstName { get; set; }

        // Фамилия клиента
        public string CustomerLastName { get; set; }

        // Количество заказов
        public int OrderCountValue { get; set; }
    }
}
