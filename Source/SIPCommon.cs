namespace Scripting.Interaction
{
	public abstract class InteractiveScriptHost : Scripting.Integration.SimpleScriptHost
	{
		protected abstract string Input(string prompt, object[] values);

		protected abstract bool Output(string message, bool confirm);

		public virtual void Inform(string text)
		{
			Output(text, false);
		}

		public virtual bool Concur(string confirmation)
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

		public virtual object Choose(string prompt, params object[] choices)
		{
			string input = Input(prompt, choices);	

			if(input == null && choices.Length > 0)
				input = choices[0].ToString();

			decimal result;
			if(decimal.TryParse(input, out result))
				return result;
			
			return input;		
		}

		public virtual object Import(string source)
		{
			return ActivateScript(source);
		}	

		public virtual object Action(string name, params object[] arguments)
		{
			if(name.ToLower() == "identify")
				return this.GetType().Assembly.GetName().Name;
			return false;	
		}

	}
}
