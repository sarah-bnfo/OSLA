function Start(){
	var greeter = Host.Attach("greeter.js");
	var name = Host.Accept("Enter your name", Host.Action("Identify"));
	var region = Host.Assign("Select your region", "East", "West", "North", "South");
	var formal = Host.Assure("Show respect?");
	Host.Assert(greeter.Greet(name, region, formal));	
}
