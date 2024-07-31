using System;
using YIUIFramework;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof(GMViewComponent))]
    public static partial class GMViewComponentSystem
    {
        [EntitySystem]
        private static void YIUIInitialize(this GMViewComponent self)
        {
            self.m_CommandComponent = self.Root().GetComponent<GMCommandComponent>();
            self.GMTypeLoop         = new YIUILoopScroll<int>(self, self.u_ComGMTypeLoop, typeof(GMTypeItemComponent), "u_EventSelect");
            self.GMTypeData         = new List<int>();

            foreach (var gmType in GMKeyHelper.GetKeys())
            {
                self.GMTypeData.Add(gmType);
            }

            self.GMCommandLoop = new YIUILoopScroll<GMCommandInfo>(self, self.u_ComGMCommandLoop, typeof(GMCommandItemComponent));
        }

        [EntitySystem]
        private static void Destroy(this GMViewComponent self)
        {
        }

        [EntitySystem]
        private static async ETTask DynamicEvent(this GMViewComponent self, OnGMEventClose message)
        {
            await self.UIView.CloseAsync(self);
        }

        [EntitySystem]
        private static async ETTask<bool> YIUIOpen(this GMViewComponent self)
        {
            if (self.Opened) return true;
            self.GMTypeLoop.ClearSelect();
            self.Opened = true;
            await self.GMTypeLoop.SetDataRefresh(self.GMTypeData, 0);
            return true;
        }

        [YIUILoopRenderer]
        public class GMViewComponentLoopRendererSystem : YIUILoopRendererSystem<GMViewComponent, GMTypeItemComponent, int>
        {
            protected override void Renderer(GMViewComponent self, int index, GMTypeItemComponent item, int data, bool select)
            {
                item.ResetItem(data);
                item.SelectItem(select);
                if (select)
                {
                    self.SelectTitleRefreshCommand(data);
                }
            }

            protected override void Click(GMViewComponent self, int index, GMTypeItemComponent item, int data, bool select)
            {
                item.SelectItem(select);
                if (select)
                {
                    self.SelectTitleRefreshCommand(data);
                }
            }
        }

        private static void SelectTitleRefreshCommand(this GMViewComponent self, int data)
        {
            if (self.CommandComponent.AllCommandInfo.TryGetValue(data, out var commandInfoList))
            {
                self.GMCommandLoop.SetDataRefresh(commandInfoList).NoContext();
            }
            else
            {
                self.GMCommandLoop.SetDataRefresh(new List<GMCommandInfo>()).NoContext();
            }
        }

        [YIUILoopRenderer]
        public class GMViewComponent2LoopRendererSystem : YIUILoopRendererSystem<GMViewComponent, GMCommandItemComponent, GMCommandInfo>
        {
            protected override void Renderer(GMViewComponent self, int index, GMCommandItemComponent item, GMCommandInfo data, bool select)
            {
                item.ResetItem(self.CommandComponent, data);
            }
        }

        #region YIUIEvent开始

        [EntitySystem]
        public class OnEventCloseAction : YIUIEventInvokeSystem<GMViewComponent>
        {
            protected override void Invoke(GMViewComponent self)
            {
                self.UIView.Close(self);
            }
        }

        #endregion YIUIEvent结束
    }
}
