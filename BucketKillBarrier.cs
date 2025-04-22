using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;

namespace Celeste.Mod.BucketHelper;

[CustomEntity("BucketHelper/BucketKillBarrier")]
public class BucketKillBarrier : SeekerBarrier
{
    private DynData<SeekerBarrier> dyn;
    private static Color baseColor = Calc.HexToColor("40c0f0");
    public BucketKillBarrier(EntityData data, Vector2 offset) : base(data, offset)
    {
        this.dyn = new DynData<SeekerBarrier>(this);
        this.Active = true;
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
                if (flag)
                {
                    bucket.Die();
                }
            }
        }
        this.Collidable = false;
    }
    public override void Render()
    {
        foreach (Vector2 vector in this.dyn.Get<List<Vector2>>("particles"))
        {
            Draw.Pixel.Draw(this.Position + vector, Vector2.Zero, BucketKillBarrier.baseColor * 0.5f);
        }
        bool flashing = this.Flashing;
        if (flashing)
        {
            Draw.Rect(base.Collider, Color.Lerp(Color.White, BucketKillBarrier.baseColor, this.Flash) * 0.5f);
        }
    }
}