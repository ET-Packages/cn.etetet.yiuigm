using System.Collections.Generic;

namespace ET.Client
{
    public static class GMHistoryDefine
    {
        public const int HistoryType = int.MinValue; // GM 历史记录虚拟页签类型，避免与真实 EGMType 冲突。
        public const int HistoryCapacity = 20; // GM 历史记录最大保留条数。
        public const int HistoryDataVersion = 1; // GM 历史记录持久化结构版本。
        public const string HistoryTypeName = "历史记录"; // GM 历史记录虚拟页签显示名称。
        public const string HistoryPrefsKey = "YIUI_GM_HistoryData"; // StringPrefs 保存 GM 历史序列化数据的键。
    }

    [EnableClass]
    public class GMHistorySaveData
    {
        public int Version = GMHistoryDefine.HistoryDataVersion;
        public List<GMHistoryRecordData> Records = new();
    }

    [EnableClass]
    public class GMHistoryRecordData
    {
        public string CommandFullName;
        public long ExecuteTime;
        public List<GMHistoryParamData> Params = new();
    }

    [EnableClass]
    public class GMHistoryParamData
    {
        public EGMParamType ParamType;
        public string Desc;
        public string Value;
        public string EnumFullName;
    }
}
