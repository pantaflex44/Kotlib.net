//
//  OperationList.cs
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
	/// Liste des opérations
	/// </summary>
	public class OperationList : ObjectList<Operation>
	{

		#region Evénements
		
		/// <summary>
		/// Délégué en charge des événements d'ajout, modification et supppression
		/// </summary>
		/// <param name="item">Elément concené</param>
		public delegate void OperationEventHandler(Operation item);
		/// <summary>
		/// Un élément a été ajouté
		/// </summary>
		public event OperationEventHandler OperationAddedEvent;
		/// <summary>
		/// Un élément a été modifié
		/// </summary>
		public event OperationEventHandler OperationUpdatedEvent;
		/// <summary>
		/// Un élément a été supprimé
		/// </summary>
		public event OperationEventHandler OperationRemovedEvent;
		
		#endregion
		
		#region Fonctions privées
		
		/// <summary>
		/// Informe qu'un élément a été ajouté
		/// </summary>
		/// <param name="item">Elément concerné</param>
		public void OnOperationAdded(Operation item)
		{
			if (OperationAddedEvent != null)
				OperationAddedEvent(item);
		}
		
		/// <summary>
		/// Informe qu'un élément a été modifié
		/// </summary>
		/// <param name="item">Elément concerné</param>
		public void OnOperationUpdated(Operation item)
		{
			if (OperationUpdatedEvent != null)
				OperationUpdatedEvent(item);
		}
		
		/// <summary>
		/// Informe qu'un élément a été supprimé
		/// </summary>
		/// <param name="item">Elément concerné</param>
		public void OnOperationRemoved(Operation item)
		{
			if (OperationRemovedEvent != null)
				OperationRemovedEvent(item);
		}
		
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

		/// <summary>
		/// Retourne l'élément à la position <c>index</c>
		/// </summary>
		/// <param name="index">Position de l'élément.</param>
		public new Operation this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				OnOperationRemoved(base[index]);
					
				base[index] = value;
				
				base[index].UpdatedEvent += (sender, e) => OnOperationUpdated((Operation)sender);
				OnOperationAdded(base[index]);
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
				OnOperationRemoved(base[index]);
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
				OnOperationRemoved(e);
			}

			base.Clear();
		}
		
		/// <summary>
		/// Supprime un élément de la liste
		/// </summary>
		/// <returns>true, l'élément est supprimé, sinon, false.</returns>
		/// <param name="item">Elément à supprimer.</param>
		public new bool Remove(Operation item)
		{
			if (base.IndexOf(item) > -1)
				OnOperationRemoved(item);
			
			return base.Remove(item);
		}
		
		/// <summary>
		/// Ajoute un élément à la liste
		/// </summary>
		/// <param name="item">Item.</param>
		public new void Add(Operation item)
		{
			OnOperationAdded(item);
			item.UpdatedEvent += (sender, e) => OnOperationUpdated((Operation)sender);
			base.Add(item);
		}
		
		/// <summary>
		/// Insert un élément à la position spécifiée
		/// </summary>
		/// <param name="index">Position d'insertion.</param>
		/// <param name="item">Elément à insérer.</param>
		public new void Insert(int index, Operation item)
		{
			OnOperationAdded(item);
			item.UpdatedEvent += (sender, e) => OnOperationUpdated((Operation)sender);
			base.Insert(index, item);
		}
		
		
		
		
	}

}
