var lu;
var rates = new Array(6, 8.5, 9.5, 11);

function Load(){
	AttachHandler(mainWindow, "PreviewKeyDown", CalcOnEnter);
	AttachHandler(calcMenuItem, "Click", Calculate);
	AttachHandler(setRateMenuItem, "Click", SetRate);
	AttachHandler(exitMenuItem, "Click", Exit);
	AttachHandler(loanTextBox, "TextChanged", Clear);
	AttachHandler(periodTextBox, "TextChanged", Clear);
	AttachHandler(schemeComboBox, "SelectionChanged", Calculate);
	AttachHandler(employeeCheckBox, "Click", Calculate);
	lu = Parent.Host.Import("LoanUtil.js");
}

function Calculate(s, e){
	var p = loanTextBox.Text;
	var n = periodTextBox.Text;
	var ss = schemeComboBox.SelectedIndex;
	var r = rates[ss];
	if(employeeCheckBox.IsChecked == true && ss < 3) r *= 0.6;
	var mi = lu.MonthlyInstallment(p, n, r);
	emiTextBox.Text = mi.toFixed(2);
}

function Clear(s, e){
	emiTextBox.Text = "";
}

function Exit(s, e){
	mainWindow.Close();
}

function CalcOnEnter(s, e){
	if(e.Key == 6) //Enter pressed
		Calculate(s, e);
}

function SetRate(s, e){
	var scb = schemeComboBox;
	var srp = NewPresentation("LoanSetRate.xaml");
	srp.setRateWindow.Title = scb.Text + " Scheme";
	srp.rateTextBox.Text = rates[scb.SelectedIndex];
	srp.setRateWindow.ShowDialog();
	if(srp.rateUpdated)
	{
		rates[scb.SelectedIndex] = srp.rateTextBox.Text;
		Calculate(s, e);
	}
}




