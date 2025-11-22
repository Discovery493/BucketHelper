using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.BucketHelper;

[CustomEntity("BucketHelper/WdAndLava")]
public class WdAndLava : Entity
{
    private TileGrid tiles;
    public WaterDispenser dispenser;
    public FireBarrier lava;
    public ExitBlock stone;
    public bool stonePersistent;
    private float lavaWidth;
    private float lavaHeight;
    private Vector2 lavaPosition;
    private readonly EntityID id;
    private bool hasTurnStone;
    private char tileType;
    public WdAndLava(Vector2 position,  float width, float height, Vector2 node, EntityID id, bool persistent, char tileType) : base(position)
    {
        this.id = id;
        dispenser = new WaterDispenser(node, id);
        this.lavaWidth = width;
        this.lavaHeight = height;
        this.lavaPosition = position;
        stonePersistent = persistent;
        this.tileType = tileType;
        BucketHelperModule.Session.AddRecord(id);
        hasTurnStone = BucketHelperModule.Session.GetPersist(id);
        //Logger.Log(LogLevel.Info, "WdAndLava", $"Constructor called with id: {id}.");
    }
    
    public WdAndLava(EntityData data, Vector2 offset, EntityID id) : this(data.Position + offset,  (float)data.Width, (float)data.Height, data.Nodes[0] + offset, id, data.Bool("stonePersistent", false),data.Char(nameof (tileType), '3'))
    {
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        scene.Add(dispenser);
        if (!BucketHelperModule.Session.GetNeedReNew(id))
        {
            if (hasTurnStone)
            {
                Logger.Log(LogLevel.Info, "WdAndLava", "Has Turn stone");
                return;
            }
            lava = new FireBarrier(lavaPosition, lavaWidth, lavaHeight);
            scene.Add(lava);
            return;
        }
        Logger.Log(LogLevel.Info, "WdAndLava", "Need re-new");
        stone = new ExitBlock(lavaPosition, lavaWidth, lavaHeight, tileType);
        scene.Add(stone);
        stone.AddTag(Tags.Global);
        BucketHelperModule.Session.ClearNeedReNew(id);
    }

    public override void Update()
    {
        base.Update();
        if (dispenser.getState() != 0) return;
        if (hasTurnStone) return;
        lava.RemoveSelf();
        stone = new ExitBlock(lavaPosition, lavaWidth, lavaHeight, tileType);
        Scene.Add(stone);
        hasTurnStone = true;
        if (stonePersistent)
        {
            stone.AddTag(Tags.Global);
            BucketHelperModule.Session.SetPersist(id);
        }
    }
}