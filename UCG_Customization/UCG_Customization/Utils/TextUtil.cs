using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCG_Customization.Utils
{
    public class TextUtil
    {

        /// <summary>
        /// 字串處理<br/>
        /// 
        /// </summary>
        /// <param name="str">代處理字串</param>
        /// <param name="lenght">長度限制</param>
        /// <param name="isLeft">True:左補空白 ; False:右補空白</param>
        /// <param name="paddingChar">填補字串</param>
        /// <returns></returns>
        public static String GetByLenght(String str, int lenght, bool isLeft, char paddingChar)
        {
            String data = str ?? "";
            data = data.Trim();
            data = data.Substring(0, lenght > data.Length ? data.Length : lenght);
            return isLeft ? data.PadLeft(lenght, paddingChar) : data.PadRight(lenght, paddingChar);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">代處理字串</param>
        /// <param name="lenght">長度限制</param>
        /// <param name="isLeft">True:左補空白 ; False:右補空白</param>
        /// <param name="_encoding">編碼</param>
        /// <returns></returns>
        public static String GetByByte(String str, int lenght, bool isLeft, String _encoding)
        {
            return GetByByte(str, lenght, isLeft, ' ', _encoding);
        }

        /// <summary>
        /// 字串處理<br/>
        /// 
        /// </summary>
        /// <param name="str">代處理字串</param>
        /// <param name="lenght">長度限制</param>
        /// <param name="isLeft">True:左補空白 ; False:右補空白</param>
        /// <param name="paddingChar">填補字串</param>
        /// <param name="_encoding">編碼</param>
        /// <returns></returns>
        public static String GetByByte(String str, int lenght, bool isLeft, char paddingChar, String _encoding)
        {
            Encoding encoding = System.Text.Encoding.GetEncoding(_encoding);
            String data = str ?? "";
            data = data.Trim();
            byte[] bytes = encoding.GetBytes(data.ToString());
            if (bytes.Length > lenght)
            {
                return encoding.GetString(bytes, 0, lenght);
            }
            else
            {
                data = isLeft ? data.PadLeft(lenght, paddingChar) : data.PadRight(lenght, paddingChar);
                bytes = encoding.GetBytes(data.ToString());
                return encoding.GetString(bytes, 0, lenght);
            }
        }
    }
}
