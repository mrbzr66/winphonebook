using System;
using System.Drawing;
using System.Windows.Forms;
namespace AminPhoneBook {
 public class SettingsForm:Form {
  AppSettings settings;
  ComboBox themeCombo;
  public SettingsForm(AppSettings s){
   settings=s;
   var theme=ThemeColors.Get(s);
   Text="تنظیمات ظاهر و برنامه";Width=620;Height=470;MinimumSize=new Size(560,430);StartPosition=FormStartPosition.CenterParent;RightToLeft=RightToLeft.Yes;RightToLeftLayout=true;Ui.StyleForm(this,theme);
   var root=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=2,RowCount=8,Padding=new Padding(24,20,24,18)};
   root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,58));root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,42));
   root.RowStyles.Add(new RowStyle(SizeType.Absolute,44));root.RowStyles.Add(new RowStyle(SizeType.Absolute,105));root.RowStyles.Add(new RowStyle(SizeType.Absolute,45));root.RowStyles.Add(new RowStyle(SizeType.Absolute,45));root.RowStyles.Add(new RowStyle(SizeType.Absolute,45));root.RowStyles.Add(new RowStyle(SizeType.Absolute,45));root.RowStyles.Add(new RowStyle(SizeType.Absolute,12));root.RowStyles.Add(new RowStyle(SizeType.Absolute,48));
   var heading=new Label{Text="ظاهر و تنظیمات",Dock=DockStyle.Fill,ForeColor=theme.Text,Font=new Font("Tahoma",15,FontStyle.Bold),TextAlign=ContentAlignment.MiddleRight};root.Controls.Add(heading,0,0);root.SetColumnSpan(heading,2);
   var themes=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=4,RowCount=1,Margin=new Padding(0,4,0,4)};for(int i=0;i<4;i++)themes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25));
   themeCombo=new ComboBox{Visible=false,DropDownStyle=ComboBoxStyle.DropDownList};themeCombo.Items.AddRange(new object[]{"Ocean","Purple","Emerald","Sunset"});themeCombo.SelectedItem=s.Theme??"Ocean";
   AddTheme(themes,"Ocean","آبی مدرن",ThemeColors.Get(new AppSettings{Theme="Ocean",DarkMode=s.DarkMode}),0);
   AddTheme(themes,"Purple","بنفش جذاب",ThemeColors.Get(new AppSettings{Theme="Purple",DarkMode=s.DarkMode}),1);
   AddTheme(themes,"Emerald","سبز آرامش‌بخش",ThemeColors.Get(new AppSettings{Theme="Emerald",DarkMode=s.DarkMode}),2);
   AddTheme(themes,"Sunset","نارنجی گرم",ThemeColors.Get(new AppSettings{Theme="Sunset",DarkMode=s.DarkMode}),3);
   root.Controls.Add(themes,0,1);root.SetColumnSpan(themes,2);
   var dark=new CheckBox{Text="حالت تاریک",Checked=s.DarkMode,AutoSize=true,ForeColor=theme.Text,Font=new Font("Tahoma",10,FontStyle.Bold)};
   var auto=new CheckBox{Text="پشتیبان‌گیری خودکار",Checked=s.AutoBackup,AutoSize=true,ForeColor=theme.Text};
   var days=new NumericUpDown{Minimum=1,Maximum=90,Value=s.AutoBackupDays,Width=120};
   var conf=new CheckBox{Text="تأیید قبل از حذف",Checked=s.ConfirmDelete,AutoSize=true,ForeColor=theme.Text};
   root.Controls.Add(dark,0,2);root.SetColumnSpan(dark,2);
   root.Controls.Add(auto,0,3);root.SetColumnSpan(auto,2);
   var daysLabel=new Label{Text="دوره پشتیبان‌گیری (روز)",Dock=DockStyle.Fill,TextAlign=ContentAlignment.MiddleRight,ForeColor=theme.Muted};root.Controls.Add(daysLabel,0,4);root.Controls.Add(days,1,4);
   root.Controls.Add(conf,0,5);root.SetColumnSpan(conf,2);
   var save=Ui.Button("✓  ذخیره تنظیمات",theme,true);root.Controls.Add(save,0,7);root.SetColumnSpan(save,2);
   Controls.Add(root);
   save.Click+=(x,e)=>{s.DarkMode=dark.Checked;s.AutoBackup=auto.Checked;s.AutoBackupDays=(int)days.Value;s.ConfirmDelete=conf.Checked;s.Theme=themeCombo.SelectedItem==null?"Ocean":themeCombo.SelectedItem.ToString();DialogResult=DialogResult.OK;Close();};
   Shown+=(s1,e)=>Ui.FadeIn(this);
  }
  void AddTheme(TableLayoutPanel host,string key,string title,ThemeColors t,int column){
   var b=Ui.Button(title+"\r\n"+key,t,true);b.Dock=DockStyle.Fill;b.Height=88;b.Margin=new Padding(5);
   b.FlatAppearance.BorderSize=0;
   b.Click+=(s,e)=>{themeCombo.SelectedItem=key;};
   host.Controls.Add(b,column,0);
  }
 }
}