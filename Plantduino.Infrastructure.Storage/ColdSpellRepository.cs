using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Rumr.Plantduino.Domain.Repositories;

namespace Rumr.Plantduino.Infrastructure.Storage
{
    public class ColdSpellRepository : IColdSpellRepository
    {
        private readonly CloudTable _coldSpellsTable;

        public ColdSpellRepository()
        {
            var account = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var tableClient = account.CreateCloudTableClient();
            _coldSpellsTable = tableClient.GetTableReference("coldspells");
            _coldSpellsTable.CreateIfNotExists();
        }

        public async Task<ColdSpell> GetAsync(string deviceId, string sensorId)
        {
            var operation = TableOperation.Retrieve<ColdSpellEntity>(deviceId, sensorId);

            var tableResult = await _coldSpellsTable.ExecuteAsync(operation);

            var coldSpellEntity = tableResult.Result as ColdSpellEntity;

            if (coldSpellEntity != null)
            {
                return new ColdSpell
                {
                    DeviceId = coldSpellEntity.PartitionKey,
                    SensorId = coldSpellEntity.RowKey,
                    AlertedAt = coldSpellEntity.LastAlertedAt
                };
            }

            return null;
        }

        public async Task SaveAsync(ColdSpell coldSpell)
        {
            var coldSpellEntity = new ColdSpellEntity
            {
                PartitionKey = coldSpell.DeviceId,
                RowKey = coldSpell.SensorId,
                LastAlertedAt = coldSpell.AlertedAt
            };

            var operation = TableOperation.InsertOrReplace(coldSpellEntity);

            await _coldSpellsTable.ExecuteAsync(operation);
        }
    }

    public class ColdSpellEntity : TableEntity
    {
        public DateTime LastAlertedAt { get; set; }
    }
}
