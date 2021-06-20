//
//  OperationList.cs
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
using System.Collections.Generic;
using System.Linq;

namespace Kotlib.Objects
{

	/// <summary>
	/// Liste des opérations
	/// </summary>
	public class OperationList : ObjectList<Operation>
	{

		#region Fonctions privées
		#endregion

		#region Propriétés publiques

		/// <summary>
		/// Retourne une liste vide
		/// </summary>
		/// <value>Liste vide.</value>
		public static OperationList Empty
		{
			get
			{
				return new OperationList();
			}
		}

		/// <summary>
		/// Retourne une liste des opérations par défaut
		/// </summary>
		public static OperationList Defaults
		{
			get
			{
				return OperationList.Empty;
			}
		}

		#endregion

		/// <summary>
		/// Constructeur
		/// </summary>
		public OperationList()
			: base()
		{
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="items">Liste à charger</param>
		public OperationList(IEnumerable<Operation> items)
			: base(items)
		{
		}

		/// <summary>
		/// Retourne la première opération trouvée possédant l'identifiant unique passé en paramètre
		/// </summary>
		/// <returns>Opération trouvée.</returns>
		/// <param name="id">Identifiant unique.</param>
		public Operation GetById(Guid id)
		{
			return this.ToList().FirstOrDefault(a => a.Id.Equals(id));
		}

	}

}
