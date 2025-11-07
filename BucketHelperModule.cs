using Monocle;

namespace Celeste.Mod.BucketHelper;

public class BucketHelperModule : EverestModule
{
    public static BucketHelperModule Instance;
    public static SpriteBank SpriteBank;
    public static WdPersisWaterManager WaterManager;
    public static bool BucketCanLoad = true;
    public override void Load()
    {
        WaterManager = new WdPersisWaterManager();
    }

    public override void Unload()
    {
    }

    public override void LoadContent(bool firstLoad)
    {
        BucketHelperModule.SpriteBank = new SpriteBank(GFX.Game, "Graphics/BucketSprites.xml");
    }
}