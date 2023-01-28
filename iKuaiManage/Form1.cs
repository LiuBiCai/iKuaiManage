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
        List<L2TP> l2tps = new List<L2TP>();
        iKuaiHelper iKuaiHelper = new iKuaiHelper();
        public Form1()
        {
            InitializeComponent();
        }
        
        private void testBtn_Click(object sender, EventArgs e)
        {
           
            //iKuaiHelper.LoginiKuai("admin2", "Sc147258");
            //l2tps.Clear();
           // iKuaiHelper.LoadCookie("admin2");
           // l2tps.AddRange(iKuaiHelper.GetL2tpList());
           // Thread.Sleep(5000);
            gridView.RefreshData();

            // Console.WriteLine(iKuaiHelper.MD5Encrypt32("Sc147258"));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gridControl.DataSource =l2tps;
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
            iKuaiHelper.LoadCookie("admin2");
            l2tps.AddRange(iKuaiHelper.GetL2tpList());
            gridView.RefreshData();
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
            iKuaiHelper.LoginiKuai("admin2", "Sc147258");
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
    }
}
