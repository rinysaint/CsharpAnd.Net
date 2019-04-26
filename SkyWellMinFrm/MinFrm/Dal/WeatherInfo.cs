using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace MinFrm.Dal
{
    public static class WeatherInfo
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
        public static Int32Rect getWeatherRect(WeatherInfoEnum Info)
        {
            switch (Info) {
                case WeatherInfoEnum.qing:
                    return new Int32Rect(0, 0, 50, 50);
                case WeatherInfoEnum.yin:
                    return new Int32Rect(95, 0, 50, 50);
                case WeatherInfoEnum.duoyun:
                    return new Int32Rect(180, 0, 50, 50);
                case WeatherInfoEnum.xiaoyu:
                    return new Int32Rect(275, 0, 50, 50);
                case WeatherInfoEnum.dayu:
                    return new Int32Rect(365, 0, 50, 50);
                case WeatherInfoEnum.baoyu:
                    return new Int32Rect(455, 0, 50, 50);
                case WeatherInfoEnum.zhenyu:
                    return new Int32Rect(0, 0, 50, 50);
                case WeatherInfoEnum.leizhenyu:
                    return new Int32Rect(83, 0, 50, 50);
                case WeatherInfoEnum.yujiaxue:
                    return new Int32Rect(0, 0, 50, 50);
                case WeatherInfoEnum.xiaoxue:
                    return new Int32Rect(83, 0, 50, 50);
                case WeatherInfoEnum.zhongxue:
                    return new Int32Rect(0, 0, 50, 50);
                case WeatherInfoEnum.daxue:
                    return new Int32Rect(83, 0, 50, 50);
                case WeatherInfoEnum.baoxue:
                    return new Int32Rect(83, 0, 50, 50);
                case WeatherInfoEnum.wu:
                    return new Int32Rect(0, 0, 50, 50);
                case WeatherInfoEnum.bingpao:
                    return new Int32Rect(83, 0, 50, 50);
                case WeatherInfoEnum.zhongyu:
                    return new Int32Rect(0, 0, 50, 50);
                case WeatherInfoEnum.wumai:
                    return new Int32Rect(83, 0, 50, 50);
                case WeatherInfoEnum.shachenbao:
                    return new Int32Rect(0, 0, 50, 50);
                default:
                    return new Int32Rect(0, 0, 0, 0);
            } 


    }
}

public enum WeatherInfoEnum
{
    [Description("晴天")]
    qing = 1,
    [Description("阴天")]
    yin = 2,
    [Description("多云")]
    duoyun = 3,
    [Description("小雨")]
    xiaoyu = 4,
    [Description("大雨")]
    dayu = 5,
    [Description("暴雨")]
    baoyu = 6,
    [Description("阵雨")]
    zhenyu = 7,
    [Description("雷阵雨")]
    leizhenyu = 8,
    [Description("雨夹雪")]
    yujiaxue = 9,
    [Description("小雪")]
    xiaoxue = 10,
    [Description("中雪")]
    zhongxue = 11,
    [Description("大雪")]
    daxue = 12,
    [Description("暴雪")]
    baoxue = 13,
    [Description("雾")]
    wu = 14,
    [Description("冰雹")]
    bingpao = 15,
    [Description("中雨")]
    zhongyu = 16,
    [Description("雾霾")]
    wumai = 17,
    [Description("沙尘暴")]
    shachenbao = 18
    }
}
