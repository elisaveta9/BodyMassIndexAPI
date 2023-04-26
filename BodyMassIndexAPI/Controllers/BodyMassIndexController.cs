using BodyMassIndexAPI.Database;
using BodyMassIndexAPI.Database.Entityes;
using BodyMassIndexAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace BodyMassIndexAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BodyMassIndexController : ControllerBase
    {
        private static readonly string[] dateFormats = new[] { "yyyy/MM/dd", "dd/MM/yyyy", "yyyy.MM.dd", "dd.MM.yyyy", "yyyy-MM-dd", "dd-MM-yyyy"}; //несколько форматов ввода даты

        private static readonly Dictionary<double, string> bmiResult = new() { { 16, "Выраженный дефицит массы тела" }, //варианты интерпритации ИМТ
            { 18.5, "Недостаточная масса тела" }, { 25, "Норма" }, { 30, "Избыточная масса тела" },                     //проверяем по верхней границе
            { 35, "Ожирение 1 степени" }, { 40, "Ожирение 2 степени" }, { 200, "Ожирение 3 степени" } };                //верхняя граница не включается в диапазон

        private CultureInfo provider = new("ru-RU");

        private readonly ILogger<BodyMassIndexController> _logger;

        private readonly DetailsRepository detailsRepository = new(new Database.Context.UsersDB());
        
        private readonly IRepository<Person> personRepository = new Repository<Person>(new Database.Context.UsersDB());

        private double GetBmIndex(double height, double weight) //эта функция используется в двух вызовах и поэтому выделена отдельно
        {
            if ((height < 44 || height > 251) || (weight < 2 || weight > 500)) //проверяем на адекватные значения
                throw new ArgumentException("Введенные данные вне выделенного диапазона. Введите сначала рост в см., а потом вес в кг.");
            height /= 100.0;
            double result = weight / (height * height);
            if (result < 5 || result > 200) 
                throw new ArgumentException("Введенные данные неверны.");
            return result;
        }

        private IList<string> GetStatistic(IEnumerable<Details> details) //эта функция используется в двух вызовах и поэтому выделена отдельно
        {                                                                //она подсчитывает статистику в в заданном множестве 
            List<StatisticsBMI> statistics = new();
            int countAllUsers = details.Count();
            foreach (Details detail in details) 
            {
                StatisticsBMI statisticsBMI = new() { BmiResult = bmiResult.Where(item => item.Key > detail.BMI).First().Value, Count = 1 }; //создаем экземпляр для статистики
                                                                                                  //итерпритацией ИМТ и количество элементов для интерпритации считаем равным 1
                var element = statistics.Where(item => item.BmiResult == statisticsBMI.BmiResult);
                if (element.Count() == 0)
                    statistics.Add(statisticsBMI); //если в списке нет этой интерпритации, то доваляем ее
                else element.First().Count++; //если уже существует, то количество этой интерпритации больше на одну
            }
            statistics.Sort((x, y) => y.Count.CompareTo(x.Count)); //сортируем список по убыванию
            List<string> result = new(); 
            foreach (var item in statistics)
                result.Add(item.ToString(countAllUsers));
            return result;
        }

        public BodyMassIndexController(ILogger<BodyMassIndexController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public BodyMassIndex GetBmi(double height, double weight) => new() { Height = height * 100.0, Weight = weight, BMI = GetBmIndex(height, weight) };        

        [HttpPost]
        public IActionResult Post(string surname, string name, string patronymic, string birthDate, double height, double weight)
        {
            double Bmi = GetBmIndex(height, weight); 

            if (!DateTime.TryParseExact(birthDate, dateFormats, provider,   //считывем дату по ранее заданным шаблонам
                DateTimeStyles.AdjustToUniversal, out DateTime BirthDate))  //если не проходит проверку, то выводим ошибку
                throw new ArgumentException("Неверное введена дата!");

            if ((DateTime.Now - BirthDate).TotalDays < 0) //используем дату дня рождения для точного посчета возвраста
                throw new ArgumentException("День рождения не может быть в заданную дату, т. к. она еще не настала!"); //если введена дата будущего, вызываем ошибку

            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(surname))
                throw new ArgumentException("Имя или фамилия не должны быть пустыми!"); 

            if (String.IsNullOrEmpty(patronymic)) //отчество может отсутствовать
                patronymic = "";

            Person person = new() { Name = name, Surname = surname, Patronymic = patronymic };
            Details detailsPerson = new() { Person =  person, Height = height, Weight = weight, 
                BirthDate = new DateOnly(BirthDate.Year, BirthDate.Month, BirthDate.Day), BMI = Bmi };

            try
            {
                personRepository.Add(person);
                detailsRepository.Add(detailsPerson);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("/GetStatistics")]
        public IList<string> GetStatistic() => GetStatistic(detailsRepository.GetAll()); //получаем статистику для всех элементов в БД

        [HttpGet("/GetStatisticsByAges")]
        public IDictionary<string, IList<string>> GetStatisticsByAges()
        {
            Dictionary<string, IList<string>> result = new();
            int startAge = 0, endAge = 10, count = 0, countAllElements = detailsRepository.GetAll().Count();
            while (count < countAllElements) //пока не проверили все элементы из БД
            {
                var group = detailsRepository.GetDetailsInRangeAge(startAge, endAge); //получаем группу для возврастов от startAge до endAge, включая endAge
                if (group.Count() > 0) //если групп не пустая, то
                {
                    var groupResult = GetStatistic(group); //получаем статистику для полученной ранне группы
                    result.Add($"Диапазон возврастов от {startAge} до {endAge}", groupResult); //добавляем результат к словарю
                    count += group.Count(); //подсчитываем количество проверенных элементов 
                }
                startAge = endAge + 1; endAge += 10;
            }
            return result;
        }
    }
}
