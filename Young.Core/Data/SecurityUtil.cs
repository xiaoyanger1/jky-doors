using System;
using System.Text;
using System.Security.Cryptography;

namespace Young.Core.Data
{
    public class SecurityUtil
    {

        public static string EncodeAES(string text, string key, string iv)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.Zeros;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
                len = keyBytes.Length;
            System.Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = Encoding.UTF8.GetBytes(iv);
            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(text);
            byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);
            return Convert.ToBase64String(cipherBytes);
        }

        /// <summary>AES解密</summary>  
        /// <param name="text">密文</param>  
        /// <param name="key">密钥,长度为的字符串</param>  
        /// <param name="iv">偏移量,长度为的字符串</param>  
        /// <returns>明文</returns>  
        public static string DecodeAES(string text, string key, string iv)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.Zeros;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            byte[] encryptedData = Convert.FromBase64String(text);
            byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
                len = keyBytes.Length;
            System.Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = Encoding.UTF8.GetBytes(iv);
            ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
            byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(plainText).Replace("\0",string.Empty);
        }

        public static string Sign(string content, string signType, string key, string inputCharset)
        {
            if (string.IsNullOrEmpty(signType))
                signType = "MD5";

            if (signType.ToUpper() == "MD5")
            {
                return SignMD5Provider(content, key, inputCharset);
            }

            throw new ArgumentNullException("sign is null");
        }

        public static string SignMD5Provider(string content, string key, string input_charset)
        {
            StringBuilder sb = new StringBuilder(32);
            MD5 md5 = new MD5CryptoServiceProvider();
            string signContent = string.Format("{0}{1}", content, key);
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(input_charset).GetBytes(signContent));
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        private static bool VerifyMD5Provider(string content, string signedString, string key, string inputCharset)
        {
            return SignMD5Provider(content, key, inputCharset).Equals(signedString);
        }


        public static bool Verify(string content, string signedString, string key, string inputCharset, string signType)
        {
            if (signType.ToUpper() == "MD5")
            {
                return VerifyMD5Provider(content, signedString, key, inputCharset);
            }

            return false;
        }
    }
}
