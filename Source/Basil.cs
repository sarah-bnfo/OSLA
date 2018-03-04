namespace Scripting.Interaction
{
	public abstract class AspectProviderBase
	{
		public virtual bool OnInform(dynamic scriptObj, string text, out bool result)
		{
			return result = false;
		}

		public virtual bool OnAccept(dynamic scriptObj, string text, object value, out object result)
		{
			result = null;
			return false;
		}

		public virtual bool OnImport(dynamic scriptObj, string source, out object result)
		{
			result = null;
			return false;
		}

		public virtual bool OnAction(dynamic scriptObj, string name, object[] arguments, out object result)
		{
			result = null;
			return false;
		}

	}

	public abstract class InteractiveScriptHost : Scripting.Integration.SimpleScriptHost
	{

		protected AspectProviderBase Aspect;

		protected abstract string Input(string Accept, string[] values);

		protected abstract bool Output(string message, bool confirm);

		public virtual bool Inform(string text)
		{
			bool result = false;
			if(Aspect != null && Aspect.OnInform(Active, text, out result)) return result;

			return Output(text, text.EndsWith("?"));
		}

		public virtual object Accept(string text, object value=null)
		{
			object result = null;
			if(Aspect != null && Aspect.OnAccept(Active, text, value, out result)) return result;

			string input;
			if(value != null && value.ToString().Contains("|"))
				input = Input(text, value.ToString().Split('|'));
			else
				input = Input(text, new string[]{value.ToString()});
			if(input == null) return null;

			decimal inputVal;
			if(decimal.TryParse(input, out inputVal))
				return inputVal;

			return input;				
		} 

		public virtual object Import(string source)
		{
			object result = null;
			if(Aspect != null && Aspect.OnImport(Active, source, out result)) return result;

			return ActivateScript(source);
		}	

		public virtual object Action(string name, params object[] arguments)
		{
			object result = null;
			if(Aspect != null && Aspect.OnAction(Active, name, arguments, out result)) return result;

			if(name.ToLower() == "identify")
				return this.GetType().Assembly.GetName().Name;

			return false;	
		}

	}
}
