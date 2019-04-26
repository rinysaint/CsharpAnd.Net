/************************************
*公司名称：开沃集团
*命名空间：MinFrm
*文件名：Form_MotorInfo.cs
*版本号：V1.0.0.0
*创建人:肖日宁
*电子邮件：a11010203@qq.com
*联系方式:QQ:164840753
*创建时间：2019年3月8日
*描述：电机信息窗体文件
*************************************/
using System;
using System.Windows.Forms;

namespace MinFrm
{
    public partial class Form_MotorInfo : Form
    {
        public Form_MotorInfo()
        {
            InitializeComponent();
        }

        private void button_confirmAndclose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
