//
//  TransferList.cs
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
	/// Liste des transferts
	/// </summary>
	public class TransferList : ObjectList<Transfer>
	{

		#region Fonctions privées
		#endregion

		#region Propriétés publiques

		/// <summary>
		/// Retourne une liste vide
		/// </summary>
		/// <value>Liste vide.</value>
		public static TransferList Empty
		{
			get
			{
				return new TransferList();
			}
		}

		/// <summary>
		/// Retourne une liste des opérations par défaut
		/// </summary>
		public static TransferList Defaults
		{
			get
			{
				return TransferList.Empty;
			}
		}

		#endregion

		/// <summary>
		/// Constructeur
		/// </summary>
		public TransferList()
			: base()
		{
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="items">Liste à charger</param>
		public TransferList(IEnumerable<Transfer> items)
			: base(items)
		{
		}

		/// <summary>
		/// Retourne le premièr transfert trouvé possédant l'identifiant unique passé en paramètre
		/// </summary>
		/// <returns>Transfert trouvé.</returns>
		/// <param name="id">Identifiant unique.</param>
		public Transfer GetById(Guid id)
		{
			return this.ToList().FirstOrDefault(a => a.Id.Equals(id));
		}

		/// <summary>
		/// Retourne le solde des transferts à la date souhaité
		/// </summary>
		/// <param name="account">Element bancaire associé aux transferts</param>
		/// <param name="date">Date souhaitée</param>
		/// <param name="addInitialAmount"><c>true</c>, ajoute le solde initial, sinon, <c>false</c></param>
		/// <returns>Solde</returns>
		public double PartialAmountAt(Account account, DateTime date, bool addInitialAmount = true)
		{
			var f = Items.Where(a => a.Date <= date && a.FromAccountId.Equals(account.Id)).Select(a => a.Amount).ToList();
			var sf = Math.Abs(f.Sum());
			
			var t = Items.Where(a => a.Date <= date && a.ToAccountId.Equals(account.Id)).Select(a => a.Amount).ToList();
			var st = Math.Abs(t.Sum());
			
			var amts = new double[] { (addInitialAmount ? account.InitialAmount : 0.0d), -sf, st };
			return amts.Sum();
		}
		
	}

}
