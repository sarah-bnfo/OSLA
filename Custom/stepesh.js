function Load(){
	actionButton.Enabled = false;
	AttachHandler(inputBox, "TextChanged", Update);
	AttachHandler(actionButton, "Click", Action);	
}

function Action(s, e){
	var text = inputBox.Text;
	var result = "";
	for(var i = text.length - 1; i >= 0; i--)
		result += text.charAt(i);
	outputBox.Text = result;
}

function Update(s, e){				
	outputBox.Text = "";
	actionButton.Enabled = inputBox.Text.Length > 0;
};
