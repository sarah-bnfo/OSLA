function Start(){
	var greeter = Host.Import("greeter.js");
	var name = Host.Accept("Enter your name", Host.Action("Identify"));
	var region = Host.Choose("Select your region", "East", "West", "North", "South");
	var formal = Host.Concur("Show respect?");
	Host.Inform(greeter.Greet(name, region, formal));	
}
