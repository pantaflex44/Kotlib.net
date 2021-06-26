//
//  Paytype.cs
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
	/// Moyen de paiement
	/// </summary>
	[XmlRoot(ElementName = "Paytype")]
	[XmlInclude(typeof(BankCard))]
	[XmlInclude(typeof(Collection))]
	[XmlInclude(typeof(Check))]
	[XmlInclude(typeof(Money))]
	[XmlInclude(typeof(BankDirectDebit))]
	[XmlInclude(typeof(DirectTransfer))]
	[XmlInclude(typeof(LocalTransfer))]
	[XmlInclude(typeof(Deposit))]
	[DisplayName("Moyen de paiement personnalisé")]
	[Description("Moyen de paiement personnalisé")]
	[Category("payment")]
	public class Paytype : INotifyPropertyChanged
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
		/// Informe que le dossier financier a été modifié
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
		/// Dénomination du moyen de paiement
		/// </summary>
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
					throw new ArgumentException("Dénomination du moyen de paiement requise.");

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
				throw new ArgumentException("Dénomination du moyen de paiement requise.");

			return true;
		}

		#endregion

		/// <summary>
		/// Constructeurs
		/// </summary>
		public Paytype()
		{
			Id = Guid.NewGuid();
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom du moyen de paiement.</param>
		public Paytype(string name)
			: this()
		{
			Name = name;
		}

	}

	/// <summary>
	/// Moyen d'encaissement
	/// </summary>
	[XmlRoot(ElementName = "Collection")]
	[DisplayName("Moyen d'encaissement personnalisé")]
	[Description("Moyen d'encaissement personnalisé")]
	[Category("collection")]
	public class Collection : Paytype
	{

		#region Propriétés publiques
		#endregion

		/// <summary>
		/// Constructeurs
		/// </summary>
		public Collection()
			: base()
		{
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom donné au moyen d'encaissement personnalisé.</param>
		public Collection(string name)
			: base(name)
		{
		}

	}

	/// <summary>
	/// Paiement par carte bancaire
	/// </summary>
	[XmlRoot(ElementName = "BankCard")]
	[DisplayName("Carte bancaire")]
	[Description("Paiement par carte bancaire")]
	[Category("payment")]
	public class BankCard : Paytype
	{

		#region Propriétés publiques

		private string _number = "";
		/// <summary>
		/// Numéro de la carte bancaire
		/// </summary>
		/// <value>Numéro de la carte bancaire.</value>
		[XmlElement(ElementName = "Number")]
		public string Number
		{
			get { return _number; }
			set
			{
				value = value.Trim().Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace("\t", "");

				if (value.Length > 18)
					value = value.Substring(0, 18);

				if (value != "" && !Regex.IsMatch(value, @"^(?:4[0-9]{12}(?:[0-9]{3})?|(?:5[1-5][0-9]{2}|222[1-9]|22[3-9][0-9]|2[3-6][0-9]{2}|27[01][0-9]|2720)[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|6(?:011|5[0-9]{2})[0-9]{12}|(?:2131|1800|35\d{3})\d{11})$", RegexOptions.IgnoreCase))
					throw new ArgumentException("Numéro de carte bancaire invalide.");

				if (value != _number)
				{
					_number = value;
					OnPropertyChanged();
				}
			}
		}

		private string _cvv = "";
		/// <summary>
		/// Numéro CVV/CCV
		/// </summary>
		/// <value>Numéro CVV/CCV.</value>
		[XmlElement(ElementName = "CVV")]
		public string CVV
		{
			get { return _cvv; }
			set
			{
				value = value.Trim().Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace("\t", "");

				if (value.Length > 4)
					value = value.Substring(0, 4);

				if (value != "" && !Regex.IsMatch(value, @"^[0-9]{3,4}$", RegexOptions.IgnoreCase))
					throw new ArgumentException("Numéro CVV/CCV invalide.");

				if (value != _cvv)
				{
					_cvv = value;
					OnPropertyChanged();
				}
			}
		}

		private CardDate _date = new CardDate(1970, 1);
		/// <summary>
		/// Date de validité (année, mois)
		/// </summary>
		/// <value>Date de validité (année, mois).</value>
		[XmlIgnore]
		public CardDate Date
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
		/// <summary>
		/// Date au format brute
		/// </summary>
		[XmlElement(ElementName = "Date")]
		public string FlatDate
		{
			get { return string.Format("{0}-{1}", Date.Year, Date.Month); }
			set
			{
				var vs = value.Split('-');
				try
				{
					int year = Convert.ToInt32(vs[0]);
					int month = Convert.ToInt32(vs[1]);
					Date = new CardDate(year, month);
				}
				catch
				{
					Date = new CardDate(1970, 1);
				}
			}
		}

		#endregion

		/// <summary>
		/// Constructeurs
		/// </summary>
		public BankCard()
			: base()
		{
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom donné à la carte bancaire.</param>
		public BankCard(string name)
			: base(name)
		{
		}

	}

	/// <summary>
	/// Paiement par chèque
	/// </summary>
	[XmlRoot(ElementName = "Check")]
	[DisplayName("Chèque")]
	[Description("Encaissement ou Paiement par chèque")]
	[Category("payment | collection")]
	public class Check : Paytype
	{

		#region Propriétés publiques

		private int _number = 0;
		/// <summary>
		/// Numéro du chèque
		/// </summary>
		/// <value>Numéro du chèque.</value>
		[XmlElement(ElementName = "Number")]
		public int Number
		{
			get { return _number; }
			set
			{
				value = Math.Abs(value);

				if (value != _number)
				{
					_number = value;
					OnPropertyChanged();
				}
			}
		}

		#endregion

		/// <summary>
		/// Constructeurs
		/// </summary>
		public Check()
			: base()
		{
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom donné au chèque.</param>
		public Check(string name)
			: base(name)
		{
		}

	}

	/// <summary>
	/// Paiement en espèces
	/// </summary>
	[XmlRoot(ElementName = "Money")]
	[DisplayName("Espèces")]
	[Description("Encaissement ou Paiement en espèces")]
	[Category("payment | collection")]
	public class Money : Paytype
	{

		#region Propriétés publiques
		#endregion

		/// <summary>
		/// Constructeurs
		/// </summary>
		public Money()
			: base()
		{
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom donné au paiment en espèces.</param>
		public Money(string name)
			: base(name)
		{
		}

	}

	/// <summary>
	/// Prélèvemenent bancaire
	/// </summary>
	[XmlRoot(ElementName = "BankDirectDebit")]
	[DisplayName("Pélèvement bancaire")]
	[Description("Paiement par prélèvement bancaire")]
	[Category("payment")]
	public class BankDirectDebit : Paytype
	{

		#region Propriétés publiques

		private static readonly XmlDocument _xmlDoc = new XmlDocument();

		private string _ref = "";
		/// <summary>
		/// Référence / Note du transfer.
		/// </summary>
		/// <value>Référence / Note du transfer, 4000 caractères maximum.</value>
		[XmlIgnore]
		public string Ref
		{
			get { return _ref; }
			set
			{
				value = value.Trim();
				if (value.Length > 4000)
					value = value.Substring(0, 4000);

				if (value != _ref)
				{
					_ref = value;
					OnPropertyChanged();
				}
			}
		}
		/// <summary>
		/// Référence au format brute
		/// </summary>
		[XmlElement(ElementName = "Ref")]
		public XmlCDataSection RefCData
		{
			get { return _xmlDoc.CreateCDataSection(Ref); }
			set { Ref = value.Data; }
		}

		#endregion

		/// <summary>
		/// Constructeurs
		/// </summary>
		public BankDirectDebit()
			: base()
		{
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom donné au prélèvement bancaire.</param>
		public BankDirectDebit(string name)
			: base(name)
		{
		}

	}

	/// <summary>
	/// Virement bancaire
	/// </summary>
	[XmlRoot(ElementName = "DirectTransfer")]
	[DisplayName("Virement bancaire")]
	[Description("Encaissement ou Paiement par virement bancaire")]
	[Category("payment | collection")]
	public class DirectTransfer : Paytype
	{

		#region Propriétés publiques

		private static readonly XmlDocument _xmlDoc = new XmlDocument();

		private string _ref = "";
		/// <summary>
		/// Référence / Note du transfer.
		/// </summary>
		/// <value>Référence / Note du transfer, 4000 caractères maximum.</value>
		[XmlIgnore]
		public string Ref
		{
			get { return _ref; }
			set
			{
				value = value.Trim();
				if (value.Length > 4000)
					value = value.Substring(0, 4000);

				if (value != _ref)
				{
					_ref = value;
					OnPropertyChanged();
				}
			}
		}
		/// <summary>
		/// Référence au format brute
		/// </summary>
		[XmlElement(ElementName = "Ref")]
		public XmlCDataSection RefCData
		{
			get { return _xmlDoc.CreateCDataSection(Ref); }
			set { Ref = value.Data; }
		}

		#endregion

		/// <summary>
		/// Constructeurs
		/// </summary>
		public DirectTransfer()
			: base()
		{
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom donné au virement bancaire.</param>
		public DirectTransfer(string name)
			: base(name)
		{
		}

	}

	/// <summary>
	/// Transfert d'argent entre 2 comptes bancaires
	/// </summary>
	[XmlRoot(ElementName = "LocalTransfer")]
	[DisplayName("Transfert entre comptes")]
	[Description("Encaissement ou Paiement par transfert entre compte")]
	[Category("payment | collection")]
	public class LocalTransfer : Paytype
	{

		#region Propriétés publiques

		private static readonly XmlDocument _xmlDoc = new XmlDocument();

		private string _ref = "";
		/// <summary>
		/// Référence / Note du transfer.
		/// </summary>
		/// <value>Référence / Note du transfer, 4000 caractères maximum.</value>
		[XmlIgnore]
		public string Ref
		{
			get { return _ref; }
			set
			{
				value = value.Trim();
				if (value.Length > 4000)
					value = value.Substring(0, 4000);

				if (value != _ref)
				{
					_ref = value;
					OnPropertyChanged();
				}
			}
		}
		/// <summary>
		/// Référence au format brute
		/// </summary>
		[XmlElement(ElementName = "Ref")]
		public XmlCDataSection RefCData
		{
			get { return _xmlDoc.CreateCDataSection(Ref); }
			set { Ref = value.Data; }
		}

		#endregion

		/// <summary>
		/// Constructeurs
		/// </summary>
		public LocalTransfer()
			: base()
		{
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom donné au transfert bancaire.</param>
		public LocalTransfer(string name)
			: base(name)
		{
		}

	}

	/// <summary>
	/// Dépot d'espèces
	/// </summary>
	[XmlRoot(ElementName = "Deposit")]
	[DisplayName("Dépot d'èspèces")]
	[Description("Encaissement par dépot d'espèces")]
	[Category("collection")]
	public class Deposit : Paytype
	{

		#region Propriétés publiques
		#endregion

		/// <summary>
		/// Constructeurs
		/// </summary>
		public Deposit()
			: base()
		{
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom donné au dépot d'espèces.</param>
		public Deposit(string name)
			: base(name)
		{
		}

	}

}
