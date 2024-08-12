using System;
using YIUIFramework;
using System.Collections.Generic;
using UnityEngine.UI;

namespace ET.Client
{
    [FriendOf(typeof(GMViewComponent))]
    public static partial class GMViewComponentSystem
    {
        [EntitySystem]
        private static void YIUIInitialize(this GMViewComponent self)
        {
            self.m_CommandComponent = self.Root().GetComponent<GMCommandComponent>();
            self.m_GMTypeLoop       = self.AddChild<YIUILoopScrollChild, LoopScrollRect, Type, string>(self.u_ComGMTypeLoop, typeof(GMTypeItemComponent), "u_EventSelect");
            self.GMTypeData         = new List<int>();

            foreach (var gmType in GMKeyHelper.GetKeys())
            {
                self.GMTypeData.Add(gmType);
            }

            self.m_GMCommandLoop = self.AddChild<YIUILoopScrollChild, LoopScrollRect, Type>(self.u_ComGMCommandLoop, typeof(GMCommandItemComponent));
        }

        [EntitySystem]
        private static void Destroy(this GMViewComponent self)
        {
        }

        [EntitySystem]
        private static async ETTask DynamicEvent(this GMViewComponent self, OnGMEventClose message)
        {
            await self.UIView.CloseAsync();
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

        [EntitySystem]
        private static void YIUILoopRenderer(this GMViewComponent self, GMCommandItemComponent item, GMCommandInfo data, int index, bool select)
        {
            item.ResetItem(self.CommandComponent, data);
        }

        [EntitySystem]
        private static void YIUILoopRenderer(this GMViewComponent self, GMTypeItemComponent item, int data, int index, bool select)
        {
            item.ResetItem(data);
            item.SelectItem(select);
            if (select)
            {
                self.SelectTitleRefreshCommand(data);
            }
        }

        [EntitySystem]
        private static void YIUILoopOnClick(this GMViewComponent self, GMTypeItemComponent item, int data, int index, bool select)
        {
            item.SelectItem(select);
            if (select)
            {
                self.SelectTitleRefreshCommand(data);
            }
        }

        #region YIUIEvent开始

        [YIUIInvoke]
        private static void OnEventCloseInvoke(this GMViewComponent self)
        {
            self.UIView.Close();
        }

        #endregion YIUIEvent结束
    }
}