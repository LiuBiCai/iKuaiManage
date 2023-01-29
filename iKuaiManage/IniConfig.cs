using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace iKuaiManage
{
    class IniConfig
    {
        public string path = "./ikuai.ini";             //INI文件名 

        //声明写INI文件的API函数
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        //声明读INI文件的API函数
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        
        //写INI文件
        public void IniWriteValue(string Section, string Key, string Value)
        {
            if (!File.Exists(path))
            {
                //File.Create(path);
                FileStream fs = new FileStream(path, FileMode.CreateNew);
                fs.Close();
            }
            WritePrivateProfileString(Section, Key, Value, path);
        }

        //读取INI文件
        public string IniReadValue(string Section, string Key)
        {
            if (!File.Exists(path))
            {
                return "";
            }
            StringBuilder temp = new StringBuilder(4096);
            int i = GetPrivateProfileString(Section, Key, "", temp, 4096, path);
            return temp.ToString();
        }
    }
}
