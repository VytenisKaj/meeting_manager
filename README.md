Meeting manager console application using C# with .NET6

Requirements:

	● Command to create a new meeting. All the meeting data should be stored in a JSON
	file. Application should retain data between restarts. Meeting model should contain
	the following properties:
		○ Name
		○ ResponsiblePerson
		○ Description
		○ Category (Fixed values - CodeMonkey / Hub / Short / TeamBuilding)
		○ Type (Fixed values - Live / InPerson)
		○ StartDate
		○ EndDate
	● Command to delete a meeting. Only the person responsible can delete the meeting.
	● Command to add a person to the meeting.
		○ Command should specify who is being added and at what time.
		○ If a person is already in a meeting which intersects with the one being added,
		a warning message should be given.
		○ Prevent the same person from being added twice.
	● Command to remove a person from the meeting.
		○ If a person is responsible for the meeting, he can not be removed.
	● Command to list all the meetings. Add the following parameters to filter the data:
		○ Filter by description (if the description is “Jono .NET meetas”, searching for
		.NET should return this entry)
		○ Filter by responsible person
		○ Filter by category
		○ Filter by type
		○ Filter by dates (e.g meetings that will happen starting from 2022-01-01 /
		meetings that will happen between 2022-01-01 and 2022-02-01)
		○ Filter by the number of attendees (e.g show meetings that have over 10
		people attending)

Unit test are included.

Due to requirements of only responsible person being able to delete meetings, simple authentification system is included and you may have to login to a specific user
to use a command. It is not required to be logged on for most comamnds though. For simplicity and testing purposes, all user information is stored as plain text(so you do not have to remember it).

Tried to keep syntax as simple as possible, but due to requirements, few things may seem odd at first:
	● Note, that "create" and "ls"(list) commands have arguments, that MUST be seperated by '|', otherwise it will interpreted as a single argument.
	● Due to descriptions of meetings being able to have several words, "ls" command has a strict syntax: <filterType>(<filter>)
	where filterType code must be led by '-' and there cannot be whitespace between filterType and opening '('
	This is made so that filter itself could contain any caracters, including whitespaces. You are also allowed to use multiple filters and connect them using '|'.
	● You can always use "?" or "help" commands to see all comamnds, their syntax and what they do.

The most straightforward(and recommended) way to run this project is to open it using Visual Studio 2022(older versions should work, but I have not tested it).
Alternatively you can download the .zip, unzip it in your desired directory, then, using terminal, navigate to the directory, that has
"meeting_manager.sln" file and running it from there. Make sure, that you have "dotnet" installed, using "dotnet --version", if you get promted by a version number, you are good to go.
	● Run the main project using command "dotnet run --project meeting_manager" 
	● Test can be run using command "dotnet run --project meeting_test" (However it does not seem to work as intended at the moment, I will try to fix it, therefore I suggest using Visual Studio option if you can until then)

Hope you like it :)