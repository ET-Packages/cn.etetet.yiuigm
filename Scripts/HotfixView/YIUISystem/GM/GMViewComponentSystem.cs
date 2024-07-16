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
            self.GMTypeLoop         = new YIUILoopScroll<int, GMTypeItemComponent>(self, self.u_ComGMTypeLoop, self.GMTypeTitleRenderer);
            self.GMTypeLoop.SetOnClickInfo("u_EventSelect", self.OnClickTitle);
            self.GMTypeData = new List<int>();

            foreach (var gmType in GMKeyHelper.GetKeys())
            {
                self.GMTypeData.Add(gmType);
            }

            self.GMCommandLoop = new YIUILoopScroll<GMCommandInfo, GMCommandItemComponent>(self, self.u_ComGMCommandLoop, self.GMCommandRenderer);
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

        private static void OnClickTitle(this GMViewComponent self, int index, int data, GMTypeItemComponent item, bool select)
        {
            item.SelectItem(select);
            if (select)
            {
                self.SelectTitleRefreshCommand(data);
            }
        }

        private static void GMTypeTitleRenderer(this GMViewComponent self, int index, int data, GMTypeItemComponent item, bool select)
        {
            item.ResetItem(data);
            item.SelectItem(select);
            if (select)
            {
                self.SelectTitleRefreshCommand(data);
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

        private static void GMCommandRenderer(this GMViewComponent self, int index, GMCommandInfo data, GMCommandItemComponent item, bool select)
        {
            item.ResetItem(self.CommandComponent, data);
        }

        #region YIUIEvent开始

        private static void OnEventCloseAction(this GMViewComponent self)
        {
            self.UIView.Close(self);
        }

        #endregion YIUIEvent结束
    }
}