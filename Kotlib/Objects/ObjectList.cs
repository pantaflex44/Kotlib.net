//
//  ObjectList.cs
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

namespace Kotlib.Objects
{

	/// <summary>
	/// Liste personnalisée d'objets 
	/// </summary>
	public class ObjectList<T> : IList<T>, INotifyCollectionChanged
	{

		List<T> _list = new List<T>();

		#region Fonctions privées

		/// <summary>
		/// Informe que la collection est modifiée
		/// </summary>
		/// <param name="action">Nom de la méthode</param>
		public void OnCollectionChanged(NotifyCollectionChangedAction action)
		{
			if (CollectionChanged != null)
				CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(action));
        	
			OnUpdated(this, new EventArgs());
		}

		/// <summary>
		/// Informe que le dossier financier a été modifié
		/// </summary>
		public void OnUpdated(object sender, EventArgs e)
		{
			if (UpdatedEvent != null)
				UpdatedEvent.Invoke(sender, e);
		}

		/// <summary>
		/// Lie l'événement UpdatedEvent d'un objet à celui de la liste d'objets
		/// </summary>
		/// <param name="item">Objet à lier.</param>
		private void _AddUpdatedEvent(T item)
		{
			var eUpdatedEvent = item.GetType().GetEvent("UpdatedEvent");
			if (eUpdatedEvent != null && eUpdatedEvent.GetAddMethod() != null)
			{
				var mOnUpdated = GetType().GetMethod("OnUpdated");
				var mOnUpdatedDelegate = Delegate.CreateDelegate(eUpdatedEvent.EventHandlerType, this, mOnUpdated);
				eUpdatedEvent.GetAddMethod().Invoke(item, new object[] { mOnUpdatedDelegate });
			}
		}

		/// <summary>
		/// Annule le routage d'événement pour l'objet supprimé
		/// </summary>
		/// <param name="item">Objet à délier.</param>
		private void _RemoveUpdatedEvent(T item)
		{
			var eUpdatedEvent = item.GetType().GetEvent("UpdatedEvent");
			if (eUpdatedEvent != null && eUpdatedEvent.GetRemoveMethod() != null)
			{
				var mOnUpdated = GetType().GetMethod("OnUpdated");
				var mOnUpdatedDelegate = Delegate.CreateDelegate(eUpdatedEvent.EventHandlerType, this, mOnUpdated);
				eUpdatedEvent.GetRemoveMethod().Invoke(item, new object[] { mOnUpdatedDelegate });
			}
		}

		#endregion

		#region Evénements

		/// <summary>
		/// Se produit lorsque que la collection est modifiée
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// Se produit lorsque le dossier financier a été modifié
		/// </summary>
		public event EventHandler UpdatedEvent;

		#endregion

		/// <summary>
		/// Constructeur
		/// </summary>
		public ObjectList()
		{
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="items">Eléments à charger</param>
		public ObjectList(IEnumerable<T> items)
		{
			_list = new List<T>(items);
		}

		/// <summary>
		/// Retourne le nombre d'éléments de la liste
		/// </summary>
		/// <value>Nombre d'éléments.</value>
		public int Count
		{
			get
			{
				return _list.Count;
			}
		}

		/// <summary>
		/// Indique si la liste est en lecture seule
		/// </summary>
		/// <value><c>true</c> si la liste est en lecture seule, sinon, <c>false</c>.</value>
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Retournel'élément à la position <c>index</c>
		/// </summary>
		/// <param name="index">Position de l'élément.</param>
		public T this[int index]
		{
			get
			{
				return _list[index];
			}
			set
			{
				_RemoveUpdatedEvent(_list[index]);
				_list[index] = value;
				_AddUpdatedEvent(_list[index]);
				OnCollectionChanged(NotifyCollectionChangedAction.Replace);

			}
		}

		/// <summary>
		/// Ajoute un élément à la liste
		/// </summary>
		/// <param name="item">Item.</param>
		public void Add(T item)
		{
			if (_list.IndexOf(item) == -1)
			{
				_AddUpdatedEvent(item);
				_list.Add(item);
				OnCollectionChanged(NotifyCollectionChangedAction.Add);
			}
		}

		/// <summary>
		/// Retourne la position d'un élément
		/// </summary>
		/// <returns>Position de l'élément base 0, -1 si non trouvée.</returns>
		/// <param name="item">Elément à rechercher.</param>
		public int IndexOf(T item)
		{
			return _list.IndexOf(item);
		}

		/// <summary>
		/// Insert un élément à la position spécifiée
		/// </summary>
		/// <param name="index">Position d'insertion.</param>
		/// <param name="item">Elément à insérer.</param>
		public void Insert(int index, T item)
		{
			if (_list.IndexOf(item) == -1)
			{
				_AddUpdatedEvent(item);
				_list.Insert(index, item);
			}
		}

		/// <summary>
		/// Supprime l'élément de la liste à la position spécifié
		/// </summary>
		/// <param name="index">Position de l'élément.</param>
		public void RemoveAt(int index)
		{
			if (index >= 0 && index < _list.Count)
			{
				_RemoveUpdatedEvent(_list[index]);
				_list.RemoveAt(index);
			}
		}

		/// <summary>
		/// Vide la liste de ses éléments
		/// </summary>
		public void Clear()
		{
			foreach (var e in _list)
				_RemoveUpdatedEvent(e);

			_list.Clear();
		}

		/// <summary>
		/// Retourne si la liste contient un élément
		/// </summary>
		/// <returns>true, si l'élément existe, sinon, false.</returns>
		/// <param name="item">Elément à trouver.</param>
		public bool Contains(T item)
		{
			return _list.Contains(item);
		}

		/// <summary>
		/// Copie une partie de la liste vers une autre
		/// </summary>
		/// <param name="array">Conteneur de la copie.</param>
		/// <param name="arrayIndex">Position de la liste .</param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Supprime un élément de la liste
		/// </summary>
		/// <returns>true, l'élément est supprimé, sinon, false.</returns>
		/// <param name="item">Elément à supprimer.</param>
		public bool Remove(T item)
		{
			if (_list.IndexOf(item) > -1)
			{
				_RemoveUpdatedEvent(item);
				return _list.Remove(item);
			}
			return false;
		}
		
		/// <summary>
		/// Supprime tous les éléments suivant les conditions predicate
		/// </summary>
		/// <param name="predicate">Conditions</param>
		/// <returns>Nombre d'éléments supprimés</returns>
		public int RemoveAll(Predicate<T> predicate)
		{
			return _list.RemoveAll(predicate);
		}

		/// <summary>
		/// Retourne l'énumérateur
		/// </summary>
		/// <returns>Enumérateur.</returns>
		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		/// <summary>
		/// Retourne l'interface d'énumération
		/// </summary>
		/// <returns>Interface d'énumération.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)_list.GetEnumerator();
		}
		
		/// <summary>
		/// Retourne la liste des éléments
		/// </summary>
		/// <value>Liste des éléments</value>
		public List<T> Items
		{
			get { return _list; }
		}

	}

}
