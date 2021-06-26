//
//  ThirdpartyList.cs
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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Kotlib.Objects
{

	/// <summary>
	/// Liste de tiers
	/// </summary>
	public class ThirdpartyList : ObjectList<Identity>
	{

		#region Propriétés publiques

		/// <summary>
		/// Retourne une liste vide
		/// </summary>
		/// <value>Liste vide.</value>
		public static ThirdpartyList Empty
		{
			get
			{
				return new ThirdpartyList();
			}
		}

		#endregion

		/// <summary>
		/// Constructeur
		/// </summary>
		public ThirdpartyList()
			: base()
		{
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="items">Liste à charger</param>
		public ThirdpartyList(IEnumerable<Identity> items)
			: base(items)
		{
		}

		/// <summary>
		/// Retourne le premier tiers trouvé possédant l'identifiant unique passé en paramètre
		/// </summary>
		/// <returns>Tiers trouvé.</returns>
		/// <param name="id">Identifiant unique.</param>
		public Identity GetById(Guid id)
		{
			return this.ToList().FirstOrDefault(a => a.Id.Equals(id));
		}

	}

}
