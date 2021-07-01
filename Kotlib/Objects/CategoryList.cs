//
//  CategoryList.cs
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
	/// Liste de catégories
	/// </summary>
	public class CategoryList : ObjectList<Category>
	{
		
		#region Evénements
		
		/// <summary>
		/// Délégué en charge des événements d'ajout, modification et supppression
		/// </summary>
		/// <param name="item">Elément concené</param>
		public delegate void CategoryEventHandler(Category item);
		/// <summary>
		/// Un élément a été ajouté
		/// </summary>
		public event CategoryEventHandler CategoryAddedEvent;
		/// <summary>
		/// Un élément a été modifié
		/// </summary>
		public event CategoryEventHandler CategoryUpdatedEvent;
		/// <summary>
		/// Un élément a été supprimé
		/// </summary>
		public event CategoryEventHandler CategoryRemovedEvent;
		
		#endregion
		
		#region Fonctions privées
		
		/// <summary>
		/// Informe qu'un élément a été ajouté
		/// </summary>
		/// <param name="item">Elément concerné</param>
		public void OnCategoryAdded(Category item)
		{
			if (CategoryAddedEvent != null)
				CategoryAddedEvent(item);
		}
		
		/// <summary>
		/// Informe qu'un élément a été modifié
		/// </summary>
		/// <param name="item">Elément concerné</param>
		public void OnCategoryUpdated(Category item)
		{
			if (CategoryUpdatedEvent != null)
				CategoryUpdatedEvent(item);
		}
		
		/// <summary>
		/// Informe qu'un élément a été supprimé
		/// </summary>
		/// <param name="item">Elément concerné</param>
		public void OnCategoryRemoved(Category item)
		{
			if (CategoryRemovedEvent != null)
				CategoryRemovedEvent(item);
		}
		
		#endregion

		#region Propriétés publiques

		/// <summary>
		/// Retourne une liste vide
		/// </summary>
		/// <value>Liste vide.</value>
		public static CategoryList Empty
		{
			get
			{
				return new CategoryList();
			}
		}

		/// <summary>
		/// Retourne une liste de moyens par défaut
		/// </summary>
		public static CategoryList Defaults
		{
			get
			{
				return new CategoryList(new List<Category>
					{
						new Category("Abonnements", new CategoryList(new List<Category>
								{
									new Category("TV"),
									new Category("Musique"),
									new Category("Téléphonie"),
									new Category("Internet"),
									new Category("Sport et Fitness"),
									new Category("Divers")
								})),
						new Category("Véhicules", new CategoryList(new List<Category>
								{
									new Category("Carburant"),
									new Category("Assurance"),
									new Category("Entretien"),
									new Category("Réparation"),
									new Category("Parking"),
									new Category("Divers")
								})),
						new Category("Maison", new CategoryList(new List<Category>
								{
									new Category("Loyer"),
									new Category("Assurance"),
									new Category("Electricité"),
									new Category("Gaz"),
									new Category("Eau"),
									new Category("Internet"),
									new Category("Courses"),
									new Category("Participation"),
									new Category("Divers")
								})),
						new Category("Revenus", new CategoryList(new List<Category>
								{
									new Category("Salaire"),
									new Category("Accompte"),
									new Category("Don"),
									new Category("Rembourssement"),
									new Category("Congés payés"),
									new Category("Divers")
								})),
						new Category("Taxes", new CategoryList(new List<Category>
								{
									new Category("Amende"),
									new Category("Impôts")
								})),
						new Category("Divers", new CategoryList(new List<Category>
								{
									new Category("Crédit"),
									new Category("Rembourssement"),
									new Category("Don"),
									new Category("Frais"),
									new Category("Frais bancaires"),
									new Category("Emprunt"),
									new Category("Participation"),
									new Category("Divers")
								})),
						new Category("Santé et bien être", new CategoryList(new List<Category>
								{
									new Category("Médecin"),
									new Category("Hôpital"),
									new Category("Pharmacie"),
									new Category("Médecine alternative"),
									new Category("Coiffeur"),
									new Category("Epilation"),
									new Category("Massage et SPA"),
									new Category("Sport et Fitness"),
									new Category("Divers")
								}))
					});
			}
		}

		#endregion

		/// <summary>
		/// Constructeur
		/// </summary>
		public CategoryList()
			: base()
		{
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="items">Liste à charger</param>
		public CategoryList(IEnumerable<Category> items)
			: base(items)
		{
		}

		/// <summary>
		/// Retourne la premiere catégorie trouvée possédant l'identifiant unique passé en paramètre
		/// </summary>
		/// <returns>Catégorie trouvée.</returns>
		/// <param name="id">Identifiant unique.</param>
		public Category GetById(Guid id)
		{
			return this.ToList().FirstOrDefault(a => a.Id.Equals(id));
		}

		/// <summary>
		/// Retourne l'élément à la position <c>index</c>
		/// </summary>
		/// <param name="index">Position de l'élément.</param>
		public new Category this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				OnCategoryRemoved(base[index]);
					
				base[index] = value;
				
				base[index].UpdatedEvent += (sender, e) => OnCategoryUpdated(base[index]);
				OnCategoryAdded(base[index]);
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
				OnCategoryRemoved(base[index]);
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
				OnCategoryRemoved(e);
			}

			base.Clear();
		}
		
		/// <summary>
		/// Supprime un élément de la liste
		/// </summary>
		/// <returns>true, l'élément est supprimé, sinon, false.</returns>
		/// <param name="item">Elément à supprimer.</param>
		public new bool Remove(Category item)
		{
			if (base.IndexOf(item) > -1)
				OnCategoryRemoved(item);
			
			return base.Remove(item);
		}
		
		/// <summary>
		/// Ajoute un élément à la liste
		/// </summary>
		/// <param name="item">Item.</param>
		public new void Add(Category item)
		{
			OnCategoryAdded(item);
			item.UpdatedEvent += (sender, e) => OnCategoryUpdated(item);
			base.Add(item);
		}
		
		/// <summary>
		/// Insert un élément à la position spécifiée
		/// </summary>
		/// <param name="index">Position d'insertion.</param>
		/// <param name="item">Elément à insérer.</param>
		public new void Insert(int index, Category item)
		{
			OnCategoryAdded(item);
			item.UpdatedEvent += (sender, e) => OnCategoryUpdated(item);
			base.Insert(index, item);
		}

		
	}

}
