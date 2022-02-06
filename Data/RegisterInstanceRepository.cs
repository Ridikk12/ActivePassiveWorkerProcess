using ActivePassive.Data.Interfaces;
using ActivePassive.Model;
using Azure.Data.Tables;

namespace ActivePassive.Data
{
    internal class RegisterInstanceRepository : IRegisterInstanceRepository
    {
        private readonly TableClient _tableClient;
        public RegisterInstanceRepository(TableClient tableClient)
        {
            _tableClient = tableClient;
        }
        public Task Delete(string instanceId)
        {
            return _tableClient.DeleteEntityAsync(InstanceRegistration.PartitionKeyName, instanceId);
        }

        public async Task<List<InstanceRegistration>> GetAll()
        {
            var instances = new List<InstanceRegistration>();

            var pages = _tableClient.QueryAsync<InstanceRegistration>().AsPages();

            await foreach (var page in pages)
            {
                instances.AddRange(page.Values);
            }

            return instances;
        }

        public Task Add(InstanceRegistration instance)
        {
            return _tableClient.UpsertEntityAsync(instance);
        }
    }
}
