function Start(){
	var greeter = Host.Import("greeter.js");
	var name = Host.Accept("Enter your name", Host.Action("identify"));
	var region = Host.Accept("Select your region", "East|West|North|South");
	var formal = Host.Inform("Show respect?");
	Host.Inform(greeter.Greet(name, region, formal));	
}
