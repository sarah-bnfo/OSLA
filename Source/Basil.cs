namespace Scripting.Interaction
{
	public abstract class AspectProviderBase
	{
		public virtual bool OnInform(dynamic scriptObj, string text, bool confirm, out bool result)
		{
			return result = false;
		}

		public virtual bool OnAccept(dynamic scriptObj, string text, object[] values, out object result)
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

		protected abstract string Input(string Accept, object[] values);

		protected abstract bool Output(string message, bool confirm);

		public virtual bool Inform(string text, bool confirm=false)
		{
			bool result = false;
			if(Aspect != null && Aspect.OnInform(Active, text, confirm, out result)) return result;

			return Output(text, confirm);
		}

		public virtual object Accept(string text, params object[] values)
		{
			object result = null;
			if(Aspect != null && Aspect.OnAccept(Active, text, values, out result)) return result;

			string input = Input(text, values);
			if(input == null) return values.Length == 1 ? values[0] : null;

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
