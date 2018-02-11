function Load(){
	AttachHandler(actionButton, "Click", Action);
	AttachHandler(regionEntry, "SelectionChanged", Action);
	AttachHandler(respectEntry, "Click", Action);
}

function Action(s, e){
	outputBlock.Text = Parent.Greet(nameEntry.Text, regionEntry.SelectedValue.Content, respectEntry.IsChecked);
}

function nameEntry_TextChanged(s, e){
	outputBlock.Text = "";
}
