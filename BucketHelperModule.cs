using Monocle;

namespace Celeste.Mod.BucketHelper;

public class BucketHelperModule : EverestModule
{
    public static BucketHelperModule Instance{get; private set;}
    public override Type SessionType => typeof(BucketHelperSession);
    public static BucketHelperSession Session => (BucketHelperSession)Instance._Session;
    public static SpriteBank SpriteBank;
    //public static WdPersisWaterManager WaterManager;
    public override void Load()
    {
        //WaterManager = new WdPersisWaterManager();
        Instance = this;
        On.Celeste.LevelExit.Begin += LevelExit_Begin;
    }

    private void LevelExit_Begin(On.Celeste.LevelExit.orig_Begin orig, LevelExit self)
    {
        //Logger.Log(LogLevel.Info,"EverestModule", $"Begin LevelExit {self}");
        Session.SetNeedReNew();
        orig(self);
    }
    
    public override void Unload()
    {
        On.Celeste.LevelExit.Begin -= LevelExit_Begin;
    }

    public override void LoadContent(bool firstLoad)
    {
        BucketHelperModule.SpriteBank = new SpriteBank(GFX.Game, "Graphics/BucketSprites.xml");
    }
}