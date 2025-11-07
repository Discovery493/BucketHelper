using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.BucketHelper;

[CustomEntity("BucketHelper/BucketKillTrigger")]
public class BucketKillTrigger : Trigger
{
    public BucketKillTrigger(EntityData data, Vector2 offset) : base(data, offset)
    { }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        BucketHelperModule.Session.BucketCanLoad = false;
    }
}