using ActivePasive.Data.Interfaces;
using ActivePasive.Model;
using ActivePasive.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivePasive.Services
{
    public class RegisterInstanceService : IRegisterInstanceService
    {
        private readonly IRegisterInstanceRepository _registerInstanceRepository;

        public RegisterInstanceService(IRegisterInstanceRepository registerInstanceRepository)
        {
            _registerInstanceRepository = registerInstanceRepository;
        }
        public async Task Register(string instanceId)
        {
            var instances = await _registerInstanceRepository.GetAll();
            var registeredInstance = instances.FirstOrDefault(x => x.InstanceRegistrationId == instanceId);

            var instance = registeredInstance ?? InstanceRegistration.New(instanceId);
            instance.UpdateRegistrationDate();

            if (!instances.Any() || !instances.Any(x => x.Active))
            {
                instance.Active = true;
            }

            await _registerInstanceRepository.Add(instance);
        }

        public async Task UnRegister(string instanceId)
        {
            await _registerInstanceRepository.Delete(instanceId);
        }

        public async Task<bool> IsActive(string instanceId)
        {
            var instances = await _registerInstanceRepository.GetAll();
            var instance = instances.FirstOrDefault(x => x.InstanceRegistrationId == instanceId);
            return instance is not null && instance.Active;
        }

        public async Task<List<string>> DeleteOrphanInstances()
        {

            var instances = await _registerInstanceRepository.GetAll();
            var oldRegistrations = instances.Where(x => x.LastUpdated.Value.AddMinutes(1) < DateTime.UtcNow).ToList();

            foreach (var instanceRegistration in oldRegistrations)
            {
                await _registerInstanceRepository.Delete(instanceRegistration.InstanceRegistrationId);
            }

            return oldRegistrations.Select(x => x.InstanceRegistrationId).ToList();
        }
    }
}
