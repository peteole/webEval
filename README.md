# webEval
Grading tool for websites written for school projects in information technology classes.
## installation
Depends on https://html-agility-pack.net . To build it, create a new c# project and paste the code. Go to your project folder and type the following commands to add the required software sources to the project:
```bash
dotnet add package HtmlAgilityPack --version 1.11.17
dotnet add package ExCSS --version 3.0.0
```
## features
Read a folder filled with website folders which shall be evaluated. Saves a detailed grading file in each folder and a summary including all results in the main folder. 
It takes into account the following parameters:
1. The amount of certain html elements used
2. The amount of valid css rules
3. The amount of valid css selectors
4. html validation
5. css validation
## How to use it
Go to the project folder, open a terminal and type
```bash
dotnet run
```
Next, enter the file path which is to be evaluated.
