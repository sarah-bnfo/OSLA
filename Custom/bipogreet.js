function Start(){

	Host.About()

	var greeter = Host.Import("Greeter.dll");

	var name = Host.Accept("Enter your name", Host.Action("Identify"));
	if(name == null) return;

	var region = Host.Accept("Enter your region", "East|West|North|South");
	if(region == null) return;

	var formal = Host.Inform("Show respect?")

	Host.Inform(greeter.Greet(name, region, formal));

}
