using System;
using YIUIFramework;
using System.Collections.Generic;

namespace ET.Client
{
    /// <summary>
    /// Author  YIUI
    /// Date    2023.11.30
    /// Desc
    /// </summary>
    [FriendOf(typeof(GMCommandItemComponent))]
    public static partial class GMCommandItemComponentSystem
    {
        [YIUILoopRenderer]
        public class GMCommandItemComponentLoopRendererSystem : YIUILoopRendererSystem<GMCommandItemComponent, GMParamItemComponent, GMParamInfo>
        {
            protected override void Renderer(GMCommandItemComponent self, int index, GMParamItemComponent item, GMParamInfo data, bool select)
            {
                item.ResetItem(data);
            }
        }

        [EntitySystem]
        private static void YIUIInitialize(this GMCommandItemComponent self)
        {
            self.GMParamLoop = new YIUILoopScroll<GMParamInfo>(self, self.u_ComParamLoop, typeof(GMParamItemComponent));
        }

        [EntitySystem]
        private static void Destroy(this GMCommandItemComponent self)
        {
        }

        public static void ResetItem(this GMCommandItemComponent self, GMCommandComponent commandComponent, GMCommandInfo info)
        {
            self.m_CommandComponent = commandComponent;
            self.Info               = info;
            self.u_DataName.SetValue(info.GMName);
            self.u_DataDesc.SetValue(info.GMDesc);
            self.u_DataShowParamLoop.SetValue(info.ParamInfoList.Count >= 1);
            self.WaitRefresh().NoContext();
        }

        private static async ETTask WaitRefresh(this GMCommandItemComponent self)
        {
            await self.GMParamLoop.SetDataRefresh(self.Info.ParamInfoList);
            await self.GMParamLoop.RefreshCells();
        }

        #region YIUIEvent开始

        private static void OnEventRunAction(this GMCommandItemComponent self)
        {
            self.CommandComponent?.Run(self.Info).NoContext();
        }

        #endregion YIUIEvent结束
    }
}