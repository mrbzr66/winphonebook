using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
namespace AminPhoneBook {
 public sealed class ThemeColors {
  public Color Accent,Accent2,Surface,Surface2,Background,Text,Muted,Border,Success,Warning;
  public ThemeColors(Color a,Color a2,Color bg,Color surface,Color surface2,Color text,Color muted,Color border){Accent=a;Accent2=a2;Background=bg;Surface=surface;Surface2=surface2;Text=text;Muted=muted;Border=border;Success=Color.FromArgb(16,185,129);Warning=Color.FromArgb(245,158,11);}
  public static ThemeColors Get(AppSettings s){
   Color a=Color.FromArgb(37,99,235),a2=Color.FromArgb(6,182,212);
   if(s.Theme=="Purple"){a=Color.FromArgb(124,58,237);a2=Color.FromArgb(236,72,153);}
   else if(s.Theme=="Emerald"){a=Color.FromArgb(5,150,105);a2=Color.FromArgb(20,184,166);}
   else if(s.Theme=="Sunset"){a=Color.FromArgb(234,88,12);a2=Color.FromArgb(244,63,94);}
   if(s.DarkMode)return new ThemeColors(a,a2,Color.FromArgb(17,24,39),Color.FromArgb(31,41,55),Color.FromArgb(55,65,81),Color.FromArgb(243,244,246),Color.FromArgb(156,163,175),Color.FromArgb(75,85,99));
   return new ThemeColors(a,a2,Color.FromArgb(245,247,251),Color.White,Color.FromArgb(249,250,252),Color.FromArgb(31,41,55),Color.FromArgb(107,114,128),Color.FromArgb(226,232,240));
  }
 }
 public static class Ui {
  public static void DoubleBuffer(Control c){try{typeof(Control).GetProperty("DoubleBuffered",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).SetValue(c,true,null);}catch{}}
  public static void Round(Control c,int radius=14){c.Paint+=(s,e)=>{using(var p=new GraphicsPath()){var r=new Rectangle(0,0,c.Width-1,c.Height-1);int d=radius*2;p.AddArc(r.X,r.Y,d,d,180,90);p.AddArc(r.Right-d,r.Y,d,d,270,90);p.AddArc(r.Right-d,r.Bottom-d,d,d,0,90);p.AddArc(r.X,r.Bottom-d,d,d,90,90);p.CloseFigure();c.Region=new Region(p);}};}
  public static Label Label(string text,ThemeColors t,int size=10,bool bold=false){return new Label{Text=text,AutoSize=false,ForeColor=t.Text,Font=new Font("Tahoma",size,bold?FontStyle.Bold:FontStyle.Regular)};}
  public static AnimatedButton Button(string text,ThemeColors t,bool filled=true){return new AnimatedButton(text,filled?t.Accent:t.Surface,filled?t.Surface:t.Text,filled?t.Accent2:t.Border);}
  public static void StyleTextBox(TextBox b,ThemeColors t){b.BackColor=t.Surface2;b.ForeColor=t.Text;b.BorderStyle=BorderStyle.FixedSingle;b.Font=new Font("Tahoma",10);b.Margin=new Padding(5);b.Height=34;}
  public static void StyleForm(Form f,ThemeColors t){f.BackColor=t.Background;f.ForeColor=t.Text;f.Font=new Font("Tahoma",9);f.FormBorderStyle=FormBorderStyle.Sizable;f.DoubleBuffered();}
  public static void FadeIn(Form f){f.Opacity=0;var timer=new Timer{Interval=15};timer.Tick+=(s,e)=>{f.Opacity=Math.Min(1,f.Opacity+0.08);if(f.Opacity>=1){timer.Stop();timer.Dispose();}};timer.Start();}
  static void DoubleBuffered(this Form f){try{typeof(Control).GetProperty("DoubleBuffered",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).SetValue(f,true,null);}catch{}}
 }
 public class AnimatedButton:Button {
  Color from,to,baseColor;Timer timer;int step;
  public AnimatedButton(string text,Color bg,Color fg,Color hover){Text=text;BackColor=bg;ForeColor=fg;FlatStyle=FlatStyle.Flat;FlatAppearance.BorderSize=0;Font=new Font("Tahoma",9,FontStyle.Bold);Height=38;Cursor=Cursors.Hand;from=bg;to=hover;baseColor=bg;Ui.Round(this,10);Ui.DoubleBuffer(this);MouseEnter+=(s,e)=>Animate(to);MouseLeave+=(s,e)=>Animate(baseColor);}
  void Animate(Color target){if(timer!=null){timer.Stop();timer.Dispose();}from=BackColor;to=target;step=0;timer=new Timer{Interval=12};timer.Tick+=(s,e)=>{step++;float p=Math.Min(1f,step/8f);BackColor=Blend(from,to,p);if(p>=1){timer.Stop();timer.Dispose();timer=null;}};timer.Start();}
  static Color Blend(Color a,Color b,float p){return Color.FromArgb(a.A+(int)((b.A-a.A)*p),a.R+(int)((b.R-a.R)*p),a.G+(int)((b.G-a.G)*p),a.B+(int)((b.B-a.B)*p));}
 }
 public class GradientPanel:Panel {
  public Color Color1=Color.SteelBlue,Color2=Color.DeepSkyBlue;public int Radius=16;
  public GradientPanel(){Ui.DoubleBuffer(this);Padding=new Padding(18);}
  protected override void OnPaintBackground(PaintEventArgs e){using(var b=new LinearGradientBrush(ClientRectangle,Color1,Color2,0f)){e.Graphics.FillRectangle(b,ClientRectangle);}}
 }
}