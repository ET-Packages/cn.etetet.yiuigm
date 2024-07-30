﻿using System.Collections.Generic;
using YIUIFramework;

namespace ET.Client
{
    public partial class GMViewComponent : Entity, IDynamicEvent<OnGMEventClose>
    {
        public bool                          Opened;
        public List<int>                     GMTypeData;
        public YIUILoopScroll<int>           GMTypeLoop;
        public YIUILoopScroll<GMCommandInfo> GMCommandLoop;
        public EntityRef<GMCommandComponent> m_CommandComponent;
        public GMCommandComponent            CommandComponent => m_CommandComponent;
    }
}