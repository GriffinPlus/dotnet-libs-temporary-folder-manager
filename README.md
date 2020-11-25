# Temporary Folder Manager

[![Azure DevOps builds (branch)](https://img.shields.io/azure-devops/build/griffinplus/2f589a5e-e2ab-4c08-bee5-5356db2b2aeb/30/master?label=Build)](https://dev.azure.com/griffinplus/DotNET%20Libraries/_build/latest?definitionId=30&branchName=master)
[![Tests (master)](https://img.shields.io/azure-devops/tests/griffinplus/DotNET%20Libraries/30/master?label=Tests)](https://dev.azure.com/griffinplus/DotNET%20Libraries/_build/latest?definitionId=30&branchName=master)
[![NuGet Version](https://img.shields.io/nuget/v/GriffinPlus.Lib.TemporaryFolderManager.svg?label=NuGet%20Version)](https://www.nuget.org/packages/GriffinPlus.Lib.TemporaryFolderManager)
[![NuGet Downloads](https://img.shields.io/nuget/dt/GriffinPlus.Lib.TemporaryFolderManager.svg?label=NuGet%20Downloads)](https://www.nuget.org/packages/GriffinPlus.Lib.TemporaryFolderManager)

## Overview

The *Temporary Folder Manager* is a helper that comes in handy when juggling around with temporary folders on *.NET*. Temporary folders tend to orphan and accumulate over time wasting disk space. The *Temporary Folder Manager* keeps track of temporary folders and cleans up orphaned folders, if appropriate.

## Supported Platforms

The library is entirely written in C# using .NET Standard 2.0.

Therefore it should work on the following platforms (or higher):
- .NET Framework 4.6.1
- .NET Core 2.0
- .NET 5.0
- Mono 5.4
- Xamarin iOS 10.14
- Xamarin Mac 3.8
- Xamarin Android 8.0
- Universal Windows Platform (UWP) 10.0.16299

The library is tested automatically on the following frameworks and operating systems:
- .NET Framework 4.6.1 (Windows Server 2019)
- .NET Core 2.1 (Windows Server 2019 and Ubuntu 20.04)
- .NET Core 3.1 (Windows Server 2019 and Ubuntu 20.04)
- .NET 5.0  (Windows Server 2019 and Ubuntu 20.04)

## Using

### Step 1: Get/Create the Temporary Folder Manager

You can either create a new instance of the  `TemporaryFolderManager` class passing a path to a base directory where temporary folders and management information are stored or simply call the static property `TemporaryFolderManager.Default` to get access to the default instance working on top of the user's temporary directory. It is safe to use multiple instances of the `TemporaryFolderManager` class on the same base directory as lock files guard the temporary folders and prevent cleaning up folders that are still in use.

### Step 2: Create a temporary folder

To create a temporary folder you only need to call the `CreateTemporaryFolder()` on the manager instance which returns a management object for the temporary folder  (`ITemporaryFolder`).

### Step 3: Use the temporary folder

Work with the temporary folder. The path to the folder can be retrieved via the `FolderPath` property of the management object.

### Step 4: Release the temporary folder

At last you should dispose the management object to delete the temporary folder. If you forget to do this, creating the next `TemporaryFolderManager` instance with the same base directory will clean up orphaned folders.

Nevertheless, to ensure everything is cleaned up properly, you should put your code in a `using` statement as follows:

```csharp
using GriffinPlus.Lib;

// create a temporary folder in the default base directory
// (the folder is deleted automatically when disposing the ITemporaryFolder object)
using (ITemporaryFolder folder = TemporaryFolderManager.Default.CreateTemporaryFolder())
{
    Console.WriteLine("Successfully created temporary folder: {0}", folder.FolderPath);
    // ...
    // use the temporary folder
    // ...
}

// create a temporary folder in a custom base directory (relative to working directory)
// (the folder is deleted automatically when disposing the ITemporaryFolder object)
TemporaryFolderManager manager = new TemporaryFolderManager("Temporary Folders");
using (ITemporaryFolder folder = manager.CreateTemporaryFolder())
{
    Console.WriteLine("Successfully created temporary folder: {0}", folder.FolderPath);
    // ...
    // use the temporary folder
    // ...
}
```
