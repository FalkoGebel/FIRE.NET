# FIRE.NET - Just another FIRE calculator

Current version: v3.0.0

## Features
- Shows the remaining amount for the months in a chart and the probability of default based on the choosen parameters
- Changing any of the following parameters will update the dependent parameters, the chart and the probability of default:
	- Starting amount
	- Annual inflation rate
	- Annual return
	- Annual volatility
	- Cash flow periods with starting month, ending month and amount of the cash flow
- Pressing button **_Calculate_** will also update the chart and the probability of default
- When using an annual return and an annual volatility, there will be a calculation of possible outcomes based on 100,000 simulations - the chart will show three lines: the average values of the top 1,000 outcomes, the average of the median 1,000 outcomes and the average values of the lowest 1,000 outcomes

![FIRE.NET](/README-Images/FIRE.NET.png)

- The monthly cash flow amounts over all the time are also shown in a separate table

![Monthly cashflow amounts](/README-Images/MonthlyCashflowAmounts.png)
	 
## Technologies used
- C# / .NET / WPF
- MVVM design pattern
- [OxyPlot](https://github.com/oxyplot/oxyplot) for charting
- MSTest with [FluentAssertion](https://github.com/fluentassertions/fluentassertions) for unit testing

## How to use
1. Download the [latest release](https://github.com/FalkoGebel/FIRE.NET/releases/latest) from the [releases page](https://github.com/FalkoGebel/FIRE.NET/releases)
2. Extract the downloaded ZIP file
3. Open the solution in Visual Studio
4. Build the solution
5. Run the application