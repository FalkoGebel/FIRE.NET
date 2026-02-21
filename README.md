# FIRE.NET - Just another FIRE calculator

Current version: v1.2.1

## Features
- Shows the remaining amount for the months in a chart based on the choosen parameters
- Changing any of the following parameters will update the dependent parameters and the chart:
	- Starting amount
	- Monthly withdrawal amount
	- Annual withdrawal amount
	- Starting month
	- Ending month
	- Number of months
	- Annual inflation rate
	- Annual return

![FIRE.NET](/README-Images/FIRE.NET.png)
	 
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