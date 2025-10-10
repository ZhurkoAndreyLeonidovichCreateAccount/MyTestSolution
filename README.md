# MyTestSolution

������ �������� ��� ���������� �������:

1. **FileParserService** � ������ ��� ��������� ����������� ������.  
2. **DataProcessorService** � ������ ��� ���������� ��������� ������.

---

## 1. ��������� � ������ RabbitMQ

����������� ���������:

�����: localhost

����: 5672

�����/������: guest / guest

������ ������� RabbitMQ �������� ����� Docker:
docker run -d --hostname my-rabbit --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
��� � ����� ������� ��������� ���� yml
docker-compose up

��������� � �������:

���� ������������: appsettings.json


## 2. ��������� � ������ ��������

### ����������:
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

### ����:

1. ������� �������� ��� PowerShell � ����� �������.
2. ������������ �����������: dotnet restore
3. ������ �������: dotnet build
4. ������������ ������� � ����� publish:
dotnet publish FileParserService.ConsoleApp -c Release -o FileParserService.ConsoleApp\publish
dotnet publish DataProcessorService.ConsoleApp -c Release -o DataProcessorService.ConsoleApp\publish

5. ��������� ������� (������ � ��������� ���� ���������):
dotnet FileParserService.ConsoleApp\publish\FileParserService.ConsoleApp.dll
dotnet DataProcessorService.ConsoleApp\publish\DataProcessorService.ConsoleApp.dll

6. ����� ������ � ����� MyTestSolution\FileParserService.ConsoleApp\publish ����� �������� ��� ������� � MyTestSolution\FileParserService.ConsoleApp\publish\Processed



## 3. ��������� � ������ SQLite.
�������� � ������������� �� ���������� �������������. ���� ����� � ����� �������





