function Start(){
	var name = Host.Prompt("Enter your name", "Owner");
	Host.Action("FormatAndInform", "Hello {0}!", name)	
}
