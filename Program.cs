using System;
using System.Windows.Forms;
namespace AminPhoneBook {
 static class Program {
  [STAThread] static void Main() {
   Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false);
   Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
   Application.ThreadException += (s,e)=>MessageBox.Show("خطای غیرمنتظره:\r\n"+e.Exception.Message,"دفترچه تلفن",MessageBoxButtons.OK,MessageBoxIcon.Error);
   using(var f=new PasswordForm()){if(f.ShowDialog()!=DialogResult.OK)return;Application.Run(new MainForm(f.Password));}
  }
}