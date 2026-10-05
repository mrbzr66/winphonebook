using System;
using System.IO;
using System.Windows.Forms;
namespace AminPhoneBook {
 public class PasswordForm:Form {
  TextBox pass,confirm;public string Password{get;private set;}
  public PasswordForm(){Text="دفترچه تلفن - ورود";Width=470;Height=260;StartPosition=FormStartPosition.CenterScreen;FormBorderStyle=FormBorderStyle.FixedDialog;MaximizeBox=false;MinimizeBox=false;RightToLeft=RightToLeft.Yes;RightToLeftLayout=true;Controls.Add(new Label{Text="رمز عبور:",Left=35,Top=30,Width=100});pass=new TextBox{Left=35,Top=58,Width=370,UseSystemPasswordChar=true};confirm=new TextBox{Left=35,Top=105,Width=370,UseSystemPasswordChar=true};Controls.Add(pass);Controls.Add(new Label{Text="تکرار رمز (فقط اولین اجرا):",Left=35,Top=82,Width=200});Controls.Add(confirm);var ok=new Button{Text="ورود / ایجاد",Left=240,Top=165,Width=165,DialogResult=DialogResult.OK};var no=new Button{Text="انصراف",Left=35,Top=165,Width=120,DialogResult=DialogResult.Cancel};Controls.AddRange(new Control[]{ok,no});AcceptButton=ok;CancelButton=no;ok.Click+=ValidateInput;}
  void ValidateInput(object s,EventArgs e){if(pass.Text.Length<8){Warn("رمز باید حداقل ۸ کاراکتر باشد.");return;}string f=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"AminPhoneBook","contacts.apb");if(!File.Exists(f)&&pass.Text!=confirm.Text){Warn("تکرار رمز صحیح نیست.");return;}Password=pass.Text;}void Warn(string m){MessageBox.Show(m,"امنیت",MessageBoxButtons.OK,MessageBoxIcon.Warning);DialogResult=DialogResult.None;}
 }
}