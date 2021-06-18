//
//  Identity.cs
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
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Kotlib.Objects {

    /// <summary>
    /// Carte d'identité
    /// </summary>
    [XmlRoot(ElementName = "Identity")]
    public class Identity: INotifyPropertyChanged {

        #region Fonctions privées

        /// <summary>
        /// Informe qu'une propriété est modifiée
        /// </summary>
        /// <param name="name">Nom de la propriété,
        /// ou vide pour le nom de la propriété appelante.</param>
        public void OnPropertyChanged([CallerMemberName] string name = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            OnUpdated(this, new EventArgs());
        }

        /// <summary>
        /// Informe que le dossier financier a été modifié
        /// </summary>
        public void OnUpdated(object sender, EventArgs e) {
            UpdatedEvent?.Invoke(sender, e);
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

        private static readonly XmlDocument _xmlDoc = new XmlDocument();

        private Guid _id = Guid.Empty;
        /// <summary>
        /// Identifiant unique
        /// </summary>
        /// <value>Identifiant unique.</value>
        [XmlElement(ElementName = "Id")]
        public Guid Id {
            get { return _id; }
            set {
                if(value != _id) {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _name = "";
        /// <summary>
        /// Dénomination de la carte d'identité
        /// </summary>
        /// <value>Nom, 255 caractères maximum.</value>
        [XmlElement(ElementName = "Name")]
        public string Name {
            get { return _name; }
            set {
                value = value.Trim();

                if(value.Length > 255)
                    value = value.Substring(0, 255);

                if(value == "")
                    throw new ArgumentException("Une carte d'identité requiert un nom.");

                if(value != _name) {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool ShouldSerializeName() {
            if(Name.Trim() == "")
                throw new ArgumentException("Une carte d'identité requiert un nom.");

            return true;
        }

        private string _forname = "";
        /// <summary>
        /// Prénom
        /// </summary>
        /// <value>Prénom, 255 caractères maximum.</value>
        [XmlElement(ElementName = "Forname")]
        public string Forname {
            get { return _forname; }
            set {
                value = value.Trim();

                if(value.Length > 255)
                    value = value.Substring(0, 255);

                if(value != _forname) {
                    _forname = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _lastname = "";
        /// <summary>
        /// Nom de famille
        /// </summary>
        /// <value>Nom de famille, 255 caractères maximum.</value>
        [XmlElement(ElementName = "Lastname")]
        public string Lastname {
            get { return _lastname; }
            set {
                value = value.Trim();

                if(value.Length > 255)
                    value = value.Substring(0, 255);

                if(value != _lastname) {
                    _lastname = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _phone = "";
        /// <summary>
        /// Numéro de téléphone
        /// </summary>
        /// <value>Numéro de téléphone.</value>
        [XmlElement(ElementName = "Phone")]
        public string Phone {
            get { return _phone; }
            set {
                value = value.Trim();

                if(value != _phone) {
                    _phone = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _url = "";
        /// <summary>
        /// Site internet
        /// </summary>
        /// <value>Site internet.</value>
        [XmlElement(ElementName = "Url")]
        public string Url {
            get { return _url; }
            set {
                value = value.Trim();

                if(value != "" && !Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute))
                    throw new ArgumentException("Url du site Internet invalide.");

                if(value != _url) {
                    _url = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _mail = "";
        /// <summary>
        /// Adresse email
        /// </summary>
        /// <value>Adresse email.</value>
        [XmlElement(ElementName = "Mail")]
        public string Mail {
            get { return _mail; }
            set {
                value = value.Trim();

                if(value != "" && !Regex.IsMatch(value, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
                    throw new ArgumentException("Adresse email invalide.");

                if(value != _mail) {
                    _mail = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _address = "";
        /// <summary>
        /// Adresse postale
        /// </summary>
        /// <value>Adresse postale, 4000 caractères maximum.</value>
        [XmlIgnore]
        public string Address {
            get { return _address; }
            set {
                value = value.Trim();
                if(value.Length > 4000)
                    value = value.Substring(0, 4000);

                if(value != _address) {
                    _address = value;
                    OnPropertyChanged();
                }
            }
        }
        [XmlElement(ElementName = "Address")]
        public XmlCDataSection AddressCData {
            get { return _xmlDoc.CreateCDataSection(Address); }
            set { Address = value.Data; }
        }

        private string _note = "";
        /// <summary>
        /// Notes appliquées à cette identité.
        /// </summary>
        /// <value>Notes, 4000 caractères maximum.</value>
        [XmlIgnore]
        public string Note {
            get { return _note; }
            set {
                value = value.Trim();
                if(value.Length > 4000)
                    value = value.Substring(0, 4000);

                if(value != _note) {
                    _note = value;
                    OnPropertyChanged();
                }
            }
        }
        [XmlElement(ElementName = "Note")]
        public XmlCDataSection NoteCData {
            get { return _xmlDoc.CreateCDataSection(Note); }
            set { Note = value.Data; }
        }

        #endregion

        /// <summary>
        /// Constructeurs
        /// </summary>
        public Identity() {
            Id = Guid.NewGuid();
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="name">Nom de la carte d'identité.</param>
        public Identity(string name) : this() {
            Name = name;
        }

    }

}
