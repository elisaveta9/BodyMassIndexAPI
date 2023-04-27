using BodyMassIndexAPI.Database.Context;
using BodyMassIndexAPI.Database.Entityes;

namespace BodyMassIndexAPI.Database
{
    internal class DetailsRepository : Repository<Details>
    {
        private readonly UsersDB _db;

        public DetailsRepository(UsersDB db) : base(db) { _db = db; }

        public IQueryable<Details> GetDetailsInRangeAge(int startAge, int endAge)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now), startDate = new(today.Year - endAge - 1, today.Month, today.Day), //находим диапазон в датах для заданного 
                endDate = new(today.Year - startAge, today.Month, today.Day);                                                   //в годах
            startDate = startDate.AddDays(1); //если день рождения завтра и количество полных лет равно startAge
            return Items.AsEnumerable().Where(item => item.BirthDate.DayNumber >= startDate.DayNumber).AsQueryable()
                .Where(item => item.BirthDate.DayNumber <= endDate.DayNumber).AsQueryable();
        }
    }
}
