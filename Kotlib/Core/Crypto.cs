//
//  Crypto.cs
//
//  Author:
//       Christophe LEMOINE <pantaflex@tuta.io>
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

using System.Security.Cryptography;
using System.Text;

namespace Kotlib.Core
{

	/// <summary>
	/// Regroupe les méthodes de cryptage et décryptage
	/// </summary>
	public static class Crypto
	{

		/// <summary>
		/// Cryptage des données
		/// </summary>
		/// <returns>Données cryptées.</returns>
		/// <param name="original">Données originales.</param>
		/// <param name="password">Mot de passe.</param>
		public static byte[] Encrypt(byte[] original, string password)
		{
			var md5 = new MD5CryptoServiceProvider();
			byte[] key = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
			md5.Clear();

			var des = new TripleDESCryptoServiceProvider
			{
				Key = key,
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			};

			var provider = des.CreateEncryptor();
			byte[] datas = provider.TransformFinalBlock(original, 0, original.Length);
			des.Clear();

			return datas;
		}

		/// <summary>
		/// Décryptage des données
		/// </summary>
		/// <returns>Données décryptées.</returns>
		/// <param name="crypted">Données cryptées.</param>
		/// <param name="password">Mot de passe.</param>
		public static byte[] Decrypt(byte[] crypted, string password)
		{
			var md5 = new MD5CryptoServiceProvider();
			byte[] key = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
			md5.Clear();

			var des = new TripleDESCryptoServiceProvider
			{
				Key = key,
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			};

			var provider = des.CreateDecryptor();
			byte[] datas = provider.TransformFinalBlock(crypted, 0, crypted.Length);
			des.Clear();

			return datas;
		}

	}

}