using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.BucketHelper;

[CustomEntity("BucketHelper/WaterDispenser")]
public class WaterDispenser : Solid
{
    private EntityID id;
    private int state; // 2:no bucket 1:have bucket but not fucked 0:fucked
    private readonly Sprite sprite;
    //public Collider catchBucket;
    // :base means call its base class's constructor function
    public WaterDispenser(Vector2 position, EntityID id) : base(position, 0f, 0f, true)
    {
        this.id = id;
        base.Add(sprite = BucketHelperModule.SpriteBank.Create("water_dispenser"));
        base.Collider = new Hitbox(22f, 32f, -11f, -33f);
    }
    
    // from loenn import data to constructor
    public WaterDispenser(EntityData data, Vector2 offset, EntityID id) : this(data.Position + offset, id)
    {
    }

    private string FlagName
    {
        get
        {
            return WaterDispenser.GetFlagName(this.id);
        }
    }
    
    public static string GetFlagName(EntityID id)
    {
        return "water_dispenser_" + id.Key;
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        this.state = 2;
        this.Collidable = true;
    }

    public void Hit(Bucket bucket, Vector2 direction)
    {
        if (this.state == 2)
        {
            bucket.RemoveSelf();
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            Audio.Play("event:/game/05_mirror_temple/button_activate", this.Position);
            this.sprite.Play("insert");
            this.state = 1;
        }
    }
}