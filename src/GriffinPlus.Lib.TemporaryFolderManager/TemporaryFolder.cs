///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// This file is part of the Griffin+ common library suite.
// Project URL: https://github.com/griffinplus/dotnet-libs-temporary-folder-manager
// The source code is licensed under the MIT license.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;

namespace GriffinPlus.Lib
{
	/// <summary>
	/// A folder created by the temporary folder manager.
	/// </summary>
	internal class TemporaryFolder : ITemporaryFolder
	{
		private readonly TemporaryFolderManager mTemporaryFolderManager;
		private readonly string mLockFilePath;
		private readonly FileStream mLockFile;
		private bool mDisposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="TemporaryFolder"/> class.
		/// </summary>
		/// <param name="temporaryFolderManager">The temporary folder manager creating the folder.</param>
		/// <param name="basePath">Path of the temporary base folder.</param>
		internal TemporaryFolder(TemporaryFolderManager temporaryFolderManager, string basePath)
		{
			mTemporaryFolderManager = temporaryFolderManager;
			string processSpecificDirectoryName = $"[TMPDIR] {Guid.NewGuid():D}";
			FolderPath = Path.Combine(basePath, processSpecificDirectoryName);
			mLockFilePath = Path.Combine(basePath, processSpecificDirectoryName + ".lock");
			mLockFile = new FileStream(mLockFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
			Directory.CreateDirectory(FolderPath);
		}

		/// <summary>
		/// Disposes the temporary folder and removes it from disk.
		/// </summary>
		public void Dispose()
		{
			if (!mDisposed)
			{
				try
				{
					Directory.Delete(FolderPath, true);
					mLockFile?.Dispose();
					File.Delete(mLockFilePath);
				}
				catch (Exception)
				{
					// swallow...
				}

				mTemporaryFolderManager.RemoveFolder(this);
				mDisposed = true;
			}
		}

		/// <summary>
		/// Gets the path of the temporary folder.
		/// </summary>
		public string FolderPath { get; }
	}
}
