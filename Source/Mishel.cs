using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;

namespace Mishel
{
    	using EXCEPINFO = System.Runtime.InteropServices.ComTypes.EXCEPINFO;

   	enum HRESULT
    	{
        	TYPE_E_ELEMENTNOTFOUND  = unchecked((int) 0x8002802B),
        	SCRIPT_E_REPORTED       = unchecked((int) 0x80020101),
        	E_NOTIMPL               = unchecked((int)0x80004001),
        	E_NOINTERFACE           = unchecked((int)0x80004002),
        	S_OK                    = 0x00000000,
        	S_FALSE                 = 0x00000001
    	}

    	public enum ScriptState : uint
    	{
        	Uninitialized   = 0,
        	Started         = 1,
        	Connected       = 2,
        	Disconnected    = 3,
        	Closed          = 4,
        	Initialized     = 5,
    	}

    	enum ScriptThreadState : uint
    	{
        	NotInScript = 0,
        	Running     = 1,
    	}

    	[Flags]
    	enum ScriptText : uint
    	{
        	None                = 0x0000,

        	DelayExecution      = 0x0001,
        	IsVisible           = 0x0002,
        	IsExpression        = 0x0020,
        	IsPersistent        = 0x0040,
        	HostManageSource    = 0x0080,
    	}

    	[Flags]
    	enum ScriptItem : uint
    	{
        	None                = 0x0000,

        	IsVisible           = 0x0002,
        	IsSource            = 0x0004,
        	GlobalMembers       = 0x0008,
        	IsPersistent        = 0x0040,
        	CodeOnly            = 0x0200,
        	NoCode              = 0x0400,
    	}

    	[Flags]
    	enum ScriptInfo : uint
    	{
        	None        = 0x0000,

        	IUnknown    = 0x0001,
        	ITypeInfo   = 0x0002,
    	}	

    	[Guid("00020400-0000-0000-C000-000000000046")]
    	[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    	interface IScript
    	{
    	}

    	[Guid("BB1A2AE1-A4F9-11cf-8F20-00805F2CD064")]
    	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    	interface IActiveScript
    	{
        	void SetScriptSite(IActiveScriptSite pass);
        	void GetScriptSite(Guid riid, out IntPtr site);
        	void SetScriptState(ScriptState state);
        	void GetScriptState(out ScriptState scriptState);
        	void Close();
        	void AddNamedItem(string name, ScriptItem flags);
        	void AddTypeLib(Guid typeLib, uint major, uint minor, uint flags);
        	void GetScriptDispatch(string itemName, out IScript dispatch);
        	void GetCurrentScriptThreadID(out uint thread);
        	void GetScriptThreadID(uint win32ThreadId, out uint thread);
        	void GetScriptThreadState(uint thread, out ScriptThreadState state);
        	void InterruptScriptThread(uint thread, out EXCEPINFO exceptionInfo, uint flags);
        	void Clone(out IActiveScript script);
    	}

    	[Guid("DB01A1E3-A42B-11cf-8F20-00805F2CD064")]
    	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    	interface IActiveScriptSite
    	{
        	void GetLCID(out int lcid);
        
        	void GetItemInfo(
            		string name,
            		ScriptInfo returnMask,
            		[Out] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.IUnknown)] object[] item,
            		[Out] [MarshalAs(UnmanagedType.LPArray)] IntPtr[] typeInfo);
        		void GetDocVersionString(out string version);
       		void OnScriptTerminate(object result, EXCEPINFO exceptionInfo);
        	void OnStateChange(ScriptState scriptState);
        	void OnScriptError(IActiveScriptError scriptError);
        	void OnEnterScript();
        	void OnLeaveScript();
    	}

    	[Guid("EAE1BA61-A4ED-11cf-8F20-00805F2CD064")]
    	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    	public interface IActiveScriptError
    	{
        	void GetExceptionInfo(out EXCEPINFO exceptionInfo);
        	void GetSourcePosition(out uint sourceContext, out uint lineNumber, out int characterPosition);
        	void GetSourceLineText(out string sourceLine);
    	}

    	[Guid("BB1A2AE2-A4F9-11cf-8F20-00805F2CD064")]
    	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    	interface IActiveScriptParse32
    	{
        	void InitNew();

        	void AddScriptlet(
            		string defaultName,
           	 	string code,
            		string itemName,
            		string subItemName,
            		string eventName,
            		string delimiter,
            		IntPtr sourceContextCookie,
            		uint startingLineNumber,
            		ScriptText flags,
            		out string name,
            		out EXCEPINFO exceptionInfo);
        	void ParseScriptText(
            		string code,
            		string itemName,
            		[MarshalAs(UnmanagedType.IUnknown)] object context,
            		string delimiter,
            		IntPtr sourceContextCookie,
            		uint startingLineNumber,
            		ScriptText flags,
            		[MarshalAs(UnmanagedType.Struct)] out object result,
            		out EXCEPINFO exceptionInfo);
    	}

    	[Guid("C7EF7658-E1EE-480E-97EA-D52CB4D76D17")]
    	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    	interface IActiveScriptParse64
    	{
        	void InitNew();

        	void AddScriptlet(
            		string defaultName,
           	 	string code,
            		string itemName,
            		string subItemName,
            		string eventName,
            		string delimiter,
            		IntPtr sourceContextCookie,
            		uint startingLineNumber,
            		ScriptText flags,
            		out string name,
            		out EXCEPINFO exceptionInfo);
        	void ParseScriptText(
            		string code,
            		string itemName,
            		[MarshalAs(UnmanagedType.IUnknown)] object context,
            		string delimiter,
            		IntPtr sourceContextCookie,
            		uint startingLineNumber,
            		ScriptText flags,
            		[MarshalAs(UnmanagedType.Struct)] out object result,
            		out EXCEPINFO exceptionInfo);
    	}

    	[ComImport]
    	[Guid("f414c260-6ac0-11cf-b6d1-00aa00bbbb58")]
    	class JScriptEngine {}

	public abstract class ScriptSiteBase : IActiveScriptSite
	{
        	public virtual void GetDocVersionString(out string v)
        	{
            		throw new NotImplementedException();
        	}
	
        	void IActiveScriptSite.GetItemInfo(
            		string name,
            		ScriptInfo returnMask,
            		[Out] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.IUnknown)] object[] item,
            		[Out] [MarshalAs(UnmanagedType.LPArray)] IntPtr[] typeInfo)
	
        	{
			object obj = GetExposedObject(name);
 	    		if(obj != null)
			{
				if(returnMask == ScriptInfo.IUnknown)
					item[0] = obj;
				else
					typeInfo[0] = Marshal.GetITypeInfoForType(obj.GetType());
			}
			else
				throw new ArgumentException();
        	}

        	public virtual void GetLCID(out int id)
        	{
            		throw new NotImplementedException();
        	}

        	public virtual void OnEnterScript(){}

        	public virtual void OnLeaveScript(){}

        	public virtual void OnScriptError(IActiveScriptError scriptError)
		{
			throw new ArgumentException();
		}

        	public virtual void OnScriptTerminate(object result, EXCEPINFO exceptionInfo)
		{
		}

        	public virtual void OnStateChange(ScriptState scriptState){}

		protected abstract object GetExposedObject(string name);

		public abstract string[] GetNamedItems();
	}

	public static class ScriptAdapter
	{
		private static Hashtable engineCache = new Hashtable();

		public static object CreateScriptObject(ScriptSiteBase scriptSite, string scriptText)
		{		
			IActiveScript engine = (IActiveScript)engineCache[scriptSite];
			IActiveScriptParse32 parser32 = null;
			IActiveScriptParse64 parser64 = null;
			
			if(IntPtr.Size == 4)
				parser32 = (IActiveScriptParse32)engine;
			else
				parser64 = (IActiveScriptParse64)engine;		

			if(engine == null)			
			{
				engine = (IActiveScript) new JScriptEngine();
				engine.SetScriptSite(scriptSite);
				foreach(string name in scriptSite.GetNamedItems())
					engine.AddNamedItem(name, ScriptItem.IsVisible);

				if(IntPtr.Size == 4)
				{
					parser32 = (IActiveScriptParse32)engine;
					parser32.InitNew();
				}
				else
				{
					parser64 = (IActiveScriptParse64)engine;
					parser64.InitNew();
				}		
				engineCache.Add(scriptSite, engine);
			}

			EXCEPINFO ei;
			object result;
			IScript scriptObject;

			if(IntPtr.Size == 4)
				parser32.ParseScriptText(scriptText, null, null, null, IntPtr.Zero, 1, ScriptText.None, out result, out ei);
			else
				parser64.ParseScriptText(scriptText, null, null, null, IntPtr.Zero, 1, ScriptText.None, out result, out ei);
			engine.GetScriptDispatch(null, out scriptObject);
			
			return scriptObject;
		}
	}

	public class HostedScriptSite : ScriptSiteBase
	{
		private object host;
		private static string[] namedItems = {"Host"};
		private static Hashtable siteCache = new Hashtable();

		public HostedScriptSite(object host)
		{
			this.host = host;	
		}

		public override string[] GetNamedItems()
		{
			return namedItems;
		}
	
        	protected override object GetExposedObject(string name)	
        	{
 	    		if(string.Compare(name, "Host", StringComparison.OrdinalIgnoreCase) == 0)
				return host;

			return null;
        	}

		public static object HostScript(object scriptHost, string scriptCode)
		{
			HostedScriptSite scriptSite = (HostedScriptSite)siteCache[scriptHost];
			if(scriptSite == null)
			{
				scriptSite = new HostedScriptSite(scriptHost);
				siteCache.Add(scriptHost, scriptSite);
			}

			return ScriptAdapter.CreateScriptObject(scriptSite, scriptCode); 
		}
	}

}

namespace Scripting.Integration
{
	using Mishel;

    	public abstract class SimpleScriptHost
    	{
		public object Host {get; private set;}

		protected object Active {get; private set;}
	
		protected object ActivateScript(string scriptFile, bool startup=false)
		{
			using(TextReader reader = File.OpenText(scriptFile))
			{
				object script = HostedScriptSite.HostScript(this, reader.ReadToEnd());
				if(startup) this.Active = script;
				return script;			
			}
		}
    	}

	public static class ScriptCaller
	{
		public static object CallScript(string code, object host, string function=null, params object[] arguments)
		{
			object script = HostedScriptSite.HostScript(host, code);
			if(function ==  null)
				return script;
			return script.GetType().InvokeMember(function, BindingFlags.InvokeMethod, null, script, arguments);  
		}
	}

	public class ScriptObjectAccessor
	{
		private object wrapped;

		public ScriptObjectAccessor(object target)
		{
			wrapped = target;
		}

		public int Length
		{
			get
			{
				return (int)wrapped.GetType().InvokeMember("length", BindingFlags.GetProperty, null, wrapped, null);
			}
		}

		public object this[object key]
		{
			get
			{
				return wrapped.GetType().InvokeMember(key.ToString(), BindingFlags.GetProperty, null, wrapped, null);
			}
			set
			{
				wrapped.GetType().InvokeMember(key.ToString(), BindingFlags.SetProperty, null, wrapped, new object[]{value});
			}
		}
	}

}

namespace Scripting.Interaction
{
	public abstract class InteractiveScriptHost : Scripting.Integration.SimpleScriptHost
	{
		protected abstract string Input(string prompt, object[] values);

		protected abstract bool Output(string message, bool confirm);

		public virtual object Attach(string source)
		{
			return ActivateScript(source);
		}	

		public virtual object Action(string name, params object[] arguments)
		{
			if(name.ToLower() == "identify")
				return this.GetType().Assembly.GetName().Name;
			return false;	
		}

		public virtual void Assert(string text)
		{
			Output(text, false);
		}

		public virtual bool Assure(string confirmation)
		{
			return Output(confirmation, true);
		}

		public virtual object Accept(string prompt, object value=null)
		{
			string input = Input(prompt, new object[]{value});
			if(input == null) return value;

			decimal result;
			if(decimal.TryParse(input, out result))
				return result;

			return input;				
		} 

		public virtual object Assign(string prompt, params object[] choices)
		{
			string input = Input(prompt, choices);	

			if(input == null && choices.Length > 0)
				input = choices[0].ToString();

			decimal result;
			if(decimal.TryParse(input, out result))
				return result;
			
			return input;		
		}
	}
}

