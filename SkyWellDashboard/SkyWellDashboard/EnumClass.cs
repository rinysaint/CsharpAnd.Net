/***************************************************************Copyright (c) 2019 All Rights Reserved.
*公司名称：开沃集团
*命名空间：SkyWellDashboard
*文件名：EnumClass
*版本号：V1.0.0.0
*创建人:肖日宁
*电子邮件：a11010203@qq.com
*联系方式:QQ:164840753
*创建时间：2019/4/16 10:39:26
*描述：EnumClass
*************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SkyWellDashboard
{
    public static partial class EnumClass
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
        public enum ControlModel
        {
            [Description("手动模式")]
            ManualDriving,//手动驾驶模式（传统驾驶模式）
            [Description("自动模式")]
            AutoPilot,//自动驾驶模式
            [Description("遥控模式")]
            RemoteDriving,//遥控驾驶模式
            [Description("远程模式")]
            LongDistanceDriving,//远程驾驶模式
            [Description("Other")]
            Other//保留  
        };
        public enum GearState
        {
            [Description("N")]
            N,//N 档
            [Description("D")]
            D,//D 档
            [Description("R")]
            R,//R 档
            [Description("Other")]
            Other//保留           
        };
    }
}
