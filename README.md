# TradingReport

Axpo's technical challenge 

Program execution:

1- Set configuration values
The user will be available to input the path where files will be created, as well as the interval (in minutes) in which the execution will be done.
The log files will always be created on a output folder in the directory the solution is being executed.

Values can also be taken from app.config file

2 -Set mode of execution
The user can choose to execute the program in either automatic or manual mode

	Automatic Mode: The program will run asynchronous on the interval set previously by the user, no input is required and the execution will continue until closure or if an unexpected error occurs.
	
	Manual Mode: The program will run synchronous as the user will be expected to input the date to fetch power trades, in this mode the timer resets after receiving and validating the date. The program can be closed by the user or by an unexpected error.
	
3- The program can be exited by the user on input using the 'Enter' key


Notes:

Path validation will be done on input, validation will be done on the format, write permissions and existence. If a path is valid and doesn't not exist the user will be promted to choose if they want to create a directory on that location, it will work recursively to also create all needed parents.

Interval validation will be done on input, valid values are integers that are greater or equal than 1, all other values will be rejected.

Date validation will be done only on manual mode. On automatic the date is taken by the system, simulating an input from another program or API
