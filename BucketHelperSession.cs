namespace Celeste.Mod.BucketHelper;

public class BucketHelperSession : EverestModuleSession
{
    public bool BucketCanLoad = true;
    //public WdPersisWaterManager LiquidManager = new();
    public struct WdRecord
    {
        public EntityID EntityId;
        public bool HasPersist;
        public bool NeedReNew;
    }

    public List<WdRecord> records = new();

    public bool CheckWdById(EntityID id)
    {
        foreach (WdRecord record in records)
        {
            if (record.EntityId.Equals(id))
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
            records.Add(new WdRecord { EntityId = id, HasPersist = false, NeedReNew = false });
        }
    }

    public void SetPersist(EntityID id)
    {
        for (int i = 0; i < records.Count; i++)
        {
            if (records[i].EntityId.Equals(id))
            {
                WdRecord record = records[i];
                record.HasPersist = true;
                records[i] = record;
            }
        }
    }
    
    public bool GetPersist(EntityID id)
    {
        foreach (WdRecord record in records)
        {
            if (record.EntityId.Equals(id))
            {
                return record.HasPersist;
            }
        }
        return false;
    }

    public void ClearRecord()
    {
        records.Clear();
    }

    public void SetNeedReNew()
    {
        for (int i = 0; i < records.Count; i++)
        {
            if (records[i].HasPersist)
            {
                WdRecord record = records[i];
                record.NeedReNew = true;
                records[i] = record;
            }
        }
    }

    public bool GetNeedReNew(EntityID id)
    {
        foreach (WdRecord record in records)
        {
            if (record.EntityId.Equals(id))
            {
                return record.NeedReNew;
            }
        }
        return false;
    }

    public void ClearNeedReNew(EntityID id)
    {
        for (int i = 0; i < records.Count; i++)
        {
            if (records[i].EntityId.Equals(id))
            {
                WdRecord record = records[i];
                record.NeedReNew = false;
                records[i] = record;
            }
        }
    }
}