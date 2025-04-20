using Monocle;

namespace Celeste.Mod.BucketHelper;

public class BucketHelperModule : EverestModule
{
    public static BucketHelperModule Instance;
    public static SpriteBank SpriteBank;
    public override void Load()
    {
    }

    public override void Unload()
    {
    }

    public override void LoadContent(bool firstLoad)
    {
        BucketHelperModule.SpriteBank = new SpriteBank(GFX.Game, "Graphics/BucketSprites.xml");
    }
}