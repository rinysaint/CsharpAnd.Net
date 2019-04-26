using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace MinFrm.Dal
{
    public static class BatteryStatus
    {
        /// <summary>
        /// 获取描述信息
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string description(this Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return en.ToString();
        }
        public static Int32Rect getIntRect(BatteryStatusEnum status)
        {
            switch (status)
            {
                case BatteryStatusEnum.full:
                    return new Int32Rect(0, 0, 70, 105);
                case BatteryStatusEnum.quarter:
                    return new Int32Rect(83, 0, 70, 105);
                case BatteryStatusEnum.half:
                    return new Int32Rect(170, 0, 70, 105);
                case BatteryStatusEnum.low:
                    return new Int32Rect(253, 0, 70, 105);
                default:
                    return new Int32Rect(338, 0, 70, 105);
            }
        }
    }

    public enum BatteryStatusEnum
    {
        [Description("全满")]
        full = 1 ,
        [Description("四分之一")]
        quarter = 2,
        [Description("过半")]
        half = 3,
        [Description("不足")]
        low = 4,
        [Description("警告")]
        warning = 0
    }
}

