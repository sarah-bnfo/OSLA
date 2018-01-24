function Start(task){

	switch(task)
	{
	case "Initialize": 
		Initialize();
		break;

	case "Update": 
		Update();
		break;

	default:
		Action();
	}
}


function Initialize(){
	Host.CanProcess(false);
}


function Update(){
	var text = Host.ReadInput();
	Host.CanProcess(text.length > 0);
	Host.WriteOutput("");
}


function Action(){
	var text = Host.ReadInput();
	var result = "";
	for(var i = text.length - 1; i >= 0; i--)
		result += text.charAt(i);
	Host.WriteOutput(result);
}

