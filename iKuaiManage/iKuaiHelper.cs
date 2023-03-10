using AppNerve.Expands.Serializer;
using AppNerve.Http;
using Newtonsoft.Json;
using System; 
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iKuaiManage
{
    class iKuaiHelper
    {
        #region def
        public static string iKuaiSeverIp = "http://192.168.9.254";
        string loginUrl = iKuaiSeverIp + "/Action/login";
        string callUrl = iKuaiSeverIp+"/Action/call";
            
        private HttpClient httpClient { get; set; }
        enum action        
        {
            show,
            edit,
            up,
            down,
            del_conn
        }
        enum funcName
        {
            l2tp_client,
            stream_ipport,
            monitor_lanip
        }
        #endregion
        #region Init
        public iKuaiHelper()
        {
            httpClient = new HttpClient();

        }
        #endregion 
        #region Login
        public bool LoginiKuai(string user,string pass)
        {
            //string loginUrl = iKuaiSeverIp + loginUrle;
            string passMd5 = MD5Encrypt32(pass);
            string passBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("salt_11"+pass));
            var login= httpClient.PostData(loginUrl, "{\"username\":\""+user+"\",\"passwd\":\""+passMd5+"\",\"pass\":\""+ passBase64+"\",\"remember_password\":true}");
            if (login.Html.Contains("10000"))
            {
                SaveCookie(user);
               // MessageBox.Show("login success");
                return true;
            }
            return false;
        }
        #endregion
        #region VPN
        public List<L2TP> GetL2tpList()
        {
            var postData = GetPostData(funcName.l2tp_client, action.show);
            var resultData = httpClient.PostData(callUrl,postData);
            Console.WriteLine(resultData.Html);
            if (resultData.Html.Contains("3000"))
            {
                var change= resultData.Html.Replace("interface", "interf");
                var result = JsonConvert.DeserializeObject<ResultDataL2tp>(change);
                return result.Data.data.ToList();
                //return true;
            }
            return new List<L2TP>();
        }

        private bool ChangeL2tpSeverIP(L2TP l2TP)
        {
            var postData = GetPostData(funcName.l2tp_client, action.edit,l2TP);
            var resultData = httpClient.PostData(callUrl, postData);
            if (resultData.Html.Contains("3000"))
            {
                return true;
            }
            return false;
        }
        public bool ChangeL2tpSeverIP(List<L2TP> l2TPs,bool addOne, string ip="")
        {
            foreach(var l2tp in l2TPs)
            {
                if (addOne)
                {
                    var nowIPs = l2tp.server;
                    var nowIP = nowIPs.Split('.');
                    nowIP[3] = (int.Parse(nowIP[3]) + 1).ToString();
                    l2tp.server = string.Join(".",nowIP);
                }
                else
                {
                    l2tp.server = ip;
                }
                ChangeL2tpSeverIP(l2tp);
            }
            return true;
        }
        private bool EnableL2tp(string id,bool enable)
        {
            var postData = GetPostData(funcName.l2tp_client, enable?action.up:action.down, id);
            var resultData = httpClient.PostData(callUrl, postData);
            if (resultData.Html.Contains("3000"))
            {
                return true;
            }
            return false;
        }
        public bool EnableL2tp(List<L2TP> l2TPs, bool enable)
        {
            foreach (var l2tp in l2TPs)
            {
                EnableL2tp(l2tp.id.ToString(), enable);  
            }
            return true;
        }



        #endregion
        #region StreamIpport
        public List<StreamIpport> GetStreamIpportList()
        {
            var postData = GetPostData(funcName.stream_ipport, action.show);
            var resultData = httpClient.PostData(callUrl, postData);
            if (resultData.Html.Contains("3000"))
            {
                var change = resultData.Html.Replace("interface", "interf");
                var result = JsonConvert.DeserializeObject<ResultDataStreamIpport>(change);
                return result.Data.data.ToList();
                //return true;
            }
            return new List<StreamIpport>();
        }
        private bool EnableStreamIpport(string id, bool enable)
        {
            var postData = GetPostData(funcName.stream_ipport, enable ? action.up : action.down, id);
            var resultData = httpClient.PostData(callUrl, postData);
            if (resultData.Html.Contains("3000"))
            {
                return true;
            }
            return false;
        }
        public bool EnableStreamIpport(List<StreamIpport> StreamIpports, bool enable,bool clear)
        {
            foreach (var streamIpport in StreamIpports)
            {
                EnableStreamIpport(streamIpport.id.ToString(), enable);
            }
            if (clear)
            {
                foreach (var streamIpport in StreamIpports)
                {
                    var ips = streamIpport.src_addr.Split(',');
                    foreach (var ip in ips)
                    {
                        ClearConn(ip);
                    }
                }
            }
            return true;
        }
        private bool ChangeStreamIpportWan(StreamIpport streamIpport)
        {
            var postData = GetPostData(funcName.stream_ipport, action.edit, streamIpport);
            var resultData = httpClient.PostData(callUrl, postData);
            if (resultData.Html.Contains("3000"))
            {
                return true;
            }
            return false;
        }
        public bool ChangeStreamIpportWan(List<StreamIpport> StreamIpports, string wan,bool clear)
        {
            foreach (var streamIpport in StreamIpports)
            {
               streamIpport.interf = wan;
               ChangeStreamIpportWan(streamIpport);
            }
            if(clear)
            {
                foreach (var streamIpport in StreamIpports)
                {
                    var ips = streamIpport.src_addr.Split(',');
                    foreach(var ip in ips)
                    {
                        ClearConn(ip);
                       
                    }
                }
            }
            return true;
        }
        public bool ClearAllConn(List<StreamIpport> StreamIpports)
        {
            foreach (var streamIpport in StreamIpports)
            {
                var ips = streamIpport.src_addr.Split(',');
                foreach (var ip in ips)
                {
                    ClearConn(ip);
                }
            }
            return true;
        }
        #endregion
        #region 终端监控
        private bool ClearConn(string ip)
        {
            Thread.Sleep(200);
            var postData = GetPostData(funcName.monitor_lanip, action.del_conn,ip);
            var resultData = httpClient.PostData(callUrl, postData);
            if (resultData.Html.Contains("30000"))
            {
                return true;
            }
            return false;
        }
        
        #endregion

        #region tool
        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5Encrypt32(string password)
        {
            string cl = password;
            string pwd = "";
            MD5 md5 = MD5.Create(); //实例化一个md5对像
                                    // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                //Console.WriteLine(i + " " + s[i].ToString("x2"));
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("x2");
            }
            return pwd;
        }
        public void SetCookie(byte[] bin)
        {
            using (MemoryStream memoryStream = new MemoryStream(bin))
                this.httpClient.CookieContainer = (CookieContainer)new BinaryFormatter().Deserialize((Stream)memoryStream);
        }

        public byte[] GetCookie()
        {
            return httpClient.CookieContainer.BinSerializeToBytes();
        }
        public void LoadCookie(string user)
        {
            if (!System.IO.File.Exists(Application.StartupPath + "\\cookies\\" +user))
                return;
            try
            {
                SetCookie(System.IO.File.ReadAllBytes(Application.StartupPath + "\\cookies\\" + user));
            }
            catch
            {
            }
        }

        private void SaveCookie(string user)
        {
            if (!Directory.Exists(Application.StartupPath + "\\cookies"))
                Directory.CreateDirectory(Application.StartupPath + "\\cookies");
            try
            {
                System.IO.File.WriteAllBytes(Application.StartupPath + "\\cookies\\" + user, GetCookie());
                
            }
            catch
            {
            }
        }
        private string GetParam(funcName fun, action action,string param)
        {            
            var result = "{\"func_name\":\"" + fun.ToString() + "\",\"action\":\"" + action.ToString() + "\",\"param\":{" + param + "}}";
            return result;
        }


        private string GetPostData(funcName fun,action action,string info="")
        {
            string param = "";
            //if (fun == funcName.l2tp_client)
            {
                switch (action)
                {
                    case action.del_conn:
                        param= "\"ip\":\"" + info + "\"";
                        break;
                    case action.show:
                        var type = "\"TYPE\":\"total,data\",";
                        var other = "\"limit\":\"0,1000\",\"ORDER_BY\":\"\",\"ORDER\":\"\"";
                        param = type + other;
                        break;
                    case action.up:
                    case action.down:
                        param = "\"id\":\""+info+"\"";
                        break;

                }
            }
                             
            
            var result = GetParam(fun, action, param);
            return result;
        }
        
        /// <summary>
        /// Change L2TP server IP
        /// </summary>
        /// <param name="action"></param>
        /// <param name="l2TP"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        private string GetPostData(funcName fun,action action,L2TP l2TP)
        {
            string param = "\"comment\":\"\",\"server\":\""+l2TP.server+"\",\"gateway\":\"\",\"server_port\":1701,\"username\":\""+l2TP.username+"\",\"passwd\":\""+l2TP.passwd+"\",\"ipsec_secret\":\""+l2TP.ipsec_secret+"\",\"interface\":\""+l2TP.interf+"\",\"leftid\":\"\",\"rightid\":\"\",\"mru\":1450,\"timing_rst_switch\":0,\"timing_rst_week\":\"1234567\",\"timing_rst_time\":\"00:00,,\",\"updatetime\":0,\"cycle_rst_time\":0,\"qos_switch\":0,\"dns2\":\"\",\"dns1\":\"\",\"mppe\":\"\",\"ip_addr\":\"\",\"id\":"+l2TP.id+",\"enabled\":\""+l2TP.enabled+"\",\"mtu\":1450,\"name\":\""+l2TP.name+"\",\"week\":\"1234567\",\"mon9\":0,\"date1\":\"00:00\",\"date2\":\"\",\"date3\":\"\"";
            


            var result = GetParam(fun, action, param);
            return result;
        }

        /// <summary>
        /// Change StreamIpport wan
        /// </summary>
        /// <param name="action"></param>
        /// <param name="StreamIpport"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        private string GetPostData(funcName fun, action action, StreamIpport streamIpport)
        {

            string param = "\"comment\":\"\",\"nexthop\":\"\",\"interface\":\""+streamIpport.interf+"\",\"week\":\"1234567\",\"mode\":1,\"src_addr\":\""+streamIpport.src_addr+"\",\"time\":\"00:00-23:59\",\"type\":0,\"protocol\":\"any\",\"dst_addr\":\"\",\"src_port\":\"\",\"dst_port\":\"\",\"iface_band\":1,\"id\":"+streamIpport.id+",\"enabled\":\""+streamIpport.enabled+"\"";

            var result = GetParam(fun, action, param);
            return result;
        }

        #endregion

    }
}
