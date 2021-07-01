//
//  PaytypeList.cs
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
using System.ComponentModel;

namespace Kotlib.Objects
{

	/// <summary>
	/// Liste de moyens de paiements
	/// </summary>
	public class PaytypeList : ObjectList<Paytype>
	{

		#region Evénements
		
		/// <summary>
		/// Délégué en charge des événements d'ajout, modification et supppression
		/// </summary>
		/// <param name="item">Elément concené</param>
		public delegate void PaytypeEventHandler(Paytype item);
		/// <summary>
		/// Un élément a été ajouté
		/// </summary>
		public event PaytypeEventHandler PaytypeAddedEvent;
		/// <summary>
		/// Un élément a été modifié
		/// </summary>
		public event PaytypeEventHandler PaytypeUpdatedEvent;
		/// <summary>
		/// Un élément a été supprimé
		/// </summary>
		public event PaytypeEventHandler PaytypeRemovedEvent;
		
		#endregion
		
		#region Fonctions privées
		
		/// <summary>
		/// Informe qu'un élément a été ajouté
		/// </summary>
		/// <param name="item">Elément concerné</param>
		public void OnPaytypeAdded(Paytype item)
		{
			if (PaytypeAddedEvent != null)
				PaytypeAddedEvent(item);
		}
		
		/// <summary>
		/// Informe qu'un élément a été modifié
		/// </summary>
		/// <param name="item">Elément concerné</param>
		public void OnPaytypeUpdated(Paytype item)
		{
			if (PaytypeUpdatedEvent != null)
				PaytypeUpdatedEvent(item);
		}
		
		/// <summary>
		/// Informe qu'un élément a été supprimé
		/// </summary>
		/// <param name="item">Elément concerné</param>
		public void OnPaytypeRemoved(Paytype item)
		{
			if (PaytypeRemovedEvent != null)
				PaytypeRemovedEvent(item);
		}
		
		#endregion

		#region Propriétés publiques

		/// <summary>
		/// Retourne une liste vide
		/// </summary>
		/// <value>Liste vide.</value>
		public static PaytypeList Empty
		{
			get
			{
				return new PaytypeList();
			}
		}

		/// <summary>
		/// Retourne une liste de moyens par défaut
		/// </summary>
		public static PaytypeList Defaults
		{
			get
			{
				return new PaytypeList(from item in GetAvaillablePaytypes()
					                      select (Paytype)Activator.CreateInstance(item.Item3, item.Item1));
			}
		}

		#endregion

		/// <summary>
		/// Constructeur
		/// </summary>
		public PaytypeList()
			: base()
		{
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="items">Liste à charger</param>
		public PaytypeList(IEnumerable<Paytype> items)
			: base(items)
		{
		}

		/// <summary>
		/// Retourne le premier moyen de paiement trouvé possédant l'identifiant unique passé en paramètre
		/// </summary>
		/// <returns>Moyen de paiement trouvé.</returns>
		/// <param name="id">Identifiant unique.</param>
		public Paytype GetById(Guid id)
		{
			return this.ToList().FirstOrDefault(a => a.Id.Equals(id));
		}

		/// <summary>
		/// Retourne la liste des moyens de paiements et d'encaissements disponibles
		/// </summary>
		/// <returns>Liste des moyens disponibles</returns>
		public static List<Tuple<string, string, Type>> GetAvaillablePaytypes()
		{
			return (from da in AppDomain.CurrentDomain.GetAssemblies()
			        from at in da.GetTypes()
			        where typeof(Paytype).IsAssignableFrom(at) && !at.Equals(typeof(Paytype)) && !at.Equals(typeof(Collection))
			        where Attribute.IsDefined(at, typeof(DisplayNameAttribute)) && Attribute.IsDefined(at, typeof(DescriptionAttribute))
			        select new Tuple<string, string, Type>((Attribute.GetCustomAttribute(at, typeof(DisplayNameAttribute)) as DisplayNameAttribute).DisplayName, (Attribute.GetCustomAttribute(at, typeof(DescriptionAttribute)) as DescriptionAttribute).Description, at)).ToList();
		}

		/// <summary>
		/// Retourne la liste des moyens de paiements ou d'encaissements disponibles
		/// </summary>
		/// <param name="categoryName">"payment" ou "collection"</param>
		/// <returns>Liste des moyens disponibles</returns>
		public static List<Tuple<string, string, Type>> GetAvaillablePaytypes(string categoryName)
		{
			return (from da in AppDomain.CurrentDomain.GetAssemblies()
			        from at in da.GetTypes()
			        where typeof(Paytype).IsAssignableFrom(at) && !at.Equals(typeof(Paytype)) && !at.Equals(typeof(Collection))
			        where Attribute.IsDefined(at, typeof(CategoryAttribute)) && Attribute.IsDefined(at, typeof(DisplayNameAttribute)) && Attribute.IsDefined(at, typeof(DescriptionAttribute))
			        where ((Attribute.GetCustomAttribute(at, typeof(CategoryAttribute)) as CategoryAttribute).Category).Contains(categoryName)
			        select new Tuple<string, string, Type>((Attribute.GetCustomAttribute(at, typeof(DisplayNameAttribute)) as DisplayNameAttribute).DisplayName, (Attribute.GetCustomAttribute(at, typeof(DescriptionAttribute)) as DescriptionAttribute).Description, at)).ToList();
		}

		/// <summary>
		/// Retourne la liste des moyens de paiements disponibles
		/// </summary>
		/// <returns>Liste des moyens de paiements (nom, description, type)</returns>
		public static List<Tuple<string, string, Type>> GetAvaillablePayments()
		{
			return GetAvaillablePaytypes("payment");
		}

		/// <summary>
		/// Retourne la liste des moyens d'encaissements disponibles
		/// </summary>
		/// <returns>Liste des moyens d'encaissements (nom, description, type)</returns>
		public static List<Tuple<string, string, Type>> GetAvaillableCollections()
		{
			return GetAvaillablePaytypes("collection");
		}

		/// <summary>
		/// Retourne l'élément à la position <c>index</c>
		/// </summary>
		/// <param name="index">Position de l'élément.</param>
		public new Paytype this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				OnPaytypeRemoved(base[index]);
					
				base[index] = value;
				
				base[index].UpdatedEvent += (sender, e) => OnPaytypeUpdated(base[index]);
				OnPaytypeAdded(base[index]);
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
				OnPaytypeRemoved(base[index]);
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
				OnPaytypeRemoved(e);
			}

			base.Clear();
		}
		
		/// <summary>
		/// Supprime un élément de la liste
		/// </summary>
		/// <returns>true, l'élément est supprimé, sinon, false.</returns>
		/// <param name="item">Elément à supprimer.</param>
		public new bool Remove(Paytype item)
		{
			if (base.IndexOf(item) > -1)
				OnPaytypeRemoved(item);
			
			return base.Remove(item);
		}
		
		/// <summary>
		/// Ajoute un élément à la liste
		/// </summary>
		/// <param name="item">Item.</param>
		public new void Add(Paytype item)
		{
			OnPaytypeAdded(item);
			item.UpdatedEvent += (sender, e) => OnPaytypeUpdated(item);
			base.Add(item);
		}
		
		/// <summary>
		/// Insert un élément à la position spécifiée
		/// </summary>
		/// <param name="index">Position d'insertion.</param>
		/// <param name="item">Elément à insérer.</param>
		public new void Insert(int index, Paytype item)
		{
			OnPaytypeAdded(item);
			item.UpdatedEvent += (sender, e) => OnPaytypeUpdated(item);
			base.Insert(index, item);
		}
		


	}

}
