using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
namespace AminPhoneBook {
 public sealed class CryptoStore {
  readonly string dir,file,backupDir; readonly byte[] key;
  public CryptoStore(string password){dir=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"AminPhoneBook");backupDir=Path.Combine(dir,"Backups");Directory.CreateDirectory(dir);Directory.CreateDirectory(backupDir);file=Path.Combine(dir,"contacts.apb");key=DeriveKey(password);}
  byte[] DeriveKey(string p){byte[] salt=Encoding.UTF8.GetBytes("AminPhoneBook-2026-Salt-v2");using(var r=new Rfc2898DeriveBytes(p,salt,200000))return r.GetBytes(32);}
  public bool Exists{get{return File.Exists(file);}}
  public PhoneBookData Load(){if(!Exists)return new PhoneBookData();byte[] a=File.ReadAllBytes(file);if(a.Length<49)throw new CryptographicException("فایل داده معتبر نیست.");byte[] iv=new byte[16],mac=new byte[32];Buffer.BlockCopy(a,0,iv,0,16);Buffer.BlockCopy(a,16,mac,0,32);byte[] cipher=new byte[a.Length-48];Buffer.BlockCopy(a,48,cipher,0,cipher.Length);byte[] expected;using(var h=new HMACSHA256(key))expected=h.ComputeHash(Combine(iv,cipher));if(!Eq(mac,expected))throw new CryptographicException("رمز عبور اشتباه است یا فایل دستکاری شده.");byte[] plain;using(var aes=new AesManaged{Key=key,IV=iv,Mode=CipherMode.CBC,Padding=PaddingMode.PKCS7})using(var d=aes.CreateDecryptor())plain=d.TransformFinalBlock(cipher,0,cipher.Length);var x=new XmlSerializer(typeof(PhoneBookData));using(var ms=new MemoryStream(plain))return(PhoneBookData)x.Deserialize(ms);}
  public void Save(PhoneBookData data){byte[] plain;var x=new XmlSerializer(typeof(PhoneBookData));using(var ms=new MemoryStream()){x.Serialize(ms,data);plain=ms.ToArray();}byte[] iv=new byte[16];using(var r=RandomNumberGenerator.Create())r.GetBytes(iv);byte[] cipher;using(var aes=new AesManaged{Key=key,IV=iv,Mode=CipherMode.CBC,Padding=PaddingMode.PKCS7})using(var e=aes.CreateEncryptor())cipher=e.TransformFinalBlock(plain,0,plain.Length);byte[] mac;using(var h=new HMACSHA256(key))mac=h.ComputeHash(Combine(iv,cipher));byte[] outp=Combine(iv,mac,cipher);string tmp=file+".tmp";File.WriteAllBytes(tmp,outp);if(File.Exists(file))File.Replace(tmp,file,null);else File.Move(tmp,file);}
  public string MakeBackup(){string p=Path.Combine(backupDir,"Backup-"+DateTime.Now.ToString("yyyyMMdd-HHmmss")+".apb");File.Copy(file,p,true);return p;} public void Restore(string p){File.Copy(p,file,true);}
  static byte[] Combine(params byte[][] aa){int n=0;foreach(var a in aa)n+=a.Length;var r=new byte[n];int p=0;foreach(var a in aa){Buffer.BlockCopy(a,0,r,p,a.Length);p+=a.Length;}return r;} static bool Eq(byte[] a,byte[] b){if(a.Length!=b.Length)return false;int d=0;for(int i=0;i<a.Length;i++)d|=a[i]^b[i];return d==0;}
 }
}