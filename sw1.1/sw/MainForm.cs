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
using System.Text‎;


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
		
		void Button2Click(object sender, EventArgs e){Close();}
		
		void MainFormLoad(object sender, EventArgs e)
		{
			//http://stackoverflow.com/questions/356543/can-i-automatically-increment-the-file-build-version-when-using-visual-studio
			/*System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
 			System.Reflection.AssemblyName assemblyName = assembly.GetName();
 			Version version = assemblyName.Version; 			 			
 			System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();*/
 			
 			this.Text=this.Text+ ": "+System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
			
			
			PathDB=Path.GetDirectoryName(Application.ExecutablePath);			
			Conn = new System.Data.OleDb.OleDbConnection();			
			//Conn.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=c:\Documents and Settings\Администратор\Мои документы\SharpDevelop Projects\sw\Database1.accdb";
			//Conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=c:\Documents and Settings\Администратор\Мои документы\SharpDevelop Projects\sw\Database1.mdb";
			//Conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=c:\Users\Andrew\Google Диск\DEVELOPMENT\sw\sw.mdb";
			//Conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=c:\Users\skorik\Google Диск\DEVELOPMENT\sw\sw.mdb";
			//Jet OLEDB:Database Password=MyDbPassword;
			//Conn.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+PathDB+"\\"+FileDB+";Mode=Share Deny None;Persist Security Info=False;";
			//*Conn.ConnectionString = @"Provider="+ProviderDB+";Data Source="+PathDB+"\\"+FileDB+";Mode=Read|Share Deny None;Persist Security Info=False";
			//*Conn.ConnectionString = @"Provider="+ProviderDB+";Data Source="+PathDB+"\\"+FileDB+";Mode=Share Deny None;Persist Security Info=False;";
			
			if (!File.Exists(PathDB+"\\"+FileDB))
			{
				PathDB=@"\\deploy2\db$\";
				if	(!File.Exists(PathDB+"\\"+FileDB)) 				
				{
					PathDB=Path.GetDirectoryName(Application.ExecutablePath);
					int di=2; // Первое появлиние \ в строке пути. Начальное значение для локального пути типа c:\users\ - 2-ая позиция начиная с нуля.
					if (PathDB.IndexOf('\\')==0) // Если сетевой путь, то Индекс первого появления  \ нужно расщитать
					{					
					di=PathDB.IndexOf('\\',3);
					}
					int i=PathDB.LastIndexOf('\\');	
					while (i>di)
					{
						PathDB=PathDB.Substring(0,i);
							if (File.Exists(PathDB+"\\"+FileDB)) break;
						 i=PathDB.LastIndexOf('\\');		
					}
				}
			}

				
			Conn.ConnectionString = @"Provider="+ProviderDB+";Data Source="+PathDB+"\\"+FileDB+";Mode=Share Deny None;Persist Security Info=False;";
			toolStripStatusLabel2.Text=PathDB;
			
		    try
		    {
		        Conn.Open();
		        OleDbDataAdapter adapter;
		        String strSql;
		        strSql="select * from DialogList order by name";
                adapter = new OleDbDataAdapter(strSql,Conn);
                
                adapter.Fill(dataSet1);
                dataGridView1.DataSource = dataSet1.Tables[0];	
                //dataGridView1.Columns[0].Visible=false;
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
			this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1CellValueChanged);
			
		    }
		        catch (Exception ex)
		    {
		        MessageBox.Show("Ошибка подключения к базе данных\n"+ex.Message,"Установка ПО",MessageBoxButtons.OK,MessageBoxIcon.Error);		        
		        Close();
		    }
		    finally
		    {
		        //Conn.Close();
		    }
		}
		
		
		void Button1Click(object sender, EventArgs e)
		{
			if (selectCnt==0) Close();
			String idList="";
			foreach (DataGridViewRow row in dataGridView1.Rows)
				{
					if (Convert.ToBoolean(row.Cells[0].Value))
						idList=idList+Convert.ToString(row.Cells[3].Value)+" ";
				}
			Debug.WriteLine(idList);			
			
			ProcessStartInfo ProcessInfo;
	        Process Process; 
	        ProcessInfo = new ProcessStartInfo();
	        ProcessInfo.Arguments=idList;
	        ProcessInfo.WorkingDirectory=Path.GetDirectoryName(Application.ExecutablePath);	
	        ProcessInfo.FileName="pump.exe"; 
	        
	        //while (!Process.WaitForExit(1000));// Waits here for the process to exit.
	        //Debug.WriteLine(Process.ExitCode);     
			try
		    {
		    Process = Process.Start(ProcessInfo);	
			}
	        catch (Exception ex)
		    {		        
		        MessageBox.Show("Ошибка запуска\n"+ex.Message,"Установка ПО",MessageBoxButtons.OK,MessageBoxIcon.Error);
		    }
		    finally
		    {
		     
		    }
		    Close();
			
		}
	 	
		void DataGridView1CurrentCellChanged(object sender, EventArgs e)
		{
			//https://msdn.microsoft.com/ru-ru/library/system.windows.forms.datagridview.currentcellchanged.aspx?cs-save-lang=1&cs-lang=csharp#code-snippet-2
			panel1.Refresh();
		}
		
		void Panel1Paint(object sender, PaintEventArgs e)
		{
			OleDbCommand cmd = new OleDbCommand();
			cmd.CommandText="select * from DialogSoftProperty  where id="+ Convert.ToString(dataGridView1.CurrentRow.Cells[3].Value);
			//cmd.CommandType=CommandType.Text;
			cmd.Connection=Conn;
			OleDbDataReader  rs ;			
			try
		    {		       
				//rs=cmd.ExecuteReader(CommandBehavior.SequentialAccess);				
				rs=cmd.ExecuteReader(CommandBehavior.SingleResult);				
				rs.Read();	
				
				string ProgName="";
				string ProgPath=EliminateEnclosingQuotes((string)(rs["cmd"]));
				lStatus.Text=rs["Status"].ToString();
				try
				{					
					ProgName=Path.GetFileName(ProgPath);
					ProgPath=Path.GetFullPath(ProgPath);
				}
				catch
				{
					ProgName=rs["cmd"].ToString();
					ProgPath="";				
				}
				finally
					{this.lProgName.Text=ProgName;
					this.lProgPath.Text=ProgPath;}    	        
    	        try 
				{					
    	        FillPictureBoxFromOLEField(this.lTypeAutoInstall,(byte[])rs["AutoInstallIcon"]);
    	        }
    	        catch(Exception ex){Debug.WriteLine("Panel1Paint AutoInstallIcon "+(string)ex.Message);}
    	        try 
				{					
    	        //https://support.microsoft.com/ru-ru/kb/308614 	
    	        FillPictureBoxFromOLEField(lTypeCI,(byte[])rs["CIicon"]);
    	        }
    	        catch(Exception ex){Debug.WriteLine("Panel1Paint CIicon "+(string)ex.Message);}

    	        
    	        
			}
	        catch (Exception ex)
		    {		        
	        	Debug.WriteLine("Panel1Paint "+(string)ex.Message);
		    }
		    finally
		    {
		        //rs.Close();	
		    }	
		}
		public string EliminateEnclosingQuotes(string str)
		{
			str=str.TrimStart('"');
			str=str.TrimEnd('"');
			return str;
		}
		public static byte[] GetImageBytesFromOLEField(byte[] oleFieldBytes)
    	{
        const string BITMAP_ID_BLOCK = "BM";
        const string JPG_ID_BLOCK = "\u00FF\u00D8\u00FF";
        const string PNG_ID_BLOCK = "\u0089PNG\r\n\u001a\n";
        const string GIF_ID_BLOCK = "GIF8";
        const string TIFF_ID_BLOCK = "II*\u0000";

        byte[] imageBytes;

        // Get a UTF7 Encoded string version
        Encoding u8 = Encoding.UTF7;
        string strTemp = u8.GetString(oleFieldBytes);

        // Get the first 300 characters from the string
        string strVTemp = strTemp.Substring(0, 300);

        // Search for the block
        int iPos = -1;
        if (strVTemp.IndexOf(BITMAP_ID_BLOCK) != -1)
            iPos = strVTemp.IndexOf(BITMAP_ID_BLOCK);
        else if (strVTemp.IndexOf(JPG_ID_BLOCK) != -1)
            iPos = strVTemp.IndexOf(JPG_ID_BLOCK);
        else if (strVTemp.IndexOf(PNG_ID_BLOCK) != -1)
            iPos = strVTemp.IndexOf(PNG_ID_BLOCK);
        else if (strVTemp.IndexOf(GIF_ID_BLOCK) != -1)
            iPos = strVTemp.IndexOf(GIF_ID_BLOCK);
        else if (strVTemp.IndexOf(TIFF_ID_BLOCK) != -1)
            iPos = strVTemp.IndexOf(TIFF_ID_BLOCK);
        else
            throw new Exception("Unable to determine header size for the OLE Object");

        // From the position above get the new image
        if (iPos == -1)
            throw new Exception("Unable to determine header size for the OLE Object");

        imageBytes = new byte[oleFieldBytes.LongLength - iPos];
        MemoryStream ms = new MemoryStream();
        ms.Write(oleFieldBytes, iPos, oleFieldBytes.Length - iPos);
        imageBytes = ms.ToArray();
        ms.Close();
        ms.Dispose();
        return imageBytes;
    	}	
		
		public void FillPictureBoxFromOLEField(PictureBox pic,byte[] fldraw)
		{
		byte[] bimg = GetImageBytesFromOLEField(fldraw);
 	    MemoryStream ms = new MemoryStream(bimg);
        pic.Image=new System.Drawing.Bitmap(ms);
        ms.Close();
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
		void DataGridView1CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			Debug.Print(D_T()+" CellDoubleClick");
			dataGridView1.CurrentRow.Cells[0].Value=!Convert.ToBoolean(dataGridView1.CurrentRow.Cells[0].Value);
			//selectCntChanged();		
		}

		
		void DataGridView1CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			Debug.Print(D_T()+" CellValueChanged");
			Debug.Print(D_T()+" IsCurrentCellDirty="+dataGridView1.IsCurrentCellDirty.ToString());
			selectCntChanged();	
		}
		
		void DataGridView1CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			Debug.Print(D_T()+" CurrentCellDirtyStateChanged");
			if (dataGridView1.IsCurrentCellDirty)
				dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
		}
		string D_T(){return DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss ff");}
	}
}

