using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComControl_7_20
{
    class HexStringToByteArray//十六进制转换为字节的类
    {

        public static byte[] HexToByteArray(string s)//   十六进制转换为字节的方法 

        {

            s = s.Replace(" ", "");//将字符串中的空格替换不空格  

            byte[] buffer = new byte[s.Length / 2];

            for (int i = 0; i < s.Length; i += 2)

                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);

            return buffer;

        }

    }
}
