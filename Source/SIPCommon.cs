namespace Scripting.Interaction
{
	public abstract class AspectProviderBase
	{
		public virtual bool OnInform(dynamic scriptObj, string text)
		{
			return false;
		}

		public virtual bool OnConcur(dynamic scriptObj, string confirmation, out bool result)
		{
			return result = false;
		}

		public virtual bool OnAccept(dynamic scriptObj, string prompt, object value, out object result)
		{
			result = null;
			return false;
		}

		public virtual bool OnChoose(dynamic scriptObj, string prompt, object[] choices, out object result)
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

		protected abstract string Input(string prompt, object[] values);

		protected abstract bool Output(string message, bool confirm);

		public virtual void Inform(string text)
		{
			if(Aspect == null || Aspect.OnInform(Active, text) == false)
				Output(text, false);
		}

		public virtual bool Concur(string confirmation)
		{
			bool result = false;
			if(Aspect != null && Aspect.OnConcur(Active, confirmation, out result)) return result;

			return Output(confirmation, true);
		}

		public virtual object Accept(string prompt, object value=null)
		{
			object result = null;
			if(Aspect != null && Aspect.OnAccept(Active, prompt, value, out result)) return result;

			string input = Input(prompt, new object[]{value});
			if(input == null) return value;

			decimal inputVal;
			if(decimal.TryParse(input, out inputVal))
				return inputVal;

			return input;				
		} 

		public virtual object Choose(string prompt, params object[] choices)
		{
			object result = null;
			if(Aspect != null && Aspect.OnChoose(Active, prompt, choices, out result)) return result;

			string input = Input(prompt, choices);	

			if(input == null && choices.Length > 0)
				input = choices[0].ToString();

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
