///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// This file is part of the Griffin+ common library suite.
// Project URL: https://github.com/griffinplus/dotnet-libs-temporary-folder-manager
// The source code is licensed under the MIT license.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

// ReSharper disable UnusedMemberInSuper.Global

namespace GriffinPlus.Lib
{
	/// <summary>
	/// Interface of a class providing temporary folders.
	/// </summary>
	public interface ITemporaryFolderManager : IDisposable
	{
		/// <summary>
		/// Creates a temporary folder.
		/// </summary>
		/// <returns>The temporary folder.</returns>
		ITemporaryFolder CreateTemporaryFolder();
	}
}
