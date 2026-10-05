using System;
using System.Collections.Generic;
namespace AminPhoneBook {
 [Serializable] public class Contact {
  public Guid Id{get;set;}=Guid.NewGuid(); public string FirstName{get;set;}=""; public string LastName{get;set;}="";
  public string Company{get;set;}=""; public string Mobile{get;set;}=""; public string Phone{get;set;}=""; public string Mobile2{get;set;}="";
  public string Email{get;set;}=""; public string Email2{get;set;}=""; public string Address{get;set;}=""; public string Group{get;set;}="";
  public string Tags{get;set;}=""; public string Notes{get;set;}=""; public string PhotoPath{get;set;}=""; public bool Favorite{get;set;}
  public bool Deleted{get;set;} public DateTime? Birthday{get;set;} public DateTime? Anniversary{get;set;} public DateTime? NextReminder{get;set;}
  public string ReminderText{get;set;}=""; public DateTime CreatedAt{get;set;}=DateTime.Now; public DateTime UpdatedAt{get;set;}=DateTime.Now;
  public string FullName{get{return(FirstName+" "+LastName).Trim();}}
 }
 [Serializable] public class AppSettings {
  public bool DarkMode{get;set;} public bool AutoBackup{get;set;}=true; public int AutoBackupDays{get;set;}=7;
  public bool MinimizeToTray{get;set;} public bool ConfirmDelete{get;set;}=true; public string Theme{get;set;}="Ocean";
 }
 [Serializable] public class PhoneBookData { public int Version{get;set;}=3; public List<Contact> Contacts{get;set;}=new List<Contact>(); public List<string> Groups{get;set;}=new List<string>(); public AppSettings Settings{get;set;}=new AppSettings(); }
}