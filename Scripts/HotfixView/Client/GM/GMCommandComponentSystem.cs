﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;
using YIUIFramework;

namespace ET.Client
{
    /// <summary>
    /// 文档: https://lib9kmxvq7k.feishu.cn/wiki/NYADwMydliVmQ7kWXOuc0yxGn7p
    /// </summary>
    [FriendOf(typeof(GMCommandComponent))]
    public static class GMCommandComponentSystem
    {
        [EntitySystem]
        public class GMCommandComponentAwakeSystem : AwakeSystem<GMCommandComponent>
        {
            protected override void Awake(GMCommandComponent self)
            {
                self.AllCommandInfo = new Dictionary<int, List<GMCommandInfo>>();
                GMKeyHelper.GetKeys();
                self.Init();
                YIUIMgrComponent.Inst.Root.OpenPanelAsync<GMPanelComponent>().NoContext();
            }
        }

        [EntitySystem]
        public class GMCommandComponentDestroySystem : DestroySystem<GMCommandComponent>
        {
            protected override void Destroy(GMCommandComponent self)
            {
            }
        }

        private static void Init(this GMCommandComponent self)
        {
            var types = CodeTypes.Instance.GetTypes(typeof(GMAttribute));
            foreach (var type in types)
            {
                var eventAttribut = type.GetCustomAttribute<GMAttribute>(true);
                var gmCommandInfo = new GMCommandInfo();
                var obj           = (IGMCommand)Activator.CreateInstance(type);
                gmCommandInfo.GMType        = eventAttribut.GMType;
                gmCommandInfo.GMTypeName    = GMKeyHelper.GetDesc(eventAttribut.GMType);
                gmCommandInfo.GMLevel       = eventAttribut.GMLevel;
                gmCommandInfo.GMName        = eventAttribut.GMName;
                gmCommandInfo.GMDesc        = eventAttribut.GMDesc;
                gmCommandInfo.Command       = obj;
                gmCommandInfo.ParamInfoList = obj.GetParams();

                self.AddInfo(gmCommandInfo);
            }
        }

        private static void AddInfo(this GMCommandComponent self, GMCommandInfo info)
        {
            if (!self.AllCommandInfo.ContainsKey(info.GMType))
            {
                self.AllCommandInfo.Add(info.GMType, new List<GMCommandInfo>());
            }

            var listInfo = self.AllCommandInfo[info.GMType];

            listInfo.Add(info);
        }

        public static async ETTask Run(this GMCommandComponent self, GMCommandInfo info)
        {
            /*
             * TODO 判断inof中的 GM命令需求等级
             * 取自身数据中的等级  判断是否符合要求
             */

            var paramList = info.ParamInfoList;
            var objData   = new List<object>();
            for (int i = 0; i < paramList.Count; i++)
            {
                var paramInfo = paramList[i];
                var objValue  = paramInfo.ParamType.TryToValue(paramInfo);
                objData.Add(objValue);
            }

            var banClickCode = YIUIMgrComponent.Inst.BanLayerOptionForever();
            try
            {
                var paramVo = ParamVo.Get(objData);
                var closeGM = await info.Command.Run(self.Root(), paramVo);
                ParamVo.Put(paramVo);
                if (closeGM)
                    await self.DynamicEvent(new OnGMEventClose());
            }
            catch (Exception e)
            {
                Log.Error($"GM 执行错误 {e.Message}");
            }
            finally
            {
                YIUIMgrComponent.Inst.RecoverLayerOptionForever(banClickCode);
            }
        }
    }
}