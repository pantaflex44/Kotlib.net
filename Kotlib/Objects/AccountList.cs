//
//  AccountList.cs
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
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace Kotlib.Objects
{

	/// <summary>
	/// Liste d'éléments bancaires
	/// </summary>
	public class AccountList : ObjectList<Account>
	{

		#region Evénements
		
		/// <summary>
		/// Délégué en charge des événements d'ajout, modification et supppression
		/// </summary>
		/// <param name="account">Elément concené</param>
		public delegate void AccountEventHandler(Account account);
		/// <summary>
		/// Un élément a été ajouté
		/// </summary>
		public event AccountEventHandler AccountAddedEvent;
		/// <summary>
		/// Un élément a été modifié
		/// </summary>
		public event AccountEventHandler AccountUpdatedEvent;
		/// <summary>
		/// Un élément a été supprimé
		/// </summary>
		public event AccountEventHandler AccountRemovedEvent;
		
		#endregion
		
		#region Fonctions privées
		
		/// <summary>
		/// Informe qu'un élément a été ajouté
		/// </summary>
		/// <param name="account">Elément concerné</param>
		public void OnAccountAdded(Account account)
		{
			if (AccountAddedEvent != null)
				AccountAddedEvent(account);
		}
		
		/// <summary>
		/// Informe qu'un élément a été modifié
		/// </summary>
		/// <param name="account">Elément concerné</param>
		public void OnAccountUpdated(Account account)
		{
			if (AccountUpdatedEvent != null)
				AccountUpdatedEvent(account);
		}
		
		/// <summary>
		/// Informe qu'un élément a été supprimé
		/// </summary>
		/// <param name="account">Elément concerné</param>
		public void OnAccountRemoved(Account account)
		{
			if (AccountRemovedEvent != null)
				AccountRemovedEvent(account);
		}
		
		#endregion
		
		#region Propriétés publiques

		/// <summary>
		/// Retourne une liste vide
		/// </summary>
		/// <value>Liste vide.</value>
		[XmlIgnore]
		public static AccountList Empty
		{
			get
			{
				return new AccountList();
			}
		}
		
		private TransferList _transfers = null;
		/// <summary>
		/// Liste des transferts
		/// </summary>
		/// <value>Liste des transferts.</value>
		[XmlArray(ElementName = "Transfers")]
		[XmlArrayItem(ElementName = "Transfer")]
		public TransferList Transfers
		{
			get { return _transfers; }
			set
			{
				if (value != null && value != _transfers)
				{
					if (_transfers != null)
						_transfers.UpdatedEvent -= OnUpdated;

					_transfers = value;
					_transfers.UpdatedEvent += OnUpdated;
				}
			}
		}

		#endregion

		/// <summary>
		/// Constructeur
		/// </summary>
		public AccountList()
			: base()
		{
			Transfers = TransferList.Empty;
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="items">Liste à charger</param>
		public AccountList(IEnumerable<Account> items)
			: base(items)
		{
			Transfers = TransferList.Empty;
		}

		/// <summary>
		/// Retourne le premier élément bancaire trouvé possédant l'identifiant unique passé en paramètre
		/// </summary>
		/// <returns>Elément bancaire trouvé.</returns>
		/// <param name="id">Identifiant unique.</param>
		public Account GetById(Guid id)
		{
			return this.ToList().FirstOrDefault(a => a.Id.Equals(id));
		}
		
		/// <summary>
		/// Retournel'élément à la position <c>index</c>
		/// </summary>
		/// <param name="index">Position de l'élément.</param>
		public new Account this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				Transfers.RemoveAll(a => a.FromAccountId.Equals(base[index].Id) || a.ToAccountId.Equals(base[index].Id));
				
				OnAccountRemoved(base[index]);
					
				base[index] = value;
				
				base[index].UpdatedEvent += (sender, e) => OnAccountUpdated(base[index]);
				OnAccountAdded(base[index]);
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
				Transfers.RemoveAll(a => a.FromAccountId.Equals(base[index].Id) || a.ToAccountId.Equals(base[index].Id));

				OnAccountRemoved(base[index]);
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
				Transfers.RemoveAll(a => a.FromAccountId.Equals(e.Id) || a.ToAccountId.Equals(e.Id));
				OnAccountRemoved(e);
			}

			base.Clear();
		}
		
		/// <summary>
		/// Supprime un élément de la liste
		/// </summary>
		/// <returns>true, l'élément est supprimé, sinon, false.</returns>
		/// <param name="item">Elément à supprimer.</param>
		public new bool Remove(Account item)
		{
			if (base.IndexOf(item) > -1)
			{
				Transfers.RemoveAll(a => a.FromAccountId.Equals(item.Id) || a.ToAccountId.Equals(item.Id));
				OnAccountRemoved(item);
			}
			return base.Remove(item);
		}
		
		/// <summary>
		/// Ajoute un élément à la liste
		/// </summary>
		/// <param name="item">Item.</param>
		public new void Add(Account item)
		{
			OnAccountAdded(item);
			item.UpdatedEvent += (sender, e) => OnAccountUpdated(item);
			base.Add(item);
		}
		
		/// <summary>
		/// Insert un élément à la position spécifiée
		/// </summary>
		/// <param name="index">Position d'insertion.</param>
		/// <param name="item">Elément à insérer.</param>
		public new void Insert(int index, Account item)
		{
			OnAccountAdded(item);
			item.UpdatedEvent += (sender, e) => OnAccountUpdated(item);
			base.Insert(index, item);
		}

		/// <summary>
		/// Retourne la liste des éléments bancaires disponibles
		/// </summary>
		/// <returns>Liste des éléments bancaires disponibles</returns>
		public static List<Tuple<string, string, Type>> GetAvaillableAccounts()
		{
			var l = new List<Tuple<string, string, Type>>();

			foreach (var da in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var at in da.GetTypes())
				{
					if (typeof(Account).IsAssignableFrom(at))
					{
						if (Attribute.IsDefined(at, typeof(DisplayNameAttribute)) &&
						    Attribute.IsDefined(at, typeof(DescriptionAttribute)))
						{

							var dname = (Attribute.GetCustomAttribute(at, typeof(DisplayNameAttribute)) as DisplayNameAttribute).DisplayName;
							var desc = (Attribute.GetCustomAttribute(at, typeof(DescriptionAttribute)) as DescriptionAttribute).Description;
							l.Add(new Tuple<string, string, Type>(dname, desc, at));

						}
					}
				}
			}

			return l;
		}

		/// <summary>
		/// Nettoie la liste des transferts.
		/// Supprime tous les transferts ayant un compte émetteur ou destinataure inconnu.
		/// </summary>
		public void CleanTransfers()
		{
			var actIdList = Items.Select(a => a.Id).ToList();
			Transfers.RemoveAll(a => !actIdList.Contains(a.FromAccountId) || !actIdList.Contains(a.ToAccountId));
		}

	}


}
