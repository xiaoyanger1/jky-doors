using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Young.Core.Common;

namespace Young.Core.Data
{
    /// <summary>
    /// 加密安全类
    /// </summary>
    public class Security
    {

        #region (public) MD5加密(大写) _MD5
        /// <summary>
        /// MD5加密(大写)
        /// </summary>
        /// <param name="strValue">输入值</param>
        public static string _MD5(string strValue)
        {
            MD5 md5Hasher = MD5.Create();  

            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes( strValue ));

            StringBuilder sBuilder = new StringBuilder();

            for (int nIndex = 0; nIndex < data.Length; ++nIndex)
            {
                sBuilder.Append(data[nIndex].ToString("X"));
            }

            return sBuilder.ToString();
        }
        #endregion

        #region (public) MD5加密(小写) _MD5Small
        /// <summary>
        /// MD5加密(小写)
        /// </summary>
        /// <param name="strValue">输入值</param>
        public static string _MD5Small ( string strValue )
        {
            MD5 md5Hasher = MD5.Create();

            byte[] data = md5Hasher.ComputeHash( Encoding.Default.GetBytes( strValue ) );

            StringBuilder sBuilder = new StringBuilder();

            for ( int nIndex = 0; nIndex < data.Length; ++nIndex )
            {
                sBuilder.Append( data[nIndex].ToString( "x2" ) );
            }

            return sBuilder.ToString();
        }
        #endregion

        #region (private) DES加密方式初始化

        private static DES mydes = new DESCryptoServiceProvider();
        private static readonly string Key = "0dcbHSD837FHWFH3(&^EFHF38ioyNKHNw3827hJH&thfedhui((((%$gasgJJH564543+=";//该字符串请勿修改
        private static readonly string IV = "728#$$%^348hGU7h447hJHUfgeggsdh#$@#sgdhs7fs*(^%$^&&(*&()$##@%%$RHGJJHHJ";//该字符串请勿修改

        /// <summary>
        /// 获得密钥
        /// </summary>
        /// <returns>密钥</returns>
        private static byte[] GetLegalKey()
        {
            string sTemp = Key;
            mydes.GenerateKey();
            byte[] bytTemp = mydes.Key;
            int KeyLength = bytTemp.Length;
            if ( sTemp.Length > KeyLength )
                sTemp = sTemp.Substring( 0, KeyLength );
            else if ( sTemp.Length < KeyLength )
                sTemp = sTemp.PadRight( KeyLength, ' ' );
            return ASCIIEncoding.ASCII.GetBytes( sTemp );
        }

        /// <summary>
        /// 获得初始向量IV
        /// </summary>
        /// <returns>初试向量IV</returns>
        private static byte[] GetLegalIV()
        {
            string sTemp = IV;
            mydes.GenerateIV();
            byte[] bytTemp = mydes.IV;
            int IVLength = bytTemp.Length;
            if ( sTemp.Length > IVLength )
                sTemp = sTemp.Substring( 0, IVLength );
            else if ( sTemp.Length < IVLength )
                sTemp = sTemp.PadRight( IVLength, ' ' );
            return ASCIIEncoding.ASCII.GetBytes( sTemp );
        } 
        #endregion

        #region (public) DES对称加密 _DESEncrypt

        /// <summary>
        /// DES对称加密
        /// </summary>
        /// <param name="Source">待加密的串</param>
        public static string _DESEncrypt( string Source )
        {
            try
            {
                byte[] bytIn = UTF8Encoding.UTF8.GetBytes( Source );
                MemoryStream ms = new MemoryStream();
                mydes.Key = GetLegalKey();
                mydes.IV = GetLegalIV();
                ICryptoTransform encrypto = mydes.CreateEncryptor();
                CryptoStream cs = new CryptoStream( ms, encrypto, CryptoStreamMode.Write );
                cs.Write( bytIn, 0, bytIn.Length );
                cs.FlushFinalBlock();
                ms.Close();
                byte[] bytOut = ms.ToArray();
                return Convert.ToBase64String( bytOut );
            } 
            catch ( Exception ex )
            {
                throw new Exception( "在文件加密的时候出现错误！错误提示：  " + ex.Message );
            }
        }

        #endregion

        #region (public) DES对称解密 _DESDecrypt
        /// <summary>
        /// DES对称解密
        /// </summary>
        /// <param name="Source">待解密的串</param>
        public static string _DESDecrypt( string Source )
        {
            try
            {
                byte[] bytIn = Convert.FromBase64String( Source );
                MemoryStream ms = new MemoryStream( bytIn, 0, bytIn.Length );
                mydes.Key = GetLegalKey();
                mydes.IV = GetLegalIV();
                ICryptoTransform encrypto = mydes.CreateDecryptor();
                CryptoStream cs = new CryptoStream( ms, encrypto, CryptoStreamMode.Read );
                StreamReader sr = new StreamReader( cs );
                return sr.ReadToEnd();
            }
            catch( Exception ex )
            {
                throw new Exception("在文件解密的时候出现错误！错误提示：  " + ex.Message);
            }
        } 
        #endregion

        /// <summary>
        ///   des加密串    uri转码
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string _DESEncryptEscape(string source)
        {
            return _DESEncrypt(source).Replace("+","_").Replace("/","-");
        }



        /// <summary>
        ///    des解密    uri转码 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string _DESDecryptEscape(string source)
        {
            return _DESDecrypt(source.Replace("_", "+").Replace("-", "/"));
        }
    }
}
