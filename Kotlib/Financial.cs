//
//  Financial.cs
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
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Kotlib.Objects;
using Kotlib.Tools;
using System.Reflection;
using System.Linq.Expressions;


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
        public int Year
        {
            get
            {
                return _year;
            }
            set
            {
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
        public int Month
        {
            get
            {
                return _month;
            }
            set
            {
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
            if (name == null)
            {
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
        /// <param name="sender">Objet emettant le signal</param>
        /// <param name="e">Arguments</param>
        public void OnUpdated(object sender, EventArgs e)
        {
            if (UpdatedEvent != null)
                UpdatedEvent.Invoke(sender, e);
        }

        /// <summary>
        /// Informe que le dossier financier viend etre sauvegardé
        /// </summary>
        /// <param name="sender">Objet emettant le signal</param>
        /// <param name="e">Arguments</param>
        public void OnSaved(object sender, EventArgs e)
        {
            if (SavedEvent != null)
                SavedEvent.Invoke(sender, e);
        }

        /// <summary>
        /// Informe qu'un événement programmé vient d'être posté
        /// </summary>
        /// <param name="date">Date programmée</param>
        /// <param name="postEvent">Evénement et ses détails</param>
        public void OnPostRaised(DateTime date, Event postEvent)
        {
            if (PostRaisedEvent != null)
                PostRaisedEvent(date, postEvent);
        }

        /// <summary>
        /// S'execute lorsque qu'un événement est posté
        /// </summary>
        /// <param name="date">Date de l'occurence</param>
        /// <param name="postEvent">Occurence et ses informations</param>
        private async void EventPosted(DateTime date, Event postEvent)
        {
            await Task.Run(() =>
                {
                    var ea_type = postEvent.EventAction.GetType();
                    if (ea_type == typeof(Operation))
                    {
                        var operation = (Operation)postEvent.EventAction;
                        operation.Date = date;
                        var to = Accounts.GetById(operation.ToId);
                        if (!to.Equals(default(Account)))
                        {
                            to.Operations.Add(operation);
                            OnPostRaised(date, postEvent);
                        }
                    }
                    else if (ea_type == typeof(Transfer))
                    {
                        var transfer = (Transfer)postEvent.EventAction;
                        transfer.Date = date;
                        var to = Accounts.GetById(transfer.ToAccountId);
                        var from = Accounts.GetById(transfer.FromAccountId);
                        if (!to.Equals(default(Account)) && !from.Equals(default(Account)))
                        {
                            Accounts.Transfers.Add(transfer);
                            OnPostRaised(date, postEvent);
                        }
                    }
                });
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

        /// <summary>
        /// Se produit lorsque le dossier financier est sauvegardé
        /// </summary>
        public event EventHandler SavedEvent;

        /// <summary>
        /// Se produit lorsque qu'un événement programmé est posté
        /// </summary>
        public event Event.PostDelegate PostRaisedEvent;

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
        /// Dénomination du dossier financier
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
                    throw new ArgumentException("Dénomination du dossier financier requise.");

                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Verifie que la propriété est correcte avant d'être sérialisée
        /// </summary>
        /// <returns><c>true</c></returns>
        private bool ShouldSerializeName()
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
        public DateTime Created
        {
            get { return _created; }
            set
            {
                if (value != _created)
                {
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
        public DateTime Updated
        {
            get { return _updated; }
            set
            {
                if (value != _updated)
                {
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
        /// Note au format données brutes
        /// </summary>
        [XmlElement(ElementName = "Note")]
        public XmlCDataSection NoteCData
        {
            get { return _xmlDoc.CreateCDataSection(Note); }
            set { Note = value.Data; }
        }

        private Identity _owner = null;
        /// <summary>
        /// Propriétaire du dossier financier
        /// </summary>
        /// <value>Propriétaire du dossier financier.</value>
        [XmlElement(ElementName = "Owner")]
        public Identity Owner
        {
            get { return _owner; }
            set
            {
                if (value == null)
                    throw new ArgumentException("Une identité correcte est requise pour le propriétaire du dossier financier.");

                if (value != _owner)
                {
                    _owner = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Vérifie si la propriété est correctement définie avant d'être sérialisée
        /// </summary>
        /// <returns><c>true</c></returns>
        private bool ShouldSerializeOwner()
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
        public PaytypeList Paytypes
        {
            get { return _paytypes; }
            set
            {
                if (value != null && value != _paytypes)
                {
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
        public CategoryList Categories
        {
            get { return _categories; }
            set
            {
                if (value != null && value != _categories)
                {
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
        public ThirdpartyList Thirdparties
        {
            get { return _thirdparties; }
            set
            {
                if (value != null && value != _thirdparties)
                {
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
        public AccountList Accounts
        {
            get { return _accounts; }
            set
            {
                if (value != null && value != _accounts)
                {
                    if (_accounts != null)
                        _accounts.UpdatedEvent -= OnUpdated;

                    _accounts = value;
                    _accounts.UpdatedEvent += OnUpdated;
                }
            }
        }

        private EventList _events = null;
        /// <summary>
        /// Liste des événements programmés
        /// </summary>
        /// <value>Liste des événements programmés.</value>
        [XmlArray(ElementName = "Events")]
        [XmlArrayItem(ElementName = "Event")]
        public EventList Events
        {
            get { return _events; }
            set
            {
                if (value != null && value != _events)
                {
                    if (_events != null)
                    {
                        _events.UpdatedEvent -= OnUpdated;
                        _events.PostRaisedEvent -= EventPosted;
                    }

                    _events = value;
                    _events.UpdatedEvent += OnUpdated;
                    _events.PostRaisedEvent += EventPosted;
                }
            }
        }

        private string _cultureName = CultureInfo.CurrentCulture.Name;
        private Currency _currency = null;
        /// <summary>
        /// Culture de l'élément bancaire
        /// </summary>
        /// <value>Culture de l'élément bancaire.</value>
        [XmlAttribute(AttributeName = "culture")]
        public string CultureName
        {
            get { return _cultureName; }
            set
            {
                value = value.Trim();
                if (value.ToLower() != _cultureName.ToLower())
                {
                    try
                    {
                        value = value.Replace("-", "_").Replace(" ", "");
                        var cs = value.Split('_');
                        value = cs[0].ToLower();
                        if (cs.Length == 2)
                            value += "_" + cs[1].ToUpper();

                        // on regarde si le code tourne sous linux,
                        // si oui, on modifie l'écriture du nom de la culture
                        int p = (int)Environment.OSVersion.Platform;
                        if (p == 4 || p == 6 || p == 128)
                            value = value.Replace("_", "-");

                        var ci = new CultureInfo(value);
                        _cultureName = ci.Name;
                        _currency = new Currency(ci);
                        ci = null;
                        OnPropertyChanged();
                    }
                    catch
                    {
                        throw new ArgumentException("La culture employée pour ce dossier financier est incorrecte.");
                    }
                }
            }
        }
        /// <summary>
        /// Retourne la culture du dossier financier
        /// </summary>
        public Currency Currency
        {
            get
            {
                if (_currency == null)
                    _currency = new Currency(new CultureInfo(CultureName));

                return _currency;
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

            OnSaved(this, new EventArgs());

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
            var fi = Deserialize<Financial>(datas);

            fi.Accounts.CleanTransfers();
            return fi;
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
        /// <param name="cultureName">Nom de la culture employée définissant la monnaie utilisée dans ce dossier financier: eg: fr_FR, en_US</param>
        /// <param name="paytypes">Liste des moyens financiers, optionnel</param>
        /// <param name="categories">Liste des catégories, optionnel</param>
        /// <param name="thirdparties">Liste des tiers, optionnel</param>
        /// <param name="events">Liste d'événements programmés</param>
        /// <param name="note">Note apposée au dossier financier, optionnel</param>
        /// <param name="loadDefaults"><c>true</c>, charge les valeurs par défaut, sinon <c>false</c>, créé un dossier vide, optionnel</param>
        /// <returns>Dossier financier nouvellement créé</returns>
        public static Financial Create(string name,
            Identity owner,
            AccountList accounts,
            string cultureName = "fr_FR",
            PaytypeList paytypes = null,
            CategoryList categories = null,
            ThirdpartyList thirdparties = null,
            EventList events = null,
            string note = "",
            bool loadDefaults = false)
        {
            return new Financial(name, owner)
            {
                Created = DateTime.Now,
                Updated = DateTime.Now,
                Accounts = accounts,
                CultureName = cultureName,
                Thirdparties = thirdparties ?? new ThirdpartyList() { owner },
                Paytypes = paytypes ?? (loadDefaults ? PaytypeList.Defaults : PaytypeList.Empty),
                Categories = categories ?? (loadDefaults ? CategoryList.Defaults : CategoryList.Empty),
                Note = note.Trim() == "" ? (loadDefaults ? "Modèle par défaut d'un dossier financier" : "") : note,
                Events = events ?? (loadDefaults ? EventList.Defaults : EventList.Empty),
            };
        }

        /// <summary>
        /// Retourne le solde total (opérations et transferts inclus) d'un élément bancaire à la date spécifiée
        /// </summary>
        /// <param name="account">Elément bancaire concerné</param>
        /// <param name="date">Date du solde</param>
        /// <param name="addInitialAmount"><c>true</c>, ajoute le solde initial, sinon, <c>false</c></param>
        /// <returns>Solde total</returns>
        public decimal AmountAt(Account account, DateTime date, bool addInitialAmount = true)
        {
            var amount_account = account.PartialAmountAt(date, addInitialAmount: false);
            var amount_transfers = Accounts.Transfers.PartialAmountAt(account, date, addInitialAmount: false);

            return (addInitialAmount ? account.InitialAmount : 0) + amount_account + amount_transfers;
        }

        /// <summary>
        /// Retourne le solde total de tous les éléments bancaires ainsi que tous les transferts associés
        /// </summary>
        /// <param name="date">Date de solde</param>
        /// <param name="addInitialAmount"><c>true</c>, ajoute le solde initial, sinon, <c>false</c></param>
        /// <returns>Solde total</returns>
        public decimal AmountAt(DateTime date, bool addInitialAmount = true)
        {
            var amounts = 0m;

            Accounts.Items.ForEach(a =>
                {
                    amounts += AmountAt(a, date, addInitialAmount: false);

                    if (addInitialAmount)
                        amounts += a.InitialAmount;
                }
            );

            return amounts;
        }

        /// <summary>
        /// Poste toutes les prochaines occurences programmées 
        /// </summary>
        public void AutoPost()
        {
            Events.Items.ForEach(e => e.Post());
        }

        /// <summary>
        /// Poste toutes les occurences programmées jusqu'a la date spécifiée
        /// </summary>
        public void AutoPostUntil(DateTime date)
        {
            Events.Items.ForEach(e => e.PostUntil(date));
        }

        /// <summary>
        /// Poste toutes les occurences restantes programmées 
        /// </summary>
        public void AutoPostOverdue()
        {
            Events.Items.ForEach(e => e.PostOverdue());
        }

        /// <summary>
        /// Poste toutes les occurences programmées 
        /// </summary>
        public void AutoPostAll()
        {
            Events.Items.ForEach(e => e.PostAll());
        }

        /// <summary>
        /// Retourne les informations sur les événements programmés pour une date souhaité
        /// </summary>
        /// <param name="accountID">Identifiant unique de l'élément bancaire concerné par les événements programmés</param>
        /// <param name="date">Date pour laquelle les informations sont retournées</param>
        /// <returns>Structure contenant dans l'ordre, le total des revenus, le total des dépenses, le nombre d'événements programmés ce jour</returns>
        public Tuple<decimal, decimal, int> GetEventsInfosAt(Guid accountID, DateTime date)
        {
            decimal i = 0m;
            decimal e = 0m;
            int c = 0;

            Events.Items
                .Where(x => x.GetCalendar().Contains(date.Date) && x.EventAction.GetType() == typeof(Operation))
                .Select(x => x.EventAction as Operation)
                .ToList()
                .ForEach(x =>
                {
                    if (x.ToId.Equals(accountID))
                    {
                        if (x.Amount < 0)
                            e += Math.Abs(x.Amount);
                        else
                            i += Math.Abs(x.Amount);

                        c++;
                    }
                });

            Events.Items
                .Where(x => x.GetCalendar().Contains(date.Date) && x.EventAction.GetType() == typeof(Transfer))
                .Select(x => x.EventAction as Transfer)
                .ToList()
                .ForEach(x =>
                {
                    if (x.FromAccountId.Equals(accountID))
                    {
                        e += Math.Abs(x.Amount);
                        c++;
                    }

                    if (x.ToAccountId.Equals(accountID))
                    {
                        i += Math.Abs(x.Amount);
                        c++;
                    }
                });

            return new Tuple<decimal, decimal, int>(i, e, c);
        }

        /// <summary>
        /// Retourne les informations sur les événements programmés entre les dates souhaitées
        /// </summary>
        /// <param name="accountID">Identifiant unique de l'élément bancaire concerné par les événements programmés</param>
        /// <param name="startdate">Date de début, incluse</param>
        /// <param name="enddate">Date de fin, incluse</param>
        /// <returns>Structure contenant dans l'ordre, le total des revenus, le total des dépenses, le nombre d'événements programmés ce jour</returns>
        public Tuple<decimal, decimal, int> GetEventsInfosAt(Guid accountID, DateTime startdate, DateTime enddate)
        {
            decimal i = 0m;
            decimal e = 0m;
            int c = 0;

            Events.Items
                .Where(x => x.GetCalendar().Count(d => d >= startdate.Date && d <= enddate.Date) > 0 && x.EventAction.GetType() == typeof(Operation))
                .Select(x => x)
                .ToList()
                .ForEach(x =>
                {
                    var count = x.GetCalendar().Count(d => d >= startdate.Date && d <= enddate.Date);
                    var operation = x.EventAction as Operation;
                    if (operation.ToId.Equals(accountID))
                    {
                        if (operation.Amount < 0)
                            e += (Math.Abs(operation.Amount) * count);
                        else
                            i += (Math.Abs(operation.Amount) * count);

                        c += count;
                    }
                });

            Events.Items
                .Where(x => x.GetCalendar().Count(d => d >= startdate.Date && d <= enddate.Date) > 0 && x.EventAction.GetType() == typeof(Transfer))
                .Select(x => x)
                .ToList()
                .ForEach(x =>
                {
                    var count = x.GetCalendar().Count(d => d >= startdate.Date && d <= enddate.Date);
                    var transfer = x.EventAction as Transfer;
                    if (transfer.FromAccountId.Equals(accountID))
                    {
                        e += (Math.Abs(transfer.Amount) * count);
                        c += count;
                    }

                    if (transfer.ToAccountId.Equals(accountID))
                    {
                        i += (Math.Abs(transfer.Amount) * count);
                        c += count;
                    }
                });

            return new Tuple<decimal, decimal, int>(i, e, c);
        }

        /// <summary>
        /// Exporte les mouvements d'un élément bancaire au format CSV
        /// </summary>
        /// <returns>
        /// Liste de structures comprenant chacune:
        /// - Item 1: Identifiant unique de l'élément bancaire
        /// - Item 2: Chemin du fichier CSV.
        /// </returns>
        /// <param name="directory">Chemin du répertoire recevant les fichiers CSV.</param>
        /// <param name="accountsId">Liste d'identifiants uniques correspondant aux éléments bancaires à traiter.</param>
        /// <param name="startDate">Date de début des mouvements pris en compte.</param>
        /// <param name="endDate">Date de fin des mouvements pris en compte.</param>
        /// <param name="delimiter">Délimiteur de colones CSV.</param>
        /// <param name="decimalSeparator">Délimiteur des décimales.</param>
        /// <param name="dateFormat">Format des dates enregistrées.</param>
        public List<Tuple<Guid, string>> ToCSV(string directory, List<Guid> accountsId, DateTime startDate, DateTime endDate, string delimiter = ";", string decimalSeparator = ",", string dateFormat = "dd/MM/yyyy")
        {
            var d = directory.Trim().Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            if (!Directory.Exists(d))
                throw new DirectoryNotFoundException();

            var filenames = new List<Tuple<Guid, string>>();

            var columns = new string[] { "Date", "Débit", "Crédit", "Libellé" };
            foreach (var uid in accountsId)
            {
                var a = Accounts.GetById(uid);
                if (a == null || a.Equals(default(Account)))
                    continue;

                var operations = a.Operations.Items
                .Where(o => o.Active && o.Date.Date >= startDate.Date && o.Date.Date <= endDate.Date)
                .Select(o => new Dictionary<string, string>()
                {
                    ["Sorter"] = o.Date.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    [columns[0]] = o.Date.Date.ToString(dateFormat),
                    [columns[1]] = (o.Amount < 0m) ? Math.Round(o.Amount, 2).ToString("0.00").Replace(".", decimalSeparator) : "0.00",
                    [columns[2]] = (o.Amount >= 0m) ? Math.Round(o.Amount, 2).ToString("0.00").Replace(".", decimalSeparator) : "0.00",
                    [columns[3]] = o.Name
                })
                .ToList();

                var transfersOut = Accounts.Transfers.Items
                .Where(t => t.Active && t.FromAccountId.Equals(a.Id) && t.Date.Date >= startDate.Date && t.Date.Date <= endDate.Date)
                .Select(t => new Dictionary<string, string>()
                {
                    ["Sorter"] = t.Date.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    [columns[0]] = t.Date.Date.ToString(dateFormat),
                    [columns[1]] = "-" + Math.Round(Math.Abs(t.Amount), 2).ToString("0.00").Replace(".", decimalSeparator),
                    [columns[2]] = "0.00",
                    [columns[3]] = t.Name
                })
                .ToList();

                var transfersIn = Accounts.Transfers.Items
                .Where(t => t.Active && t.ToAccountId.Equals(a.Id) && t.Date.Date >= startDate.Date && t.Date.Date <= endDate.Date)
                .Select(t => new Dictionary<string, string>()
                {
                    ["Sorter"] = t.Date.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    [columns[0]] = t.Date.Date.ToString(dateFormat),
                    [columns[1]] = "0.00",
                    [columns[2]] = Math.Round(Math.Abs(t.Amount), 2).ToString("0.00").Replace(".", decimalSeparator),
                    [columns[3]] = t.Name
                })
                .ToList();

                var mvts = new List<Dictionary<string, string>>();
                mvts.AddRange(operations);
                mvts.AddRange(transfersIn);
                mvts.AddRange(transfersOut);

                mvts.Sort((mvt1, mvt2) =>
                {
                    var d1 = DateTime.ParseExact(mvt1["Sorter"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    var d2 = DateTime.ParseExact(mvt2["Sorter"], "yyyy-MM-dd", CultureInfo.InvariantCulture);

                    if (d2 > d1)
                        return -1;

                    if (d2 < d1)
                        return 1;

                    return 0;
                });

                var lines = new List<string>() { string.Join(delimiter, columns.ToArray()) };
                foreach (var m in mvts)
                {
                    var lColumns = new List<string>();
                    foreach (var c in columns)
                    {
                        if (m.Keys.Contains(c))
                            lColumns.Add(m[c]);
                        else
                            lColumns.Add("");
                    }
                    lines.Add(string.Join(delimiter, lColumns.ToArray()));
                }

                var filename = String.Join("_", a.Name.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');

                var fp = Path.Combine(d, filename) + ".csv";
                fp = Path.GetFullPath(fp);

                using (StreamWriter sw = new StreamWriter(File.Open(fp, FileMode.Create), Encoding.UTF8))
                {
                    foreach (var line in lines)
                        sw.WriteLine(line);
                }
                lines.Clear();

                filenames.Add(new Tuple<Guid, string>(a.Id, fp));
            }

            return filenames;
        }

        /// <summary>
        /// Exporte les mouvements des éléments bancaires souhaités entre les dates spécifiées.
        /// </summary>
        /// <returns>
        /// Liste des fichiers HTML résultants:
        /// - Key: Identifiant unique de l'élément bancaire
        /// - Value: Chemin du fichier HTML correspondant
        /// </returns>
        /// <param name="directory">Chemin du répertoire recevant les fichiers HTML.</param>
        /// <param name="accountsId">Liste d'identifiants uniques correspondant aux éléments bancaires à traiter.</param>
        /// <param name="startDate">Date de début des mouvements pris en compte.</param>
        /// <param name="endDate">Date de fin des mouvements pris en compte.</param>
        /// <param name="contentReturns"><c>true</c>, retourne le contenu HTML sans crééer de fichier HTML, sinon, <c>false</c></param>
        public Dictionary<Guid, string> ToHtml(string directory, List<Guid> accountsId, DateTime startDate, DateTime endDate, bool contentReturns = false)
        {
            var d = directory.Trim().Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            if (!Directory.Exists(d))
                throw new DirectoryNotFoundException();

            var filenames = new Dictionary<Guid, string>();

            var lst = Accounts.GetAccountStatements(directory, accountsId, startDate, endDate);

            foreach (KeyValuePair<Guid, Dictionary<DateTime, List<IEventAction>>> kvp in lst)
            {
                var a = Accounts.GetById(kvp.Key);
                var filename = String.Join("_", a.Name.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
                var fp = Path.Combine(d, filename) + ".html";
                fp = Path.GetFullPath(fp);

                string html = string.Format(@"<!doctype html><html><head lang=""{0}""><meta charset=""UTF-8"">
                <title>{1}</title></head><body style=""width: 97%; font-family: sans-serif; font-size: 0.9em;"">
                <p style=""font-weight: bold; font-size: large;"">{1}</p>
                <table align=""center"" style=""border-collapse: collapse; width: 100%; border: 1px solid gray;"" border=""0"" cellpadding=""5"" cellspacing=""0"">
                <tr style=""width: 100%; background: gray; color: white; font-weight: bold; border-bottom: 1px solid gray;"">
                <td style=""width: 35%; max-width: 35%;"">{2}</td>
                <td style=""width: 30%; max-width: 30%;"">{3}</td>
                <td style=""width: 15%; max-width: 10%;"">{4}</td>
                <td style=""width: 10%; max-width: 15%; text-align: right;"">{5}</td>
                <td style=""width: 10%; max-width: 15%; text-align: right;"">{6}</td>
                </tr>",
                    CultureName, a.Name, "Dénomination", "Note", "Type", "Débit", "Crédit");

                var mvts = kvp.Value.OrderByDescending(i => i.Key);
                foreach (KeyValuePair<DateTime, List<IEventAction>> kvp2 in mvts)
                {
                    var amount = AmountAt(a, kvp2.Key, true);

                    html += string.Format(@"<tr style=""background: lightgray; border-top: 1px solid gray; border-bottom: 1px dashed gray;"">
                    <td colspan=""2"" style=""text-align: left; font-weight: bold; font-size: small;"">{0}</td>
                    <td colspan=""3"" style=""text-align: right; font-weight: bold; font-size: small; color: {3};"">{1} {2}</td>
                    </tr>",
                        kvp2.Key.ToLongDateString(), "solde:", Currency.Format(amount), (amount < 0m ? "red" : "black"));

                    foreach (var ot in kvp2.Value)
                    {
                        var name = ot.GetType() == typeof(Operation)
                            ? (ot as Operation).Name
                            : ot.GetType() == typeof(Transfer)
                                ? (ot as Transfer).Name
                                : "";
                        var note = ot.GetType() == typeof(Operation)
                            ? (ot as Operation).Note
                            : ot.GetType() == typeof(Transfer)
                                ? (ot as Transfer).Note
                                : "";

                        var type = ot.GetType() == typeof(Operation)
                            ? Paytypes.GetById((ot as Operation).TypeId).Name
                            : ot.GetType() == typeof(Transfer)
                                ? "Transfert"
                                : "";

                        var expense = ot.GetType() == typeof(Operation)
                            ? ((ot as Operation).Amount < 0m ? Currency.Format((ot as Operation).Amount) : "")
                            : ot.GetType() == typeof(Transfer)
                                ? ((ot as Transfer).FromAccountId.Equals(a.Id) ? Currency.Format(-Math.Abs((ot as Operation).Amount)) : "")
                                : "";

                        var income = ot.GetType() == typeof(Operation)
                            ? ((ot as Operation).Amount >= 0m ? Currency.Format((ot as Operation).Amount) : "")
                            : ot.GetType() == typeof(Transfer)
                                ? ((ot as Transfer).ToAccountId.Equals(a.Id) ? Currency.Format(Math.Abs((ot as Operation).Amount)) : "")
                                : "";

                        html += string.Format(@"<tr style=""border-bottom: 1px dotted gray;"">
                        <td>{0}</td><td>{1}</td><td>{2}</td><td style=""text-align: right;"">{3}</td><td style=""text-align: right;"">{4}</td>
                        </tr>",
                            name, note, type, expense, income);
                    }
                }

                html += string.Format(@"</table><p style=""font-size: smaller; text-align: center; color: gray;"">{0} {1}</p></body></html>",
                    GetType().Assembly.GetName().Name, GetType().Assembly.GetName().Version);

                if (!contentReturns)
                {
                    using (StreamWriter sw = new StreamWriter(File.Open(fp, FileMode.Create), Encoding.UTF8))
                        sw.Write(html);

                    filenames.Add(a.Id, fp);
                }
                else
                    filenames.Add(a.Id, html);

            }

            return filenames;
        }

        /// <summary>
        /// Exporte les mouvements des éléments bancaires souhaités entre les dates spécifiées.
        /// </summary>
        /// <returns>
        /// Liste des fichiers OFX résultants:
        /// - Key: Identifiant unique de l'élément bancaire
        /// - Value: Chemin du fichier OFX correspondant
        /// </returns>
        /// <param name="directory">Chemin du répertoire recevant les fichiers OFX.</param>
        /// <param name="accountsId">Liste d'identifiants uniques correspondant aux éléments bancaires à traiter.</param>
        /// <param name="startDate">Date de début des mouvements pris en compte.</param>
        /// <param name="endDate">Date de fin des mouvements pris en compte.</param>
        public Dictionary<Guid, string> ToOfx(string directory, List<Guid> accountsId, DateTime startDate, DateTime endDate)
        {
            var d = directory.Trim().Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            if (!Directory.Exists(d))
                throw new DirectoryNotFoundException();

            var filenames = new Dictionary<Guid, string>();

            var lst = Accounts.GetAccountStatements(directory, accountsId, startDate, endDate);
            foreach (KeyValuePair<Guid, Dictionary<DateTime, List<IEventAction>>> kvp in lst)
            {
                var a = Accounts.GetById(kvp.Key);
                var filename = String.Join("_", a.Name.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
                var fp = Path.Combine(d, filename) + ".ofx";
                fp = Path.GetFullPath(fp);

                var utcNow = DateTime.UtcNow;

                var ofx = @"OFXHEADER:100" + "\n";
                ofx += @"DATA:OFXSGML" + "\n";
                ofx += @"VERSION:102" + "\n";
                ofx += @"SECURITY:NONE" + "\n";
                ofx += @"ENCODING:UTF-8" + "\n";
                ofx += @"CHARSET:1252" + "\n";
                ofx += @"COMPRESSION:NONE" + "\n";
                ofx += @"OLDFILEUID:NONE" + "\n";
                ofx += @"NEWFILEUID:NONE" + "\n";
                ofx += "\n";
                ofx += @"<OFX>" + "\n";
                ofx += "\t" + @"<SIGNONMSGSRSV1>" + "\n";
                ofx += "\t\t" + @"<SONRS>" + "\n";
                ofx += "\t\t\t" + @"<STATUS>" + "\n";
                ofx += "\t\t\t\t" + @"<CODE>0</CODE>" + "\n";
                ofx += "\t\t\t\t" + @"<SEVERITY>INFO</SEVERITY>" + "\n";
                ofx += "\t\t\t" + @"</STATUS>" + "\n";
                ofx += string.Format(CultureInfo.InvariantCulture,
                    "\t\t\t" + @"<DTSERVER>{0}</DTSERVER>" + "\n",
                    utcNow.ToString("yyyyMMdd") + "000000");
                ofx += string.Format(CultureInfo.InvariantCulture,
                    "\t\t\t" + @"<LANGUAGE>{0}</LANGUAGE>" + "\n",
                    CultureInfo.GetCultureInfo(CultureName).ThreeLetterISOLanguageName.ToUpper());
                ofx += "\t\t" + @"</SONRS>" + "\n";
                ofx += "\t" + @"</SIGNONMSGSRSV1>" + "\n";
                ofx += "\t" + @"<BANKMSGSRSV1>" + "\n";
                ofx += "\t\t" + @"<STMTTRNRS>" + "\n";
                ofx += string.Format(CultureInfo.InvariantCulture,
                    "\t\t\t" + @"<TRNUID>{0}</TRNUID>" + "\n",
                     utcNow.ToString("yyyyMMdd") + "000000");
                ofx += "\t\t\t" + @"<STATUS>" + "\n";
                ofx += "\t\t\t\t" + @"<CODE>0</CODE>" + "\n";
                ofx += "\t\t\t\t" + @"<SEVERITY>INFO</SEVERITY>" + "\n";
                ofx += "\t\t\t" + @"</STATUS>" + "\n";
                ofx += "\t\t\t" + @"<STMTRS>" + "\n";
                ofx += string.Format(CultureInfo.InvariantCulture,
                    "\t\t\t\t" + @"<CURDEF>{0}</CURDEF>" + "\n",
                    Currency.Name.ToUpper());
                ofx += "\t\t\t\t" + @"<BANKTRANLIST>" + "\n";
                ofx += string.Format(CultureInfo.InvariantCulture,
                    "\t\t\t\t\t" + @"<DTSTART>{0}</DTSTART>" + "\n",
                    startDate.ToUniversalTime().ToString("yyyyMMdd") + "000000");
                ofx += string.Format(CultureInfo.InvariantCulture,
                    "\t\t\t\t\t" + @"<DTEND>{0}</DTEND>" + "\n",
                    endDate.ToUniversalTime().ToString("yyyyMMdd") + "000000");

                var mvts = kvp.Value.OrderByDescending(i => i.Key);
                foreach (KeyValuePair<DateTime, List<IEventAction>> kvp2 in mvts)
                {
                    var mvtDate = kvp2.Key;
                    foreach (var ot in kvp2.Value)
                    {
                        ofx += "\t\t\t\t\t" + @"<STMTTRN>" + "\n";
                        ofx += string.Format(CultureInfo.InvariantCulture,
                            "\t\t\t\t\t\t" + @"<TRNTYPE>{0}</TRNTYPE>" + "\n",
                            ot.GetType() == typeof(Operation) && (ot as Operation).ToId.Equals(a.Id)
                            ? (ot as Operation).Amount < 0
                                ? "DEBIT"
                                : "CREDIT"
                            : ot.GetType() == typeof(Transfer)
                                ? (ot as Transfer).FromAccountId.Equals(a.Id)
                                    ? "DEBIT"
                                    : (ot as Transfer).ToAccountId.Equals(a.Id)
                                        ? "CREDIT"
                                        : "OTHER"
                                : "OTHER");
                        ofx += string.Format(CultureInfo.InvariantCulture,
                            "\t\t\t\t\t\t" + @"<DTPOSTED>{0}</DTPOSTED>" + "\n",
                            mvtDate.ToUniversalTime().ToString("yyyyMMdd"));
                        ofx += string.Format(CultureInfo.InvariantCulture,
                            "\t\t\t\t\t\t" + @"<DTUSER>{0}</DTUSER>" + "\n",
                            mvtDate.ToUniversalTime().ToString("yyyyMMdd"));
                        ofx += string.Format(CultureInfo.InvariantCulture,
                            "\t\t\t\t\t\t" + @"<TRNAMT>{0:F}</TRNAMT>" + "\n",
                            ot.GetType() == typeof(Operation) && (ot as Operation).ToId.Equals(a.Id)
                            ? (ot as Operation).Amount
                            : ot.GetType() == typeof(Transfer)
                                ? (ot as Transfer).FromAccountId.Equals(a.Id)
                                    ? -Math.Abs((ot as Transfer).Amount)
                                    : (ot as Transfer).ToAccountId.Equals(a.Id)
                                        ? Math.Abs((ot as Transfer).Amount)
                                        : 0
                                : 0);
                        ofx += string.Format(CultureInfo.InvariantCulture,
                            "\t\t\t\t\t\t" + @"<FITID>{0}</FITID>" + "\n",
                            ot.GetType() == typeof(Operation) && (ot as Operation).ToId.Equals(a.Id)
                            ? (ot as Operation).Id.ToString("N").ToUpper()
                            : ot.GetType() == typeof(Transfer)
                                ? (ot as Transfer).Id.ToString("N").ToUpper()
                                : Guid.NewGuid().ToString("N"));
                        ofx += string.Format(CultureInfo.InvariantCulture,
                            "\t\t\t\t\t\t" + @"<NAME>{0}</NAME>" + "\n",
                            ot.GetType() == typeof(Operation) && (ot as Operation).ToId.Equals(a.Id)
                            ? SecurityElement.Escape((ot as Operation).Name)
                            : ot.GetType() == typeof(Transfer)
                                ? SecurityElement.Escape((ot as Transfer).Name)
                                : "");
                        ofx += string.Format(CultureInfo.InvariantCulture,
                            "\t\t\t\t\t\t" + @"<MEMO>{0}</MEMO>" + "\n",
                            ot.GetType() == typeof(Operation) && (ot as Operation).ToId.Equals(a.Id)
                            ? SecurityElement.Escape((ot as Operation).Note)
                            : ot.GetType() == typeof(Transfer)
                                ? SecurityElement.Escape((ot as Transfer).Note)
                                : "");
                        ofx += "\t\t\t\t\t" + @"</STMTTRN>" + "\n";
                    }
                }

                ofx += "\t\t\t\t" + @"</BANKTRANLIST>" + "\n";
                ofx += "\t\t\t\t" + @"<LEDGERBAL>" + "\n";
                ofx += string.Format(CultureInfo.InvariantCulture,
                    "\t\t\t\t\t" + @"<BALAMT>{0:F}</BALAMT>" + "\n",
                    AmountAt(a, endDate, true));
                ofx += string.Format(CultureInfo.InvariantCulture,
                    "\t\t\t\t\t" + @"<DTASOF>{0}</DTASOF>" + "\n",
                    endDate.ToUniversalTime().ToString("yyyyMMdd"));
                ofx += "\t\t\t\t" + @"</LEDGERBAL>" + "\n";
                ofx += "\t\t\t" + @"</STMTRS>" + "\n";
                ofx += "\t\t" + @"</STMTTRNRS>" + "\n";
                ofx += "\t" + @"</BANKMSGSRSV1>" + "\n";
                ofx += @"</OFX>" + "\n";

                using (StreamWriter sw = new StreamWriter(File.Open(fp, FileMode.Create), Encoding.UTF8))
                    sw.Write(ofx);

                filenames.Add(a.Id, fp);
            }

            return filenames;
        }

        /// <summary>
        /// Exporte les mouvements des éléments bancaires souhaités entre les dates spécifiées.
        /// </summary>
        /// <returns>
        /// Chemin du fichier QIF correspondant
        /// </returns>
        /// <param name="directory">Chemin du répertoire recevant les fichiers QIF.</param>
        /// <param name="accountsId">Liste d'identifiants uniques correspondant aux éléments bancaires à traiter.</param>
        /// <param name="startDate">Date de début des mouvements pris en compte.</param>
        /// <param name="endDate">Date de fin des mouvements pris en compte.</param>
        public string ToQif(string directory, List<Guid> accountsId, DateTime startDate, DateTime endDate)
        {
            var d = directory.Trim().Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            if (!Directory.Exists(d))
                throw new DirectoryNotFoundException();

            var filename = String.Join("_", GetType().Assembly.GetName().Name.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
            var fp = Path.Combine(d, filename) + ".qif";
            fp = Path.GetFullPath(fp);

            string qif = "";

            qif += "!Type:Cat\n";
            void scrib(Category cat, string sParent)
            {
                var sFullName = string.Join(":", new string[] { sParent, cat.Name });
                var sName = string.Format(CultureInfo.InvariantCulture, "{0}",
                    SecurityElement.Escape(sFullName));

                qif += "N" + sName + "\n";
                qif += "^\n";

                cat.Childs.Items.ForEach(ccat => scrib(ccat, sFullName));
            }
            Categories.Items.ForEach(cat => scrib(cat, ""));

            var lst = Accounts.GetAccountStatements(directory, accountsId, startDate, endDate);
            foreach (KeyValuePair<Guid, Dictionary<DateTime, List<IEventAction>>> kvp in lst)
            {
                var a = Accounts.GetById(kvp.Key);

                var sType = "Bank";
                if (a.GetType() == typeof(Paycard))
                    sType = "CCard";
                else if (a.GetType() == typeof(Wallet))
                    sType = "Cash";

                qif += "!Account\n";
                qif += string.Format(CultureInfo.InvariantCulture, "N{0}\n",
                    SecurityElement.Escape(a.Name));
                qif += string.Format(CultureInfo.InvariantCulture, "D{0}\n",
                    SecurityElement.Escape(a.Note));
                qif += string.Format(CultureInfo.InvariantCulture, "T{0}\n",
                    sType);
                qif += string.Format(CultureInfo.InvariantCulture, "/{0}\n",
                    endDate.ToUniversalTime().ToString("dd/MM/yyyy"));
                qif += string.Format(CultureInfo.InvariantCulture, "${0}\n",
                    AmountAt(a, endDate, true));
                qif += "^\n";

                if (kvp.Value.Any())
                {
                    qif += string.Format(CultureInfo.InvariantCulture, "!Type:{0}\n",
                        sType);

                    var mvts = kvp.Value.OrderByDescending(i => i.Key);
                    foreach (KeyValuePair<DateTime, List<IEventAction>> kvp2 in mvts)
                    {
                        var mvtDate = kvp2.Key;
                        var sMvtDate = string.Format(CultureInfo.InvariantCulture, "{0}",
                                mvtDate.ToUniversalTime().ToString("yyyyMMdd"));

                        foreach (var ot in kvp2.Value)
                        {
                            var sAmount = string.Format(CultureInfo.InvariantCulture, "{0:F}",
                                ot.GetType() == typeof(Operation) && (ot as Operation).ToId.Equals(a.Id)
                                ? (ot as Operation).Amount
                                : ot.GetType() == typeof(Transfer)
                                    ? (ot as Transfer).FromAccountId.Equals(a.Id)
                                        ? -Math.Abs((ot as Transfer).Amount)
                                        : (ot as Transfer).ToAccountId.Equals(a.Id)
                                            ? Math.Abs((ot as Transfer).Amount)
                                            : 0
                                    : 0);

                            var sId = ot.GetType() == typeof(Operation) && (ot as Operation).ToId.Equals(a.Id)
                                ? (ot as Operation).Id.ToString("N").ToUpper()
                                : ot.GetType() == typeof(Transfer)
                                    ? (ot as Transfer).Id.ToString("N").ToUpper()
                                    : Guid.NewGuid().ToString("N");

                            var sName = string.Format(CultureInfo.InvariantCulture, "{0}",
                                ot.GetType() == typeof(Operation) && (ot as Operation).ToId.Equals(a.Id)
                                ? SecurityElement.Escape((ot as Operation).Name)
                                : ot.GetType() == typeof(Transfer)
                                    ? SecurityElement.Escape((ot as Transfer).Name)
                                    : "");

                            var sMemo = string.Format(CultureInfo.InvariantCulture, "{0}",
                                ot.GetType() == typeof(Operation) && (ot as Operation).ToId.Equals(a.Id)
                                ? SecurityElement.Escape((ot as Operation).Note)
                                : ot.GetType() == typeof(Transfer)
                                    ? SecurityElement.Escape((ot as Transfer).Note)
                                    : "");

                            var sCategory = string.Format(CultureInfo.InvariantCulture, "{0}",
                                ot.GetType() == typeof(Operation) && (ot as Operation).ToId.Equals(a.Id)
                                ? Categories.GetById((ot as Operation).CategoryId).Name
                                : ot.GetType() == typeof(Transfer)
                                    ? "Transfert"
                                    : "");

                            var sState = "*";

                            qif += "D" + sMvtDate + "\n";
                            qif += "T" + sAmount + "\n";
                            qif += "P" + sName + "\n";
                            qif += "M" + sMemo + "\n";
                            qif += "N" + sId + "\n";
                            qif += "L" + sCategory + "\n";
                            qif += "N" + sState + "\n";
                            qif += "^\n";
                        }
                    }
                }

            }

            using (StreamWriter sw = new StreamWriter(File.Open(fp, FileMode.Create), Encoding.UTF8))
                sw.Write(qif);

            return fp;
        }













    }

}
