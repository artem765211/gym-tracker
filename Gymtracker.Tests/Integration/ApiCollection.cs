namespace Gymtracker.Tests.Integration;

// Все классы с [Collection("Api")] получают один общий ApiFactory:
// контейнер запускается один раз, а не заново для каждого класса с тестами.
[CollectionDefinition("Api")]
public class ApiCollection : ICollectionFixture<ApiFactory>;
