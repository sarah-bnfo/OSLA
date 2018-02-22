function Start(){
	var greeter = Host.Import("greeter.js");
	var name = Host.Accept("Enter your name", Host.Action("Identify"));
	var region = Host.Accept("Select your region", "East", "West", "North", "South");
	var formal = Host.Confirm("Show respect?");
	Host.Inform(greeter.Greet(name, region, formal));	
}
