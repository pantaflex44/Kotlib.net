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

		#region Evénements
		
		/// <summary>
		/// Délégué en charge des événements d'ajout, modification et supppression
		/// </summary>
		/// <param name="item">Elément concené</param>
		public delegate void TransferEventHandler(Transfer item);
		/// <summary>
		/// Un élément a été ajouté
		/// </summary>
		public event TransferEventHandler TransferAddedEvent;
		/// <summary>
		/// Un élément a été modifié
		/// </summary>
		public event TransferEventHandler TransferUpdatedEvent;
		/// <summary>
		/// Un élément a été supprimé
		/// </summary>
		public event TransferEventHandler TransferRemovedEvent;
		
		#endregion
		
		#region Fonctions privées
		
		/// <summary>
		/// Informe qu'un élément a été ajouté
		/// </summary>
		/// <param name="item">Elément concerné</param>
		public void OnTransferAdded(Transfer item)
		{
			if (TransferAddedEvent != null)
				TransferAddedEvent(item);
		}
		
		/// <summary>
		/// Informe qu'un élément a été modifié
		/// </summary>
		/// <param name="item">Elément concerné</param>
		public void OnTransferUpdated(Transfer item)
		{
			if (TransferUpdatedEvent != null)
				TransferUpdatedEvent(item);
		}
		
		/// <summary>
		/// Informe qu'un élément a été supprimé
		/// </summary>
		/// <param name="item">Elément concerné</param>
		public void OnTransferRemoved(Transfer item)
		{
			if (TransferRemovedEvent != null)
				TransferRemovedEvent(item);
		}
		
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
		public decimal PartialAmountAt(Account account, DateTime date, bool addInitialAmount = true)
		{
			var f = Items.Where(a => a.Date.Date <= date.Date && a.FromAccountId.Equals(account.Id)).Select(a => a.Amount).ToList();
			var sf = Math.Abs(f.Sum());
			
			var t = Items.Where(a => a.Date.Date <= date.Date && a.ToAccountId.Equals(account.Id)).Select(a => a.Amount).ToList();
			var st = Math.Abs(t.Sum());
			
			var amts = new decimal[] { (addInitialAmount ? account.InitialAmount : 0m), -sf, st };
			return amts.Sum();
		}
		
		/// <summary>
		/// Retourne l'élément à la position <c>index</c>
		/// </summary>
		/// <param name="index">Position de l'élément.</param>
		public new Transfer this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				OnTransferRemoved(base[index]);
					
				base[index] = value;
				
				base[index].UpdatedEvent += (sender, e) => OnTransferUpdated(base[index]);
				OnTransferAdded(base[index]);
			}
		}
		
		/// <summary>
		/// Supprime l'élément de la liste à la position spécifié
		/// </summary>
		/// <param name="index">Position de l'élément.</param>
		public new void RemoveAt(int index)
		{
			if (index >= 0 && index < base.Count)
			{
				OnTransferRemoved(base[index]);
				base.RemoveAt(index);
			}
		}

		/// <summary>
		/// Vide la liste de ses éléments
		/// </summary>
		public new void Clear()
		{
			foreach (var e in Items)
			{
				OnTransferRemoved(e);
			}

			base.Clear();
		}
		
		/// <summary>
		/// Supprime un élément de la liste
		/// </summary>
		/// <returns>true, l'élément est supprimé, sinon, false.</returns>
		/// <param name="item">Elément à supprimer.</param>
		public new bool Remove(Transfer item)
		{
			if (base.IndexOf(item) > -1)
				OnTransferRemoved(item);
			
			return base.Remove(item);
		}
		
		/// <summary>
		/// Ajoute un élément à la liste
		/// </summary>
		/// <param name="item">Item.</param>
		public new void Add(Transfer item)
		{
			OnTransferAdded(item);
			item.UpdatedEvent += (sender, e) => OnTransferUpdated(item);
			base.Add(item);
		}
		
		/// <summary>
		/// Insert un élément à la position spécifiée
		/// </summary>
		/// <param name="index">Position d'insertion.</param>
		/// <param name="item">Elément à insérer.</param>
		public new void Insert(int index, Transfer item)
		{
			OnTransferAdded(item);
			item.UpdatedEvent += (sender, e) => OnTransferUpdated(item);
			base.Insert(index, item);
		}
		
		
		
	}

}
