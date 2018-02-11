using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.CodeDom.Compiler;

namespace Evita
{
	public static class ScriptAdapter
	{
		private static Dictionary<string, object> scriptCache = new Dictionary<string, object>();
		private static Dictionary<string, DynamicMethod> eventCache = new Dictionary<string, DynamicMethod>();
		private static CodeDomProvider prov = CodeDomProvider.CreateProvider("js");

		public static S CreateScriptObject<S>(string name, TextReader script, IEnumerable<string> members=null)
		{
			string className = name.Replace(".", "_") + "$" + Guid.NewGuid().ToString("N");
			Type baseType = typeof(S);
			Type scriptType = null;

			CompilerParameters cp = new CompilerParameters();
			cp.GenerateInMemory = true;
			cp.ReferencedAssemblies.Add(baseType.Assembly.Location);
			
			StringBuilder src = new StringBuilder();
			src.Append("class " + className + " extends " + baseType.FullName + "{\r\n\r\n");
			int lines = 1;
			if(members != null)
			{
				foreach(string entry in members)
				{
					if(entry.EndsWith("()"))
					{
						string func = entry.Substring(0, entry.Length - 2);
						src.Append("function " + func + "(...args:Object[]){return CallMethod(" 
							+ "\"" + func + "\", args);}\r\n");
					}
					else	
						src.Append("function get " + entry + "(){return GetPropertyValue(" 
							+ "\"" + entry + "\");}\r\n");
					
					lines++;
				}
			}
			
			src.Append(script.ReadToEnd());
			src.Append("}");

			string[] sc = {src.ToString()};
			CompilerResults cr = prov.CompileAssemblyFromSource(cp, sc);
			if(cr.Errors.Count > 0)
			{
				src.Remove(0, src.Length);
				foreach(CompilerError er in cr.Errors)
					src.Append(String.Format("{0} line {1} - {2}\r\n", name, er.Line - lines - 1, er.ErrorText));
				throw new ArgumentException(src.ToString());
			}
			else
			{
				scriptType = cr.CompiledAssembly.GetType(className);				
				if(scriptType == null) throw new ArgumentException(cr.CompiledAssembly.GetTypes().Length.ToString());			
			}
			
			return (S)Activator.CreateInstance(scriptType);
		}

		public static S CreateScriptObject<S>(string scriptFile, IEnumerable<string> members=null)
		{
			object scriptObject;

			if(!scriptCache.TryGetValue(scriptFile, out scriptObject))
			{
				using(StreamReader reader = File.OpenText(scriptFile))
				{
					scriptObject = CreateScriptObject<S>(scriptFile, reader, members);
					scriptCache.Add(scriptFile, scriptObject);
				}
			}

			return (S)scriptObject;
		}

		public static Delegate CreateEventDelegate(MethodInfo delegateMethod, Type delegateType, object delegateTarget)
		{
			string name = delegateTarget.GetType().Name + "." + delegateMethod.Name;
			DynamicMethod dm;

			if(!eventCache.TryGetValue(name, out dm))
			{
				
				Type[] signature = {typeof(object), typeof(object), typeof(object)};
				dm = new DynamicMethod("Dispatch_" + delegateMethod.Name, null, signature, delegateTarget.GetType());
				ILGenerator ilgen = dm.GetILGenerator();

                		ilgen.Emit(OpCodes.Ldarg_0);
                		ilgen.Emit(OpCodes.Ldarg_1);
                		ilgen.Emit(OpCodes.Ldarg_2);
                		ilgen.Emit(OpCodes.Call, delegateMethod);
                		ilgen.Emit(OpCodes.Ret);
		
				eventCache.Add(name, dm);
			}			

			return dm.CreateDelegate(delegateType, delegateTarget);
		}
		
	}

	public class SimpleScriptSite
	{
		public object Host {get; internal set;}

		public static SimpleScriptSite HostScript(string scriptCode, object scriptHost)
		{
			SimpleScriptSite script = ScriptAdapter.CreateScriptObject<SimpleScriptSite>("script", new StringReader(scriptCode));				
			script.Host = scriptHost;
			return script;
		}
	}

	public delegate void ScriptedEventHandler(object sender, object e);
}

namespace Scripting.Integration
{
	using Evita;

	public abstract class EventfulScriptHost
	{
		protected Delegate AttachHandler(object target, string eventName, ScriptedEventHandler handler)
        	{
           		Type t = target.GetType();
            		EventInfo ei = t.GetEvent(eventName);
            		Delegate eh = Evita.ScriptAdapter.CreateEventDelegate(handler.Method, ei.EventHandlerType, handler.Target);

            		ei.AddEventHandler(target, eh);

            		return eh;
        	}

		protected Delegate AttachHandler(object target, string eventName, string handlerMethodName)
		{
			var handler = (ScriptedEventHandler) Delegate.CreateDelegate(typeof(ScriptedEventHandler), this, handlerMethodName);
			AttachHandler(target, eventName, handler);

			return handler;
		}

       		protected void DetachHandler(object target, string eventName, Delegate handler)
        	{
           		Type t = target.GetType();
            		EventInfo ei = t.GetEvent(eventName);

            		ei.RemoveEventHandler(target, handler);
        	}

		protected virtual object GetPropertyValue(string name)
		{
			return null;
		}

		protected virtual object CallMethod(string name, object[] arguments)
		{
			return null;
		}

		protected static S ActivateScript<S>(string source, IEnumerable<string> members)
		{
			return ScriptAdapter.CreateScriptObject<S>(source, members);
		}

		public virtual void Load(){}
	}

	public abstract class SimpleScriptHost : IReflect
	{
		private Dictionary<string, MemberInfo[]> extCache = new Dictionary<string, MemberInfo[]>(); 

		protected SimpleScriptHost Active{get; private set;}

		public object Host {get; internal set;}

		protected object ActivateScript(string scriptFile, bool startup=false)
		{
			if(!startup)
			{
				SimpleScriptSite obj = ScriptAdapter.CreateScriptObject<SimpleScriptSite>(scriptFile);
				obj.Host = this;
				return obj;	
			}

			Active = ScriptAdapter.CreateScriptObject<SimpleScriptHost>(scriptFile);
			Active.Host = this;
		
			string[] extAsms = Directory.GetFiles(".", "*.dll");
			foreach(string extAsm in extAsms)
			{
				string cmdName = Path.GetFileNameWithoutExtension(extAsm);
				try
				{
					Assembly asm = Assembly.LoadFrom(extAsm);
					Type t = asm.GetType("Scripting.HostExtension");
					if(t == null) continue;
					MemberInfo[] info = t.GetMember("Invoke", BindingFlags.Public | BindingFlags.Static);
					if(info != null && info.Length > 0)
					{
						FieldInfo hostField = t.GetField("Invoker", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
						if(hostField != null)
							hostField.SetValue(null, Active);
						extCache.Add(cmdName, info);
					}
				}
				catch{}
			}
			
			return Active;			
		}

		protected virtual MemberInfo[] GetExtensionMember(string name)
		{		
			MemberInfo[] info;
			if(extCache.TryGetValue(name, out info)) return info;

			try
			{
				Type t = Type.GetType("Scripting.HostExtension," + name);
				MemberInfo[] result = t.GetMember("Invoke", BindingFlags.Public | BindingFlags.Static);
				
				if(result != null && result.Length > 0)
				{
					FieldInfo hostField = t.GetField("Invoker", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
					if(hostField != null)
						hostField.SetValue(null, Active);

					extCache.Add(name, result);
				}
				return result;
			}
			catch
			{
				return null;
			}
		}

		MemberInfo[] IReflect.GetMember(string name, BindingFlags invokeAttr)
		{
			MemberInfo[] info = this.GetType().GetMember(name, invokeAttr);
			if(info.Length == 0)
			{
				MemberInfo[] einfo = GetExtensionMember(name);
				if(einfo != null)
					return einfo;

				throw new NotSupportedException("Object doesn't support " + name + " function");
			}
			  
			return info;
		}

        	Type IReflect.UnderlyingSystemType {get {return this.GetType().UnderlyingSystemType;}}
        	FieldInfo IReflect.GetField(string name, BindingFlags bindingAttr) {return this.GetType().GetField(name, bindingAttr);}
        	FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr) {return this.GetType().GetFields(bindingAttr);}
        	MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr) {return this.GetType().GetMembers(bindingAttr);}
        	MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr) {return this.GetType().GetMethod(name, bindingAttr);}
        	MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers) {return this.GetType().GetMethod(name, bindingAttr, binder, types, modifiers);}
        	MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr) {return this.GetType().GetMethods(bindingAttr);}
        	PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr) {return this.GetType().GetProperties(bindingAttr);}
        	PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr) {return this.GetType().GetProperty(name, bindingAttr);}
		PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers) {return this.GetType().GetProperty(name, bindingAttr, binder, returnType, types, modifiers);}        
		object IReflect.InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] namedParameters) {return this.GetType().InvokeMember(name, invokeAttr, binder, target, args);}
 		
		public virtual void Start(object task){Start();}

		public virtual void Start(){throw new ArgumentException("Start function missing");}

	}

	public static class ScriptCaller
	{
		public static object CallScript(string code, object host, string function=null, params object[] arguments)
		{
			object script = SimpleScriptSite.HostScript(code, host);
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
				return (int) wrapped.GetType().InvokeMember("length", BindingFlags.GetProperty, null, wrapped, null);
			}
		}

		public object this[object key]
		{
			get
			{
				return wrapped.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, wrapped, new object[]{key});
			}
			set
			{
				wrapped.GetType().InvokeMember("Item", BindingFlags.SetProperty, null, wrapped, new object[]{key, value});
			}
		}
	}
}

