# ISTBirthday
## This is a telegram bot written in C# designed to remind people about the birthdays of people added to the MySQL database. The database is generated based on Entity Framework Core models. 
#### The Model(Student) has such fields as: 
* name
* surname
* patronymic
* date of birth
* contact information
* *and so on.*


#### To build this application you need [.NET 5 SDK](https://dotnet.microsoft.com/download).

#### To use the application, you need to configure MySQL server version 8.0.

### Environment Variables:
Name  | Description
:---  | :---
TOKEN | Token for telegram bot
CONNECTION_STRING | Connection string for MySQL server 8.0.*
  
### Bot commands(RU):
*  allbirthdays - список всех дней рождений
*  allbirthdayssorted - отсортированный список всех дней рождений
*  nearestbirthday - информация о ближайшем дне рождения
*  notificate - изменение состояния уведомлений
*  all - информация о всех людях в базе
*  find - поиск по ключевому слову
*  start - начало работы


###### *P.S. (All passwords and tokens that were revealed in early commits have already been reset or changed (I didn't think I would want to put the source code into an public repository))*
