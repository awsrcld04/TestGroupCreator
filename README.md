
# TestGroupCreator

DESCRIPTION: 
- Create one or more test groups.

> NOTES: Code in initial repo commit is from 2011. 

## Requirements:

Operating System Requirements:
- Windows Server 2003 or higher (32-bit)
- Windows Server 2008 or higher (32-bit)

Additional software requirements:
Microsoft .NET Framework v3.5

Additional requirements:
Administrative access is required to perform operations by TestGroupCreator


## Operation and Configuration:

Command-line parameters:
- run (Required parameter)
- name (specify the name prefix for the test groups)
- num (specify the number of test groups to create)

Examples:
TestGroupCreator -run -name:TestGroup -num:10 
