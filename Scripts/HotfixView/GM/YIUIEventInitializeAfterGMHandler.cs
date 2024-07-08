namespace ET.Client
{
    [Event(SceneType.All)]
    public class YIUIEventInitializeAfterGMHandler : AEvent<Scene, YIUIEventInitializeAfter>
    {
        protected override async ETTask Run(Scene scene, YIUIEventInitializeAfter arg)
        {
            //根据需求自行处理 在editor下自动打开  也可以根据各种外围配置 或者 GM等级打开
            //if (Define.IsEditor) //这里默认都打开
            {
                scene.AddComponent<GMCommandComponent>();
            }

            await ETTask.CompletedTask;
        }
    }
}