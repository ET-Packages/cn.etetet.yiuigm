using System;
using YIUIFramework;
using System.Collections.Generic;

namespace ET.Client
{
    [FriendOf(typeof(GMViewComponent))]
    public static partial class GMViewComponentSystem
    {
        [EntitySystem]
        private static void YIUIInitialize(this GMViewComponent self)
        {
            self.m_CommandComponent = self.Root().GetComponent<GMCommandComponent>();
            self.m_GMTypeLoop = self.AddChild<YIUISuperScrollListComponent, SuperScrollView.LoopListView2>(self.u_ComTitleLoopLoopListView2);
            self.GMTypeLoop.SetOnClick();
            self.GMTypeData = new List<int>();

            self.GMTypeData.Add(GMHistoryDefine.HistoryType);
            foreach (var gmType in GMKeyHelper.GetKeys())
            {
                self.GMTypeData.Add(gmType);
            }

            self.m_GMCommandLoop = self.AddChild<YIUISuperScrollListComponent, SuperScrollView.LoopListView2>(self.u_ComCommandLoopLoopListView2);
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
        private static async ETTask DynamicEvent(this GMViewComponent self, OnGMEventHistoryChanged message)
        {
            await self.Rebuild();
        }

        [EntitySystem]
        private static async ETTask<bool> YIUIOpen(this GMViewComponent self)
        {
            if (self.Opened) return true;
            self.GMTypeLoop.ClearSelect();
            self.Opened = true;
            var openIndex = self.GetOpenTypeIndex();
            self.GMTypeLoop.SetDataRefreshSelect(self.GMTypeData.Count, openIndex);
            await ETTask.CompletedTask;
            return true;
        }

        private static int GetOpenTypeIndex(this GMViewComponent self)
        {
            if (YIUIConstHelper.Const.OpenGMViewFirstType)
            {
                return 0;
            }

            var typeIndex = self.GMTypeData.IndexOf(self.m_GMType.Value);
            return typeIndex >= 0 ? typeIndex : 0;
        }

        private static async ETTask Rebuild(this GMViewComponent self)
        {
            EntityRef<GMViewComponent> selfRef = self;
            var panelComponent = self.UIView.GetPanelComponent();
            if (panelComponent == null)
            {
                return;
            }

            self.Opened = false;
            await self.UIView.CloseAsync(false);

            self = selfRef;
            if (self == null)
            {
                return;
            }

            panelComponent = self.UIView.GetPanelComponent();
            if (panelComponent == null)
            {
                return;
            }

            await panelComponent.OpenViewAsync<GMViewComponent>();
        }

        private static void SelectTitleRefreshCommand(this GMViewComponent self, int data)
        {
            if (data == GMHistoryDefine.HistoryType)
            {
                self.CommandComponent.RebuildHistoryCommandInfoList(true);
                self.CurrentCommandInfoList = self.CommandComponent.HistoryCommandInfoList;
            }
            else if (self.CommandComponent.AllCommandInfo.TryGetValue(data, out var commandInfoList))
            {
                self.CurrentCommandInfoList = commandInfoList;
            }
            else
            {
                self.CurrentCommandInfoList = new List<GMCommandInfo>();
            }

            self.GMCommandLoop.SetDataRefresh(self.CurrentCommandInfoList.Count);
        }

        [EntitySystem]
        private static void YIUISuperScrollListRenderer(this GMViewComponent self, GMCommandItemComponent item, YIUISuperScrollListComponent superScrollList, int index, bool select)
        {
            item.ResetItem(self.CommandComponent, self.CurrentCommandInfoList[index]);
        }

        [EntitySystem]
        private static void YIUISuperScrollListRenderer(this GMViewComponent self, GMTypeItemComponent item, YIUISuperScrollListComponent superScrollList, int index, bool select)
        {
            var data = self.GMTypeData[index];
            item.ResetItem(data);
            item.SelectItem(select);
            if (select)
            {
                self.SelectTitleRefreshCommand(data);
            }
        }

        [EntitySystem]
        private static void YIUISuperScrollListOnClick(this GMViewComponent self, GMTypeItemComponent item, YIUISuperScrollListComponent superScrollList, int index, bool select)
        {
            item.SelectItem(select);
            if (select)
            {
                var data = self.GMTypeData[index];
                self.m_GMType.Value = data;
                self.SelectTitleRefreshCommand(data);
            }
        }

        #region YIUIEvent开始

        #endregion YIUIEvent结束
    }
}
