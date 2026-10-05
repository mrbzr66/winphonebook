using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace AminPhoneBook {
 public class MainForm:Form {
  CryptoStore store;PhoneBookData data;DataGridView grid;TextBox search;Label status,countLabel,favLabel,groupLabel;Panel content,sidebar;ThemeColors theme;
  Timer reminderTimer;
  public MainForm(string password){store=new CryptoStore(password);try{data=store.Load();}catch(Exception ex){MessageBox.Show("باز کردن اطلاعات ممکن نشد:\r\n"+ex.Message);Environment.Exit(1);return;}Text="دفترچه تلفن حرفه‌ای";Width=1320;Height=800;MinimumSize=new Size(1050,650);StartPosition=FormStartPosition.CenterScreen;RightToLeft=RightToLeft.Yes;RightToLeftLayout=true;theme=ThemeColors.Get(data.Settings);Ui.StyleForm(this,theme);Build();reminderTimer=new Timer{Interval=60000};reminderTimer.Tick+=(s,e)=>CheckReminders();reminderTimer.Start();CheckReminders();Shown+=(s,e)=>Ui.FadeIn(this);}
  void Build(){
   var menu=new MenuStrip{Dock=DockStyle.Top,BackColor=theme.Surface,ForeColor=theme.Text,Height=28,RenderMode=ToolStripRenderMode.System};BuildMenu(menu);Controls.Add(menu);
   sidebar=new Panel{Dock=DockStyle.Right,Width=245,BackColor=theme.Surface,Padding=new Padding(18,18,18,12)};BuildSidebar();Controls.Add(sidebar);
   content=new Panel{Dock=DockStyle.Fill,BackColor=theme.Background,Padding=new Padding(22,18,22,18)};BuildContent();Controls.Add(content);
   RefreshGrid();
  }
  void BuildMenu(MenuStrip menu){var file=new ToolStripMenuItem("فایل");file.DropDownItems.Add("پشتیبان‌گیری",null,(s,e)=>Backup());file.DropDownItems.Add("بازیابی",null,(s,e)=>Restore());file.DropDownItems.Add("خروجی CSV",null,(s,e)=>ExportCsv());file.DropDownItems.Add("ورود CSV",null,(s,e)=>ImportCsv());file.DropDownItems.Add("خروج",null,(s,e)=>Close());var tools=new ToolStripMenuItem("ابزار");tools.DropDownItems.Add("مخاطبین تکراری",null,(s,e)=>Duplicates());tools.DropDownItems.Add("سطل زباله",null,(s,e)=>Trash());tools.DropDownItems.Add("یادآوری‌ها",null,(s,e)=>new ReminderForm(data).ShowDialog(this));tools.DropDownItems.Add("تنظیمات",null,(s,e)=>Settings());menu.Items.Add(file);menu.Items.Add(tools);}
  void BuildSidebar(){
   var title=new Label{Text="AMIN\nPHONEBOOK",Dock=DockStyle.Top,Height=70,ForeColor=theme.Text,Font=new Font("Tahoma",15,FontStyle.Bold),TextAlign=ContentAlignment.MiddleCenter};
   sidebar.Controls.Add(title);
   var sub=new Label{Text="دفترچه مخاطبین مدرن",Dock=DockStyle.Top,Height=30,ForeColor=theme.Muted,TextAlign=ContentAlignment.MiddleCenter};sidebar.Controls.Add(sub);
   var add=Ui.Button("＋  مخاطب جدید",theme,true);add.Dock=DockStyle.Top;add.Margin=new Padding(0,14,0,8);add.Click+=(s,e)=>Edit(null);sidebar.Controls.Add(add);
   var all=SideButton("▣  همه مخاطبین");all.Click+=(s,e)=>{search.Clear();RefreshGrid();};sidebar.Controls.Add(all);
   var fav=SideButton("★  علاقه‌مندی‌ها");fav.Click+=(s,e)=>{search.Text="__FAVORITES__";RefreshGrid();search.SelectAll();};sidebar.Controls.Add(fav);
   var groups=SideButton("◈  گروه‌ها");groups.Click+=(s,e)=>GroupSummary();sidebar.Controls.Add(groups);
   var trash=SideButton("⌫  سطل زباله");trash.Click+=(s,e)=>Trash();sidebar.Controls.Add(trash);
   var sep=new Panel{Dock=DockStyle.Top,Height=18};sidebar.Controls.Add(sep);
   var settings=SideButton("⚙  تنظیمات");settings.Click+=(s,e)=>Settings();sidebar.Controls.Add(settings);
   var footer=new Label{Text="امن • سریع • فارسی",Dock=DockStyle.Bottom,Height=30,ForeColor=theme.Muted,TextAlign=ContentAlignment.MiddleCenter};sidebar.Controls.Add(footer);
  }
  AnimatedButton SideButton(string text){var b=Ui.Button(text,theme,false);b.Dock=DockStyle.Top;b.Height=42;b.Margin=new Padding(0,2,0,2);return b;}
  void BuildContent(){
   var header=new Panel{Dock=DockStyle.Top,Height=62};content.Controls.Add(header);
   var title=new Label{Text="مخاطبین",Dock=DockStyle.Top,Height=32,ForeColor=theme.Text,Font=new Font("Tahoma",18,FontStyle.Bold)};header.Controls.Add(title);
   var hint=new Label{Text="همه ارتباطاتت، مرتب و همیشه در دسترس",Dock=DockStyle.Bottom,Height=25,ForeColor=theme.Muted};header.Controls.Add(hint);
   var searchPanel=new Panel{Dock=DockStyle.Top,Height=52,Padding=new Padding(0,6,0,8)};content.Controls.Add(searchPanel);
   search=new TextBox{Dock=DockStyle.Fill,RightToLeft=RightToLeft.Yes};Ui.StyleTextBox(search,theme);searchPanel.Controls.Add(search);var sl=new Label{Text="⌕",Dock=DockStyle.Right,Width=45,TextAlign=ContentAlignment.MiddleCenter,Font=new Font("Tahoma",18),ForeColor=theme.Accent};searchPanel.Controls.Add(sl);search.TextChanged+=(s,e)=>RefreshGrid();
   var cards=new TableLayoutPanel{Dock=DockStyle.Top,Height=92,ColumnCount=3,RowCount=1};for(int i=0;i<3;i++)cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,33.333f));content.Controls.Add(cards);
   cards.Controls.Add(Card("مخاطبین فعال","0",theme.Accent,out countLabel),0,0);cards.Controls.Add(Card("علاقه‌مندی‌ها","0",theme.Accent2,out favLabel),1,0);cards.Controls.Add(Card("گروه‌ها","0",theme.Success,out groupLabel),2,0);
   var gridPanel=new Panel{Dock=DockStyle.Fill,Padding=new Padding(0,14,0,0)};content.Controls.Add(gridPanel);
   grid=new DataGridView{Dock=DockStyle.Fill,ReadOnly=true,AllowUserToAddRows=false,AutoGenerateColumns=false,SelectionMode=DataGridViewSelectionMode.FullRowSelect,MultiSelect=false,BackgroundColor=theme.Surface,BorderStyle=BorderStyle.None,RowHeadersVisible=false,GridColor=theme.Border,EnableHeadersVisualStyles=false,ColumnHeadersHeight=42,RowTemplate={Height=42},Font=new Font("Tahoma",9),RightToLeft=RightToLeft.Yes};
   grid.ColumnHeadersDefaultCellStyle.BackColor=theme.Accent;grid.ColumnHeadersDefaultCellStyle.ForeColor=Color.White;grid.ColumnHeadersDefaultCellStyle.Font=new Font("Tahoma",9,FontStyle.Bold);grid.DefaultCellStyle.BackColor=theme.Surface;grid.DefaultCellStyle.ForeColor=theme.Text;grid.DefaultCellStyle.SelectionBackColor=Color.FromArgb(Math.Min(255,theme.Accent.R+35),Math.Min(255,theme.Accent.G+35),Math.Min(255,theme.Accent.B+35));grid.DefaultCellStyle.SelectionForeColor=Color.White;grid.AlternatingRowsDefaultCellStyle.BackColor=theme.Surface2;
   Col("نام","FullName",210);Col("موبایل","Mobile",145);Col("تلفن","Phone",125);Col("شرکت","Company",160);Col("گروه","Group",125);Col("ایمیل","Email",210);Col("★","Favorite",45);
   grid.DoubleClick+=(s,e)=>Edit(Selected());grid.KeyDown+=(s,e)=>{if(e.KeyCode==Keys.Delete)Delete();if(e.KeyCode==Keys.Enter){Edit(Selected());e.SuppressKeyPress=true;}};grid.CellFormatting+=(s,e)=>{if(e.RowIndex>=0&&grid.Columns[e.ColumnIndex].DataPropertyName=="Favorite")e.Value=(e.Value is bool&&(bool)e.Value)?"★":"";};
   gridPanel.Controls.Add(grid);
   status=new Label{Dock=DockStyle.Bottom,Height=25,TextAlign=ContentAlignment.MiddleRight,ForeColor=theme.Muted};content.Controls.Add(status);
  }
  Control Card(string title,string value,ThemeColors t,out Label valueLabel){var p=new GradientPanel{Dock=DockStyle.Fill,Margin=new Padding(4),Color1=Color.FromArgb(Math.Min(255,t.Accent.R+35),Math.Min(255,t.Accent.G+35),Math.Min(255,t.Accent.B+35)),Color2=t.Accent2};Ui.Round(p,14);var l=new Label{Text=title,Dock=DockStyle.Top,Height=26,ForeColor=Color.FromArgb(220,255,255,255),Font=new Font("Tahoma",9)};valueLabel=new Label{Text=value,Dock=DockStyle.Fill,ForeColor=Color.White,Font=new Font("Tahoma",22,FontStyle.Bold),TextAlign=ContentAlignment.MiddleLeft};p.Controls.Add(valueLabel);p.Controls.Add(l);return p;}
  void Col(string h,string p,int w){grid.Columns.Add(new DataGridViewTextBoxColumn{HeaderText=h,DataPropertyName=p,Width=w,SortMode=DataGridViewColumnSortMode.NotSortable});}
  Contact Selected(){return grid.CurrentRow==null?null:grid.CurrentRow.DataBoundItem as Contact;}
  void RefreshGrid(){string q=search==null?"":search.Text.Trim();IEnumerable<Contact> a=data.Contacts.Where(c=>!c.Deleted);if(q=="__FAVORITES__")a=a.Where(c=>c.Favorite);else if(!string.IsNullOrEmpty(q))a=a.Where(c=>SearchIn(c,q));var list=a.OrderByDescending(c=>c.Favorite).ThenBy(c=>c.LastName).ThenBy(c=>c.FirstName).ToList();grid.DataSource=null;grid.DataSource=list;if(countLabel!=null)countLabel.Text=list.Count.ToString();if(favLabel!=null)favLabel.Text=data.Contacts.Count(x=>!x.Deleted&&x.Favorite).ToString();if(groupLabel!=null)groupLabel.Text=data.Contacts.Where(x=>!x.Deleted&&!string.IsNullOrWhiteSpace(x.Group)).Select(x=>x.Group).Distinct().Count().ToString();if(status!=null)status.Text="نمایش "+list.Count+" مخاطب    •    "+DateTime.Now.ToString("yyyy/MM/dd");}
  bool SearchIn(Contact c,string q){string s=string.Join(" ",new[]{c.FirstName,c.LastName,c.Company,c.Mobile,c.Phone,c.Mobile2,c.Email,c.Email2,c.Address,c.Group,c.Tags,c.Notes});return s.IndexOf(q,StringComparison.CurrentCultureIgnoreCase)>=0;}
  void Edit(Contact c){using(var f=new ContactForm(c,data.Settings)){if(f.ShowDialog(this)==DialogResult.OK){if(c==null)data.Contacts.Add(f.Result);store.Save(data);RefreshGrid();}}}
  void Delete(){var c=Selected();if(c==null)return;if(data.Settings.ConfirmDelete&&MessageBox.Show("این مخاطب به سطل زباله منتقل شود؟","حذف مخاطب",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)return;c.Deleted=true;c.UpdatedAt=DateTime.Now;store.Save(data);RefreshGrid();}
  void ToggleFav(){var c=Selected();if(c==null)return;c.Favorite=!c.Favorite;store.Save(data);RefreshGrid();}
  void Trash(){var x=data.Contacts.Where(c=>c.Deleted).OrderByDescending(c=>c.UpdatedAt).ToList();if(x.Count==0){MessageBox.Show("سطل زباله خالی است.");return;}var f=new Form{Text="سطل زباله",Width=720,Height=470,StartPosition=FormStartPosition.CenterParent,RightToLeft=RightToLeft.Yes,RightToLeftLayout=true};Ui.StyleForm(f,theme);var l=new ListBox{Dock=DockStyle.Fill,Font=new Font("Tahoma",10),BackColor=theme.Surface,ForeColor=theme.Text,BorderStyle=BorderStyle.None};foreach(var c in x)l.Items.Add(c.FullName+"  •  "+c.Mobile);var restore=Ui.Button("↩  بازیابی",theme,true);restore.Dock=DockStyle.Bottom;restore.Height=44;f.Controls.Add(l);f.Controls.Add(restore);restore.Click+=(s,e)=>{if(l.SelectedIndex>=0){x[l.SelectedIndex].Deleted=false;store.Save(data);f.Close();RefreshGrid();}};Ui.FadeIn(f);f.ShowDialog(this);}
  void Duplicates(){var groups=data.Contacts.Where(c=>!c.Deleted&&!string.IsNullOrWhiteSpace(c.Mobile)).GroupBy(c=>Normalize(c.Mobile)).Where(g=>g.Count()>1).ToList();if(groups.Count==0){MessageBox.Show("مورد تکراری پیدا نشد.");return;}MessageBox.Show(string.Join("\r\n",groups.Select(g=>"• "+string.Join("، ",g.Select(c=>c.FullName)))),"مخاطبین احتمالی تکراری");}
  void GroupSummary(){var g=data.Contacts.Where(c=>!c.Deleted&&!string.IsNullOrWhiteSpace(c.Group)).GroupBy(c=>c.Group).OrderByDescending(x=>x.Count());MessageBox.Show(g.Any()?string.Join("\r\n",g.Select(x=>"• "+x.Key+"  ("+x.Count()+")")):"هنوز گروهی ثبت نشده است.","گروه‌ها");}
  string Normalize(string s){return new string((s??"").Where(char.IsDigit).ToArray()).TrimStart('0');}
  void Backup(){try{if(!store.Exists){MessageBox.Show("هنوز داده‌ای برای پشتیبان‌گیری وجود ندارد.");return;}MessageBox.Show("پشتیبان ساخته شد:\r\n"+store.MakeBackup());}catch(Exception ex){MessageBox.Show(ex.Message);}}
  void Restore(){using(var d=new OpenFileDialog{Filter="Encrypted Backup (*.apb)|*.apb"})if(d.ShowDialog()==DialogResult.OK){try{store.Restore(d.FileName);data=store.Load();theme=ThemeColors.Get(data.Settings);Rebuild();MessageBox.Show("بازیابی با موفقیت انجام شد.");}catch(Exception ex){MessageBox.Show("بازیابی ناموفق:\r\n"+ex.Message);}}}
  void ExportCsv(){using(var d=new SaveFileDialog{Filter="CSV UTF-8 (*.csv)|*.csv",FileName="Contacts.csv"})if(d.ShowDialog()==DialogResult.OK){var sb=new StringBuilder();sb.AppendLine("FirstName,LastName,Company,Mobile,Phone,Mobile2,Email,Email2,Address,Group,Tags,Notes,Favorite");foreach(var c in data.Contacts.Where(x=>!x.Deleted))sb.AppendLine(string.Join(",",new[]{c.FirstName,c.LastName,c.Company,c.Mobile,c.Phone,c.Mobile2,c.Email,c.Email2,c.Address,c.Group,c.Tags,c.Notes,c.Favorite.ToString()}.Select(E)));File.WriteAllText(d.FileName,sb.ToString(),new UTF8Encoding(true));MessageBox.Show("خروجی با موفقیت ساخته شد.");}}
  void ImportCsv(){using(var d=new OpenFileDialog{Filter="CSV (*.csv)|*.csv"})if(d.ShowDialog()==DialogResult.OK){var lines=File.ReadAllLines(d.FileName,Encoding.UTF8);int n=0;foreach(var line in lines.Skip(1)){var a=ParseCsv(line);if(a.Count<5)continue;data.Contacts.Add(new Contact{FirstName=a[0],LastName=a[1],Company=a[2],Mobile=a[3],Phone=a[4],Email=a.Count>5?a[5]:""});n++;}store.Save(data);RefreshGrid();MessageBox.Show(n+" مخاطب وارد شد.");}}
  List<string> ParseCsv(string s){var r=new List<string>();var b=new StringBuilder();bool q=false;foreach(char c in s){if(c=='"'){q=!q;continue;}if(c==','&&!q){r.Add(b.ToString());b.Clear();}else b.Append(c);}r.Add(b.ToString());return r;}
  string E(string s){return """+(s??"").Replace(""","""")+""";}
  void Settings(){using(var f=new SettingsForm(data.Settings)){if(f.ShowDialog(this)==DialogResult.OK){store.Save(data);theme=ThemeColors.Get(data.Settings);Rebuild();}}}
  void Rebuild(){foreach(Control c in Controls.OfType<Control>().ToList())if(!(c is MenuStrip))Controls.Remove(c);Build();BackColor=theme.Background;Invalidate();}
  void CheckReminders(){var now=DateTime.Now;foreach(var c in data.Contacts.Where(x=>!x.Deleted&&x.NextReminder.HasValue&&x.NextReminder.Value<=now&&x.NextReminder.Value>now.AddMinutes(-2)))MessageBox.Show("یادآوری برای "+c.FullName+"\r\n"+c.ReminderText,"یادآوری",MessageBoxButtons.OK,MessageBoxIcon.Information);}
 }
}