//
//  Account.cs
//
//  Author:
//       Christophe LEMOINE <pantafle@tuta.io>
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
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Kotlib.Objects
{

	/// <summary>
	/// Moyen de paiement
	/// </summary>
	[XmlRoot(ElementName = "Account")]
	[XmlInclude(typeof(BankAccount))]
	[XmlInclude(typeof(Paycard))]
	[XmlInclude(typeof(Wallet))]
	[DisplayName("Elément bancaire personnalisé")]
	[Description("Représente un élément bancaire personnalisé")]
	public class Account: INotifyPropertyChanged
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
			if (name == null) {
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
		public Guid Id {
			get { return _id; }
			set {
				if (value != _id) {
					_id = value;
					OnPropertyChanged();
				}
			}
		}

		private string _name = "";
		/// <summary>
		/// Dénomination de l'élément bancaire
		/// </summary>
		/// <value>Nom, 255 caractères maximum.</value>
		[XmlElement(ElementName = "Name")]
		public string Name {
			get { return _name; }
			set {
				value = value.Trim();

				if (value.Length > 255)
					value = value.Substring(0, 255);

				if (value == "")
					throw new ArgumentException("Dénomination de l'élément bancaire requis.");

				if (value != _name) {
					_name = value;
					OnPropertyChanged();
				}
			}
		}
		/// <summary>
		/// Vérifie si la propriété est correctement définie avant d'être sérialisée
		/// </summary>
		/// <returns><c>true</c></returns>
		public bool ShouldSerializeName()
		{
			if (Name.Trim() == "")
				throw new ArgumentException("Dénomination de l'élément bancaire requis.");

			return true;
		}

		private PaytypeList _paytypes = null;
		/// <summary>
		/// Liste des moyens de paiements
		/// </summary>
		/// <value>Liste des moyens de paiements.</value>
		[XmlArray(ElementName = "Paytypes")]
		[XmlArrayItem(ElementName = "Paytype")]
		public PaytypeList Paytypes {
			get { return _paytypes; }
			set {
				if (value != null && value != _paytypes) {
					if (_paytypes != null)
						_paytypes.UpdatedEvent -= OnUpdated;

					_paytypes = value;
					_paytypes.UpdatedEvent += OnUpdated;
				}
			}
		}

		private double _initialamount = 0.0d;
		/// <summary>
		/// Solde initial
		/// </summary>
		/// <value>Solde initial.</value>
		[XmlAttribute(AttributeName = "initial_amount")]
		public double InitialAmount {
			get { return _initialamount; }
			set {
				if (!value.Equals(_initialamount)) {
					_initialamount = value;
					OnPropertyChanged();
				}
			}
		}

		private double _allowedcredit = 0.0d;
		/// <summary>
		/// Découvert autorisé
		/// </summary>
		/// <value>Découvert autorisé.</value>
		[XmlAttribute(AttributeName = "allowed_credit")]
		public double AllowedCredit {
			get { return _allowedcredit; }
			set {
				if (value < 0.0d)
					throw new ArgumentException("Le montant du découvert autorisé doit être positif.");

				if (!value.Equals(_allowedcredit)) {
					_allowedcredit = value;
					OnPropertyChanged();
				}
			}
		}

		private string _note = "";
		/// <summary>
		/// Notes appliquées à cet élément bancaire.
		/// </summary>
		/// <value>Notes, 4000 caractères maximum.</value>
		[XmlIgnore]
		public string Note {
			get { return _note; }
			set {
				value = value.Trim();
				if (value.Length > 4000)
					value = value.Substring(0, 4000);

				if (value != _note) {
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
		public XmlCDataSection NoteCData {
			get { return _xmlDoc.CreateCDataSection(Note); }
			set { Note = value.Data; }
		}

		private Identity _owner = null;
		/// <summary>
		/// Propriétaire de cet élément bancaire
		/// </summary>
		/// <value>Propriétaire de cet élément bancaire.</value>
		[XmlElement(ElementName = "Owner")]
		public Identity Owner {
			get { return _owner; }
			set {
				if (value == null)
					throw new ArgumentException("Une identité correcte est requise pour le propriétaire de cet élément bancaire.");

				if (value != _owner) {
					_owner = value;
					OnPropertyChanged();
				}
			}
		}
		/// <summary>
		/// Vérifie si la propriété est correctement définie avant d'être sérialisée
		/// </summary>
		/// <returns><c>true</c></returns>
		public bool ShouldSerializeOwner()
		{
			if (Owner == null)
				throw new ArgumentException("Une identité correcte est requise pour le propriétaire de cet élément bancaire.");

			return true;
		}

		private string _culture = CultureInfo.CurrentCulture.Name;
		/// <summary>
		/// Culture de l'élément bancaire
		/// </summary>
		/// <value>Culture de l'élément bancaire.</value>
		[XmlAttribute(AttributeName = "culture")]
		public string Culture {
			get { return _culture; }
			set {
				value = value.Trim();
				if (value.ToLower() != _culture.ToLower()) {
					try {
						var ci = new CultureInfo(value);
						_culture = ci.Name;
						ci = null;
						OnPropertyChanged();
					} catch {
						throw new ArgumentException("La culture employée pour ce compte est incorrecte.");
					}
				}
			}
		}

		#endregion

		/// <summary>
		/// Constructeurs
		/// </summary>
		public Account()
		{
			Id = Guid.NewGuid();
			Paytypes = PaytypeList.Empty;
			InitialAmount = 0.0d;
			AllowedCredit = 0.0d;
			Culture = CultureInfo.CurrentCulture.Name;
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom de l'élément bancaire.</param>
		/// <param name="owner">Identité du propriétaire.</param>
		public Account(string name, Identity owner)
			: this()
		{
			Name = name;
			Owner = owner;
		}

	}

	/// <summary>
	/// Compte bancaire
	/// </summary>
	[XmlRoot(ElementName = "BankAccount")]
	[DisplayName("Compte bancaire")]
	[Description("Représente un compte bancaire")]
	public class BankAccount: Account
	{

		#region Propriétés publiques

		private string _bankname = "";
		/// <summary>
		/// Dénomination de la banque
		/// </summary>
		/// <value>Nom, 255 caractères maximum.</value>
		[XmlElement(ElementName = "BankName")]
		public string BankName {
			get { return _bankname; }
			set {
				value = value.Trim();

				if (value.Length > 255)
					value = value.Substring(0, 255);

				if (value == "")
					throw new ArgumentException("Dénomination de la banque requise.");

				if (value != _bankname) {
					_bankname = value;
					OnPropertyChanged();
				}
			}
		}
		/// <summary>
		/// Vérifie si la propriété est correctement définie avant d'être sérialisée
		/// </summary>
		/// <returns><c>true</c></returns>
		public bool ShouldSerializeBankName()
		{
			if (Owner == null)
				throw new ArgumentException("Dénomination de la banque requise.");

			return true;
		}

		private string _iban = "";
		/// <summary>
		/// Iban
		/// </summary>
		/// <value>Iban.</value>
		[XmlElement(ElementName = "Iban")]
		public string Iban {
			get { return _iban; }
			set {
				value = value.Trim().Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace("\t", "");

				if (value.Length > 34)
					value = value.Substring(0, 34);

				if (value != "" && !Regex.IsMatch(value, @"^(?:(?:IT|SM)\d{2}[A-Z]\d{22}|CY\d{2}[A-Z]\d{23}|NL\d{2}[A-Z]{4}\d{10}|LV\d{2}[A-Z]{4}\d{13}|(?:BG|BH|GB|IE)\d{2}[A-Z]{4}\d{14}|GI\d{2}[A-Z]{4}\d{15}|RO\d{2}[A-Z]{4}\d{16}|KW\d{2}[A-Z]{4}\d{22}|MT\d{2}[A-Z]{4}\d{23}|NO\d{13}|(?:DK|FI|GL|FO)\d{16}|MK\d{17}|(?:AT|EE|KZ|LU|XK)\d{18}|(?:BA|HR|LI|CH|CR)\d{19}|(?:GE|DE|LT|ME|RS)\d{20}|IL\d{21}|(?:AD|CZ|ES|MD|SA)\d{22}|PT\d{23}|(?:BE|IS)\d{24}|(?:FR|MR|MC)\d{25}|(?:AL|DO|LB|PL)\d{26}|(?:AZ|HU)\d{27}|(?:GR|MU)\d{28})$", RegexOptions.IgnoreCase))
					throw new ArgumentException("Iban incorrect.");

				if (value != _iban) {
					_iban = value;
					OnPropertyChanged();
				}
			}
		}

		private string _bic = "";
		/// <summary>
		/// Bic
		/// </summary>
		/// <value>Bic.</value>
		[XmlElement(ElementName = "Bic")]
		public string Bic {
			get { return _bic; }
			set {
				value = value.Trim().Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace("\t", "");

				if (value.Length > 34)
					value = value.Substring(0, 34);

				if (value != "" && !Regex.IsMatch(value, @"^{6}[a-z]{2}[0-9a-z]{2}([0-9a-z]{4})?\z", RegexOptions.IgnoreCase))
					throw new ArgumentException("Bic incorrect.");

				if (value != _bic) {
					_bic = value;
					OnPropertyChanged();
				}
			}
		}

		private Identity _contact = null;
		/// <summary>
		/// Coordonnées et contact
		/// </summary>
		/// <value>Coordonnées et contact.</value>
		[XmlElement(ElementName = "Contact")]
		public Identity Contact {
			get { return _contact; }
			set {
				if (value != _contact) {
					_contact = value;
					OnPropertyChanged();
				}
			}
		}

		#endregion

		/// <summary>
		/// Constructeur
		/// </summary>
		public BankAccount()
			: base()
		{
			Contact = null;
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom du compte en banque.</param>
		/// <param name="owner">Identité du propriétaire.</param>
		public BankAccount(string name, Identity owner)
			: base(name, owner)
		{
		}

	}

	/// <summary>
	/// Carte de paiement
	/// </summary>
	[XmlRoot(ElementName = "Paycard")]
	[DisplayName("Carte de paiement")]
	[Description("Représente une carte de paiement, telle Moneybookers, PCS, etc.")]
	public class Paycard: Account
	{

		#region Propriétés publiques

		private BankCard _card = null;
		/// <summary>
		/// Informations de la carte de paiement
		/// </summary>
		/// <value>Informations de la carte de paiement.</value>
		[XmlElement(ElementName = "Card")]
		public BankCard Card {
			get { return _card; }
			set {
				if (value == null)
					throw new ArgumentException("Les informations de la carte de paiement sont requises.");

				if (value != _card) {
					_card = value;
					OnPropertyChanged();
				}
			}
		}
		/// <summary>
		/// Vérifie si la propriété est correctement définie avant d'être sérialisée
		/// </summary>
		/// <returns><c>true</c></returns>
		public bool ShouldSerializeCard()
		{
			if (Card == null)
				throw new ArgumentException("Les informations de la carte de paiement sont requises.");

			return true;
		}

		#endregion

		/// <summary>
		/// Constructeur
		/// </summary>
		public Paycard()
			: base()
		{
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom du moyen de paiement.</param>
		/// <param name="owner">Identité du propriétaire.</param>
		/// <param name="card">Informations de la carte de paiement</param>
		public Paycard(string name, Identity owner, BankCard card)
			: base(name, owner)
		{
			Card = card;
		}

	}

	/// <summary>
	/// Carte bancaire
	/// </summary>
	[XmlRoot(ElementName = "Wallet")]
	[DisplayName("Portefeuille d'espèces")]
	[Description("Représente votre poche, un portefeille, un porte monnaie ou votre matela")]
	public class Wallet: Account
	{

		#region Propriétés publiques

		private bool _electronic = false;
		/// <summary>
		/// Portefeuille électronique
		/// </summary>
		/// <value><c>true</c> si c'est un portefeuille électronique; sinon, <c>false</c>.</value>
		[XmlAttribute(AttributeName = "electronic")]
		public bool Electronic {
			get { return _electronic; }
			set {
				if (value != _electronic) {
					_electronic = value;
					OnPropertyChanged();
				}
			}
		}

		#endregion

		/// <summary>
		/// Constructeurs
		/// </summary>
		public Wallet()
			: base()
		{
			AllowedCredit = 0.0d;
			InitialAmount = (double)Math.Abs(InitialAmount);
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom donné au portefeuille d'espèces.</param>
		/// <param name="owner">Identité du propriétaire</param>
		public Wallet(string name, Identity owner)
			: base(name, owner)
		{
			AllowedCredit = 0.0d;
			InitialAmount = (double)Math.Abs(InitialAmount);
		}
	}

}
