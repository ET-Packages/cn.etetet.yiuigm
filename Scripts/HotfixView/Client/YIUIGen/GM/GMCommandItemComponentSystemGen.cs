using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;

namespace ET.Client
{
    /// <summary>
    /// 由YIUI工具自动创建 请勿修改
    /// </summary>
    [FriendOf(typeof(YIUIChild))]
    [EntitySystemOf(typeof(GMCommandItemComponent))]
    public static partial class GMCommandItemComponentSystem
    {
        [EntitySystem]
        private static void Awake(this GMCommandItemComponent self)
        {
        }

        [EntitySystem]
        private static void YIUIBind(this GMCommandItemComponent self)
        {
            self.UIBind();
        }

        private static void UIBind(this GMCommandItemComponent self)
        {
            self.u_UIBase = self.GetParent<YIUIChild>();

            self.u_ComSuperScrollViewLoopListView2 = self.UIBase.ComponentTable.FindComponent<SuperScrollView.LoopListView2>("u_ComSuperScrollViewLoopListView2");
            self.u_DataName = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueString>("u_DataName");
            self.u_DataShowParamLoop = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueBool>("u_DataShowParamLoop");
            self.u_DataDesc = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueString>("u_DataDesc");
            self.u_DataIsHistoryRecord = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueBool>("u_DataIsHistoryRecord");
            self.u_EventRun = self.UIBase.EventTable.FindEvent<UIEventP0>("u_EventRun");
            self.u_EventRunHandle = self.u_EventRun.Add(self,GMCommandItemComponent.OnEventRunInvoke);
            self.u_EventDelete = self.UIBase.EventTable.FindEvent<UIEventP0>("u_EventDelete");
            self.u_EventDeleteHandle = self.u_EventDelete.Add(self,GMCommandItemComponent.OnEventDeleteInvoke);
            self.u_EventTop = self.UIBase.EventTable.FindEvent<UIEventP0>("u_EventTop");
            self.u_EventTopHandle = self.u_EventTop.Add(self,GMCommandItemComponent.OnEventTopInvoke);

        }
    }
}
