using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.BucketHelper;

[CustomEntity("BucketHelper/WdAndWater")]
public class WdAndWater : Entity
{
    public WaterDispenser dispenser;
    public Water water;
    public bool waterPersistent;
    private float waterWidth;
    private float waterHeight;
    private Vector2 waterPosition;
    private readonly EntityID id;
    private bool hasNewWater;
    public WdAndWater(Vector2 position,  float width, float height, Vector2 node, EntityID id, bool persistent) : base(position)
    {
        this.id = id;
        dispenser = new WaterDispenser(node, id);
        this.waterWidth = width;
        this.waterHeight = height;
        this.waterPosition = position;
        waterPersistent = persistent;
        BucketHelperModule.WaterManager.AddRecord(id);
        hasNewWater = BucketHelperModule.WaterManager.GetPersist(id);
        //Logger.Log(LogLevel.Info, "WdAndWater", $"Constructor called with id: {id}.");
    }

    public WdAndWater(EntityData data, Vector2 offset, EntityID id) : this(data.Position + offset,  (float)data.Width, (float)data.Height, data.Nodes[0] + offset, id, data.Bool("waterPersistent", false))
    {
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        scene.Add(dispenser);
    }

    public override void Update()
    {
        base.Update();
        if (dispenser.getState() != 0) return;
        if (hasNewWater) return;
        water = new Water(waterPosition, true, false, waterWidth, waterHeight);
        Scene.Add(water);
        hasNewWater = true;
        if (waterPersistent)
        {
            water.AddTag(Tags.Global);
            BucketHelperModule.WaterManager.SetPersist(id);
        }
    }
}