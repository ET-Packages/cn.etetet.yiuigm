using System;
using System.Collections.Generic;

namespace ET.Client
{
    //GM实例信息
    [EnableClass]
    public class GMCommandInfo
    {
        public int               GMType;        //命令类型
        public string            GMTypeName;    //命令名称
        public int               GMLevel;       //命令等级
        public string            GMName;        //命令名称
        public string            GMDesc;        //命令描述
        public string            CommandFullName; //命令类全名
        public bool              IsHistoryRecord; //是否历史记录克隆项
        public long              HistoryExecuteTime; //历史记录执行时间戳
        public List<GMParamInfo> ParamInfoList; //泛型参数类型
        public IGMCommand        Command;       //实例
    }
}
