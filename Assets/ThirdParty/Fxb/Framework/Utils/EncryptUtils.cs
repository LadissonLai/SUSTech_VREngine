namespace Framework.Utils
{
	using System;
	using System.Security.Cryptography;
    using System.Text;

    public static class EncryptUtils
    {
        public static string GenMD5Str(string str, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;

            return GenMD5Str(encoding.GetBytes(str));
        }

        /// <summary>
        /// Gens the bytes MD.
        /// </summary>
        /// <returns>The bytes MD.</returns>
        /// <param name="bytes">Bytes.</param>
        public static string GenMD5Str(byte[] bytes)
        {
            var md5 = new MD5CryptoServiceProvider();

            var result = md5.ComputeHash(bytes);
			 
            var sb = new StringBuilder();

			for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("X2"));
            }
 
			return sb.ToString();
        }
 
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="text">加密字符</param>
        /// <param name="password">加密的密码</param>
        /// <returns></returns>
        public static string AESEncrypt(string text, string password)
		{
            try
            {
                RijndaelManaged rijndaelCipher = new RijndaelManaged
                {
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7,
                    KeySize = 128,
                    BlockSize = 128
                };

                byte[] pwdBytes = Encoding.UTF8.GetBytes(password);

                byte[] keyBytes = new byte[16];

                int len = pwdBytes.Length;

                if (len > keyBytes.Length)
                    len = keyBytes.Length;

                Array.Copy(pwdBytes, keyBytes, len);

                rijndaelCipher.Key = keyBytes;

                ICryptoTransform transform = rijndaelCipher.CreateEncryptor();

                byte[] plainText = Encoding.UTF8.GetBytes(text);

                byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);

                return Convert.ToBase64String(cipherBytes);
            }
            catch (Exception)
            {
                return "";
            }
		}

		/// <summary>
		/// AES解密
		/// </summary>
		/// <param name="text"></param>
		/// <param name="password"></param>
		/// <param name="iv"></param>
		/// <returns></returns>
		public static string AESDecrypt(string text, string password)
		{
            try
            {
                RijndaelManaged rijndaelCipher = new RijndaelManaged
                {
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7,
                    KeySize = 128,
                    BlockSize = 128
                };

                var encryptedData = Convert.FromBase64String(text);

                var pwdBytes = Encoding.UTF8.GetBytes(password);

                var keyBytes = new byte[16];

                int len = pwdBytes.Length;

                if (len > keyBytes.Length) len = keyBytes.Length;

                Array.Copy(pwdBytes, keyBytes, len);

                rijndaelCipher.Key = keyBytes;

                var transform = rijndaelCipher.CreateDecryptor();

                var plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

                return Encoding.UTF8.GetString(plainText);
            }
            catch (Exception)
            {
                return "";
            }
		}
	}
}

