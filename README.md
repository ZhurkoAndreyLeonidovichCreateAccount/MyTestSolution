# MyTestSolution

Проект включает два консольных сервиса:

1. **FileParserService** — сервис для обработки загруженных файлов.  
2. **DataProcessorService** — сервис для дальнейшей обработки данных.

---

## 1. Установка и запуск RabbitMQ

Стандартные настройки:

Адрес: localhost

Порт: 5672

Логин/Пароль: guest / guest

Пример запуска RabbitMQ локально через Docker:
docker run -d --hostname my-rabbit --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
или в корне проекта запустить файл yml
docker-compose up

Настройка в проекте:

Файл конфигурации: appsettings.json


## 2. Установка и запуск сервисов

### Требования:
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

### Шаги:

1. Открыть терминал или PowerShell в корне проекта.
2. Восстановить зависимости: dotnet restore
3. Собери проекты: dotnet build
4. Опубликовать проекты в папку publish:
dotnet publish FileParserService.ConsoleApp -c Release -o FileParserService.ConsoleApp\publish
dotnet publish DataProcessorService.ConsoleApp -c Release -o DataProcessorService.ConsoleApp\publish

5. Запустить сервисы (каждый в отдельном окне терминала):
dotnet FileParserService.ConsoleApp\publish\FileParserService.ConsoleApp.dll
dotnet DataProcessorService.ConsoleApp\publish\DataProcessorService.ConsoleApp.dll

6. Файлы ложить в папку MyTestSolution\FileParserService.ConsoleApp\publish после парсинга они попадут в MyTestSolution\FileParserService.ConsoleApp\publish\Processed



## 3. Установка и запуск SQLite.
Создание и инициализация бд происходит автоматически. Файл лежит в корне проекта





