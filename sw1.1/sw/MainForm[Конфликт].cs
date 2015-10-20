/*
 * Created by SharpDevelop.
 * User: Администратор
 * Date: 08.08.2015
 * Time: 18:46
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
using System.Data.SqlClient;
using System.ComponentModel;
using System.Diagnostics;

namespace sw
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public System.Data.OleDb.OleDbConnection Conn;
		public int selectCnt =0;
		public String FileDB="sw.mdb";
		public String ProviderDB="Microsoft.Jet.OLEDB.4.0";
		public String PathDB;
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
		
		void Button2Click(object sender, EventArgs e)
		{
			Close();
		}
		
		void MainFormLoad(object sender, EventArgs e)
		{
			PathDB=Path.GetDirectoryName(Application.ExecutablePath);			
			Conn = new System.Data.OleDb.OleDbConnection();			
			//Conn.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=c:\Documents and Settings\Администратор\Мои документы\SharpDevelop Projects\sw\Database1.accdb";
			//Conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=c:\Documents and Settings\Администратор\Мои документы\SharpDevelop Projects\sw\Database1.mdb";
			//Conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=c:\Users\Andrew\Google Диск\DEVELOPMENT\sw\sw.mdb";
			//Conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=c:\Users\skorik\Google Диск\DEVELOPMENT\sw\sw.mdb";
			Conn.ConnectionString = @"Provider="+ProviderDB+";Data Source="+PathDB+"\\"+FileDB;
			
		    try
		    {
		        Conn.Open();
		        OleDbDataAdapter adapter;
		        String strSql;
		        strSql="select * from DialogList";
                adapter = new OleDbDataAdapter(strSql,Conn);
                adapter.Fill(dataSet1);
                dataGridView1.DataSource = dataSet1.Tables[0];	
                dataGridView1.Columns[0].Visible=false;
//                for (int i = 0; i!=dataGridView1.Rows.Count; i++)
//					{                	
//					if (dataGridView1.Rows[i].Cells[1].Value==null)
//						{						
//						dataGridView1.Rows[i].Cells[1].Value=imageList1.Images[0];
//						
//						}
//				
//					}
			this.dataGridView1.CurrentCellChanged += new System.EventHandler(this.DataGridView1CurrentCellChanged);
		    }
		        catch (Exception ex)
		    {
		        MessageBox.Show("Ошибка подключения к базе данных\n"+ex.Message,"Установка ПО",MessageBoxButtons.OK,MessageBoxIcon.Error);		        
		        Close();
		    }
		    finally
		    {
		        Conn.Close();
		    }
		}
		
		void DataGridView1CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			
			dataGridView1.CurrentRow.Cells[0].Value=!Convert.ToBoolean(dataGridView1.CurrentRow.Cells[0].Value);
			selectCntChanged();
			
		}
		
		void Button1Click(object sender, EventArgs e)
		{
//			if (selectCnt==0) Close();
			dataGridView1.Enabled=false;	
			String idList="";
//			foreach (DataGridViewRow row in dataGridView1.Rows)        {
//				{
//					if (Convert.ToBoolean(row.Cells[0].Value))
//						idList=idList+Convert.ToString(row.Cells[3].Value)+",";
//				}

			
			idList=Convert.ToString(dataGridView1.CurrentRow.Cells[3].Value);
//			for (int i = 0; i!=dataGridView1.Rows.Count; i++)
//			{
//				if (Convert.ToBoolean(dataGridView1.Rows[i].Cells[0].Value))
//				{
//					idList=idList+Convert.ToString(dataGridView1.Rows[i].Cells[3].Value)+",";	
//					dataGridView1.Rows[i].Cells[0].Value=false;
//				}
//				else					
//					dataGridView1.Rows.RemoveAt(i--);
//			}
			//Debug.WriteLine(idList);			
			try
		    {
		        Conn.Open();
		        OleDbCommand cmd = new OleDbCommand();
		        cmd.CommandText="select id,name,cmd from software where id in ("+idList+")";
		        cmd.CommandText="select * from DialogInstall where id in ("+idList+")";
		        cmd.CommandType=CommandType.Text;
		        cmd.Connection=Conn;
		        OleDbDataReader  rs ;
		        	//= cmd.ExecuteReader();
				
				rs=cmd.ExecuteReader(CommandBehavior.CloseConnection);				
				
				while(rs.Read())
				{	
//					for (int i = 0; i!=dataGridView1.Rows.Count; i++)
//					{
//						if (Convert.ToInt16(rs["id"])==Convert.ToInt16(dataGridView1.Rows[i].Cells[3].Value))
//							
//							dataGridView1.CurrentCell=dataGridView1.Rows[i].Cells[0];
//						    dataGridView1.Rows[i].Selected = true;
//					}					
//					//ExecCmd(Convert.ToString(rs["cmd"]));
					ExecCmd("", (string)rs["FileName"], (string)rs["Argument"]);
					//dataGridView1.CurrentRow.Cells[0].Value=true;					
					//MessageBox.Show(Convert.ToString(rs["name"]));
				}		        
				rs.Close();	    
				
			}
	        catch (Exception ex)
		    {		        
		        MessageBox.Show("Ошибка подключения к базе данных\n"+ex.Message,"Установка ПО",MessageBoxButtons.OK,MessageBoxIcon.Error);		        
		    }
		    finally
		    {
		        Conn.Close();
		    }
			
			
			//Process.Start("cmd","/C copy c:\\file.txt lpt1");
			//MessageBox.Show(idList);
			//Close();
			dataGridView1.Enabled=true;
        }
	 	//public bool ExecCmd(String WorkingDirectory,String FileName, String Arguments)
 		public bool ExecCmd(String WorkingDirectory, String FileName,String Arguments)
		{
		Debug.WriteLine(FileName);	
		//https://msdn.microsoft.com/ru-ru/library/system.diagnostics.processstartinfo(v=vs.110).aspx
		ProcessStartInfo ProcessInfo;
        Process Process; 
        ProcessInfo = new ProcessStartInfo(Environment.ExpandEnvironmentVariables(FileName),Environment.ExpandEnvironmentVariables(Arguments));
        
        //ProcessInfo = new ProcessStartInfo("cmd.exe", "/C \"" + cmd+"\"");
        //ProcessInfo = new ProcessStartInfo();        
        //ProcessInfo.FileName=Environment.ExpandEnvironmentVariables(FileName);
        ProcessInfo.WorkingDirectory=WorkingDirectory;
        //ProcessInfo.Arguments=Arguments;
        //ProcessInfo.CreateNoWindow = true;
        //ProcessInfo.UseShellExecute = true;
        Debug.WriteLine(String.Format("{0} {1} {2}",ProcessInfo.WorkingDirectory,ProcessInfo.FileName,ProcessInfo.Arguments));
        try
        {
       	this.Hide();
       	Process = Process.Start(ProcessInfo);
        while (!Process.WaitForExit(1000));// Waits here for the process to exit.
        Debug.WriteLine(Process.ExitCode);
        this.Show();
        }        
        catch (Exception ex)
	    {		        
	        MessageBox.Show("Ошибка запуска программы установки\n"+ProcessInfo.FileName+"\n"+ex.Message,"Установка ПО",MessageBoxButtons.OK,MessageBoxIcon.Error);		        
	    }
	    finally
	    {
	    	
	    }
        
		
		
		/*
		7z_cmd="a"			
		Dim p As New ProcessStartInfo
		p.FileName = path_7z	
		'p.Arguments = _7z_cmd+" -r -mx9 -ms=off -mtc=on """+target_path+target_file+""" """+source_path+"*"""	
		'p.Arguments = _7z_cmd+" -r -mx9  """+TargetFile+""" """+SourcePath+"*"""	
		p.Arguments = _7z_cmd+" -r -mx9 -i@"+incFile+" -x@"+ExcFile+" """+TargetPath+TargetFile+"_("+Format(Now,TargetFileSuffix) +").7z"" "
		p.WindowStyle = ProcessWindowStyle.Normal
		Dim myProcess As Process = Nothing
		Try
		Console.WriteLine(p.FileName+" "+p.Arguments)
		myProcess =Process.Start(p)	
		Catch e As FormatException
		End Try
		do
		Loop While Not myProcess.WaitForExit(1000)
		Console.WriteLine("Process exit code: {0}", myProcess.ExitCode)
		fso.deletefile(incFile)
		fso.deletefile(ExcFile)
		If myProcess.ExitCode=0 Then

		
		Dim p As New ProcessStartInfo
		p.FileName = Prog	
		p.Arguments = Args
		p.WindowStyle = ProcessWindowStyle.Normal
		Dim myProcess As Process = Nothing
		Try
			myProcess =Process.Start(p)	
		Catch e As Exception
			If Not Log Is Nothing Then
				Log.Items.Add(e.Message)
			End If	
		End Try

		
		
		*/
		return (true);
		}
		
	 	void selectCntChanged()
	 	{
	 		if (Convert.ToBoolean(dataGridView1.CurrentRow.Cells[0].Value)) 
				selectCnt++;
			else
				selectCnt--;
			if (selectCnt==0)
				button1.Enabled=false;
			else
				button1.Enabled=true;
			toolStripStatusLabel1.Text="Выбрано "+Convert.ToString(selectCnt);	
	 	}
			
			
		
		
		
		
		
		
		void DataGridView1CurrentCellChanged(object sender, EventArgs e)
		{
			//https://msdn.microsoft.com/ru-ru/library/system.windows.forms.datagridview.currentcellchanged.aspx?cs-save-lang=1&cs-lang=csharp#code-snippet-2
			//Debug.WriteLine(String.Format("DataGridView1RowEnter {0} ",dataGridView1.CurrentCell.RowIndex));
			Debug.WriteLine(String.Format("CellChanged rows.cnt={0}",dataGridView1.Rows.Count));
			Debug.WriteLine(dataGridView1.RowCount);
			try
			{
			Debug.WriteLine(dataGridView1.CurrentRow.Cells[2].Value);
			//Debug.WriteLine(dataGridView1.CurrentRow.Cells[3].Value);		       	
			panel1.Refresh();
	        }        
	        catch (Exception ex)
		    {		        
		        Debug.WriteLine(ex.Message);		        
		    }
		    finally
		    {		    	
		    }
		}
		
		
		void Panel1Paint(object sender, PaintEventArgs e)
		{
		try
		    {
		        Conn.Open();
		        OleDbCommand cmd = new OleDbCommand();
		        cmd.CommandText="select * from DialogSoftProperty where id="+ Convert.ToString(dataGridView1.CurrentRow.Cells[3].Value);
		        cmd.CommandType=CommandType.Text;
		        cmd.Connection=Conn;
		        OleDbDataReader  rs ;
				rs=cmd.ExecuteReader(CommandBehavior.CloseConnection);
				this.lProgName.Text=Convert.ToString(rs["cmd"]);
				this.lProgPath.Text="";
				rs.Close();	    
				
			}
	        catch (Exception ex)
		    {		        
	        	Debug.WriteLine(ex.Message);		        
		    }
		    finally
		    {
		        Conn.Close();
		    }	
		}
	}
}
