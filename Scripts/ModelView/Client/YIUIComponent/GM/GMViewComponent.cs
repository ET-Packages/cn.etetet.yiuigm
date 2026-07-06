using System.Collections.Generic;
using YIUIFramework;

namespace ET.Client
{
    public partial class GMViewComponent : Entity, IDynamicEvent<OnGMEventClose>, IDynamicEvent<OnGMEventHistoryChanged>
    {
        public bool                           Opened;
        public List<int>                      GMTypeData;
        public EntityRef<YIUISuperScrollListComponent> m_GMTypeLoop;
        public YIUISuperScrollListComponent            GMTypeLoop => m_GMTypeLoop;

        public EntityRef<YIUISuperScrollListComponent> m_GMCommandLoop;
        public YIUISuperScrollListComponent            GMCommandLoop => m_GMCommandLoop;

        public EntityRef<GMCommandComponent> m_CommandComponent;
        public GMCommandComponent            CommandComponent => m_CommandComponent;

        public List<GMCommandInfo> CurrentCommandInfoList;

        public IntPrefs m_GMType = new("GMType", null, GMHistoryDefine.HistoryType);
    }
}
