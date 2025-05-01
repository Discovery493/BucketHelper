using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.BucketHelper;

[CustomEntity("BucketHelper/TestNewWater")]
public class TestNewWater : Entity
{
    private bool hasNewWater;
    public TestNewWater(Vector2 position) : base(position)
    {
        Position = position;
    }

    public TestNewWater(EntityData data, Vector2 offset) : this(data.Position + offset)
    {
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        hasNewWater = false;
    }

    public override void Update()
    {
        base.Update();
        var player = Scene.Tracker.GetEntity<Player>();
        if (!hasNewWater && (player != null) && (player.Dashes == 0))
        {
            var water = new Water(Position, true, false, 4f, 4f);
            Scene.Add(water);
            hasNewWater = true;
        }
    }
}