#region Using directives

using System;
using System.Reflection;
using System.Runtime.InteropServices;

#endregion

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Software installer")]
[assembly: AssemblyDescription("Software installer")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("SW")]
[assembly: AssemblyCopyright("by Skorik 2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM, use [ComVisible(true)] on that type.
[assembly: ComVisible(false)]

// The assembly version has following format :
//
// Major.Minor.Build.Revision
//
// You can specify all the values or you can use the default the Revision and 
// Build Numbers by using the '*' as shown below:
[assembly: AssemblyVersion("1.15.08.25")]
//[assembly: AssemblyFileVersion("1.1.*.0")]
//[assembly: AssemblyInformationalVersion("1.0 RC1")]

/*
using System.Reflection;   
  
// assembly version   
string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();   
  
// assembly file version   
string assemblyFileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(   
Assembly.GetExecutingAssembly().Location).ProductVersion;   
  
// any file version   
string fileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(   
@"C:\Windows\notepad.exe").ProductVersion;   */