using BodyMassIndexAPI.Database.Entityes.Base;

namespace BodyMassIndexAPI.Database.Entityes
{
    internal class Person : Entity
    {
        public override int Id { get; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string? Patronymic { get; set; }
    }
}