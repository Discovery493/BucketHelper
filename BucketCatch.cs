using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.BucketHelper;

[CustomEntity("BucketHelper/BucketCatch")]
public class BucketCatch : Entity
{
    private EntityID wdID;
    public BucketCatch(Vector2 position, EntityID wdid) : base(position)
    {
        wdID = wdid;
        Hitbox hitbox = new Hitbox(20f, 23f);
        Collider = hitbox;
    }

    public override void Update()
    {
        base.Update();
        this.Collidable = true;
        List<Holdable> list = base.CollideAllByComponent<Holdable>();
        if (list != null)
        {
            foreach (Holdable holdable in list)
            {
                Bucket bucket;
                bool flag;
                if (holdable.Entity != null)
                {
                    bucket = holdable.Entity as Bucket;
                    flag = bucket != null;
                }
                else
                {
                    bucket = null;
                    flag = false;
                }
                if (flag && !bucket.Hold.IsHeld)
                {
                    List<WaterDispenser> wdlist = Scene.Tracker.GetEntities<WaterDispenser>().OfType<WaterDispenser>().ToList();
                    foreach (WaterDispenser wd in wdlist)
                    {
                        if (wd.GetId().Equals(wdID))
                        {
                            wd.Hit(bucket, Vector2.Zero);
                        }
                    }
                }
            }
        }
        this.Collidable = false;
    }

    /*public override void Render()
    {
        base.Render();
        Color color = Color.Red;
        color.A = 127;
        Draw.Rect(Position, 20f, 15f, color);
    }*/
}