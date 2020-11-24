///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// This file is part of the Griffin+ common library suite.
// Project URL: https://github.com/griffinplus/dotnet-libs-temporary-folder-manager
// The source code is licensed under the MIT license.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

namespace GriffinPlus.Lib
{
	/// <summary>
	/// Interface of a class representing a temporary folder.
	/// </summary>
	public interface ITemporaryFolder : IDisposable
	{
		/// <summary>
		/// Gets the path of the temporary folder.
		/// </summary>
		string FolderPath { get; }
	}
}
