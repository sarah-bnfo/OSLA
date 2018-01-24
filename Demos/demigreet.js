function Start(){
	Host.DoPresentation("demigreet.xaml");
}	

function Greet(name, region, respect){
	var greeting = respect ? "Hello " : "Hi ";
	return greeting + name + " from the " + region;
}
