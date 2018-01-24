var rateUpdated;

function Load(){
	AttachHandler(okButton, "Click", Action);
	AttachHandler(cancelButton, "Click", Action);
}

function Action(s, e){
	if(s == okButton) rateUpdated = true;
	setRateWindow.Close();		
}


