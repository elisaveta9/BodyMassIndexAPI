# BodyMassIndexAPI
**Задание 2. Разработать WebApi для решения следующих задач, по уровням сложности.**

**a. Вес – Макс 20 баллов.** Реализовать WebApi с методом определения Индекса Массы Тела (ИМТ, анг. термин Body Mass Index, https://ru.wikipedia.org/wiki/Индекс_массы_тела ). Вызов через GET. Метод должен принимать на вход параметры, необходимые для вычисления ИМТ (Рост и Вес человека). Метод должен проверять входные параметры на адекватность, в т.ч. применительно к человеку. Метод возвращает объект с рассчитанным индексом и его описанием. К решению приложить запросы из Postman с корректными и некорректными параметрами.

На рисунке 1 показан корректный вариант ввода значений.

![GetBmi trueValues](https://user-images.githubusercontent.com/89607033/234840975-c0a27808-7d3f-472a-a9e7-881a924c6355.PNG)

*Рисунок 1 - корректные значения*

*Запрос GET принимает два числа с плавающей запятой, они вводятся с точкой в виде разделителя. Выделенный диапазон значений для запроса составляет числа от 45 до 250 для роста (в см.) и числа от 2 до 500 для веса (в кг.). Так же проверяется если ИМТ будет меньше 5, то скорее всего данные введены неверно.*

Ниже приведены некорректные варианты запроса

![GetBmi falseValues](https://user-images.githubusercontent.com/89607033/234842352-49d084d3-9e05-4e5f-aacb-de1e4b4e5e6b.PNG)

*Рисунок 2 - В данном случае ИМТ < 5*

![GetBmi falseValues2](https://user-images.githubusercontent.com/89607033/234842361-f288b092-0969-4b05-a946-e2dc57a7119f.PNG)

*Рисунок 3 - В этом примере рост вне пределов заданного диапазона от 45 до 250. Напоминается в каких единицах измерения принимаются числа, так как возможен вариант, что вес ввели в граммах (например, значение 60000 не входит в ранее определенный диапазон значений, а число 60 входит) или рост в метрах (например, значение 1.9 не входит в заданный диапазон, а 190 входит).*

![GetBmi falseValues3](https://user-images.githubusercontent.com/89607033/234842364-e76a8d66-ef32-436f-9074-3051126969a1.PNG)

*Рисунок 4 - Такой же случай как и на рисунке 3*

![GetBmi falseValues4](https://user-images.githubusercontent.com/89607033/234842376-581f14c1-081e-43f7-a553-dfd6e754bf39.PNG)

*Рисунок 5 - В этом примере вес вне пределов заданного диапазона от 2 до 500*

**b. Вес – Макс 40 баллов.** Доработать WebApi методом добавления пациента (ФИО, рост, вес, возраст), вызов через POST, метод автоматически вычисляет ИМТ для данного пациента и заносит информацию в Базу данных (спроектировать структуру самостоятельно). Метод должен проверять входные параметры, в случае ошибки возвращать правильный статус ошибки. К решению приложить запросы из Postman с корректными и некорректными параметрами. Приложить ER-диаграмму БД.

![image](https://user-images.githubusercontent.com/89607033/234845409-0c9466d7-9c95-4979-aae0-4ddb7221d5fb.png)

*Рисунок 6 - Схема для базы данных*

Будем хранить данные о пользователях в разных таблицах. Первая таблица Люди будет хранить Id, имя, фамилию, отчество, Id деталей пользователя, причем Id - первичный ключ, а Id деталей - внешний ключ, который ссылается на таблицу Деталей. Таблица Детали будет хранить Id, рост, вес, ИМТ, дату рождения. Будем запрашивать не возраст, а дату рождения для точных рассчетов.

В задании используем PostgreSQL, для удобной разработки используем Entity Framework с паттерном репозиторий.

Проверка для значений веса и роста остается прежней. Добавляются новые переменные, для который будут новые проверки. Имя или фамилия не должны быть пустыми, отчество может отсутствовать. Дата рождения не может быть из будущего или возраст человека не может превышать 150 лет. Введенная дата может быть несколькими типами разделителей это тире, точка и слэш.

![image](https://user-images.githubusercontent.com/89607033/234851215-4a74052f-9a15-4f84-b633-c033bbab1983.png)

![image](https://user-images.githubusercontent.com/89607033/234852128-8a777dba-83f8-4bd0-abf0-7b06556e2254.png)

*Рисуноки 7 и 8 - корректные значения для запроса POST*

![Post falseValues](https://user-images.githubusercontent.com/89607033/234852365-9de5c645-593e-4e17-adee-62baff49f7e5.PNG)

*Рисунок 9 - Не введена фамилия*

![image](https://user-images.githubusercontent.com/89607033/234852760-e9c5b902-666e-434f-86ff-586999f18f87.png)

*Рисунок 10 - Дата рождения завтра - некорректное значение*

![image](https://user-images.githubusercontent.com/89607033/234853100-52289304-0c0a-44b5-99b9-acd07058f451.png)

*Рисунок 11 - Дата рождения более чем 150 лет назад - некорректное значение*

**c. Вес – Макс 50 баллов.** Доработать WebApi методом получения статистики по параметрам ИМТ пациентов из базы данных, вызов через GET. Метод вычисляет статистику посредством SQL-запроса и возвращает список характеристик ИМТ и процентное отношение клиентов в этой категории, по убыванию процентного соотношения, например: Норма – 70% Ниже нормы – 20% Ожирение – 10% Метод не принимает параметры. К решению приложить запросы из Postman, исходный код SQL-запроса.

![image](https://user-images.githubusercontent.com/89607033/234853847-2eed0d62-2af1-42b6-81f2-9aebe1883bbf.png)

*Рисунок 12 - Запрос GET GetStatistics*

**d. Вес – Макс 50 баллов.** Разработать хранимую процедуру для получения статистики по параметрам ИМТ пациентов из базы данных в разрезе возраста. Хранимка вычисляет статистику посредством SQL-запроса с группировкой по возрастам и возвращает список характеристик ИМТ и процентное отношение клиентов в этой категории, по убыванию процентного соотношения, например: Норма – 70% Ниже нормы – 20% Ожирение – 10%

Группировка происходит по диапазонам возрастов, вида:

0..10,

11..20,

21..30,

31..40, и тд.

Для каждой группы должно быть посчитано процентное отношение для каждой категории ИМТ. Исходный код SQL-запроса хранимой процедуры приложить в свой репозиторий. Приложить экранные формы вызова хранимой процедуры с результатом работы.

Так как в базе данных хранятся даты рождения, а не возраст пользователей, то группировка по датам мало что принесет, поэтому в репозитории DetailsRepository существует метод для получения коллекции данных для заданного диапазона возрастов.

![image](https://user-images.githubusercontent.com/89607033/234855295-56ed8a09-1a81-487d-8f60-de7a7b97ffe8.png)
![image](https://user-images.githubusercontent.com/89607033/234855425-9954d990-e783-4e79-bb75-4f8f01007faf.png)

*Рисунок 13 - Результат запроса GET GetStatisticsByAges*
