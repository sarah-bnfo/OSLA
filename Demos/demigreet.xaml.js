function Load(){
	AttachHandler(actionButton, "Click", Action);
	AttachHandler(regionEntry, "SelectionChanged", Action);
	AttachHandler(respectEntry, "Click", Action);
	AttachHandler(nameEntry, "TextChanged", Clear);
}

function Action(s, e){
	outputBlock.Text = Parent.Greet(nameEntry.Text, regionEntry.SelectedValue.Content, respectEntry.IsChecked);
}

function Clear(s, e){
	outputBlock.Text = "";
}
