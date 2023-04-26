using BodyMassIndexAPI.Database.Entityes.Base;

namespace BodyMassIndexAPI.Database.Entityes
{
    internal class Details : Entity
    {
        public override int Id { get; }

        public Person Person { get; set; }

        public double Height { get; set; }

        public double Weight { get; set; }

        public double BMI { get; set; }

        public DateOnly BirthDate { get; set; }
    }
}
