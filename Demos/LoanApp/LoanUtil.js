function MonthlyInstallment(p, n, r){
	return p * (1 + n * r / 100) / (12 * n);
}
