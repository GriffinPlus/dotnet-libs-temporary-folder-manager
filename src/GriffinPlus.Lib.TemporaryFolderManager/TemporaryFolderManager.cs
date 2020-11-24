///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// This file is part of the Griffin+ common library suite.
// Project URL: https://github.com/griffinplus/dotnet-libs-temporary-folder-manager
// The source code is licensed under the MIT license.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace GriffinPlus.Lib
{
	/// <summary>
	/// A manager for creating and maintaining temporary folders (thread-safe).
	/// </summary>
	public class TemporaryFolderManager : ITemporaryFolderManager
	{
		private static readonly Regex sLockFileRegex = new Regex(@"^\[TMPDIR\] (?<guid>[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})$", RegexOptions.Compiled);
		private static readonly Lazy<TemporaryFolderManager> sDefault = new Lazy<TemporaryFolderManager>(() => new TemporaryFolderManager(), LazyThreadSafetyMode.ExecutionAndPublication);
		private readonly List<TemporaryFolder> mFolders;

		/// <summary>
		/// Initializes a new instance of the <see cref="TemporaryFolderManager"/> class.
		/// </summary>
		/// <param name="path">Path of the base folder temporary folders are created beneath (may contain environment variables).</param>
		public TemporaryFolderManager(string path = null)
		{
			if (path == null) path = Path.Combine(Path.GetTempPath(), "Griffin+ Temporary Folders");
			TemporaryFolderPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables(path));
			Directory.CreateDirectory(TemporaryFolderPath);
			mFolders = new List<TemporaryFolder>();
			CleanupOrphanedFolders();
		}

		/// <summary>
		/// Disposes the current object releasing managed and native resources.
		/// </summary>
		public void Dispose()
		{
			lock (mFolders)
			{
				// dispose folder objects to remove them from disk
				foreach (var folder in mFolders) {
					folder.Dispose();
				}

				mFolders.Clear();
			}
		}

		/// <summary>
		/// Gets the default temporary folder manager.
		/// </summary>
		public static TemporaryFolderManager Default => sDefault.Value;

		/// <summary>
		/// Gets the base path of the temporary directories.
		/// </summary>
		internal string TemporaryFolderPath { get; }

		/// <summary>
		/// Creates a new temporary folder.
		/// </summary>
		/// <returns>The created temporary folder.</returns>
		public ITemporaryFolder CreateTemporaryFolder()
		{
			lock (mFolders)
			{
				TemporaryFolder folder = new TemporaryFolder(this, TemporaryFolderPath);
				mFolders.Add(folder);
				return folder;
			}
		}

		/// <summary>
		/// Removes the specified temporary folder.
		/// </summary>
		/// <param name="folder">Folder to remove.</param>
		internal void RemoveFolder(TemporaryFolder folder)
		{
			lock (mFolders)
			{
				mFolders.Remove(folder);
			}
		}

		/// <summary>
		/// Scans the base directory for orphaned temporary folders and tries to remove them.
		/// </summary>
		public void CleanupOrphanedFolders()
		{
			try
			{
				foreach (string directoryPath in Directory.GetDirectories(TemporaryFolderPath))
				{
					string directoryName = Path.GetFileName(directoryPath);
					if (directoryName == null) continue;
					Match match = sLockFileRegex.Match(directoryName);
					if (match.Success)
					{
						string folderLockFilePath = Path.Combine(TemporaryFolderPath, directoryName + ".lock");
						try
						{
							// try to open the folder lock file
							if (File.Exists(folderLockFilePath)) {
								using (File.OpenRead(folderLockFilePath)) { }
							}

							// opening the folder lock file succeeded
							// => directory is not in use any more
							// => try to remove the directory
							Directory.Delete(directoryPath, true);

							// delete the folder lock file at lat
							File.Delete(folderLockFilePath);
						}
						catch
						{
							// most probably the lock file is in use
							// => swallow, we must not remove any files...
						}
					}
				}
			}
			catch
			{
				// some error regarding the temporary directory itself occurred
				// => swallow
			}
		}
	}
}
