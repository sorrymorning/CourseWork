namespace MPICoursework
{
    // Класс для группировки пользователей по Id в таблице Applications(применяется в Commands.SumApps)
    public class OrderCount
    {
        public int? CustomerId { get; set; } // Id покупателя
        public string Name { get; set; }    // Имя покупателя
        public string Surname { get; set; } // Фамилия покупателя
        public int Count { get; set; }      // Количество заказов
    }

}
