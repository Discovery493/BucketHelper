using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.BucketHelper;

[CustomEntity("BucketHelper/Bucket")]
public class Bucket : Actor
{
    public bool HasWater;
    public Holdable Hold;
    public Vector2 Speed;
    private HoldableCollider hitSeeker;
    private float noGravityTimer;
    private Vector2 previousPosition;
    private Level Level;
    private bool dead;
    private float hardVerticalHitSoundCooldown = 0f;
    private float swatTimer;
    private Collision onCollideH;
    private Collision onCollideV;
    private EntityID id;
    private Vector2 prevLiftSpeed;
    private Image image;
    //private readonly Sprite sprite;
    public Bucket(Vector2 position, bool hasWater, EntityID id) : base(position)
    {
        Position.X = position.X;
        Position.Y = position.Y;
        this.previousPosition = Position;
        HasWater = hasWater;
        Collider = new Hitbox(8f, 14f, -4f, -8f);
        Depth = 100;
        //base.Add(this.sprite = BucketHelperModule.SpriteBank.Create("bucket"));
        //this.sprite.Scale.X = -1f;
        //this.sprite.RenderPosition = this.Center - this.TopLeft;
        base.Add(this.Hold = new Holdable(0.1f));
        this.Hold.PickupCollider = new Hitbox(16f, 26f, -8f, -14f);
        this.Hold.SlowFall = false;
        this.Hold.SlowRun = false;
        this.Hold.OnPickup = OnPickup;
        this.Hold.OnRelease = OnRelease;
        this.Hold.DangerousCheck = Dangerous;
        this.Hold.OnSwat = Swat;
        this.Hold.OnHitSeeker = HitSeeker;
        this.Hold.OnHitSpinner = HitSpinner;
        this.Hold.OnHitSpring = HitSpring;
        this.Hold.SpeedGetter = () => this.Speed;
        this.onCollideH = new Collision(OnCollideH);
        this.onCollideV = new Collision(OnCollideV);
        LiftSpeedGraceTime = 0.1f;
        this.Add((Component) new VertexLight(this.Collider.Center, Color.White, 1f, 32, 64));
        this.Add((Component) new MirrorReflection());
        this.Tag = (int) Tags.TransitionUpdate;
        MTexture texture = GFX.Game["BucketHelper/bucket"];
        image = new(texture);
        image.JustifyOrigin(new Vector2(0.5f, 0.6875f));
        this.Add(image);
        //base.Add(sprite = BucketHelperModule.SpriteBank.Create("bucket"));
        //sprite.Play("idle");
    }

    // from loenn import data to constructor
    public Bucket(EntityData data, Vector2 offset, EntityID id) : this(data.Position + offset, data.Bool("hasWater", true), id)
    {
    }
    
    private string FlagName
    {
        get
        {
            return Bucket.GetFlagName(this.id);
        }
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        this.Level = this.SceneAs<Level>();
        if (this.Level.Session.GetFlag(this.FlagName))
        {
            base.RemoveSelf();
        }
    }

    public override void Update()
    {
        base.Update();
        if (this.dead)
        {
            return;
        }

        if (this.swatTimer > 0f)
        {
            this.swatTimer -= Engine.DeltaTime;
        }
        this.hardVerticalHitSoundCooldown -= Engine.DeltaTime;
        base.Depth = 100;
        if (this.Hold.IsHeld)
        {
            this.prevLiftSpeed = Vector2.Zero;
        }
        else
        {
            if (base.OnGround())
            {
                this.Speed.X = Calc.Approach(this.Speed.X, this.OnGround(this.Position + Vector2.UnitX * 3f) ? (this.OnGround(this.Position - Vector2.UnitX * 3f) ? 0.0f : -20f) : 20f, 800f * Engine.DeltaTime);
                Vector2 liftSpeed = base.LiftSpeed;
                if (liftSpeed == Vector2.Zero && this.prevLiftSpeed != Vector2.Zero)
                {
                    this.Speed = this.prevLiftSpeed;
                    this.prevLiftSpeed = Vector2.Zero;
                    this.Speed.Y = Math.Min(this.Speed.Y * 0.6f, 0f);
                    if (this.Speed.X != 0f && this.Speed.Y == 0f)
                    {
                        this.Speed.Y = -60f;
                    }

                    if (this.Speed.Y < 0f)
                    {
                        this.noGravityTimer = 0.15f;
                    }
                }
                else
                {
                    this.prevLiftSpeed = liftSpeed;
                    if (liftSpeed.Y < 0f && this.Speed.Y < 0f)
                    {
                        this.Speed.Y = 0f;
                    }
                }
            }
            else if (this.Hold.ShouldHaveGravity)
            {
                float num1 = 800f;
                if (Math.Abs(this.Speed.Y) <= 30f)
                {
                    num1 *= 0.5f;
                }
                float num2 = 350f;
                if (this.Speed.Y < 0f)
                {
                    num2 *= 0.5f;
                }
                this.Speed.X = Calc.Approach(this.Speed.X, 0f, num2 * Engine.DeltaTime);
                if ((double)this.noGravityTimer > 0f)
                {
                    this.noGravityTimer -= Engine.DeltaTime;
                }
                else
                {
                    this.Speed.Y = Calc.Approach(this.Speed.Y, 200f, num1 * Engine.DeltaTime);
                }
            }
            this.previousPosition = base.ExactPosition;
            this.MoveH(this.Speed.X * Engine.DeltaTime, this.onCollideH);
            this.MoveV(this.Speed.Y * Engine.DeltaTime, this.onCollideV);
            // TODO: right copy left
            if ((double) this.Center.X > (double) this.Level.Bounds.Right)
            {
                this.MoveH(32f * Engine.DeltaTime);
                if ((double) this.Left - 8.0 > (double) this.Level.Bounds.Right)
                    this.RemoveSelf();
            }
            else if ((double) this.Center.X < (double) this.Level.Bounds.Left)
            {
                this.MoveH(32f * Engine.DeltaTime);
                if ((double) this.Right + 8.0 < (double) this.Level.Bounds.Left)
                    this.RemoveSelf();
            }
            else if ((double) this.Top < (double) (this.Level.Bounds.Top - 4))
            {
                this.Top = (float) (this.Level.Bounds.Top + 4);
                this.Speed.Y = 0.0f;
            }
            else if ((double) this.Bottom > (double) this.Level.Bounds.Bottom && SaveData.Instance.Assists.Invincible)
            {
                this.Bottom = (float) this.Level.Bounds.Bottom;
                this.Speed.Y = -300f;
                Audio.Play("event:/game/general/assist_screenbottom", this.Position);
            }
            else if ((double)this.Top > (double)this.Level.Bounds.Bottom)
            {
                this.Die();
            }

            if ((double)this.X < (double)(this.Level.Bounds.Left + 10))
            {
                this.MoveH(32f * Engine.DeltaTime);
            }

            if (!this.dead)
            {
                this.Hold.CheckAgainstColliders();
            }

            if (this.hitSeeker != null && (double)this.swatTimer <= 0.0 && !this.hitSeeker.Check(this.Hold))
            {
                this.hitSeeker = (HoldableCollider)null;
            }
        }
    }
    
    public static string GetFlagName(EntityID id)
    {
        return "bucket_" + id.Key;
    }

    public bool Dangerous(HoldableCollider holdableCollider)
    {
        return !this.Hold.IsHeld && this.Speed != Vector2.Zero && this.hitSeeker != holdableCollider;
    }

    public void Die()
    {
        if (!this.dead)
        {
            Vector2 effectCenter = base.Center;
            if (this.Hold.IsHeld)
            {
                this.Hold.Holder.Drop();
            }

            this.dead = true;
            Audio.Play("event:/char/madeline/death", this.Position);
            base.Add(new DeathEffect(Color.DodgerBlue, new Vector2?(effectCenter - this.Position)));
            //this.sprite.Visible = false;
            image.Visible = false;
            base.Depth = -1000000;
            base.AllowPushing = false;
            base.Collidable = false;
        }
    }

    protected override void OnSquish(CollisionData data)
    {
        if (!base.TrySquishWiggle(data) && !SaveData.Instance.Assists.Invincible)
        {
            this.Die();
        }
    }

    public void Swat(HoldableCollider hc, int dir)
    {
        if (this.Hold.IsHeld && this.hitSeeker == null)
        {
            this.swatTimer = 0.1f;
            this.hitSeeker = hc;
            this.Hold.Holder.Swat(dir);
        }
    }

    public void HitSeeker(Seeker seeker)
    {
        if (!this.Hold.IsHeld)
        {
            this.Speed = (base.Center - seeker.Center).SafeNormalize(120f);
        }
        Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_side", this.Position);
    }

    public void HitSpinner(Entity spinner)
    {
        if (this.Hold.IsHeld && this.Speed.Length() < 0.01f && base.LiftSpeed.Length() < 0.01f && (this.previousPosition - base.ExactPosition).Length() < 0.01f && base.OnGround(1))
        {
            int num = Math.Sign(base.X - spinner.X);
            if (num == 0)
            {
                num = 1;
            }
            this.Speed.X = (float)num * 120f;
            this.Speed.Y = -30f;
        }
    }

    public bool HitSpring(Spring spring)
    {
        if (!this.Hold.IsHeld)
        {
            if (spring.Orientation == Spring.Orientations.Floor && this.Speed.Y >= 0f)
            {
                this.Speed.X *= 0.5f;
                this.Speed.Y = -160f;
                this.noGravityTimer = 0.15f;
                return true;
            }
            if (spring.Orientation == Spring.Orientations.WallLeft && this.Speed.X <= 0f)
            {
                base.MoveTowardsY(spring.Position.Y + 5f, 4f);
                this.Speed.X = 220f;
                this.Speed.Y = -80f;
                this.noGravityTimer = 0.1f;
                return true;
            }

            if (spring.Orientation == Spring.Orientations.WallRight && this.Speed.X >= 0f)
            {
                base.MoveTowardsY(spring.Position.Y + 5f, 4f);
                this.Speed.X = -220f;
                this.Speed.Y = -80f;
                this.noGravityTimer = 0.1f;
                return true;
            }
        }
        return false;
    }

    public override bool IsRiding(Solid solid)
    {
        return this.Speed.Y == 0f && base.IsRiding(solid);
    }

    public void ExplodeLaunch(Vector2 from)
    {
        if (!this.Hold.IsHeld)
        {
            this.Speed = (base.Center - from).SafeNormalize(120f);
            SlashFx.Burst(base.Center, this.Speed.Angle());
        }
    }

    private void OnPickup()
    {
        this.Speed = Vector2.Zero;
        base.AddTag((int)Tags.Persistent);
    }

    private void OnRelease(Vector2 force)
    {
        base.RemoveTag((int)Tags.Persistent);
        if (force.X != 0f && force.Y == 0f)
        {
            force.Y = -0.4f;
        }
        this.Speed = force * 200f;
        if (this.Speed != Vector2.Zero)
        {
            this.noGravityTimer = 0.1f;
        }
    }

    private void OnCollideH(CollisionData data)
    {
        if (data.Hit is DashSwitch)
        {
            int num = (int)(data.Hit as DashSwitch).OnDashCollide((Player)null, Vector2.UnitX * (float)Math.Sign(this.Speed.X));
        }
        Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_side", this.Position);
        if (Math.Abs(this.Speed.X) > 100f)
        {
            this.ImpactParticles(data.Direction);
        }
        this.Speed.X *= -0.4f;
    }

    private void OnCollideV(CollisionData data)
    {
        if (data.Hit is DashSwitch)
        {
            int num = (int)(data.Hit as DashSwitch).OnDashCollide((Player)null, Vector2.UnitY * (float)Math.Sign(this.Speed.Y));
        }
        if (this.Speed.Y > 0f)
        {
            if (this.hardVerticalHitSoundCooldown <= 0f)
            {
                Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_ground", this.Position, "crystal_velocity", Calc.ClampedMap(this.Speed.Y, 0f, 200f));
                this.hardVerticalHitSoundCooldown = 0.5f;
            }
            else
            {
                Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_ground", this.Position, "crystal_velocity", 0f);
            }
        }

        if (this.Speed.Y > 160f)
        {
            this.ImpactParticles(data.Direction);
        }

        if (this.Speed.Y > 140f && !(data.Hit is SwapBlock) && !(data.Hit is DashSwitch))
        {
            this.Speed.Y *= -0.6f;
        }
        else
        {
            this.Speed.Y = 0f;
        }
    }

    private void ImpactParticles(Vector2 dir)
    {
        float direction;
        Vector2 position;
        Vector2 positionRange;
        if ((double) dir.X > 0.0)
        {
            direction = (float)Math.PI;
            position = new Vector2(this.Right, this.Y - 4f);
            positionRange = Vector2.UnitY * 6f;
        }
        else if ((double) dir.X < 0.0)
        {
            direction = 0.0f;
            position = new Vector2(this.Left, this.Y - 4f);
            positionRange = Vector2.UnitY * 6f;
        }
        else if ((double) dir.Y > 0.0)
        {
            direction = (float)(-Math.PI / 2.0f);
            position = new Vector2(this.X, this.Bottom);
            positionRange = Vector2.UnitX * 6f;
        }
        else
        {
            direction = (float)(Math.PI / 2.0f);
            position = new Vector2(this.X, this.Top);
            positionRange = Vector2.UnitX * 6f;
        }
        this.Level.Particles.Emit(TheoCrystal.P_Impact, 12, position, positionRange, direction);
    }
}