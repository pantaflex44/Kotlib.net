//
//  EventList.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Kotlib.Objects
{

	/// <summary>
	/// Liste des événements programmés
	/// </summary>
	public class EventList : ObjectList<Event>
	{

		#region Fonctions privées
		
		/// <summary>
		/// Informe que le dossier financier a été modifié
		/// </summary>
		public void OnPostRaised(DateTime date, Event postEvent)
		{
			if (PostRaisedEvent != null)
				PostRaisedEvent.Invoke(date, postEvent);
		}
		
		/// <summary>
		/// Informe qu'un élément a été ajouté
		/// </summary>
		/// <param name="item">Elément concerné</param>
		public void OnPostEventAdded(Event item)
		{
			if (PostEventAddedEvent != null)
				PostEventAddedEvent(item);
		}
		
		/// <summary>
		/// Informe qu'un élément a été modifié
		/// </summary>
		/// <param name="item">Elément concerné</param>
		public void OnPostEventUpdated(Event item)
		{
			if (PostEventUpdatedEvent != null)
				PostEventUpdatedEvent(item);
		}
		
		/// <summary>
		/// Informe qu'un élément a été supprimé
		/// </summary>
		/// <param name="item">Elément concerné</param>
		public void OnPostEventRemoved(Event item)
		{
			if (PostEventRemovedEvent != null)
				PostEventRemovedEvent(item);
		}

		/// <summary>
		/// Lie l'événement UpdatedEvent d'un objet à celui de la liste d'objets
		/// </summary>
		/// <param name="item">Objet à lier.</param>
		private void _AddPostRaisedEvent(Event item)
		{
			var ePostRaisedEvent = item.GetType().GetEvent("PostRaisedEvent");
			if (ePostRaisedEvent != null && ePostRaisedEvent.GetAddMethod() != null)
			{
				var mOnUpdated = GetType().GetMethod("OnPostRaised");
				var mOnUpdatedDelegate = Delegate.CreateDelegate(ePostRaisedEvent.EventHandlerType, this, mOnUpdated);
				ePostRaisedEvent.GetAddMethod().Invoke(item, new object[] { mOnUpdatedDelegate });
			}
		}

		/// <summary>
		/// Annule le routage d'événement pour l'objet supprimé
		/// </summary>
		/// <param name="item">Objet à délier.</param>
		private void _RemovePostRaisedEvent(Event item)
		{
			var ePostRaisedEvent = item.GetType().GetEvent("PostRaisedEvent");
			if (ePostRaisedEvent != null && ePostRaisedEvent.GetRemoveMethod() != null)
			{
				var mOnUpdated = GetType().GetMethod("OnUpdated");
				var mOnUpdatedDelegate = Delegate.CreateDelegate(ePostRaisedEvent.EventHandlerType, this, mOnUpdated);
				ePostRaisedEvent.GetRemoveMethod().Invoke(item, new object[] { mOnUpdatedDelegate });
			}
		}
		
		#endregion
		
		#region Evénements
		
		/// <summary>
		/// Délégué en charge de transmettre les informations des événements postés
		/// </summary>
		/// <param name="date">Date de l'occurence</param>
		/// <param name="postEvent">Objet représentant l'occurence postée</param>
		public delegate void PostDelegate(DateTime date, Event postEvent);
		/// <summary>
		/// Se produit lorsqu'une occurence programmée est postée
		/// </summary>
		public event PostDelegate PostRaisedEvent;
		
		/// <summary>
		/// Délégué en charge des événements d'ajout, modification et supppression
		/// </summary>
		/// <param name="item">Elément concené</param>
		public delegate void PostEventEventHandler(Event item);
		/// <summary>
		/// Un élément a été ajouté
		/// </summary>
		public event PostEventEventHandler PostEventAddedEvent;
		/// <summary>
		/// Un élément a été modifié
		/// </summary>
		public event PostEventEventHandler PostEventUpdatedEvent;
		/// <summary>
		/// Un élément a été supprimé
		/// </summary>
		public event PostEventEventHandler PostEventRemovedEvent;
		
		#endregion

		#region Propriétés publiques

		/// <summary>
		/// Retourne une liste vide
		/// </summary>
		/// <value>Liste vide.</value>
		public static EventList Empty
		{
			get
			{
				return new EventList();
			}
		}

		/// <summary>
		/// Retourne une liste d'évnelent prgrammés par défaut
		/// </summary>
		public static EventList Defaults
		{
			get
			{
				return EventList.Empty;
			}
		}

		#endregion

		/// <summary>
		/// Constructeur
		/// </summary>
		public EventList()
			: base()
		{
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="items">Liste à charger</param>
		public EventList(IEnumerable<Event> items)
			: base(items)
		{
		}

		/// <summary>
		/// Retourne le premier événement programmé trouvé possédant l'identifiant unique passé en paramètre
		/// </summary>
		/// <returns>Evénement programmé trouvé.</returns>
		/// <param name="id">Identifiant unique.</param>
		public Event GetById(Guid id)
		{
			return this.ToList().FirstOrDefault(a => a.Id.Equals(id));
		}
		
		/// <summary>
		/// Retournel'élément à la position <c>index</c>
		/// </summary>
		/// <param name="index">Position de l'élément.</param>
		public new Event this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				_RemovePostRaisedEvent(base[index]);
				OnPostEventRemoved(base[index]);
				
				base[index] = value;
				base[index].UpdatedEvent += (sender, e) => OnPostEventUpdated(base[index]);
				
				_AddPostRaisedEvent(base[index]);
				OnPostEventAdded(base[index]);
			}
		}
		
		/// <summary>
		/// Ajoute un élément à la liste
		/// </summary>
		/// <param name="item">Item.</param>
		public new void Add(Event item)
		{
			if (base.IndexOf(item) == -1)
			{
				_AddPostRaisedEvent(item);
				OnPostEventAdded(item);
				item.UpdatedEvent += (sender, e) => OnPostEventUpdated(item);
				base.Add(item);
			}
		}
		
		/// <summary>
		/// Insert un élément à la position spécifiée
		/// </summary>
		/// <param name="index">Position d'insertion.</param>
		/// <param name="item">Elément à insérer.</param>
		public new void Insert(int index, Event item)
		{
			if (base.IndexOf(item) == -1)
			{
				OnPostEventAdded(item);
				item.UpdatedEvent += (sender, e) => OnPostEventUpdated(item);
				_AddPostRaisedEvent(item);
				base.Insert(index, item);
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
				_RemovePostRaisedEvent(base[index]);
				OnPostEventRemoved(base[index]);
				base.RemoveAt(index);
			}
		}
		
		/// <summary>
		/// Vide la liste de ses éléments
		/// </summary>
		public new void Clear()
		{
			foreach (var e in base.Items)
			{
				_RemovePostRaisedEvent(e);
				OnPostEventRemoved(e);
			}

			base.Clear();
		}
		
		/// <summary>
		/// Supprime un élément de la liste
		/// </summary>
		/// <returns>true, l'élément est supprimé, sinon, false.</returns>
		/// <param name="item">Elément à supprimer.</param>
		public new bool Remove(Event item)
		{
			if (base.IndexOf(item) > -1)
			{
				_RemovePostRaisedEvent(item);
				OnPostEventRemoved(item);
				return base.Remove(item);
			}
			return false;
		}

		
		

	}

}
