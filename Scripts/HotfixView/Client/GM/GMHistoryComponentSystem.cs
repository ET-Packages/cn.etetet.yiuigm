using System;
using System.Collections.Generic;
using System.Text;
using Sirenix.Serialization;

namespace ET.Client
{
    [FriendOf(typeof(GMCommandComponent))]
    public static class GMHistoryComponentSystem
    {
        public static void LoadHistory(this GMCommandComponent self)
        {
            self.HistorySaveData = DeserializeHistory(self.HistoryPrefs.Value);
            self.RebuildHistoryCommandInfoList(true);
        }

        public static void RecordHistory(this GMCommandComponent self, GMCommandInfo info)
        {
            if (info == null || info.IsHistoryRecord)
            {
                return;
            }

            var record = CreateRecord(info);
            if (record == null)
            {
                return;
            }

            self.HistorySaveData ??= new GMHistorySaveData();
            self.HistorySaveData.Records ??= new List<GMHistoryRecordData>();

            self.HistorySaveData.Records.RemoveAll(item => IsSameHistoryRecord(item, record));
            self.HistorySaveData.Records.Insert(0, record);
            while (self.HistorySaveData.Records.Count > GMHistoryDefine.HistoryCapacity)
            {
                self.HistorySaveData.Records.RemoveAt(self.HistorySaveData.Records.Count - 1);
            }

            self.RebuildHistoryCommandInfoList(false);
            self.SaveHistory();
        }

        public static bool DeleteHistory(this GMCommandComponent self, GMCommandInfo info)
        {
            if (info == null || !info.IsHistoryRecord || string.IsNullOrEmpty(info.CommandFullName))
            {
                return false;
            }

            self.HistorySaveData ??= new GMHistorySaveData();
            self.HistorySaveData.Records ??= new List<GMHistoryRecordData>();

            var removeCount = self.HistorySaveData.Records.RemoveAll(record => IsSameHistoryIdentity(record, info));
            if (removeCount <= 0)
            {
                return false;
            }

            self.RebuildHistoryCommandInfoList(false);
            self.SaveHistory();
            self.DynamicEvent(new OnGMEventHistoryChanged()).NoContext();
            return true;
        }

        public static bool TopHistory(this GMCommandComponent self, GMCommandInfo info)
        {
            if (info == null || !info.IsHistoryRecord || string.IsNullOrEmpty(info.CommandFullName))
            {
                return false;
            }

            self.HistorySaveData ??= new GMHistorySaveData();
            self.HistorySaveData.Records ??= new List<GMHistoryRecordData>();

            var index = FindHistoryIndex(self.HistorySaveData.Records, info);
            if (index <= 0)
            {
                return false;
            }

            var record = self.HistorySaveData.Records[index];
            self.HistorySaveData.Records.RemoveAt(index);
            self.HistorySaveData.Records.Insert(0, record);
            self.RebuildHistoryCommandInfoList(false);
            self.SaveHistory();
            self.DynamicEvent(new OnGMEventHistoryChanged()).NoContext();
            return true;
        }

        public static void RebuildHistoryCommandInfoList(this GMCommandComponent self, bool saveWhenDirty)
        {
            self.HistoryCommandInfoList = new List<GMCommandInfo>();
            self.HistorySaveData ??= new GMHistorySaveData();
            self.HistorySaveData.Records ??= new List<GMHistoryRecordData>();

            var validRecords = new List<GMHistoryRecordData>();
            var dirty = false;
            foreach (var record in self.HistorySaveData.Records)
            {
                if (TryBuildHistoryCommandInfo(self, record, out var info))
                {
                    validRecords.Add(record);
                    self.HistoryCommandInfoList.Add(info);
                }
                else
                {
                    dirty = true;
                }
            }

            if (dirty)
            {
                self.HistorySaveData.Records = validRecords;
                if (saveWhenDirty)
                {
                    self.SaveHistory();
                }
            }
        }

        private static GMHistoryRecordData CreateRecord(GMCommandInfo info)
        {
            if (string.IsNullOrEmpty(info.CommandFullName))
            {
                return null;
            }

            var record = new GMHistoryRecordData
            {
                CommandFullName = info.CommandFullName,
                ExecuteTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Params = new List<GMHistoryParamData>(),
            };

            var paramList = info.ParamInfoList ?? new List<GMParamInfo>();
            foreach (var paramInfo in paramList)
            {
                record.Params.Add(new GMHistoryParamData
                {
                    ParamType = paramInfo.ParamType,
                    Desc = paramInfo.Desc,
                    Value = paramInfo.Value,
                    EnumFullName = paramInfo.EnumFullName,
                });
            }

            return record;
        }

        private static bool TryBuildHistoryCommandInfo(GMCommandComponent self, GMHistoryRecordData record, out GMCommandInfo historyInfo)
        {
            historyInfo = null;
            if (record == null || string.IsNullOrEmpty(record.CommandFullName))
            {
                return false;
            }

            if (self.AllCommandInfoByFullName == null ||
                !self.AllCommandInfoByFullName.TryGetValue(record.CommandFullName, out var currentInfo))
            {
                return false;
            }

            var currentParams = currentInfo.ParamInfoList ?? new List<GMParamInfo>();
            var recordParams = record.Params ?? new List<GMHistoryParamData>();
            if (currentParams.Count != recordParams.Count)
            {
                return false;
            }

            var clonedParams = new List<GMParamInfo>();
            for (var i = 0; i < currentParams.Count; ++i)
            {
                var currentParam = currentParams[i];
                var recordParam = recordParams[i];
                if (!IsSameParamSignature(currentParam, recordParam) ||
                    !IsValidHistoryParamValue(currentParam, recordParam))
                {
                    return false;
                }

                clonedParams.Add(new GMParamInfo(
                    currentParam.ParamType,
                    currentParam.Desc,
                    recordParam.Value,
                    currentParam.EnumFullName));
            }

            historyInfo = new GMCommandInfo
            {
                GMType = currentInfo.GMType,
                GMTypeName = currentInfo.GMTypeName,
                GMLevel = currentInfo.GMLevel,
                GMName = currentInfo.GMName,
                GMDesc = currentInfo.GMDesc,
                CommandFullName = currentInfo.CommandFullName,
                IsHistoryRecord = true,
                HistoryExecuteTime = record.ExecuteTime,
                ParamInfoList = clonedParams,
                Command = currentInfo.Command,
            };
            return true;
        }

        private static bool IsSameParamSignature(GMParamInfo currentParam, GMHistoryParamData recordParam)
        {
            if (currentParam == null || recordParam == null)
            {
                return false;
            }

            return currentParam.ParamType == recordParam.ParamType &&
                   string.Equals(currentParam.Desc ?? string.Empty, recordParam.Desc ?? string.Empty, StringComparison.Ordinal) &&
                   string.Equals(currentParam.EnumFullName ?? string.Empty, recordParam.EnumFullName ?? string.Empty, StringComparison.Ordinal);
        }

        private static bool IsValidHistoryParamValue(GMParamInfo currentParam, GMHistoryParamData recordParam)
        {
            if (currentParam.ParamType != EGMParamType.Enum)
            {
                return true;
            }

            var enumType = CodeTypes.Instance.GetType(currentParam.EnumFullName);
            return enumType != null &&
                   enumType.IsEnum &&
                   !string.IsNullOrEmpty(recordParam.Value) &&
                   Enum.IsDefined(enumType, recordParam.Value);
        }

        private static bool IsSameHistoryIdentity(GMHistoryRecordData record, GMCommandInfo info)
        {
            return record != null &&
                   string.Equals(record.CommandFullName, info.CommandFullName, StringComparison.Ordinal) &&
                   record.ExecuteTime == info.HistoryExecuteTime;
        }

        private static int FindHistoryIndex(List<GMHistoryRecordData> records, GMCommandInfo info)
        {
            for (var i = 0; i < records.Count; ++i)
            {
                if (IsSameHistoryIdentity(records[i], info))
                {
                    return i;
                }
            }

            return -1;
        }

        private static bool IsSameHistoryRecord(GMHistoryRecordData left, GMHistoryRecordData right)
        {
            if (left == null || right == null ||
                !string.Equals(left.CommandFullName, right.CommandFullName, StringComparison.Ordinal))
            {
                return false;
            }

            var leftParams = left.Params ?? new List<GMHistoryParamData>();
            var rightParams = right.Params ?? new List<GMHistoryParamData>();
            if (leftParams.Count != rightParams.Count)
            {
                return false;
            }

            for (var i = 0; i < leftParams.Count; ++i)
            {
                if (!string.Equals(leftParams[i].Value ?? string.Empty, rightParams[i].Value ?? string.Empty, StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        private static void SaveHistory(this GMCommandComponent self)
        {
            try
            {
                self.HistorySaveData ??= new GMHistorySaveData();
                self.HistorySaveData.Version = GMHistoryDefine.HistoryDataVersion;
                var bytes = SerializationUtility.SerializeValue(self.HistorySaveData, DataFormat.JSON);
                self.HistoryPrefs.Value = bytes == null ? string.Empty : Encoding.UTF8.GetString(bytes);
            }
            catch (Exception e)
            {
                Log.Error($"GM 历史记录保存失败: {e}");
            }
        }

        private static GMHistorySaveData DeserializeHistory(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new GMHistorySaveData();
            }

            try
            {
                var bytes = Encoding.UTF8.GetBytes(value);
                var data = SerializationUtility.DeserializeValue<GMHistorySaveData>(bytes, DataFormat.JSON);
                if (data == null || data.Version != GMHistoryDefine.HistoryDataVersion)
                {
                    return new GMHistorySaveData();
                }

                data.Records ??= new List<GMHistoryRecordData>();
                return data;
            }
            catch (Exception e)
            {
                Log.Error($"GM 历史记录反序列化失败，将按空历史处理: {e}");
                return new GMHistorySaveData();
            }
        }
    }
}
