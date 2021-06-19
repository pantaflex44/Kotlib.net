//
//  Compression.cs
//
//  Author:
//       Christophe LEMOINE <pantafle@tuta.io>
//
//  Copyright (c) 2021 Christophe LEMOINE
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.IO.Compression;

namespace Kotlib.Core
{

	/// <summary>
	/// Regroupe les méthodes de compression / décompression
	/// </summary>
	public static class Compression
	{

		/// <summary>
		/// Compresse des données
		/// </summary>
		/// <returns>Les données compressées.</returns>
		/// <param name="original">Données originales.</param>
		public static byte[] Compress(byte[] original)
		{
			byte[] datas = { };

			using (var original_stream = new MemoryStream(original)) {
				using (var compressed_stream = new MemoryStream()) {
					using (var compression_stream = new DeflateStream(compressed_stream, CompressionMode.Compress)) {
						original_stream.CopyTo(compression_stream);
						compression_stream.Close();
		
						datas = compressed_stream.ToArray();
					}
				}
			}

			return datas;
		}

		/// <summary>
		/// Décompresse les données
		/// </summary>
		/// <returns>Données décompressées.</returns>
		/// <param name="compressed">Données compressées.</param>
		public static byte[] Decompress(byte[] compressed)
		{
			byte[] datas = { };

			using (var compressed_stream = new MemoryStream(compressed)) {
				using (var decompression_stream = new DeflateStream(compressed_stream, CompressionMode.Decompress)) {
					using (var original_stream = new MemoryStream()) {
						decompression_stream.CopyTo(original_stream);
						decompression_stream.Close();
		
						datas = original_stream.ToArray();
					}
				}
			}

			return datas;
		}

	}

}