function Start(){
	var greeter = Host.Import("greeter.js");
	var name = Host.Accept("Enter your name", Host.Action("Identify"));
	var region = Host.Accept("Select your region", "East", "West", "North", "South");
	var formal = Host.Inform("Show respect?", true);
	Host.Inform(greeter.Greet(name, region, formal));	
}
