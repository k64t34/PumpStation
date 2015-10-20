/*
 * Created by SharpDevelop.
 * User: skorik
 * Date: 14.08.2015
 * Time: 10:16
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms; 
using System.Data.OleDb;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Timers;

namespace pump
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		const string _BR = "<BR>";
		const string _SP = "&nbsp;";
		const string _SP3 = "&nbsp;&nbsp;&nbsp;";
		
		System.Data.OleDb.OleDbConnection Conn;
		string DBFile="sw.mdb";
		string DBPath="";
		string ProviderDB="Microsoft.Jet.OLEDB.4.0";
		int cmdTimeCounter=0;
		string stridsw;
		Thread thread;
		System.Timers.Timer timer1 = new System.Timers.Timer();
		
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		
		}				
		void MainFormLoad(object sender, EventArgs e)
		{
			int button_Width=Convert.ToInt16(panel1.Width/3) - 3;
			button1.Left=0;
			button1.Top=0;
			button1.Height=panel1.Height;
			button1.Width=button_Width;
			button2.Left=button1.Width+1;
			button2.Top=0;
			button2.Height=panel1.Height;
			button2.Width=button_Width;
			button3.Left=button2.Left+button2.Width+1;
			button3.Top=0;
			button3.Height=panel1.Height;
			button3.Width=button_Width;
			thread = new Thread(Work);
			thread.IsBackground=true;
			thread.Start();						
		}
		
		void pClose()
		{	timer1.Stop();
			WriteLog(D_T()  +  t_color("white"," Выключение насоса")+_BR);
			WriteLog("</body></html>");
			/*try 
			{
				
				BeginInvoke(new MethodInvoker(delegate
					{
					File.WriteAllText("file2.html",webBrowser1.DocumentText);
					}));

			}
			catch (Exception ex)
			{
				WriteLog(D_T()  +  t_color("red"," ERR Ошибка записи журнала. {0}",ex.Message)+_BR);
			}
			finally
			{
			
			}*/
			
			
		}
		string D_T(){return DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");}
		string t_color(string color,string format,params string[] text){return String.Format("<font color=\"{0}\">{1}</font>",color,String.Format(format,text));}
		string t_color(string color,string format){return t_color( color, format,"");}
	
		
		void Button1Click(object sender, EventArgs e)
		{
			
			CheckBox Button = sender as CheckBox;
			if (Button.Name=="button3") 
			{
				
				Close();
			}
			foreach(CheckBox c in panel1.Controls)
			{
				if (c.Name!=Button.Name)
					c.Checked=false;
			}
			
		}
		int ExecCmd(String WorkingDirectory, String FileName,String Arguments)
		{
		int ExitCode=-1;
		Debug.WriteLine(FileName);	
		//https://msdn.microsoft.com/ru-ru/library/system.diagnostics.processstartinfo(v=vs.110).aspx
		ProcessStartInfo ProcessInfo;
        Process Process; 
        ProcessInfo = new ProcessStartInfo(Environment.ExpandEnvironmentVariables(FileName),Environment.ExpandEnvironmentVariables(Arguments));
        ProcessInfo.WorkingDirectory=WorkingDirectory;
        //ProcessInfo.Arguments=Arguments;
        //ProcessInfo.CreateNoWindow = true;
        //ProcessInfo.UseShellExecute = true;
        Debug.WriteLine(String.Format("{0} {1} {2}",ProcessInfo.WorkingDirectory,ProcessInfo.FileName,ProcessInfo.Arguments));
        try
        {
       	
       	Process = Process.Start(ProcessInfo);
       	
        while (!Process.WaitForExit(1000)){;}// Waits here for the process to exit.        
        Debug.WriteLine(Process.ExitCode);
        ExitCode=Process.ExitCode;
        }
        catch (Exception ex)
	    {	
        	WriteLog(D_T()  +  t_color("red"," ERR Ошибка запуска программы установки. {0}",ex.Message)+_BR);

	    }
	    finally
	    {
	    
	    }        
		return (ExitCode);
	
		}	
	
		
		void WriteLog(string message)
		{
		    if (webBrowser1.InvokeRequired)
		    {
		        webBrowser1.BeginInvoke(new Action<string>((s)=> webBrowser1.Document.Write(s)), message);
		    }
		    else
		    {
		        webBrowser1.Document.Write(message);
		    }
			
		}
		void Work()
		{
			string userNameWin, compName, compIP, myHost; 
			try 
			{
			// имя хоста
			myHost = System.Net.Dns.GetHostName();
			// IP по имени хоста, выдает список, можно обойти в цикле весь, здесь берется первый адрес
			compIP = System.Net.Dns.GetHostEntry(myHost).AddressList[1].ToString();			 
			userNameWin = System.Environment.UserName;
			compName = System.Environment.MachineName;
			WriteLog(String.Format("<!--{0} {1} {2} {3} -->",myHost,compIP,userNameWin,compName));
			}
			finally
			{
				
			}
			WriteLog("<html><meta charset=\"utf-8\"><style>body{background-color: black;color:grey;font-family: monospace;font-size:16;padding:0}</style><body text=\"grey\" bgcolor=\"black\">");
			//-*  В оригинале цвет шрифта 192,192,192,
			WriteLog(D_T() + t_color("white"," Включение насоса") +_BR);
			WriteLog(_SP3+"Параметры задания: ");
			string[] args = Environment.CommandLine.Split(new[] { ' ' },StringSplitOptions.RemoveEmptyEntries);
			if (args.Length==0)
			{
				WriteLog(_BR+D_T()  +  t_color("YELLOW"," WARN Нет параметров задания")+_BR);
				pClose();
				return;
			}
			string idList="";
			int testInt;
			for (int i=0;i!=args.Length;i++)			
			{
				
				if (int.TryParse(args[i],out testInt))
				{
					WriteLog(args[i] + " " );
					idList+=args[i]+",";
				}
				else
				{
					WriteLog(D_T()  +  t_color("yellow",_BR+" Параметр {0} неверный. В обработке не используется",args[i]));
				}
			}
			WriteLog("<br>");	
			
			if (idList=="")
			{
				WriteLog(_BR+D_T()  +  t_color("red"," ERR Нет верных параметров для задания")+_BR);
				pClose();
				return;
			}
			
			WriteLog(D_T()  +  t_color("white"," Подключение к базе данных")+_BR);
			//---Open DB 
			DBFile="sw.mdb";
			DBPath=Path.GetDirectoryName(Application.ExecutablePath);
			if (!File.Exists(DBPath+"\\"+DBFile))
				{
					DBPath=@"\\deploy2\db$\";
					if	(!File.Exists(DBPath+"\\"+DBFile)) 				
					{
						int di=2; // Первое появлиние \ в строке пути. Начальное значение для локального пути типа c:\users\ - 2-ая позиция начиная с нуля.
						if (DBPath.IndexOf('\\')==0) // Если сетевой путь, то Индекс первого появления  \ нужно расщитать
						{					
						di=DBPath.IndexOf('\\',3);
						}
						int i=DBPath.LastIndexOf('\\');	
						while (i>di)
						{
							DBPath=DBPath.Substring(0,i);
								if (File.Exists(DBPath+"\\"+DBFile)) break;
							 i=DBPath.LastIndexOf('\\');		
						}
					}
				}			
			
			if (!File.Exists(DBPath+"\\"+DBFile))
			{
				WriteLog(D_T()  +  t_color("RED"," ERR: Файл базы данных {0} не найден",DBPath+"\\"+DBFile)+_BR);
				pClose();
				return;
			}
			Conn = new System.Data.OleDb.OleDbConnection();			
			Conn.ConnectionString = @"Provider="+ProviderDB+";Data Source="+DBPath+"\\"+DBFile+";Mode=Read;";
			OleDbCommand cmd = new OleDbCommand();		        
		    cmd.CommandText="select * from DialogInstall where id in ("+idList+")";		    
		    cmd.Connection=Conn;
		    OleDbDataReader  rs ;
		    try
		    {
		    	Conn.Open();
		    	rs=cmd.ExecuteReader(CommandBehavior.CloseConnection);
		    	if (rs.HasRows)
		    		WriteLog(_SP3+"Подключение к БД прошло успешно"+_BR);		    				    		
		    	else
		    	{
		    		WriteLog(D_T()  + " "+ t_color("red","ERR Ошибка получения данных из БД"+_BR));
		    		pClose();
		    		
		    	}
		    	WriteLog(D_T()  + "  "+ t_color("white","Начало установки")+_BR);
		    	int tcnt=1;		    	
		    	
		    	timer1.Interval=1000;
		    	timer1.AutoReset=true;
		    	timer1.Elapsed += new ElapsedEventHandler(Timer1Tick);
		    		
				while(rs.Read())
				{	
					while (!button2.Checked)Thread.Sleep(1000);
						
					WriteLog(D_T()  + "  "+ t_color("white","{0}.{1}",tcnt.ToString(),rs["name"].ToString())+_BR);
					WriteLog(_SP3+rs["FileName"]+_BR);
					if (rs["Argument"].ToString().Trim()!="")
						WriteLog(_SP3+rs["Argument"]+_BR);
					stridsw="p"+rs["id"].ToString();
					WriteLog(_SP3+String.Format("Время установки <span id=\"{0}\">0</span> сек",stridsw)+_BR);
		
					cmdTimeCounter=0;					
					timer1.Start();
					if ((bool)rs["waitness"])
					{
					button1.BeginInvoke(new Action<bool>
						((s)=> button1.Checked=s), true);
					button2.BeginInvoke(new Action<bool>
						((s)=> button2.Checked=s), false);
					}
					int ExitCode=ExecCmd("", (string)rs["FileName"], /*(string)*/rs["Argument"].ToString());
					if (ExitCode==0)
					{
						WriteLog(_SP3+t_color("green","Установка прошла успешно")+_BR);
					}
					else
					{
						WriteLog(D_T()  + " "+ t_color("red","ERR Ошибка установки. Код возврата "+ExitCode.ToString()+_BR));
					}
					timer1.Stop();
					tcnt++;
				}	
				button1.BeginInvoke(new Action<bool>
					((s)=> button1.Enabled=s), false);
				button1.BeginInvoke(new Action<bool>
					((s)=> button1.Checked=s), false);
				button2.BeginInvoke(new Action<bool>
					((s)=> button2.Enabled=s), false);
				button2.BeginInvoke(new Action<bool>
					((s)=> button2.Checked=s), false);
				rs.Close();				
			}
	        catch (Exception ex)
		    {		        
	        	WriteLog(D_T()  +  t_color("red"," ERR ошибка открытия БД {0}",ex.Message)+_BR);		        
		    }
		    finally
		    {
		    	
		    }			
			
			pClose();
		}
		
		void Timer1Tick(object source, ElapsedEventArgs e)
		{
			cmdTimeCounter++;
			webBrowser1.BeginInvoke(new Action<string>((s)=> webBrowser1.Document.GetElementById(stridsw).InnerText=s), cmdTimeCounter.ToString());
			//webBrowser1.Document.GetElementById("p16").InnerText=cmdTimeCounter.ToString();
		}
		
		void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{
			File.WriteAllText(@"\\deploy2\log$\ps_"+DateTime.Now.ToString("ddMMyyyy-HHmmss")+Environment.OSVersion.ToString()+"-"+Environment.MachineName+"-"+Environment.UserName+".html"
				                  ,webBrowser1.DocumentText);
		}
	}
}
