# GUIDE

Этот файл содержит подробное пояснение решения с разбором ключевых моментов

## Содержание

- [Структура проекта](#структура-проекта)
- [Работа с базой данных](#работа-с-базой-данных)
    - [Предварительные требования](#предварительные-требования)
    - [Как использовать EF Core?](#как-использовать-ef-core)
        - [Scaffold DB Context](#scaffold-db-context)
            - [MS SQL Server](#для-ms-sql-server)
            - [PostgreSQL](#для-postgresql)
    - [Обзор сгенерированных классов](#обзор-сгенерированных-классов)
        - [Models](#models)
        - [Data](#data)
- [Dependency Injection](#dependency-injection)
    - [Жизненный цикл](#жизненный-цикл)
    - [Как регистрировать сервисы?](#как-регистрировать-сервисы)
    - [Как доставать сервисы из DI?](#как-доставать-сервисы-из-di)
- [MVVM](#mvvm)

# Структура проекта

```text
DemExam.Desktop/
├── Data/                       # Контекст базы данных
│   └── AppDbContext.cs
├── Exceptions/                 # Кастомные исключения
│   └── NotFoundException.cs
├── Models/                     # Модели данных
│   ├── User.cs
│   ├── UserRole.cs
│   └── UserStatus.cs
├── Services/                   # Сервисы, общие для приложения
│   ├── INavigationService.cs
│   └── NavigationService.cs
├── Store/                      # Хранение состояния
│   └── Session.cs
├── ViewModels/                 # Логика для представлений
│   ├── AdminViewModel.cs
│   └── ViewModelBase.cs
├── Views/                      # Представления
│   ├── AdminView.xaml
│   ├── AdminView.xaml.cs
│   ├── CaptchaWindow.xaml
│   ├── CaptchaWindow.xaml.cs
│   ├── LoginWindow.xaml
│   ├── LoginWindow.xaml.cs
│   ├── MainWindow.xaml
│   └── MainWindow.xaml.cs
├── App.xaml                    # Точка входа в программу
├── App.xaml.cs
├── appsettings.json            # Конфигурация приложения
└── AssemblyInfo.cs
```

# Работа с базой данных

Работа с БД осуществляется с помощью ORM (_Object-Relational Mapping_) `Entity Framework Core`.
Все сложности с подключением и отправкой запросов забирает на себя именно `EF Core`,
позволяя нам сконцентрироваться на бизнес логике, а не на шаблонном и рутинном коде.
`EF Core` отлично работает с `LINQ`, что дает нам возможность писать запросы к таблице так,
будто она коллекция в нашем C# коде.

## Предварительные требования

Перед началом работы рекомендую установить `EF CLI` чтобы можно было работать с Entity Framework из терминала.
Установить `dotnet ef` можно так:

1. Откройте терминал (PowerShell, CMD, Bash)
2. В открывшимся окне введите

```powershell
# Глобальная установка dotnet ef
dotnet tool install --global dotnet-ef
```

3. Проверьте установку, введя

```powerhell
# Проверка установки
dotnet ef
```

4. Ответ должен быть примерно таким:

```powershell
PS C:\Users\elev6n> dotnet ef

                     _/\__
               ---==/    \\
         ___  ___   |.    \|\
        | __|| __|  |  )   \\\
        | _| | _|   \_/ |  //|\\
        |___||_|       /   \\\/\\

Entity Framework Core .NET Command-line Tools 10.0.3

Usage: dotnet ef [options] [command]

Options:
  --version        Show version information
  -h|--help        Show help information
  -v|--verbose     Show verbose output.
  --no-color       Don't colorize output.
  --prefix-output  Prefix output with level.

Commands:
  database    Commands to manage the database.
  dbcontext   Commands to manage DbContext types.
  migrations  Commands to manage migrations.

Use "dotnet ef [command] --help" for more information about a command.
```

_Теперь вы можете использовать `Entity Framework` прямо из терминала_

## Как использовать `EF Core`?

Есть несколько способов как можно подружить базу данных и программу,
но сюда лучше всего подойдет генерация C# кода на основе существующей базы данных.
С помощью ровно одной команды мы можем сгенерировать контекст и модели данных,
а потом той-же командой обновить сгенерированный код,
если в базе данных например добавилась новая таблица или новые атрибуты.

### Scaffold DB Context

Операция которую мы используем называется **Scaffold DB Context**, она сгенерирует наш контекст и модели данных.
Для этой операции нужно передать два аргумента:

1. `строка подключения` - это набор различных параметров необходимый для подключения к **базе данных**
2. `провайдер` - специальный инструмент предоставляющий удобные методы для настройки подключения к **базе данных**

Также у этой операции есть опциональные параметры, которые необязательные, но о них важно знать и полезно использовать.
Параметры, которые мы будем использовать:

1. `output dir` - директория в которую будут помещены модели данных
2. `context dir` - директория в которую будет помещен контекст данных (_по умолчанию `output dir`_)
3. `context` - позволяет установить имя контекста (_по умолчанию название базы данных_)

_Для выполнения этой команды вы можете использовать два интерфейса:
обычный терминал в папке с проектом или "Консоль диспетчера пакетов" из Visual Studio.
Я предоставлю примеры как для PowerShell, так и для консоли диспетчера пакетов.
Открыть эту консоль можно по этому пути: `Средства` > `Управление пакетами NuGet` > `Консоль диспетчера пакетов`._
_Также, для SQL Server важно отметить что рассматриваются только случаи с проверкой подлинности SQL Server_

#### Использование команды:

1. Для начала убедитесь в вашем клиенте СУБД, что у вас создана нужная вам база данных
2. Откройте интерфейс для работы с `dotnet ef`
3. Введите команду с аргументом и опциональными параметрами

###### Для MS SQL Server

_Консоль диспетчера пакетов_

```powershell
Scaffold-DbContext "Server=localhost;Database=имя_вашей_бд;TrustServerCertificate=True;User Id=логин;Password=пароль" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -ContextDir Data -Context AppDbContext
```

_Терминал_

```bash
dotnet ef dbcontext scaffold "Server=localhost;Database=имя_вашей_бд;TrustServerCertificate=True;User Id=логин;Password=пароль" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir Data --context AppDbContext
```

###### Для PostgreSQL

_Консоль диспетчера пакетов_

```powershell
Scaffold-DbContext "Host=localhost;Port=5432;Database=имя_вашей_бд;Username=логин;Password=пароль" Npgsql.EntityFrameworkCore.PostgreSQL -OutputDir Models -ContextDir Data -Context AppDbContext
```

_Терминал_

```bash
dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Database=имя_вашей_бд;Username=логин;Password=пароль" Npgsql.EntityFrameworkCore.PostgreSQL --output-dir Models --context-dir Data --context AppDbContext
```

###### _Если вы внесли изменения в базу данных и вам необходимо обновить контекст и модели, то достаточно еще раз

выполнить ту-же самую команду, но с флагом `--Force` и тогда все классы автоматически обновятся_

_Консоль диспетчера пакетов_

```powershell
Scaffold-DbContext "Server=localhost;Database=имя_вашей_бд;TrustServerCertificate=True;User Id=логин;Password=пароль" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -ContextDir Data -Context AppDbContext -Force
```

_Терминал_

```bash
dotnet ef dbcontext scaffold "Server=localhost;Database=имя_вашей_бд;TrustServerCertificate=True;User Id=логин;Password=пароль" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir Data --context AppDbContext --force
```

_Из этих команд наглядно видно что является аргументов, а что опциональным параметром:_

* `"Server=localhost;Database=имя_вашей_бд;TrustServerCertificate=True;User Id=логин;Password=пароль"` -
  `строка подключения`
* `Microsoft.EntityFrameworkCore.SqlServer` - `провайдер`
* `Models` - `output dir`
* `Data` - `context dir`
* `AppDbContext` - `context`

## Обзор сгенерированных классов

Теперь когда классы моделей и контекста сгенерированы важно понять для чего мы их создавали и как их использовать.

### Models

В папке `Models` находятся **модели данных**, то есть наши таблицы из БД, но на языке C#. **Модели данных** нужны для
того, чтобы задать тип данных объектам возвращаемыми базой данных. Главное приемущество в этом то, что **объекты базы
данных** становятся **объектами C#**, а значит имеют все то-же самое, что и любые другие объекты в C#.
Это повышает согласованность и безопасность кода.

Атрибуты таблиц в модели данных представляются как обычные **автосвойства**, а вшение ключи как **virtual
автосвойства** с типом данных **объект другой таблицы**. Благодаря таким virtual ссылкам мы можем обратиться к атрибуту
который является другой связной таблицей

```csharp
namespace DemExam.Desktop.Models;

public partial class User
{
    public int Id { get; set; } // Идентификатор

    public int UserRole { get; set; } // Атрибут INT NOT NULL

    public int UserStatus { get; set; }

    public string LastName { get; set; } = null!; // Атрибут VARCHAR NOT NULL

    public string FirstName { get; set; } = null!;

    public string? Patronymic { get; set; } // Атрибут VARCHAR (допускает NULL)

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual UserRole UserRoleNavigation { get; set; } = null!; // Внешний ключ (один из 1:М)

    public virtual UserStatus UserStatusNavigation { get; set; } = null!;
}

```

```csharp
namespace DemExam.Desktop.Models;

public partial class UserRole
{
    public int Id { get; set; } // Идентификатор
 
    public string Name { get; set; } = null!; // Атрибут VARCHAR NOT NULL

    public virtual ICollection<User> Users { get; set; } = new List<User>(); // Внешний ключ (многие из 1:М)
}

```

### Data

Контекст данных отвечает за доступ к БД через модели. Он забирает на себя большую часть рутинного кода работы с базой
данных. Также он отвечает за настройку базы данных: можно писать конфигурации для таблиц, а потом применять их в SQL БД,
такой подход называется **миграции**, он удобнее чем Scaffold, потому что позволяет разрабатывать бизнес-логику
параллельно с проектированием таблиц, но требует большего понимания и кода. Знать об этом полезно, но совсем
необязательно.

```csharp
using ...

namespace DemExam.Desktop.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; } // Автосвойство = Таблица

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserStatus> UserStatuses { get; set; }

     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) // Метод соединения с БД (хранить строку подключения в коде крайне небезопасно, но для дем экзамена проблем не вызовет)
         => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=имя_вашей_бд;Username=логин;Password=пароль");

    protected override void OnModelCreating(ModelBuilder modelBuilder) // Метод создания базы данных из кода
    {
        modelBuilder.Entity<User>(entity => // Обращение к таблице
        {
            entity.HasKey(e => e.Id).HasName("users_pkey"); // PRIMARY KEY

            entity.ToTable("users"); // CREATE TABLE

            entity.HasIndex(e => e.Login, "users_login_key").IsUnique(); // UNIQUE

            entity.Property(e => e.Id).HasColumnName("id"); 
            entity.Property(e => e.FirstName) // first_name VARCHAR(50)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.Login)
                .HasMaxLength(20)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(50)
                .HasColumnName("patronymic");
            entity.Property(e => e.UserRole).HasColumnName("user_role"); 
            entity.Property(e => e.UserStatus).HasColumnName("user_status");

            entity.HasOne(d => d.UserRoleNavigation).WithMany(p => p.Users) // REFERENCES user_roles (id) 
                .HasForeignKey(d => d.UserRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_user_role_fkey");

            entity.HasOne(d => d.UserStatusNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserStatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_user_status_fkey");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_roles_pkey");

            entity.ToTable("user_roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<UserStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_statuses_pkey");

            entity.ToTable("user_statuses");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

```

# Dependency Injection

**Dependency Injection (DI)** - это паттерн проектирования суть которого создать общий для всего
приложения **контейнер**, где регистрируются все компоненты.
_Killer Feature_ паттерна - это **управление жизненным циклом** компонентов.
Существуют сервисы, которые должны хранить состояние в течении всей жизни приложения, они инициализируются один раз и
потом этот один и тот-же объект получают все классы, а бывают компоненты с одним **Instance** на один запрос,
такие объекты инициализируются каждый раз когда к ним обращаются. Если бы мы контролировали жизненный цикл каждого
объекта вручную, то нам приходилось бы писать new для каждого класса в приложении, в случае с Singleton объектами нам
пришлось бы передавать его во все классы, создавая дерево из состояний. Такой код сложен в отладке и поддержке из-за
сильной связанности.
Использование такого подхода необязательно на дем экзамене, но я могу его рекомендовать, потому что тогда у вас будет
централизованное управление всеми экземплярами компонентов в приложении. При том времени на реализацию такого паттерна
тратиться совсем мало зато бонусов он дает невероятно много.

_Пространство имен `Microsoft.Extensions.DependencyInjection` по умолчанию подключено в проекты на **.NET 8.0** и выше,_
_но если вдруг его у вас нет, полезно знать как его подключить:_

```bash
dotnet add package Microsoft.Extensions.DependencyInjection --version 8.0.0
```

## Жизненный цикл

Как я и писал выше, у сервисов может быть разный **жизненный цикл**. Одни сервисы должны жить пока живет приложение,
а другие живут только пока к ним обращаются. Интерфейс `IServiceCollection` предоставляет удобные методы для регистрации
сервисов с разным жизненным циклом:

* Transient (`AddTransient()`) - новый экземпляр создается при каждом запросе, _подходит для сервисов не имеющих
  состояние_
* Scoped (`AddScoped()`) - один экземпляр на область (обычно HTTP-запрос), ~~_нам не нужен_~~, _пока нужен для сервиса
  навигации_
* Singleton (`AddSingleton()`) - один экземпляр на все приложение, _подходит для сервисов с состоянием_

## Как регистрировать сервисы?

Зависимости обычно регистрируются в точке входа в программу (_класс `Program`/`App`_), а тип, который регистрирует их,
реализует IServiceCollection. Для создания **DI** контейнера (_для последующего извлечения сервисов из него_)
нужен тип ServiceProvider. Ниже пример регистрации сервисов в нашем приложении:

```csharp
using ...

namespace DemExam.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly ServiceProvider _serviceProvider; // Поле для создания ServiceProvider

    public App()
    {
        var services = new ServiceCollection(); // Инициализация коллекции сервисов
        ConfigureServices(services); // Регистрация сервисов в коллекции
        _serviceProvider = services.BuildServiceProvider(); // Создание объекта ServiceProvider
    }

    // Метод для регистрации сервисов
    private void ConfigureServices(IServiceCollection services) 
    {
        // Строка подключения к БД 
        var connectionString = "Host=localhost;Port=5432;Database=имя_вашей_бд;Username=логин;Password=пароль";
        
        // Data
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        // Services
        services.AddScoped<INavigationService, NavigationService>(); // Scoped потому что начинает жить после Авторизации

        // ViewModels
        services.AddTransient<AdminViewModel>(); // Transient потому что живет только пока открыта страница Администратора

        // Views
        services.AddSingleton<MainWindow>();    // Singleton потому что это главное окно приложения, пока живет оно - живет приложение.
        services.AddTransient<LoginWindow>();   // Используем Transient для всех окон и страниц (кроме MainWindow) 
        services.AddTransient<CaptchaWindow>(); // так как их состояние нам нужно, пока они открыты.
        services.AddTransient<AdminView>();     
    }
}

```

## Как доставать сервисы из DI?

Для того чтобы использовать сервис в других достаточно указать его как параметр в конструкторе класса, а **DI**
сам подставит параметр при инициализации:

```csharp
using ...

namespace DemExam.Desktop.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly INavigationService _navigationService; // Поле для использования сервиса в классе

    public MainWindow(INavigationService navigationService) // Конструктор с параметром
    {
        InitializeComponent();
        _navigationService = navigationService;             // Инициализация поля с состоянием сервиса из DI
    }
}

```

Это называется **внедрение зависимостей через конструктор**.
Но есть и другой способ получения сервиса из DI:

```csharp
using ...

namespace DemExam.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly ServiceProvider _serviceProvider; // Поле ServiceProvider для получения сервисов из DI
    
    ...
        
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Получение NavigationService из DI через ServiceProvider
        var navigationService = _serviceProvider.GetRequiredService<INavigationService>();

        // Использование сервиса
        navigationService.RegisterView<AdminView, AdminViewModel>();                        

        // Получение LoginWindow из DI через ServiceProvider
        var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
        
        // Использование окна логина
        loginWindow.Show(); 
    }
}
```

```csharp
using ...

namespace DemExam.Desktop.Views;

public partial class LoginWindow : Window
{
    private readonly IServiceProvider _serviceProvider; // Поле ServiceProvider для получения сервисов из DI

    public LoginWindow(AppDbContext context, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider; // Получение ServiceProvider из DI
    }
    
    private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
    {
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>(); // Получение MainWindow из DI через ServiceProvider 
        mainWindow.Show(); // Использование MainWindow

        Close();
    }
}
```

Второй подход рекомендуется использовать крайне редко, так как это повышает связность между классами.
Нужно это только если мы хотим **динамически создавать объекты в run-time**, во всех остальных случаях лучше
использовать **внедрение зависимостей через конструктор**.

# MVVM

**Model-View-ViewModel (MVVM)** - это современный подход к созданию клиентских приложений. Основная цель подхода -
разделить логику работы с данными и отображение данных. Эта архитектура считается эталонной и реализуется повсеместно.
В **.NET** есть библиотека предоставляющая удобные абстракции для генерации кода **MVVM**.

_Для решения демонстрационного экзамена использовать эту архитектуру и библиотеку абсолютно необязательно и может даже
навредить
если вы неподготовленный студент, но в руках знающего человека - это потрясающий инструмент, который может сильно
упростить жизнь_

## Model

**Model** - это модель данных которая является объектом для отображения. Это и есть наши [модели](Models/MODELS.md) 
которые мы сгенерировали из базы данных. В других решениях обычно используется **Data-Transfer Object (DTO)**, но в этом
проекте это будет лишним.

## View

## ViewModel

asd

asd

asd

asd

asd

