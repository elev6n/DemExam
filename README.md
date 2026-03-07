# DemExam.Desktop

**"Создать настольное приложение"** - задание из Модуля 4,
является самым щедрым на баллы, но вместе с тем и самым непростым среди других модулей.
В этом репозитории представлены решения помогающие выполнить
задание как можно более быстро и безопасно.

## Внимание!

Это проект написан на отличном стеке от того что ожидается на демонстрационном экзамене.
**Внимательно читайте описание!**

Настоятельно рекомендую при подготовке несколько раз переписать этот проект, каждый раз все меньше поглядывая на
референс. Так вы сможете выработать понимание и память. Нерекомендую использовать этот проект как шаблон - создавайте
свой собственный проект и пишите с самого нуля.

Информация здесь может обновляться и дополняться. **Поставьте звезду на этот репозиторий, чтобы не упустить последние
обновления!**

## Содержание

- [Начиная](#getting-started)
    - [Проверка инструментов](#проверка-инструментов)
        - [.NET](#net)
        - [СУБД](#субд)
    - [Создание базы данных](#создание-базы-данных)
        - [ERD](#erd)
        - [SQL-Script](#sql-script)
    - [Библиотеки](#библиотеки)
        - [Установка](#установка)
- [Особенности решения](#особенности-решения)

# Начиная

Здесь представлены общие сведения по решению. Для получения объяснений
обратитесь [сюда](src/DemExam.Desktop/GUIDE.md)

## Проверка инструментов

### .NET

Прежде чем начать убедитесь что у вас установлена платформа **.NET 8.0**. Сделать это можно так:

1. Откройте терминал (Powershell, CMD, Bash)
2. В открывшемся окне введите

```powershell
# Последняя установленная версия .NET
dotnet --version
# Подробные сведения о всех установленных версиях
dotnet --info
```

3. Вывод должен быть примерно следующим

```powershell
PS C:\Users\elev6n> dotnet --version
10.0.103
PS C:\Users\elev6n> dotnet --info
ПАКЕТ SDK ДЛЯ .NET:
...
.NET SDKs installed:
  8.0.418 [C:\Program Files\dotnet\sdk]
  9.0.311 [C:\Program Files\dotnet\sdk]
  10.0.103 [C:\Program Files\dotnet\sdk]
...
```

4. Убедитесь что среди *.NET SDKs installed:* есть SDK с версией 8.0.\*

В случае если необходимая версия платформы у вас неустановлена, установить ее
можно [здесь](https://dotnet.microsoft.com/ru-ru/download/dotnet/8.0)

### СУБД

Проще всего проверить установлена ваша СУБД или нет - это поискать ее в списке установленных приложений, если
используете Windows (_MS SQL Server_, _PostgreSQL_). Тут же можно и проверить установку клиента СУБД (_MS SQL Server
Management Studio_, _pgAdmin4_,
_DBeaver_).

## Создание базы данных

Прежде чем приступить к написанию приложения создайте базу данных. На самом деле, для решения этого модуля вам
достаточно создать лишь три таблицы: `users`, `user_roles` и `user_statuses`. Таблица `users` нужна для
аутентификации и ввода/вывода пользователей (*основной функционал*). Таблица `user_roles` нужна для редиректа
администраторов на страницу
администратора (*просто формальность*). Таблица `user_statuses` нужна для реализации блокировки пользователей 
(*бизнес-правило*).

Согласно заданию к модулю другие таблицы не предусматриваются, поэтому если вы неуверены в своих навыках проектирования
**выучите хотя бы эти 3 таблицы**, без них вам неудасться выполнить этот модуль.

Ниже представлена диаграмма отношения сущностей (_ERD_) и SQL-скрипт создания этих трех таблиц для СУБД PostgreSQL

### ERD

![ERD.png](docs/ERD.png)

### SQL-Script

```postgresql
CREATE TABLE user_roles
(
    id   SERIAL PRIMARY KEY,
    name VARCHAR(20) NOT NULL
);

CREATE TABLE user_statuses
(
    id   SERIAL PRIMARY KEY,
    name VARCHAR(20) NOT NULL
);

CREATE TABLE users
(
    id          SERIAL PRIMARY KEY,
    user_role   INT REFERENCES user_roles (id)    NOT NULL,
    user_status INT REFERENCES user_statuses (id) NOT NULL,
    last_name   VARCHAR(50)                       NOT NULL,
    first_name  VARCHAR(50)                       NOT NULL,
    patronymic  VARCHAR(50),
    login       VARCHAR(20) UNIQUE                NOT NULL,
    password    VARCHAR(50)                       NOT NULL
);
```

## Библиотеки

Решение использует **10** версию платформы .NET, поэтому и версии библиотек [здесь](Directory.Packages.props) могут быть
отличными от тех что
будут доступны на экзамене. Ниже представлен список библиотек и их безопасные версии для **.NET 8.0**

* `Microsoft.EntityFrameworkCore` (_v 8.0.4_) - основная библиотека ORM Entity Framework, берет на себя большую часть
  работы с бд
* `Microsoft.EntityFrameworkCore.Design` (_v 8.0.4_) - библиотека для генерации кода моделей и контекста
* `Microsoft.EntityFrameworkCore.Tools` (_v 8.0.4_) - нужна для миграций из VisualStudio
* `Npgsql.EntityFrameworkCore.PostgreSQL` _или_ `Microsoft.EntityFrameworkCore.SqlServer` (_v 8.0.4_) - библиотеки
  предоставляющие провайдеров конкретных СУБД, нужны для создания подключения, используйте ту которая подходит вашей
  СУБД
* `Microsoft.Extensions.Configuration` (_v 8.0.0_) - нужна для работы с файлами конфигураций
* `Microsoft.Extensions.Configuration.Json` (_v 8.0.0_) - поддержка JSON конфигураций
* `CommunityToolkit.Mvvm` (_v 8.4.0_) - готовые абстракции для архитектуры MVVM, нужна для генерации кода.

### Установка

Установить все эти библиотеки можно либо через графический интерфейс (_"Управление пакетами NuGet"_), либо через CLI
(_ниже версия с PostgreSQL_)

```powershell
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.4
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.4
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.4
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.4
dotnet add package Microsoft.Extensions.Configuration --version 8.0.0
dotnet add package Microsoft.Extensions.Configuration.Json --version 8.0.0
dotnet add package CommunityToolkit.Mvvm --version 8.4.0
```

_Этот список может изменяться и дополняться! Следите за ним, чтобы не упустить лучшее решение_

## _Всегда используйте только ту версию библиотеки в которой уверены!_

Если на версию .NET 8.0 вы установите версию библиотеки которая поддерживается только на .NET 10.0 (_например
Microsoft.EntityFrameworkCore 10.0.3_), то ваш проект **не сможет собраться!** Экспериментируйте с версиями библиотек
исключительно в учебных целях, для демонстрационного экзамена используйте проверенные варианты.

# Особенности решения

- Работа с базой данных через ORM Entity Framework
- Использование DI для регистрации компонентов
- Архитектура MVVM (Model-View-ViewModel)
