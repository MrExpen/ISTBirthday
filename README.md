# ISTBirthday
## This is a telegram bot written in C# designed to remind people about the birthdays of people added to the MySQL database. The database is generated based on Entity Framework Core models. The Model(Student) has such fields as: 
* name
* surname
* patronymic
* date of birth
* contact information
* *and so on.*


#### To build this application you need [.NET 5 SDK](https://dotnet.microsoft.com/download).

#### To use the application, you need to configure MySQL server version 8.0.

### Input parameters:
* -h, --help                      - show help message.
* -tf, --token-file               - set a file name for bot token (token.txt by default).
* -cf, --connection-string-file   - set a file name for db connection string (connectionString.txt by default).
* -t, --token                     - set a bot token.
* -c, --connection-string         - set a db connection string.
* --log-config                    - set a xml config for log4net (log4net.config by default).
  
### Bot commands(RU):
*  allbirthdays - список всех дней рождений
*  allbirthdayssorted - отсортированный список всех дней рождений
*  nearestbirthday - информация о ближайшем дне рождения
*  notificate - изменение состояния уведомлений
*  all - информация о всех людях в базе
*  find - поиск по ключевому слову
*  start - начало работы
