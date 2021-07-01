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

		#region Evénements
		
		/// <summary>
		/// Délégué en charge des événements d'ajout, modification et supppression
		/// </summary>
		/// <param name="item">Elément concené</param>
		public delegate void ThirdpartyEventHandler(Identity item);
		/// <summary>
		/// Un élément a été ajouté
		/// </summary>
		public event ThirdpartyEventHandler ThirdpartyAddedEvent;
		/// <summary>
		/// Un élément a été modifié
		/// </summary>
		public event ThirdpartyEventHandler ThirdpartyUpdatedEvent;
		/// <summary>
		/// Un élément a été supprimé
		/// </summary>
		public event ThirdpartyEventHandler ThirdpartyRemovedEvent;
		
		#endregion
		
		#region Fonctions privées
		
		/// <summary>
		/// Informe qu'un élément a été ajouté
		/// </summary>
		/// <param name="item">Elément concerné</param>
		public void OnThirdpartyAdded(Identity item)
		{
			if (ThirdpartyAddedEvent != null)
				ThirdpartyAddedEvent(item);
		}
		
		/// <summary>
		/// Informe qu'un élément a été modifié
		/// </summary>
		/// <param name="item">Elément concerné</param>
		public void OnThirdpartyUpdated(Identity item)
		{
			if (ThirdpartyUpdatedEvent != null)
				ThirdpartyUpdatedEvent(item);
		}
		
		/// <summary>
		/// Informe qu'un élément a été supprimé
		/// </summary>
		/// <param name="item">Elément concerné</param>
		public void OnThirdpartyRemoved(Identity item)
		{
			if (ThirdpartyRemovedEvent != null)
				ThirdpartyRemovedEvent(item);
		}
		
		#endregion
		
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

		/// <summary>
		/// Retourne l'élément à la position <c>index</c>
		/// </summary>
		/// <param name="index">Position de l'élément.</param>
		public new Identity this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				OnThirdpartyRemoved(base[index]);
					
				base[index] = value;
				
				base[index].UpdatedEvent += (sender, e) => OnThirdpartyUpdated(base[index]);
				OnThirdpartyAdded(base[index]);
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
				OnThirdpartyRemoved(base[index]);
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
				OnThirdpartyRemoved(e);
			}

			base.Clear();
		}
		
		/// <summary>
		/// Supprime un élément de la liste
		/// </summary>
		/// <returns>true, l'élément est supprimé, sinon, false.</returns>
		/// <param name="item">Elément à supprimer.</param>
		public new bool Remove(Identity item)
		{
			if (base.IndexOf(item) > -1)
				OnThirdpartyRemoved(item);
			
			return base.Remove(item);
		}
		
		/// <summary>
		/// Ajoute un élément à la liste
		/// </summary>
		/// <param name="item">Item.</param>
		public new void Add(Identity item)
		{
			OnThirdpartyAdded(item);
			item.UpdatedEvent += (sender, e) => OnThirdpartyUpdated(item);
			base.Add(item);
		}
		
		/// <summary>
		/// Insert un élément à la position spécifiée
		/// </summary>
		/// <param name="index">Position d'insertion.</param>
		/// <param name="item">Elément à insérer.</param>
		public new void Insert(int index, Identity item)
		{
			OnThirdpartyAdded(item);
			item.UpdatedEvent += (sender, e) => OnThirdpartyUpdated(item);
			base.Insert(index, item);
		}	
		
		
	}

}
