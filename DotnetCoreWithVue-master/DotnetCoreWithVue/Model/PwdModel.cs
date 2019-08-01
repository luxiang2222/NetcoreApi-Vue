using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DotnetCoreWithVue.Model
{
  public static class PwdModel
  {
    static string keyEncrypt = "bocoboco";
    static byte[] Key = { 0x10, 0x30, 0x50, 0x73, 0x97, 0xAB, 0xCD, 0xEF }; 
    public static string GetKeyPwd(string pwbyte)
    {
      try
      {
        byte[] rgbKey = Encoding.UTF8.GetBytes(keyEncrypt);
        byte[] rgIV = Key;
        byte[] inputByteArry = Convert.FromBase64String(pwbyte);
        DESCryptoServiceProvider Dsc = new DESCryptoServiceProvider();
        MemoryStream MS = new MemoryStream();
        CryptoStream CS = new CryptoStream(MS, Dsc.CreateDecryptor(rgbKey, Key), CryptoStreamMode.Write);
        CS.Write(inputByteArry, 0, inputByteArry.Length);
        CS.FlushFinalBlock();
        return Encoding.UTF8.GetString(MS.ToArray());
      }
      catch(Exception ex)
      {
        throw new Exception("error for Key password"+ex.Message);
      }
    }
  }
}
