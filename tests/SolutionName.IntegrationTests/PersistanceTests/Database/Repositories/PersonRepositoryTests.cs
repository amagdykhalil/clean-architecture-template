

namespace SolutionName.IntegrationTests.PersistanceTests2.Database.Repositories
{
    [Collection(TestCollections.DatabaseTests)]
    public class PersonRepositoryTests
    {
        private readonly IPersonRepository _personRepository;
        private readonly IUnitOfWork _unitOfWork;

        private readonly IServiceProvider _serviceProvider;

        public PersonRepositoryTests(DatabaseTestEnvironmentFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
            _personRepository = _serviceProvider.GetRequiredService<IPersonRepository>();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
        }

        [Fact]
        public async Task AddAsync_ShouldAddNewPerson()
        {
            // Arrange
            var person = TestDataGenerators.PersonFaker().Generate();

            // Act
            await _personRepository.AddAsync(person);
            await _unitOfWork.SaveChangesAsync();

            // Assert
            Assert.NotEqual(0, person.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnPerson()
        {
            // Arrange
            var person = TestDataGenerators.PersonFaker().Generate();
            await _personRepository.AddAsync(person);
            await _unitOfWork.SaveChangesAsync();

            // Act
            var foundPerson = await _personRepository.GetByIdAsync(person.Id);

            // Assert
            Assert.NotNull(foundPerson);
            Assert.Equal(person.FirstName, foundPerson.FirstName);
            Assert.Equal(person.LastName, foundPerson.LastName);
        }
    }
}