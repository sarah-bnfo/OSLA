function Start(){
	
	var defStore = Host.OpenStore();
	var stateGroup = defStore.Choose("state", true);
	
	var name = Host.Accept("Your name: ", stateGroup.Peek("name"));
	if(name){
		Host.Assert("Hello " + name + "!");
		stateGroup.Put("name", name);
	}		
}
