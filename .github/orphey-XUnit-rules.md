# Полное руководство по написанию модульных тестов с xUnit и NSubstitute

## Оглавление
1. [Основные принципы](#основные-принципы)
2. [Структура тестов](#структура-тестов)
3. [Именование тестов](#именование-тестов)
4. [Параметризованные тесты](#параметризованные-тесты)
5. [Асинхронные тесты](#асинхронные-тесты)
6. [Мокирование зависимостей](#мокирование-зависимостей)
7. [Обработка исключений](#обработка-исключений)
8. [Организация тестовых классов](#организация-тестовых-классов)
9. [Рекомендации для CI/CD](#рекомендации-для-cicd)
10. [Запрещенные практики](#запрещенные-практики)


- Все комментарии на русском языке
- Четкое следование AAA-паттерну
- Изолированность и детерминированность тестов
- Использование NSubstitute для мокинга
- Следование соглашениям об именовании

## Основные принципы

1. **Паттерн AAA (Arrange-Act-Assert)**:
   - Arrange: подготовка тестовых данных и моков
   - Act: выполнение тестируемого метода
   - Assert: проверка результатов

2. **Изолированность тестов**:
   - Каждый тест должен работать независимо от других
   - Не должно быть зависимостей между тестами

3. **Детерминированность**:
   - Тесты должны всегда давать одинаковый результат при одинаковых входных данных
   - Избегайте недетерминированных данных (например, DateTime.Now)

4. **Все тесты используют xUnit + NSubstitute**
  Единый стек технологий для всего проекта

5. **Строгое следование AAA-паттерну**
  Четкое разделение на Arrange-Act-Assert

6. **Тесты должны быть атомарными**
  Один тест = один сценарий проверки

7. **Библиотеку Automapper НЕ моккировать. Использоывать из DI-контейнера.

7. **Комментарии на русском**
   - Все пояснительные комментарии должны быть на русском языке
   - Комментируйте неочевидные моменты и сложную логику



## Структура тестов

```csharp
[Fact]
[Trait("Category", "Unit")]
public void MethodName_Scenario_ExpectedBehavior()
{
    // Arrange
    var dependency = Substitute.For<IDependency>();
    dependency.GetValue().Returns(42);
    var sut = new SystemUnderTest(dependency);

    // Act
    var result = sut.MethodToTest();

    // Assert
    Assert.Equal(42, result);
    dependency.Received(1).GetValue();
}
```

## Именование тестов

Формат: `[ТестируемыйМетод]_[Сценарий]_[ОжидаемыйРезультат]`

Примеры:
- `CalculateDiscount_WhenPremiumCustomer_Returns20Percent`
- `ValidateUser_WithNullEmail_ThrowsArgumentNullException`
- `SaveData_WhenDatabaseUnavailable_RetriesThreeTimes`

## Параметризованные тесты

### Простые случаи с InlineData
```csharp
[Theory]
[InlineData(1, 1, 2)]
[InlineData(2, 3, 5)]
[InlineData(0, 0, 0)]
public void Add_TwoNumbers_ReturnsSum(int a, int b, int expected)
{
    var calculator = new Calculator();
    var result = calculator.Add(a, b);
    Assert.Equal(expected, result);
}
```

### Сложные данные с MemberData
```csharp
public static IEnumerable<object[]> TestData =>
    new List<object[]>
    {
        new object[] { 10, 2, 5 },
        new object[] { 9, 3, 3 },
        new object[] { 15, 5, 3 }
    };

[Theory]
[MemberData(nameof(TestData))]
public void Divide_TwoNumbers_ReturnsCorrectResult(int a, int b, int expected)
{
    var calculator = new Calculator();
    var result = calculator.Divide(a, b);
    Assert.Equal(expected, result);
}
```

## Асинхронные тесты

```csharp
[Fact]
public async Task GetDataAsync_ValidRequest_ReturnsData()
{
    // Arrange
    var mockService = Substitute.For<IDataService>();
    mockService.GetDataAsync().Returns(Task.FromResult(new Data()));

    // Act
    var result = await new Controller(mockService).GetDataAsync();

    // Assert
    Assert.NotNull(result);
}

[Fact]
public async Task ProcessAsync_InvalidInput_ThrowsException()
{
    var service = Substitute.For<IService>();
    service.ProcessAsync(Arg.Any<string>()).ThrowsAsync(new InvalidOperationException());

    var sut = new ServiceConsumer(service);

    await Assert.ThrowsAsync<InvalidOperationException>(
        () => sut.ProcessDataAsync("invalid"));
}
```

## Мокирование зависимостей

### Базовое мокирование
```csharp
var mock = Substitute.For<IDependency>();
mock.GetValue().Returns(42);
```

### Проверка вызовов
```csharp
mock.Received(1).GetValue(); // Проверяет, что метод вызван ровно 1 раз
mock.DidNotReceive().GetValue(); // Проверяет, что метод не вызывался
```

### Мокирование с параметрами
```csharp
mock.Process(Arg.Any<string>()).Returns(true);
mock.Process(Arg.Is<string>(s => s.Length > 5)).Returns(false);
```

### Мокирование последовательных вызовов
```csharp
mock.GetCount().Returns(1, 2, 3);
// Первый вызов вернет 1, второй - 2, третий - 3
```

## Обработка исключений

```csharp
[Fact]
public void Process_InvalidInput_ThrowsArgumentException()
{
    var sut = new Processor();

    var ex = Assert.Throws<ArgumentException>(
        () => sut.Process(null));

    Assert.Equal("Input cannot be null", ex.Message);
}

[Fact]
public void Dependency_WhenFails_ThrowsSpecificException()
{
    var mock = Substitute.For<IDependency>();
    mock.GetData().Throws(new InvalidOperationException("Error"));

    var sut = new Service(mock);

    Assert.Throws<InvalidOperationException>(
        () => sut.ProcessData());
}
```

## Организация тестовых классов

### Использование конструктора и IDisposable
```csharp
public class MyTests : IDisposable
{
    private readonly IDependency _mockDependency;
    private readonly SystemUnderTest _sut;

    public MyTests()
    {
        _mockDependency = Substitute.For<IDependency>();
        _sut = new SystemUnderTest(_mockDependency);
    }

    public void Dispose()
    {
        // Очистка ресурсов при необходимости
    }

    [Fact]
    public void Test1() { /* ... */ }
}
```

### Группировка тестов с помощью Trait
```csharp
[Fact]
[Trait("Category", "Unit")]
public void UnitTest1() { /* ... */ }

[Fact]
[Trait("Category", "Integration")]
public void IntegrationTest1() { /* ... */ }
```

### Общие ресурсы с IClassFixture
```csharp
public class DatabaseFixture : IDisposable
{
    public DatabaseFixture() { /* Инициализация */ }
    public void Dispose() { /* Очистка */ }
}

[CollectionDefinition("DatabaseCollection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }

[Collection("DatabaseCollection")]
public class DatabaseTest1
{
    public DatabaseTest1(DatabaseFixture fixture) { /* ... */ }
}
```

## Рекомендации для CI/CD

1. **Покрытие кода**:
   - Минимальное покрытие: 80% для критических компонентов
   - Используйте coverlet.collector для сбора метрик

2. **Параллельное выполнение**:
   ```csharp
   [Collection("NonParallelTests")]
   public class DatabaseTests
   {
       // Тесты, которые нельзя выполнять параллельно
   }
   ```

3. **Таймауты**:
   ```csharp
   [Fact(Timeout = 1000)] // 1 секунда
   public void FastAlgorithm_CompletesInUnder1Second()
   {
       // Тест
   }
   ```

4. **Категоризация тестов**:
   - `[Trait("Category", "Unit")]` для модульных тестов
   - `[Trait("Category", "Integration")]` для интеграционных тестов

## Запрещенные практики

-  **Тестирование приватных методов**
Тестируйте только публичное API. Если приватный метод сложный, вынесите его в отдельный класс.

-  **Избыточные моки**
Мокируйте только те зависимости, которые действительно влияют на тестируемый метод.

-  **Зависимые тесты**
Тесты не должны зависеть от порядка выполнения или состояния, оставшегося от других тестов.

-  **Магические числа**
Используйте константы с понятными именами вместо "магических" чисел в тестах.

-  **Логика в тестах**
Избегайте циклов, условий и сложной логики в тестах. Тесты должны быть простыми и линейными.

-  **Интеграционные тесты как модульные**
Четко разделяйте модульные и интеграционные тесты с помощью атрибутов `[Trait]`.

## Заключение

Следуя этим рекомендациям, вы сможете создавать:
- Читаемые и поддерживаемые тесты
- Надежные проверки функциональности
- Быстрые и изолированные тестовые прогоны
- Легко интегрируемые в CI/CD процессы тесты

Помните, что хорошие тесты - это такая же важная часть кодовой базы, как и основной код продукта.