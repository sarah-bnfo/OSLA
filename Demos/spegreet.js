function Start(){

	var greeter = Host.Attach("greeter.js");

	var greetInput = Host.AcceptMany("Greetings",
		["Your name", "Owner"], 			//prompt 0
		["Your region", "East|West|North|South", 2], 	//prompt 1
		["Respect", "false|true", 1]			//prompt 2
	);
	if(greetInput){
		var name = greetInput.ForPrompt(0);			//get input for prompt 0 
		var region = greetInput.ForPrompt("Your region");	//get input for prompt "Your region"
		var respect = greetInput.ForPrompt("Respect", true);	//get input for prompt "Respect" as choice
		Host.Assert(greeter.Greet(name, region, respect), "Greetings");
	}
}
