namespace BodyMassIndexAPI.Models
{
    public class StatisticsBMI
    {
        public string BmiResult { get; set; }

        public int Count { get; set; }        

        public string ToString(int CountAllElements)
        {
            double Statistic = Math.Round((double)Count * 100 / CountAllElements, 2);
            return $"{BmiResult} - {Statistic}%";
        }
    }
}
