function Load(){
	AttachHandler(regionEntry, "SelectionChanged", actionButton_Click);
	AttachHandler(respectEntry, "Click", actionButton_Click);
}

function actionButton_Click(s, e){
	outputBlock.Text = Parent.Greet(nameEntry.Text, regionEntry.SelectedValue.Content, respectEntry.IsChecked);
}

function nameEntry_TextChanged(s, e){
	outputBlock.Text = "";
}
