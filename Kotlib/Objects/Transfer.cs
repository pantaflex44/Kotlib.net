//
//  Transfer.cs
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

namespace Kotlib.Objects
{
	
	/// <summary>
	/// Représente un transfert bancaire.
	/// </summary>
	[XmlRoot(ElementName = "Transfer")]
	public class Transfer: INotifyPropertyChanged
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
		/// Nom donné au transfert
		/// </summary>
		/// <example>Virement Livret A</example>
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
					throw new ArgumentException("Dénomination du transfert requis.");

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
				throw new ArgumentException("Dénomination du transfert requis.");

			return true;
		}
		
		private DateTime _date = DateTime.Now;
		/// <summary>
		/// Date du transfert
		/// </summary>
		/// <value>Date du transfert</value>
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
		/// Montant du transfert
		/// </summary>
		/// <value>Montant du transfert.</value>
		[XmlAttribute(AttributeName = "amount")]
		public double Amount
		{
			get { return _amount; }
			set
			{
				if (!Math.Abs(value).Equals(_amount))
				{
					_amount = Math.Abs(value);
					OnPropertyChanged();
				}
			}
		}
		
		private Guid _fromActId = Guid.Empty;
		/// <summary>
		/// Identifiant unique du compte émetteur
		/// </summary>
		/// <value>Identifiant unique du compte émetteur</value>
		[XmlElement(ElementName = "From")]
		public Guid FromAccountId
		{ 
			get { return _fromActId; }
			set
			{
				if (value == Guid.Empty)
					throw new ArgumentException("L'identifiant unique du compte émetteur est requis.");
				
				_fromActId = value;
			}
		}
		/// <summary>
		/// Vérifie si la propriété est correctement définie avant d'être sérialisée
		/// </summary>
		/// <returns><c>true</c></returns>
		private bool ShouldSerializeFromAccountId()
		{
			if (FromAccountId == Guid.Empty)
				throw new ArgumentException("L'identifiant unique du compte émetteur est requis.");

			return true;
		}
				
		private Guid _toActId = Guid.Empty;
		/// <summary>
		/// Identifiant unique du compte destinataire
		/// </summary>
		/// <value>Identifiant unique du compte destinataire</value>
		[XmlElement(ElementName = "To")]
		public Guid ToAccountId
		{ 
			get { return _toActId; }
			set
			{
				if (value == Guid.Empty)
					throw new ArgumentException("L'identifiant unique du compte destinataire est requis.");
				
				_toActId = value;
			}
		}
		/// <summary>
		/// Vérifie si la propriété est correctement définie avant d'être sérialisée
		/// </summary>
		/// <returns><c>true</c></returns>
		private bool ShouldSerializeToAccountId()
		{
			if (ToAccountId == Guid.Empty)
				throw new ArgumentException("L'identifiant unique du compte destinataire est requis.");

			return true;
		}
		
		private string _note = "";
		/// <summary>
		/// Notes appliquées à ce transfert.
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
		/// Retourne ou définit si le transfert comptabilisée ou non
		/// </summary>
		/// <value>Etat du transfert</value>
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
		public Transfer()
		{
			Id = Guid.NewGuid();
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom du transfert.</param>
		/// <param name="date">Date du transfert</param>
		/// <param name="amount">Montant du transfert</param>"
		/// <param name="fromAccountId">Identifiant unique du compte émetteur</param>
		/// <param name="toAccountId">Identifiant unique du compte destinataire</param>
		/// <param name="active"><c>true</c>, le transfert est comptabilisé, sinon, <c>false</c>. Optionnel, <c>true</c> par défaut</param>
		public Transfer(string name, DateTime date, double amount, Guid fromAccountId, Guid toAccountId, bool active = true)
			: this()
		{
			Name = name;
			Date = date;
			Amount = amount;
			FromAccountId = fromAccountId;
			ToAccountId = toAccountId;
			Active = active;
		}
		
	}
	
}
