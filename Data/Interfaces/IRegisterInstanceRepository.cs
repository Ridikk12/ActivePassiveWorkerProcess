using ActivePasive.Model;

namespace ActivePasive.Data.Interfaces;

public interface IRegisterInstanceRepository
{
    Task Delete(string instanceId);
    Task<List<InstanceRegistration>> GetAll();
    Task Add(InstanceRegistration instance);
}