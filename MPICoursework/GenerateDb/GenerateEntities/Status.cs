namespace MPICoursework.GenerateDb.GenerateEntities
{
    public class Status
    {
        // Id статуса
        public int Id { get; set; }
        // Название статуса (например, "В ожидании", "Готовится", "Доставляется", "Доставлено")
        public string StatusName { get; set; }
    }
}
