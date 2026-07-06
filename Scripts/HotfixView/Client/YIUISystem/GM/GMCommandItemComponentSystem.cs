using UnityEngine;
using UnityEngine.UI;
using YIUIFramework;

namespace ET.Client
{
    [FriendOf(typeof(GMCommandItemComponent))]
    public static partial class GMCommandItemComponentSystem
    {
        [EntitySystem]
        private static void YIUIInitialize(this GMCommandItemComponent self)
        {
            self.m_GMParamLoop = self.AddChild<YIUISuperScrollListComponent, SuperScrollView.LoopListView2>(self.u_ComSuperScrollViewLoopListView2);
        }

        [EntitySystem]
        private static void YIUISuperScrollListRenderer(this GMCommandItemComponent self, GMParamItemComponent item, YIUISuperScrollListComponent superScrollList, int index, bool select)
        {
            item.ResetItem(self.Info.ParamInfoList[index]);
        }

        [EntitySystem]
        private static void Destroy(this GMCommandItemComponent self)
        {
        }

        public static void ResetItem(this GMCommandItemComponent self, GMCommandComponent commandComponent, GMCommandInfo info)
        {
            self.m_CommandComponent = commandComponent;
            self.Info = info;
            self.u_DataName.SetValue(info.GMName);
            self.u_DataDesc.SetValue(info.GMDesc);
            self.u_DataShowParamLoop.SetValue(info.ParamInfoList.Count >= 1);
            self.u_DataIsHistoryRecord.SetValue(info.IsHistoryRecord);
            self.WaitRefresh();
        }

        private static void WaitRefresh(this GMCommandItemComponent self)
        {
            self.GMParamLoop.SetDataRefresh(self.Info.ParamInfoList.Count);
            // GMCommandItem 高度随参数数量动态变化，嵌套列表刷新后 LayoutGroup 尚未重算
            // 必须在 renderer 回调返回前强制重建布局，否则 LoopListView2 读到的是旧高度
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)self.UIBase.OwnerGameObject.transform);
        }

        #region YIUIEvent开始

        [YIUIInvoke(GMCommandItemComponent.OnEventRunInvoke)]
        private static void OnEventRunInvoke(this GMCommandItemComponent self)
        {
            self.CommandComponent?.Run(self.Info).NoContext();
        }

        [YIUIInvoke(GMCommandItemComponent.OnEventDeleteInvoke)]
        private static void OnEventDeleteInvoke(this GMCommandItemComponent self)
        {
            self.CommandComponent?.DeleteHistory(self.Info);
        }

        [YIUIInvoke(GMCommandItemComponent.OnEventTopInvoke)]
        private static void OnEventTopInvoke(this GMCommandItemComponent self)
        {
            self.CommandComponent?.TopHistory(self.Info);
        }

        #endregion YIUIEvent结束
    }
}
