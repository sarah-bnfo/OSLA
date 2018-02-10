using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls;

namespace Demi
{

    	public class MarkupHost : Scripting.Integration.EventfulScriptHost
    	{

	       	private Window hostedWindow;

		private object parent;

        	private static void AddElementField(FrameworkElement fe, IList<string> fields)
        	{
            		foreach (object e in LogicalTreeHelper.GetChildren(fe))
            		{
                		FrameworkElement c = e as FrameworkElement;
                		if (c != null) AddElementField(c, fields);
            		}

            		if (fe.Name != "") fields.Add(fe.Name);
        	}

		protected override object GetPropertyValue(string name)
		{
			if(name == "Parent")
				return parent;

			return hostedWindow.FindName(name);
		}

        	public object NewPresentation(string markupFile, string scriptFile=null, object context=null)
        	{
   	    		try
	    		{
				MarkupHost host = null;
 	    			using(StreamReader sr = File.OpenText(markupFile))
	    			{
					var nsm = new XmlNamespaceManager(new NameTable());
					nsm.AddNamespace(string.Empty, "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
					var pc = new XmlParserContext(null, nsm, null, XmlSpace.None);

					Window win = (Window) XamlReader.Load(XmlReader.Create(sr, null, pc));
		    			win.Owner = hostedWindow;
					if(win.Title == "")
						win.Title = Path.GetFileNameWithoutExtension(markupFile);

					if(scriptFile == null) scriptFile = win.Tag as string;
					if(scriptFile == null && File.Exists(markupFile + ".js")) scriptFile = markupFile + ".js";						

					if(scriptFile != null)
					{
		    				List<string> fields = new List<string>();
						fields.Add("Parent");
		    				AddElementField(win, fields);
		    				host = ActivateScript<MarkupHost>(scriptFile, fields);
		    				host.hostedWindow = win;
						host.parent = context;
		    				host.Load();
					}
					else
					{
						host = new MarkupHost();
						host.hostedWindow = win;
					}

	    			}

				return host;	    
	    		}
	    		catch(XamlParseException ex)
	    		{
				MessageBox.Show(markupFile + " - " + ex.Message , "Markup Error", MessageBoxButton.OK, MessageBoxImage.Error);
	    		}
	    		catch(ArgumentException ex)
	    		{
				MessageBox.Show(ex.Message, "Script Error", MessageBoxButton.OK, MessageBoxImage.Error);
	    		}
	    		catch(TargetInvocationException ex)
	    		{
				MessageBox.Show(ex.InnerException.Message, "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
	    		}
	    		catch(Exception ex)
	    		{
				MessageBox.Show(ex.ToString() , "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
	    		}

	    		return null;
        	}

		public object NewInstance(string typeName, string assemblyName, params object[] arguments)
		{			
			Type objType = Type.GetType(typeName + ", " + assemblyName);
			if(objType == null)
			{
				Assembly asm = Assembly.LoadFrom(assemblyName + ".dll");
				objType = asm.GetType(typeName, true);
			}

			return Activator.CreateInstance(objType, arguments);
		}

		public bool ShowMessage(string text, bool critical=false)
		{
			return MessageBox.Show(text, hostedWindow.Title, MessageBoxButton.OKCancel, critical ? MessageBoxImage.Error : MessageBoxImage.Exclamation) == MessageBoxResult.OK;
		}

        	private static void App_Exception(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        	{
            		StreamWriter sw = File.CreateText("error.log");
            		sw.WriteLine("{0} - {1}", e.Exception.GetType().Name, e.Exception);
            		sw.Close();

            		string msg = string.Format("Unhandled {0}\r\n{1}\r\nDo you want to resume execution?", e.Exception.GetType().Name, e.Exception.Message);
            		MessageBoxResult mr = MessageBox.Show(msg, "Runtime Error", MessageBoxButton.YesNo, MessageBoxImage.Error);

            		if (mr == MessageBoxResult.No) Environment.Exit(-1);

            		e.Handled = true;
        	}

		internal static void Run(string markup, string script, object parent)
		{
	              	MarkupHost mh = new MarkupHost();
			MarkupHost mhp = (MarkupHost) mh.NewPresentation(markup, script, parent);
 
                	if (mhp != null)
                	{
                    		Application app = new Application();
                    		app.DispatcherUnhandledException += App_Exception;
                    		app.Run(mhp.hostedWindow);
                	}
		}

    	}

}

namespace Scripting
{
    	public static class HostExtension
    	{
		private static dynamic Invoker = null;

		public static void Invoke(string markup, string script=null)
		{
			if(markup == null)
			{
				string name = Invoker.Host.TargetName;
				markup = name.Replace(".js", ".xaml");
			}

			Demi.MarkupHost.Run(markup, script, Invoker);
		}
	}

}

