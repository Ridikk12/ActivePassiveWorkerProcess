using Azure;
using Azure.Data.Tables;

namespace ActivePasive.Model;

public class InstanceRegistration : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public string InstanceRegistrationId { get; set; }
    public bool Active { get; set; }
    public DateTime? LastUpdated { get; set; }

    public const string PartitionKeyName = "Instance";

    public void UpdateRegistrationDate()
    {
        LastUpdated = DateTime.UtcNow;
    }

    public static InstanceRegistration New(string name)
    {
        return new InstanceRegistration
        {
            LastUpdated = DateTime.UtcNow,
            PartitionKey = PartitionKeyName,
            InstanceRegistrationId = name,
            RowKey = name
        };
    }
}