function Start(){
	var name = Host.Accept("Enter your name", "Owner");
	Host.Action("FormatAndInform", "Hello {0}!", name)	
}
