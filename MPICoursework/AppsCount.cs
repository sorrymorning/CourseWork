namespace MPICoursework
{
    // Класс для группировки пользователей по Id в таблице Applications(применяется в Commands.SumApps)
    public class MoviesCount
    {
        // Id режиссера
        public int DirectorId { get; set; }
        // Имя режиссера
        public string Name { get; set; }
        // Фамилия режиссера
        public string Surname { get; set; }
        // Количество фильмов
        public int Count { get; set; }
    }

}
