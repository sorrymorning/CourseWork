namespace MPICoursework.GenerateDb.GenerateEntities
{
    public class Order
    {
        // Id заказа
        public int Id { get; set; }
        // Имя покупателя
        public string CustomerFirstName { get; set; }
        // Фамилия покупателя
        public string CustomerLastName { get; set; }
        // FK на покупателя
        public int? CustomerId { get; set; }
        // FK навигационное свойство
        public Customer? Customer { get; set; }
        // Имя менеджера
        public string ManagerFirstName { get; set; }
        // Фамилия менеджера
        public string ManagerLastName { get; set; }
        // FK на менеджера
        public int? ManagerId { get; set; }
        // FK навигационное свойство
        public Manager? Manager { get; set; }
        // FK статус заказа
        public int StatusId { get; set; }
        // Навигационное свойство для статуса
        public Status Status { get; set; }
    }
}

    public class Manager
    {
        // Id
        public int Id { get; set; }
        // Имя
        public string FirstName { get; set; }
        // Фамилия
        public string LastName { get; set; }
        // Возраст
        public int Age { get; set; }
        // FK статус
        public int StatusId { get; set; }
        // FK навигационное свойство
        public Status Status { get; set; }
    }



    public class Status
    {
        // Id статуса
        public int Id { get; set; }
        // Название статуса (например: "Новый", "Оплачен", "Доставлен")
        public string StatusName { get; set; }
    }

    public class Customer
    {
        // Id покупателя
        public int Id { get; set; }
        // Имя
        public string FirstName { get; set; }
        // Фамилия
        public string LastName { get; set; }
        // Возраст
        public int Age { get; set; }
        // FK статус покупателя (например: "Активный", "Заблокирован")
        public int StatusId { get; set; }
        // Навигационное свойство
        public Status Status { get; set; }
    }

    public class Product
    {
        // Id товара
        public int Id { get; set; }
        // Название товара
        public string ProductName { get; set; }
        // Цена
        public decimal Price { get; set; }
        // Количество на складе
        public int Stock { get; set; }
        // FK категория товара
        public int CategoryId { get; set; }
        // Навигационное свойство для категории
        public Category Category { get; set; }
    }

    public class Category
    {
        // Id категории
        public int Id { get; set; }
        // Название категории (например: "Электроника", "Одежда")
        public string CategoryName { get; set; }
    }

