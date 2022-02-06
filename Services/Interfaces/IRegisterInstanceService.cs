namespace ActivePasive.Services.Interfaces;

public interface IRegisterInstanceService
{
    Task Register(string instanceId);
    Task UnRegister(string instanceId);
    Task<bool> IsActive(string instanceId);
    Task<List<string>> DeleteOrphanInstances();
}