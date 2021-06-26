//
//  Operation.cs
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
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace Kotlib.Objects
{
	
	/// <summary>
	/// Représente une opération bancaire.
	/// </summary>
	[XmlRoot(ElementName = "Operation")]
	public class Operation: INotifyPropertyChanged
	{
		
		#region Fonctions privées

		/// <summary>
		/// Informe qu'une propriété est modifiée
		/// </summary>
		/// <param name="name">Nom de la propriété,
		/// ou vide pour le nom de la propriété appelante.</param>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void OnPropertyChanged(string name = null)
		{
			if (name == null)
			{
				var stackTrace = new StackTrace(1, false);
				var type = stackTrace.GetFrame(1).GetMethod().DeclaringType;
				name = type.Name;
			}
            
			if (PropertyChanged != null)
				PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            
			OnUpdated(this, new EventArgs());
		}

		/// <summary>
		/// Informe que l'opération a été modifiée
		/// </summary>
		public void OnUpdated(object sender, EventArgs e)
		{
			if (UpdatedEvent != null)
				UpdatedEvent.Invoke(sender, e);
		}

		#endregion

		#region Evénements

		/// <summary>
		/// Se produit lorsque qu'une propriété est modifiée
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Se produit lorsque le dossier financier a été modifié
		/// </summary>
		public event EventHandler UpdatedEvent;

		#endregion

		#region Propriétés publiques

		private Guid _id = Guid.Empty;
		/// <summary>
		/// Identifiant unique
		/// </summary>
		/// <value>Identifiant unique.</value>
		[XmlElement(ElementName = "Id")]
		public Guid Id
		{
			get { return _id; }
			set
			{
				if (value != _id)
				{
					_id = value;
					OnPropertyChanged();
				}
			}
		}

		private string _name = "";
		/// <summary>
		/// Nom donné à l'opération
		/// </summary>
		/// <example>Prélèvement électricité</example>
		/// <value>Nom, 255 caractères maximum.</value>
		[XmlElement(ElementName = "Name")]
		public string Name
		{
			get { return _name; }
			set
			{
				value = value.Trim();

				if (value.Length > 255)
					value = value.Substring(0, 255);

				if (value == "")
					throw new ArgumentException("Dénomination de l'opération requise.");

				if (value != _name)
				{
					_name = value;
					OnPropertyChanged();
				}
			}
		}
		/// <summary>
		/// Vérifie si la propriété est correctement définie avant d'être sérialisée
		/// </summary>
		/// <returns><c>true</c></returns>
		private bool ShouldSerializeName()
		{
			if (Name.Trim() == "")
				throw new ArgumentException("Dénomination de l'opération requise.");

			return true;
		}
		
		private DateTime _date = DateTime.Now;
		/// <summary>
		/// Date de l'opération
		/// </summary>
		/// <value>Date de l'opération</value>
		[XmlAttribute(AttributeName = "date")]
		public DateTime Date
		{
			get { return _date; }
			set
			{
				if (value != _date)
				{
					_date = value;
					OnPropertyChanged();
				}
			}
		}
		
		private double _amount = 0.0d;
		/// <summary>
		/// Montant de l'opération
		/// </summary>
		/// <value>Montant de l'opération.</value>
		[XmlAttribute(AttributeName = "amount")]
		public double Amount
		{
			get { return _amount; }
			set
			{
				if (!value.Equals(_amount))
				{
					_amount = value;
					OnPropertyChanged();
				}
			}
		}
		
		private Guid _toId = Guid.Empty;
		/// <summary>
		/// Identité du destinataire
		/// </summary>
		/// <value>Identifiant unique de l'identité du destinataire</value>
		[XmlElement(ElementName = "To")]
		public Guid ToId
		{ 
			get { return _toId; }
			set
			{
				if (value == Guid.Empty)
					throw new ArgumentException("L'identité d'un destinataire est requis.");
				
				_toId = value;
			}
		}
		/// <summary>
		/// Vérifie si la propriété est correctement définie avant d'être sérialisée
		/// </summary>
		/// <returns><c>true</c></returns>
		private bool ShouldSerializeToId()
		{
			if (CategoryId == Guid.Empty)
				throw new ArgumentException("L'identité d'un destinataire est requis.");

			return true;
		}
				
		private Guid _paytypeId = Guid.Empty;
		/// <summary>
		/// Type d'opération (moyen de paiement ou d'encaissement)
		/// </summary>
		/// <value>Identifiant unique du moyen de paiement ou d'encaissement</value>
		[XmlElement(ElementName = "Type")]
		public Guid TypeId
		{ 
			get { return _paytypeId; }
			set
			{
				if (value == Guid.Empty)
					throw new ArgumentException("Un moyen de paiement / d'encaissement est requis.");
				
				_paytypeId = value;
			}
		}
		/// <summary>
		/// Vérifie si la propriété est correctement définie avant d'être sérialisée
		/// </summary>
		/// <returns><c>true</c></returns>
		private bool ShouldSerializeTypeId()
		{
			if (TypeId == Guid.Empty)
				throw new ArgumentException("Un moyen de paiement / d'encaissement est requis.");

			return true;
		}
		
		private Guid _categoryId = Guid.Empty;
		/// <summary>
		/// Catégorie dans laquelle classer cette opération
		/// </summary>
		/// <value>Identifiant unique de la catégorie</value>
		[XmlElement(ElementName = "Category")]
		public Guid CategoryId
		{ 
			get { return _categoryId; }
			set
			{
				if (value == Guid.Empty)
					throw new ArgumentException("Une catégorie est requise.");
				
				_categoryId = value;
			}
		}
		/// <summary>
		/// Vérifie si la propriété est correctement définie avant d'être sérialisée
		/// </summary>
		/// <returns><c>true</c></returns>
		private bool ShouldSerializeCategoryId()
		{
			if (CategoryId == Guid.Empty)
				throw new ArgumentException("Une catégorie est requise.");

			return true;
		}
		
		private string _note = "";
		/// <summary>
		/// Notes appliquées à cette catégory.
		/// </summary>
		/// <value>Notes, 4000 caractères maximum.</value>
		[XmlIgnore]
		public string Note
		{
			get { return _note; }
			set
			{
				value = value.Trim();
				if (value.Length > 4000)
					value = value.Substring(0, 4000);

				if (value != _note)
				{
					_note = value;
					OnPropertyChanged();
				}
			}
		}
		private static readonly XmlDocument _xmlDoc = new XmlDocument();
		/// <summary>
		/// Note au format brute
		/// </summary>
		[XmlElement(ElementName = "Note")]
		public XmlCDataSection NoteCData
		{
			get { return _xmlDoc.CreateCDataSection(Note); }
			set { Note = value.Data; }
		}

		private bool _active = true;
		/// <summary>
		/// Retourne ou définit si l'opération est comptabilisée ou non
		/// </summary>
		/// <value>Etat de l'opération</value>
		[XmlAttribute(AttributeName = "active")]
		public  bool Active
		{
			get { return _active; }
			set
			{
				if (value != _active)
				{
					_active = value;
					OnPropertyChanged();
				}
			}
		}
		
		#endregion

		/// <summary>
		/// Constructeurs
		/// </summary>
		public Operation()
		{
			Id = Guid.NewGuid();
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom de l'opération.</param>
		/// <param name="date">Date de l'opération</param>
		/// <param name="amount">Montant de l'opération</param>"
		/// <param name="toId">Identifiant unique de l'identité du destinataire</param>
		/// <param name="typeId">Identifiant unique du moyen d'encaissement ou de paiement</param>
		/// <param name="categoryId">Identifiant unique de la catégorie de classement</param>
		/// <param name="active"><c>true</c>, l'opération est comptabilisée, sinon, <c>false</c>. Optionnel, <c>true</c> par défaut</param>
		public Operation(string name, DateTime date, double amount, Guid toId, Guid typeId, Guid categoryId, bool active = true)
			: this()
		{
			Name = name;
			Date = date;
			Amount = amount;
			ToId = toId;
			TypeId = typeId;
			CategoryId = categoryId;
			Active = active;
		}
		
	}
	
}
