namespace Application.Interfaces.Service
{
    public interface ISetupService
    {
        public Task<bool> SeedDataAdmin();
    }
}