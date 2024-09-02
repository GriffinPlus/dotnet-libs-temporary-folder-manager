///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// This file is part of the Griffin+ common library suite.
// Project URL: https://github.com/griffinplus/dotnet-libs-temporary-folder-manager
// The source code is licensed under the MIT license.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Runtime.CompilerServices;

using Xunit;

namespace GriffinPlus.Lib;

/// <summary>
/// Unit tests targeting the <see cref="TemporaryFolder"/> class.
/// </summary>
public class TemporaryFolderTests
{
	/// <summary>
	/// Tests creating a new temporary folder manager using the default temporary path as base
	/// directory for temporary folders.
	/// </summary>
	[Fact]
	public void Create_Default()
	{
		var manager = new TemporaryFolderManager();
		Assert.True(Path.IsPathRooted(manager.TemporaryFolderPath));
		Assert.StartsWith(Path.GetTempPath(), manager.TemporaryFolderPath);
	}

	/// <summary>
	/// Tests creating a new temporary folder manager using a specific directory as base directory
	/// for temporary folders.
	/// </summary>
	[Fact]
	public void Create_SpecificFolder()
	{
		string tempFolder = Path.Combine(Environment.CurrentDirectory, "TEMP");
		var manager = new TemporaryFolderManager(tempFolder);
		Assert.True(Path.IsPathRooted(manager.TemporaryFolderPath));
		Assert.StartsWith(manager.TemporaryFolderPath, tempFolder);
	}

	/// <summary>
	/// Tests accessing the default temporary folder manager provided by the <see cref="TemporaryFolderManager.Default"/>
	/// property.
	/// </summary>
	[Fact]
	public void GetDefault()
	{
		var manager = TemporaryFolderManager.Default;
		Assert.True(Path.IsPathRooted(manager.TemporaryFolderPath));
		Assert.StartsWith(Path.GetTempPath(), manager.TemporaryFolderPath);
	}

	/// <summary>
	/// Tests whether getting the default temporary folder manager provided by the <see cref="TemporaryFolderManager.Default"/>
	/// property twice returns the same instance.
	/// </summary>
	[Fact]
	public void GetDefault_Twice()
	{
		var manager1 = TemporaryFolderManager.Default;
		var manager2 = TemporaryFolderManager.Default;
		Assert.Same(manager1, manager2);
	}

	/// <summary>
	/// Tests creating a new temporary folder.
	/// </summary>
	[Fact]
	public void CreateTemporaryFolder_CreateOnly()
	{
		var manager = new TemporaryFolderManager();
		ITemporaryFolder folder = manager.CreateTemporaryFolder();
		Assert.StartsWith(manager.TemporaryFolderPath, folder.FolderPath);
	}

	/// <summary>
	/// Tests creating a new temporary folder and adding some content.
	/// </summary>
	[Fact]
	public void CreateTemporaryFolder_CreateAndAddContent()
	{
		var manager = new TemporaryFolderManager();
		ITemporaryFolder folder = manager.CreateTemporaryFolder();
		Assert.StartsWith(manager.TemporaryFolderPath, folder.FolderPath);

		// write a file to the temporary folder
		string filePath = Path.Combine(folder.FolderPath, "file.txt");
		File.WriteAllText(filePath, "Test");

		// add a directory to the temporary folder
		string directoryPath = Path.Combine(folder.FolderPath, "Test");
		Directory.CreateDirectory(directoryPath);

		// dispose temporary folder
		Assert.True(Directory.Exists(folder.FolderPath));
		folder.Dispose();
		Assert.False(Directory.Exists(folder.FolderPath));
	}

	/// <summary>
	/// Tests running two temporary folder managers on the same base directory at the same time.
	/// It is expected that the second manager does not destroy data managed by the first manager.
	/// </summary>
	[Fact]
	public void MultipleManagers()
	{
		// create a temporary folder and drop a file into it...
		var manager1 = new TemporaryFolderManager();
		ITemporaryFolder folder1 = manager1.CreateTemporaryFolder();
		string filePath1 = Path.Combine(folder1.FolderPath, "file1.txt");
		File.WriteAllText(filePath1, "Test");

		// create another temporary folder manager
		var manager2 = new TemporaryFolderManager();
		ITemporaryFolder folder2 = manager2.CreateTemporaryFolder();
		string filePath2 = Path.Combine(folder2.FolderPath, "file2.txt");
		File.WriteAllText(filePath2, "Test");

		// both managers should still have their files as both managers are alive
		// (manager 2 must not remove directories still managed by manager 1)
		Assert.True(File.Exists(filePath1));
		Assert.True(File.Exists(filePath2));

		// keep managers alive, otherwise the finalizer might clean up and remove directories
		GC.KeepAlive(manager1);
		GC.KeepAlive(manager2);
	}

	/// <summary>
	/// Tests whether orphaned directories/files and lock files are removed automatically when starting a new
	/// temporary folder manager.
	/// </summary>
	[Fact]
	public void CleanupAfterRestart()
	{
		const string temporaryFolderBaseName = "Temporary Folders";

		// create a temporary folder and drop a file into it...
		string dirPath1 = CreateTempManagerAndFolder(temporaryFolderBaseName);
		string filePath1 = Path.Combine(dirPath1, "file1.txt");
		File.WriteAllText(filePath1, "Test");

		// create another temporary folder manager
		string dirPath2 = CreateTempManagerAndFolder(temporaryFolderBaseName);
		string filePath2 = Path.Combine(dirPath2, "file2.txt");
		File.WriteAllText(filePath2, "Test");

		// kick both managers out of memory (without disposing, would otherwise clean up properly)
		GC.Collect(2, GCCollectionMode.Forced, true);
		GC.WaitForPendingFinalizers();

		// the files, the folders and the lock files should still be there...
		Assert.True(File.Exists(filePath1));
		Assert.True(File.Exists(filePath2));
		Assert.True(Directory.Exists(dirPath1));
		Assert.True(Directory.Exists(dirPath2));
		Assert.True(File.Exists(dirPath1 + ".lock"));
		Assert.True(File.Exists(dirPath2 + ".lock"));

		// starting a new folder manager instance should clean up orphaned directories and lock files
		_ = new TemporaryFolderManager(temporaryFolderBaseName);
		Assert.False(Directory.Exists(dirPath1));
		Assert.False(Directory.Exists(dirPath2));
		Assert.False(File.Exists(dirPath1 + ".lock"));
		Assert.False(File.Exists(dirPath2 + ".lock"));
	}

	/// <summary>
	/// Helper: Creates a new temporary folder manager and a new temporary folder.
	/// </summary>
	/// <param name="temporaryFolderBaseName">Name of the base directory for temporary folders (relative to working directory).</param>
	/// <returns>Full path of the temporary folder.</returns>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static string CreateTempManagerAndFolder(string temporaryFolderBaseName)
	{
		var manager = new TemporaryFolderManager(temporaryFolderBaseName);
		ITemporaryFolder folder = manager.CreateTemporaryFolder();
		return folder.FolderPath;
	}
}
