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

namespace Kotlib.Objects {

    /// <summary>
    /// Liste de moyens de paiements
    /// </summary>
    public class PaytypeList: ObjectList<Paytype> {

        #region Fonctions privées

        

        #endregion

        /// <summary>
        /// Retourne une liste vide
        /// </summary>
        /// <value>Liste vide.</value>
        public static PaytypeList Empty {
            get {
                return new PaytypeList();
            }
        }

        /// <summary>
        /// Retourne la liste des moyens de paiements ou d'encaissements disponibles
        /// </summary>
        /// <param name="categoryName">"payment" ou "collection"</param>
        /// <returns>Liste des moyens disponibles</returns>
        public static List<Tuple<string, string, Type>> GetAvaillablePaytypes(string categoryName)
        {
            var l = new List<Tuple<string, string, Type>>();

            foreach (var da in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var at in da.GetTypes())
                {
                    if (typeof(Paytype).IsAssignableFrom(at))
                    {
                        if (Attribute.IsDefined(at, typeof(CategoryAttribute)) &&
                            Attribute.IsDefined(at, typeof(DisplayNameAttribute)) &&
                            Attribute.IsDefined(at, typeof(DescriptionAttribute)))
                        {

                            var category = (Attribute.GetCustomAttribute(at, typeof(CategoryAttribute)) as CategoryAttribute).Category;
                            if (category.Contains(categoryName))
                            {
                                var dname = (Attribute.GetCustomAttribute(at, typeof(DisplayNameAttribute)) as DisplayNameAttribute).DisplayName;
                                var desc = (Attribute.GetCustomAttribute(at, typeof(DescriptionAttribute)) as DescriptionAttribute).Description;
                                l.Add(new Tuple<string, string, Type>(dname, desc, at));
                            }

                        }
                    }
                }
            }

            return l;
        }

        /// <summary>
        /// Retourne la liste des moyens de paiements disponibles
        /// </summary>
        /// <returns>Liste des moyens de paiements (nom, description, type)</returns>
        public static List<Tuple<string, string, Type>> GetAvaillablePayments() {
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
