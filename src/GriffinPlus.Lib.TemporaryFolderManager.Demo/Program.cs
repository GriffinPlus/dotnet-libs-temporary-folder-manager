///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// This file is part of the Griffin+ common library suite.
// Project URL: https://github.com/griffinplus/dotnet-libs-temporary-folder-manager
// The source code is licensed under the MIT license.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using GriffinPlus.Lib;

namespace TemporaryFolderManagerDemo
{
	internal class Program
	{
		private static void Main(string[] args)
		{
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

			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
		}
	}
}
