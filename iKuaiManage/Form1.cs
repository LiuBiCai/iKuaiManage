using DevExpress.XtraBars.Ribbon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace iKuaiManage
{
    public partial class Form1 : RibbonForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void testBtn_Click(object sender, EventArgs e)
        {
            iKuaiHelper iKuaiHelper = new iKuaiHelper();
            iKuaiHelper.LoadCookie("admin2");
            iKuaiHelper.GetL2tpList();
            //iKuaiHelper.LoginiKuai("admin2", "Sc147258");
           // Console.WriteLine(iKuaiHelper.MD5Encrypt32("Sc147258"));
        }
    }
}
