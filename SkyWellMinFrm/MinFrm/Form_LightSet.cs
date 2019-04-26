/************************************
*公司名称：开沃集团
*命名空间：MinFrm
*文件名：Form_LightSet.cs
*版本号：V1.0.0.0
*创建人:肖日宁
*电子邮件：a11010203@qq.com
*联系方式:QQ:164840753
*创建时间：2019年3月8日
*描述：灯光设置窗体文件
*************************************/
using System;
using System.Windows.Forms;

namespace MinFrm
{
    public partial class Form_LightSet : Form
    {
        public Form_LightSet()
        {
            InitializeComponent();
        }

        private void button_confirmAndclose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
