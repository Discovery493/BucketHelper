using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.BucketHelper;

[CustomEntity("BucketHelper/WdAndLava")]
public class WdAndLava : Entity
{
    public WaterDispenser dispenser;
    public Water water;
    public bool waterPersistent;
    private float waterWidth;
    private float waterHeight;
    private Vector2 waterPosition;
    private readonly EntityID id;
    private bool hasNewWater;
    public WdAndLava(Vector2 position,  float width, float height, Vector2 node, EntityID id, bool persistent) : base(position)
    {
        this.id = id;
        dispenser = new WaterDispenser(node, id);
        this.waterWidth = width;
        this.waterHeight = height;
        this.waterPosition = position;
        waterPersistent = persistent;
        BucketHelperModule.Session.AddRecord(id);
        hasNewWater = BucketHelperModule.Session.GetPersist(id);
        //Logger.Log(LogLevel.Info, "WdAndLava", $"Constructor called with id: {id}.");
    }
}