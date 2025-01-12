namespace MPICoursework
{
    // Класс для группировки пользователей по Id в таблице Applications(применяется в Commands.SumApps)
    public class AppsCount
    {
        // Id завполняется вручную
        public int? AppsCountId {  get; set; }
        // Имя пользователя
        public string Name {  get; set; }
        // Фамилия пользователя
        public string Surname { get; set; }
        // Число заявок
        public int Count { get; set; }
    }
}
