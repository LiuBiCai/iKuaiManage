using AppNerve.Expands.Serializer;
using AppNerve.Http;
using iKuaiManage.JSON;
using Newtonsoft.Json;
using System; 
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
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
        }
        enum funcName
        {
            l2tp_client,
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
                return true;
            }
            return false;
        }
        #endregion
        #region VPN
        public bool GetL2tpList()
        {
            var postData = GetPostData(funcName.l2tp_client, action.show);
            var resultData = httpClient.PostData(callUrl,postData);
            if (resultData.Html.Contains("3000"))
            {
                var change= resultData.Html.Replace("interface", "interf");
                var result = JsonConvert.DeserializeObject<ResultData>(change);

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
        private string GetPostData(funcName fun,action action)
        {
            string type = "";
            string other = "";
            if(fun==funcName.l2tp_client&&action==action.show)
            {
                type = "\"TYPE\":\"total,data\",";
                other = "\"limit\":\"0,100\",\"ORDER_BY\":\"\",\"ORDER\":\"\"}";
            }

            var result = "{\"func_name\":\""+fun.ToString()+"\",\"action\":\""+action.ToString()+"\",\"param\":{"+type+other+"}";
            return result;
        }
        #endregion

    }
}
