//
//  PaytypeList.cs
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
using System.Linq;
using System.ComponentModel;

namespace Kotlib.Objects
{

    /// <summary>
    /// Liste de moyens de paiements
    /// </summary>
    public class PaytypeList : ObjectList<Paytype>
    {

        #region Fonctions privées



        #endregion

        #region Propriétés publiques

        /// <summary>
        /// Retourne une liste vide
        /// </summary>
        /// <value>Liste vide.</value>
        public static PaytypeList Empty
        {
            get
            {
                return new PaytypeList();
            }
        }

        /// <summary>
        /// Retourne une liste de moyens par défaut
        /// </summary>
        public static PaytypeList Defaults
        {
            get
            {
                return new PaytypeList(from item in GetAvaillablePaytypes()
                                       select (Paytype)Activator.CreateInstance(item.Item3, item.Item1));
            }
        }

        #endregion

        /// <summary>
        /// Constructeur
        /// </summary>
        public PaytypeList() : base() { }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="items">Liste à charger</param>
        public PaytypeList(IEnumerable<Paytype> items) : base(items) { }

        /// <summary>
        /// Retourne la liste des moyens de paiements et d'encaissements disponibles
        /// </summary>
        /// <returns>Liste des moyens disponibles</returns>
        public static List<Tuple<string, string, Type>> GetAvaillablePaytypes()
        {
            return (from da in AppDomain.CurrentDomain.GetAssemblies()
                    from at in da.GetTypes()
                    where typeof(Paytype).IsAssignableFrom(at) && !at.Equals(typeof(Paytype)) && !at.Equals(typeof(Collection))
                    where Attribute.IsDefined(at, typeof(DisplayNameAttribute)) && Attribute.IsDefined(at, typeof(DescriptionAttribute))
                    select new Tuple<string, string, Type>((Attribute.GetCustomAttribute(at, typeof(DisplayNameAttribute)) as DisplayNameAttribute).DisplayName, (Attribute.GetCustomAttribute(at, typeof(DescriptionAttribute)) as DescriptionAttribute).Description, at)).ToList();
        }

        /// <summary>
        /// Retourne la liste des moyens de paiements ou d'encaissements disponibles
        /// </summary>
        /// <param name="categoryName">"payment" ou "collection"</param>
        /// <returns>Liste des moyens disponibles</returns>
        public static List<Tuple<string, string, Type>> GetAvaillablePaytypes(string categoryName)
        {
            return (from da in AppDomain.CurrentDomain.GetAssemblies()
                    from at in da.GetTypes()
                    where typeof(Paytype).IsAssignableFrom(at) && !at.Equals(typeof(Paytype)) && !at.Equals(typeof(Collection))
                    where Attribute.IsDefined(at, typeof(CategoryAttribute)) && Attribute.IsDefined(at, typeof(DisplayNameAttribute)) && Attribute.IsDefined(at, typeof(DescriptionAttribute))
                    where ((Attribute.GetCustomAttribute(at, typeof(CategoryAttribute)) as CategoryAttribute).Category).Contains(categoryName)
                    select new Tuple<string, string, Type>((Attribute.GetCustomAttribute(at, typeof(DisplayNameAttribute)) as DisplayNameAttribute).DisplayName, (Attribute.GetCustomAttribute(at, typeof(DescriptionAttribute)) as DescriptionAttribute).Description, at)).ToList();
        }

        /// <summary>
        /// Retourne la liste des moyens de paiements disponibles
        /// </summary>
        /// <returns>Liste des moyens de paiements (nom, description, type)</returns>
        public static List<Tuple<string, string, Type>> GetAvaillablePayments()
        {
            return GetAvaillablePaytypes("payment");
        }

        /// <summary>
        /// Retourne la liste des moyens d'encaissements disponibles
        /// </summary>
        /// <returns>Liste des moyens d'encaissements (nom, description, type)</returns>
        public static List<Tuple<string, string, Type>> GetAvaillableCollections()
        {
            return GetAvaillablePaytypes("collection");
        }



    }

}
