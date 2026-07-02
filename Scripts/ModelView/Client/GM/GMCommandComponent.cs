using System.Collections.Generic;
using YIUIFramework;

namespace ET.Client
{
    /// <summary>
    /// 文档: https://lib9kmxvq7k.feishu.cn/wiki/NYADwMydliVmQ7kWXOuc0yxGn7p
    /// </summary>
    [ComponentOf(typeof (Scene))]
    public class GMCommandComponent: Entity, IAwake, IDestroy
    {
        public Dictionary<int, List<GMCommandInfo>> AllCommandInfo { get; set; }
        public Dictionary<string, GMCommandInfo> AllCommandInfoByFullName { get; set; }
        public List<GMCommandInfo> HistoryCommandInfoList { get; set; }
        public GMHistorySaveData HistorySaveData { get; set; }
        public StringPrefs HistoryPrefs = new(GMHistoryDefine.HistoryPrefsKey, null, string.Empty);
    }

    //GM相关消息 关闭GMView
    public struct OnGMEventClose
    {
    }

    public struct OnGMEventHistoryChanged
    {
    }
}
