using DevExpress.XtraBars.Ribbon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace iKuaiManage
{
    public partial class Form1 : RibbonForm
    {
        iKuaiHelper iKuaiHelper = new iKuaiHelper();
        List<L2TP> l2tps = new List<L2TP>();
        List<StreamIpport> streamIpports = new List<StreamIpport>();
        string user = "admin";
        string pass = "Sc147258";
        public Form1()
        {
            InitializeComponent();
        }
        
        private void testBtn_Click(object sender, EventArgs e)
        {
           
            //iKuaiHelper.LoginiKuai("admin", "Sc147258");
            //l2tps.Clear();
           // iKuaiHelper.LoadCookie("admin");
           // l2tps.AddRange(iKuaiHelper.GetL2tpList());
           // Thread.Sleep(5000);
            gridView.RefreshData();
            
            // Console.WriteLine(iKuaiHelper.MD5Encrypt32("Sc147258"));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IniConfig config = new IniConfig();
            user = config.IniReadValue("ikuai", "user");
            pass = config.IniReadValue("ikuai", "pass");
            iKuaiHelper.iKuaiSeverIp= config.IniReadValue("ikuai", "server");
            iKuaiHelper.LoginiKuai(user, pass);
            gridControl.DataSource = l2tps;
            gridControl1.DataSource = streamIpports;
            RefreshL2tpList();
            RefreshStreamIpportList();
           
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void 服务器IP1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int[] rows = gridView.GetSelectedRows();
            if (rows.Count() != 0)
            {
                var selectedL2tps = new List<L2TP>();
                foreach (var row in rows)
                {
                    int pos = (int)gridView.GetRowCellValue(row, "id");
                    foreach (var info in l2tps)
                    {
                        if (info.id == pos)
                        {
                            selectedL2tps.Add(info);
                            break;
                        }
                    }

                }
                iKuaiHelper.ChangeL2tpSeverIP(selectedL2tps, true);
            }

            RefreshL2tpList();

        }

        private void bbiRefreshL2tpList_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RefreshL2tpList();
        }
        
        private void RefreshL2tpList()
        {
            l2tps.Clear();
            iKuaiHelper.LoadCookie(user);
            l2tps.AddRange(iKuaiHelper.GetL2tpList());
            gridView.RefreshData();
            UpdateL2tpWan();
        }

        private void UpdateL2tpWan()
        {
            bliWan.Strings.Clear();
            foreach(var l2tp in l2tps)
            {
                if(!string.IsNullOrEmpty(l2tp.ip_addr))
                {
                    if(!bliWan.Strings.Contains(l2tp.name))
                    {
                        bliWan.Strings.Add(l2tp.name);

                    }
                }
            }

        }
        private void 服务器改为指定IPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int[] rows = gridView.GetSelectedRows();
            if (rows.Count() != 0)
            {
                var selectedL2tps = new List<L2TP>();
                foreach (var row in rows)
                {
                    int pos = (int)gridView.GetRowCellValue(row, "id");
                    foreach (var info in l2tps)
                    {
                        if (info.id == pos)
                        {
                            selectedL2tps.Add(info);
                            break;
                        }
                    }

                }
                iKuaiHelper.ChangeL2tpSeverIP(selectedL2tps,false,(string)beiL2tpServerIP.EditValue);
            }

            RefreshL2tpList();
        }

        private void sbLogin_Click(object sender, EventArgs e)
        {
            iKuaiHelper.LoginiKuai(user, pass);
        }

        private void 线路开启ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int[] rows = gridView.GetSelectedRows();
            if (rows.Count() != 0)
            {
                var selectedL2tps = new List<L2TP>();
                foreach (var row in rows)
                {
                    int pos = (int)gridView.GetRowCellValue(row, "id");
                    foreach (var info in l2tps)
                    {
                        if (info.id == pos)
                        {
                            selectedL2tps.Add(info);
                            break;
                        }
                    }

                }
                iKuaiHelper.EnableL2tp(selectedL2tps, true);
            }

            RefreshL2tpList();
        }

        private void 线路关闭ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int[] rows = gridView.GetSelectedRows();
            if (rows.Count() != 0)
            {
                var selectedL2tps = new List<L2TP>();
                foreach (var row in rows)
                {
                    int pos = (int)gridView.GetRowCellValue(row, "id");
                    foreach (var info in l2tps)
                    {
                        if (info.id == pos)
                        {
                            selectedL2tps.Add(info);
                            break;
                        }
                    }

                }
                iKuaiHelper.EnableL2tp(selectedL2tps, false);
            }

            RefreshL2tpList();

        }

        private void bbiUpdateStreamIpportList_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RefreshStreamIpportList();
        }
        private void RefreshStreamIpportList()
        {
            streamIpports.Clear();
            iKuaiHelper.LoadCookie(user);
            streamIpports.AddRange(iKuaiHelper.GetStreamIpportList());
            gridView1.RefreshData();
        }

        private void 启用ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int[] rows = gridView1.GetSelectedRows();
            if (rows.Count() != 0)
            {
                var selectedStreamIpports = new List<StreamIpport>();
                foreach (var row in rows)
                {
                    int pos = (int)gridView1.GetRowCellValue(row, "id");
                    foreach (var info in streamIpports)
                    {
                        if (info.id == pos)
                        {
                            selectedStreamIpports.Add(info);
                            break;
                        }
                    }

                }
                iKuaiHelper.EnableStreamIpport(selectedStreamIpports, true, bciAutoClearConn.Checked);
            }

            RefreshStreamIpportList();
        }

        private void 停用ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int[] rows = gridView1.GetSelectedRows();
            if (rows.Count() != 0)
            {
                var selectedStreamIpports = new List<StreamIpport>();
                foreach (var row in rows)
                {
                    int pos = (int)gridView1.GetRowCellValue(row, "id");
                    foreach (var info in streamIpports)
                    {
                        if (info.id == pos)
                        {
                            selectedStreamIpports.Add(info);
                            break;
                        }
                    }

                }
                iKuaiHelper.EnableStreamIpport(selectedStreamIpports, false,bciAutoClearConn.Checked);
            }

            RefreshStreamIpportList();
        }

        private void 改为指定线路ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int[] rows = gridView1.GetSelectedRows();
            if (rows.Count() != 0)
            {
                var selectedStreamIpports = new List<StreamIpport>();
                foreach (var row in rows)
                {
                    int pos = (int)gridView1.GetRowCellValue(row, "id");
                    foreach (var info in streamIpports)
                    {
                        if (info.id == pos)
                        {
                            selectedStreamIpports.Add(info);
                            break;
                        }
                    }

                }
                iKuaiHelper.ChangeStreamIpportWan(selectedStreamIpports, bliWan.Caption,bciAutoClearConn.Checked);
            }

            RefreshStreamIpportList();
           
        }

        private void bliWan_ListItemClick(object sender, DevExpress.XtraBars.ListItemClickEventArgs e)
        {
            bliWan.Caption = bliWan.Strings[e.Index];
        }

        private void ribbonControl_Click(object sender, EventArgs e)
        {

        }

        private void bbiClearAllConn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            iKuaiHelper.ClearAllConn(streamIpports);
        }
    }
}
