//
//  Financial.cs
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Kotlib.Objects;

namespace Kotlib
{

	/// <summary>
	/// Représente la date de validité d'une carte bancaire
	/// </summary>
	[XmlRoot("Date")]
	public class CardDate
	{
		
		private int _year = 1970;
		/// <summary>
		/// Année de validité
		/// </summary>
		[XmlAttribute("year")]
		public int Year { 
			get {
				return _year;
			}
			set {
				if (value < 1970)
					throw new ArgumentException("L'année de validité d'une carte bancaire doit être supérieure à 1970");
				
				_year = value;
			}
		}
		
		private int _month = 1;
		/// <summary>
		/// Mois de validité
		/// </summary>
		[XmlAttribute("month")]
		public int Month {
			get {
				return _month;
			}
			set {
				if (value < 1 || value > 12)
					throw new ArgumentException("Le mois de validité d'une carte bancaire doit être compris entre Janvier (1) et Décembre (12)");
				
				_month = value;
			}
		}
		
		/// <summary>
		/// Constructeur
		/// </summary>
		public CardDate()
		{
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="year">Année de validité</param>
		/// <param name="month">Mois de validité</param>
		public CardDate(int year, int month)
		{
			Year = year;
			Month = month;
		}
		
	}
	
	/// <summary>
	/// Dossier financier
	/// </summary>
	[XmlRoot(ElementName = "Financial")]
	public class Financial : Core.Serializable, INotifyPropertyChanged
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
			Updated = DateTime.Now;
            
			if (PropertyChanged != null)
				PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
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
		/// Dénomination du dossier financier
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
					throw new ArgumentException("Dénomination du dossier financier requise.");

				if (value != _name) {
					_name = value;
					OnPropertyChanged();
				}
			}
		}
		/// <summary>
		/// Verifie que la propriété est correcte avant d'être sérialisée
		/// </summary>
		/// <returns><c>true</c></returns>
		public bool ShouldSerializeName()
		{
			if (Name.Trim() == "")
				throw new ArgumentException("Dénomination du dossier financier requise.");

			return true;
		}

		private DateTime _created = DateTime.Now;
		/// <summary>
		/// Date de création
		/// </summary>
		/// <value>Date de création.</value>
		[XmlAttribute(AttributeName = "created")]
		public DateTime Created {
			get { return _created; }
			set {
				if (value != _created) {
					_created = value;
					OnPropertyChanged();
				}
			}
		}

		private DateTime _updated = DateTime.Now;
		/// <summary>
		/// Date de la dernière modification
		/// </summary>
		/// <value>Date de modification.</value>
		[XmlAttribute(AttributeName = "updated")]
		public DateTime Updated {
			get { return _updated; }
			set {
				if (value != _updated) {
					_updated = value;
					if (_updated < _created)
						_updated = _created;

					OnUpdated(this, new EventArgs());
				}
			}
		}

		private string _note = "";
		/// <summary>
		/// Notes appliquées au dossier financier.
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
		/// Note au format données brutes
		/// </summary>
		[XmlElement(ElementName = "Note")]
		public XmlCDataSection NoteCData {
			get { return _xmlDoc.CreateCDataSection(Note); }
			set { Note = value.Data; }
		}

		private Identity _owner = null;
		/// <summary>
		/// Propriétaire du dossier financier
		/// </summary>
		/// <value>Propriétaire du dossier financier.</value>
		[XmlElement(ElementName = "Owner")]
		public Identity Owner {
			get { return _owner; }
			set {
				if (value == null)
					throw new ArgumentException("Une identité correcte est requise pour le propriétaire du dossier financier.");

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
				throw new ArgumentException("Une identité correcte est requise pour le propriétaire du dossier financier.");

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

		private CategoryList _categories = null;
		/// <summary>
		/// Liste des catégories
		/// </summary>
		/// <value>Liste des catégories.</value>
		[XmlArray(ElementName = "Categories")]
		[XmlArrayItem(ElementName = "Category")]
		public CategoryList Categories {
			get { return _categories; }
			set {
				if (value != null && value != _categories) {
					if (_categories != null)
						_categories.UpdatedEvent -= OnUpdated;

					_categories = value;
					_categories.UpdatedEvent += OnUpdated;
				}
			}
		}

		private ThirdpartyList _thirdparties = null;
		/// <summary>
		/// Liste des tiers
		/// </summary>
		/// <value>Liste des tiers.</value>
		[XmlArray(ElementName = "Thirdparties")]
		[XmlArrayItem(ElementName = "Identity")]
		public ThirdpartyList Thirdparties {
			get { return _thirdparties; }
			set {
				if (value != null && value != _thirdparties) {
					if (_thirdparties != null)
						_thirdparties.UpdatedEvent -= OnUpdated;

					_thirdparties = value;
					_thirdparties.UpdatedEvent += OnUpdated;
				}
			}
		}

		private AccountList _accounts = null;
		/// <summary>
		/// Liste des éléments bancaires
		/// </summary>
		/// <value>Liste des éléments bancaires.</value>
		[XmlArray(ElementName = "Accounts")]
		[XmlArrayItem(ElementName = "Account")]
		public AccountList Accounts {
			get { return _accounts; }
			set {
				if (value != null && value != _accounts) {
					if (_accounts != null)
						_accounts.UpdatedEvent -= OnUpdated;

					_accounts = value;
					_accounts.UpdatedEvent += OnUpdated;
				}
			}
		}

		#endregion

		/// <summary>
		/// Constructeurs
		/// </summary>
		public Financial()
		{
			Id = Guid.NewGuid();
			Created = DateTime.Now;
			Paytypes = PaytypeList.Empty;
			Categories = CategoryList.Empty;
			Thirdparties = ThirdpartyList.Empty;
			Accounts = AccountList.Empty;
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom du dossier financier.</param>
		/// <param name="owner">Identité du propriétaire.</param>
		public Financial(string name, Identity owner)
			: this()
		{
			Name = name;
			Owner = owner;
		}

		/// <summary>
		/// Sauvegarde ce dossier financier
		/// </summary>
		/// <param name="password">Mot de passe pour le cryptage des données, laisser vide pour ne pas crypter</param>
		/// <returns>Données brutes représentant le dossier financier</returns>
		public byte[] Save(string password = "")
		{
			var datas = Serialize();
			datas = Core.Compression.Compress(datas);

			if (password.Trim() != "")
				datas = Core.Crypto.Encrypt(datas, password);

			return datas;
		}

		/// <summary>
		/// Sauvegarde ce dossier financier dans un fichier
		/// </summary>
		/// <param name="directory">Chemin du répertoire recevant le dossier financier</param>
		/// <param name="password">Mot de passe pour le cryptage des données, laisser vide pour ne pas crypter</param>
		public string SaveToFile(string directory, string password = "")
		{
			var d = directory.Trim().Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.Personal));
			if (!Directory.Exists(d))
				throw new DirectoryNotFoundException();

			var filename = String.Join("_", Name.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');

			var fp = Path.Combine(d, filename) + ".kot";
			fp = Path.GetFullPath(fp);

			var datas = Save(password);

			File.WriteAllBytes(fp, datas);
			return fp;
		}

		/// <summary>
		/// Transforme les données brutes en dossier financier
		/// </summary>
		/// <param name="datas">Données brutes à traiter</param>
		/// <param name="password">Mot de passe pour le décryptage des données, laisser vide pour ne pas décrypter</param>
		/// <returns>Dossier financier résultant des données brutes passées en paramètre</returns>
		public static Financial Load(byte[] datas, string password = "")
		{
			if (password.Trim() != "")
				datas = Core.Crypto.Decrypt(datas, password);

			datas = Core.Compression.Decompress(datas);
			return Deserialize<Financial>(datas);
		}

		/// <summary>
		/// Transforme les données brutes d'un fichier en dossier financier
		/// </summary>
		/// <param name="filepath">Chemin du fichier à traiter</param>
		/// <param name="password">Mot de passe pour le décryptage des données, laisser vide pour ne pas décrypter</param>
		/// <returns>Tâche permettant la conversion du fichier en dossier financier</returns>
		public static Financial LoadFromFile(string filepath, string password = "")
		{
			if (!File.Exists(filepath))
				throw new FileNotFoundException();

			var datas = File.ReadAllBytes(filepath);
			return Load(datas, password);
		}

		/// <summary>
		/// Créé un nouveau dossier financier
		/// </summary>
		/// <param name="name">Nom du dossier financier</param>
		/// <param name="owner">Identité du propriétaire</param>
		/// <param name="accounts">Liste des éléments bancaires rattachés au dossier financier</param>
		/// <param name="paytypes">Liste des moyens financiers, optionnel</param>
		/// <param name="categories">Liste des catégories, optionnel</param>
		/// <param name="thirdparties">Liste des tiers, optionnel</param>
		/// <param name="note">Note apposée au dossier financier, optionnel</param>
		/// <param name="loadDefaults"><c>true</c>, charge les valeurs par défaut, sinon <c>false</c>, créé un dossier vide, optionnel</param>
		/// <returns>Dossier financier nouvellement créé</returns>
		public static Financial Create(string name,
			Identity owner,
			AccountList accounts,
			PaytypeList paytypes = null,
			CategoryList categories = null,
			ThirdpartyList thirdparties = null,
			string note = "",
			bool loadDefaults = false)
		{
			return new Financial(name, owner) {
				Created = DateTime.Now,
				Updated = DateTime.Now,
				Accounts = accounts,
				Thirdparties = thirdparties ?? new ThirdpartyList() { owner },
				Paytypes = paytypes ?? (loadDefaults ? PaytypeList.Defaults : PaytypeList.Empty),
				Categories = categories ?? (loadDefaults ? CategoryList.Defaults : CategoryList.Empty),
				Note = note.Trim() == "" ? (loadDefaults ? "Modèle par défaut d'un dossier financier" : "") : note
			};
		}
        
	}
    
}
