using Celeste.Mod.Entities;
using Monocle;

namespace Celeste.Mod.BucketHelper;

[CustomEntity("BucketHelper/WdPersisWaterManager")]
public class WdPersisWaterManager : Entity
{
    private struct WdRecord
    {
        public EntityID entityID;
        public bool hasPersist;
    }

    private List<WdRecord> records;

    public WdPersisWaterManager()
    {
        records = new List<WdRecord>();
    }

    private bool CheckWdById(EntityID id)
    {
        foreach (WdRecord record in records)
        {
            if (record.entityID.Equals(id))
            {
                return true;
            }
        }
        return false;
    }

    public void AddRecord(EntityID id)
    {
        if (!CheckWdById(id))
        {
            records.Add(new WdRecord { entityID = id , hasPersist = false });
        }
    }

    public void SetPersist(EntityID id)
    {
        for (int i = 0; i < records.Count; i++)
        {
            if (records[i].entityID.Equals(id))
            {
                WdRecord record = records[i];
                record.hasPersist = true;
                records[i] = record;
            }
        }
    }
    
    public bool GetPersist(EntityID id)
    {
        foreach (WdRecord record in records)
        {
            if (record.entityID.Equals(id))
            {
                return record.hasPersist;
            }
        }
        return false;
    }
}