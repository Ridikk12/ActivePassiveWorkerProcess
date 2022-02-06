using ActivePassive.Model;

namespace ActivePassive.Data.Interfaces;

public interface IRegisterInstanceRepository
{
    Task Delete(string instanceId);
    Task<List<InstanceRegistration>> GetAll();
    Task Add(InstanceRegistration instance);
}