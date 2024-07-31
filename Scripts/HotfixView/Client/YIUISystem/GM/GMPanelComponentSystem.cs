using System;
using YIUIFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ET.Client
{
    /// <summary>
    /// Author  YIUI
    /// Date    2023.11.30
    /// Desc
    /// </summary>
    [FriendOf(typeof(GMPanelComponent))]
    public static partial class GMPanelComponentSystem
    {
        [EntitySystem]
        private static void YIUIInitialize(this GMPanelComponent self)
        {
        }

        [EntitySystem]
        private static void Destroy(this GMPanelComponent self)
        {
        }

        [EntitySystem]
        private static async ETTask<bool> YIUIOpen(this GMPanelComponent self)
        {
            var rect = self.u_ComLimitRange.rect;
            self._LimitSize = new Vector2(rect.width, rect.height);
            var InitX = Mathf.Clamp(self._GMBtn_Pos_X.Value, 0, self._LimitSize.x);
            var InitY = Mathf.Clamp(self._GMBtn_Pos_Y.Value, 0, self._LimitSize.y);
            if (InitY == 0)
                InitY = self._LimitSize.y;
            self.u_ComGMButton.anchoredPosition = new Vector2(InitX, InitY);
            await ETTask.CompletedTask;
            return true;
        }

        [EntitySystem]
        private static void Update(this GMPanelComponent self)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!self.UIPanel.CurrentOpenViewActiveSelf)
                {
                    self.OnEventOpenGMView();
                }
                else
                {
                    if (self.UIPanel.CurrentOpenView != null)
                    {
                        var view = self.UIPanel.CurrentOpenView.GetParent<YIUIChild>().GetComponent<YIUIViewComponent>();
                        view.Close(self.UIPanel.CurrentOpenView);
                    }
                }
            }
        }

        private static void OnEventOpenGMView(this GMPanelComponent self)
        {
            self.UIPanel.OpenViewAsync<GMViewComponent>().NoContext();
        }

        #region YIUIEvent开始

        [EntitySystem]
        [FriendOf(typeof(GMPanelComponent))]
        public class OnEventOpenGMViewAction : YIUIEventInvokeSystem<GMPanelComponent>
        {
            protected override void Invoke(GMPanelComponent self)
            {
                self.OnEventOpenGMView();
            }
        }

        private static void OnEventDragAction(this GMPanelComponent self, object p1)
        {
            var data = (PointerEventData)p1;
            self.u_ComGMButton.anchoredPosition = data.position + self._Offset;
        }

        private static void OnEventEndDragAction(this GMPanelComponent self, object p1)
        {
            var endPos = self.u_ComGMButton.anchoredPosition;
            endPos.x                            = Mathf.Clamp(endPos.x, 0, self._LimitSize.x);
            endPos.y                            = Mathf.Clamp(endPos.y, 0, self._LimitSize.y);
            self.u_ComGMButton.anchoredPosition = endPos;
            self._GMBtn_Pos_X.Value             = endPos.x;
            self._GMBtn_Pos_Y.Value             = endPos.y;
        }

        private static void OnEventBeginDragAction(this GMPanelComponent self, object p1)
        {
            var data = (PointerEventData)p1;
            self._Offset = self.u_ComGMButton.anchoredPosition - data.position;
        }

        #endregion YIUIEvent结束
    }
}
