//
//  Category.cs
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
using System.Xml;
using System.Xml.Serialization;

namespace Kotlib.Objects
{

    /// <summary>
    /// Catégorie
    /// </summary>
    [XmlRoot(ElementName = "Category")]
    public class Category : INotifyPropertyChanged
    {

        #region Fonctions privées

        /// <summary>
        /// Informe qu'une propriété est modifiée
        /// </summary>
        /// <param name="name">Nom de la propriété,
        /// ou vide pour le nom de la propriété appelante.</param>
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            OnUpdated(this, new EventArgs());
        }

        /// <summary>
        /// Informe que le dossier financier a été modifié
        /// </summary>
        public void OnUpdated(object sender, EventArgs e)
        {
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
        /// Dénomination de la catégorie
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
                    throw new ArgumentException("Dénomination de la catégorie requise.");

                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool ShouldSerializeName()
        {
            if (Name.Trim() == "")
                throw new ArgumentException("Dénomination de la catégorie requise.");

            return true;
        }

        private CategoryList _childs = null;
        /// <summary>
        /// Liste des sous catégories
        /// </summary>
        /// <value>Sous catégories.</value>
        [XmlArray("Childs")]
        [XmlArrayItem("Category")]
        public CategoryList Childs
        {
            get { return _childs; }
            set
            {
                if (value != null && value != _childs)
                {
                    if (_childs != null)
                        _childs.UpdatedEvent -= OnUpdated;

                    _childs = value;
                    _childs.UpdatedEvent += OnUpdated;
                }
            }
        }

        #endregion

        /// <summary>
        /// Constructeurs
        /// </summary>
        public Category()
        {
            Id = Guid.NewGuid();
            Childs = CategoryList.Empty;
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="name">Nom de la catégorie.</param>
        /// <param name="childs">Liste des catégories enfants.</param>
        public Category(string name, CategoryList childs = default) : this()
        {
            Name = name;
            Childs = childs == default || childs == null ? CategoryList.Empty : childs;
        }

    }

}
