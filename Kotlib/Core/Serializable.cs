//
//  Serializable.cs
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

using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Kotlib.Core
{

	/// <summary>
	/// Méthodes pour la sérialisation et la désérialisation d'un objet
	/// </summary>
	public class Serializable
	{

		/// <summary>
		/// Serialisation d'un objet
		/// </summary>
		/// <returns>Tableau de données représentant un objet sérialisé.</returns>
		public byte[] Serialize()
		{
			byte[] datas = { };

			using (var ms = new MemoryStream())
			{
				var xs = new XmlSerializer(this.GetType());

				var ns = new XmlSerializerNamespaces();
				ns.Add("", "");

				xs.Serialize(ms, this, ns);

				ms.Seek(0, SeekOrigin.Begin);
				datas = new byte[ms.Length];
				int count = ms.Read(datas, 0, datas.Length);
			}

			return datas;
		}

		/// <summary>
		/// Désérialisation d'un objet
		/// </summary>
		/// <returns>Objet reconstitué.</returns>
		/// <param name="datas">Données à sérialisées.</param>
		/// <typeparam name="T">Type d'objet à reconstituer.</typeparam>
		public static T Deserialize<T>(byte[] datas)
		{
			using (var ms = new MemoryStream(datas))
			{
				var xs = new XmlSerializer(typeof(T));
				return (T)xs.Deserialize(ms);
			}
		}

	}

}
