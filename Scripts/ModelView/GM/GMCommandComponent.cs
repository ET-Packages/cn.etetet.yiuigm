using System.Collections.Generic;

namespace ET.Client
{
    [ComponentOf(typeof (Scene))]
    public class GMCommandComponent: Entity, IAwake, IDestroy
    {
        public Dictionary<int, List<GMCommandInfo>> AllCommandInfo { get; set; }
    }

    //GM相关消息 关闭GMView
    public struct OnGMEventClose
    {
    }
}