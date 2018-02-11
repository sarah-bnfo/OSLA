var lu;
var rates = new Array(6, 8.5, 9.5, 11);

function Load(){
	lu = Parent.Host.Import("LoanUtil.js");
}

function Calculate(){
	var p = loanTextBox.Text;
	var n = periodTextBox.Text;
	var ss = schemeComboBox.SelectedIndex;
	var r = rates[ss];
	if(employeeCheckBox.IsChecked == true && ss < 3) r *= 0.6;
	var mi = lu.MonthlyInstallment(p, n, r);
	emiTextBox.Text = mi.toFixed(2);
}

function calcMenuItem_Click(s, e){
	Calculate();
}

function schemeComboBox_SelectionChanged(s, e){
	Calculate();
}

function employeeCheckBox_Click(s, e){
	Calculate();
}

function mainWindow_PreviewKeyDown(s, e){
	if(e.Key == 6) //Enter pressed
		Calculate();
}

function loanTextBox_TextChanged(s, e){
	emiTextBox.Text = "";
}

function periodTextBox_TextChanged(s, e){
	emiTextBox.Text = "";
}

function exitMenuItem_Click(s, e){
	mainWindow.Close();
}

function setRateMenuItem_Click(s, e){
	var scb = schemeComboBox;
	var srp = NewPresentation("LoanSetRate.xaml");
	srp.setRateWindow.Title = scb.Text + " Scheme";
	srp.rateTextBox.Text = rates[scb.SelectedIndex];
	srp.setRateWindow.ShowDialog();
	if(srp.rateUpdated)
	{
		rates[scb.SelectedIndex] = srp.rateTextBox.Text;
		Calculate();
	}
}




