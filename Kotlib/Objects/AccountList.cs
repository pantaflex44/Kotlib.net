//
//  AccountList.cs
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
using System.Linq;

namespace Kotlib.Objects {

    /// <summary>
    /// Liste d'éléments bancaires
    /// </summary>
    public class AccountList: ObjectList<Account> {

        /// <summary>
        /// Retourne une liste vide
        /// </summary>
        /// <value>Liste vide.</value>
        public static AccountList Empty {
            get {
                return new AccountList();
            }
        }

        /// <summary>
        /// Retourne le premier élément bancaire trouvé possédant l'identifiant unique passé en paramètre
        /// </summary>
        /// <returns>Elément bancaire trouvé.</returns>
        /// <param name="id">Identifiant unique.</param>
        public Account GetById(string id) {
            return this.ToList().First(a => a.Id.Equals(Guid.Parse(id.Trim())));
        }

        /// <summary>
        /// Retourne la liste des éléments bancaires disponibles
        /// </summary>
        /// <returns>Liste des éléments bancaires disponibles</returns>
        public static List<Tuple<string, string, Type>> GetAvaillableAccounts()
        {
            var l = new List<Tuple<string, string, Type>>();

            foreach (var da in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var at in da.GetTypes())
                {
                    if (typeof(Account).IsAssignableFrom(at))
                    {
                        if (Attribute.IsDefined(at, typeof(DisplayNameAttribute)) &&
                            Attribute.IsDefined(at, typeof(DescriptionAttribute)))
                        {

                                var dname = (Attribute.GetCustomAttribute(at, typeof(DisplayNameAttribute)) as DisplayNameAttribute).DisplayName;
                                var desc = (Attribute.GetCustomAttribute(at, typeof(DescriptionAttribute)) as DescriptionAttribute).Description;
                                l.Add(new Tuple<string, string, Type>(dname, desc, at));

                        }
                    }
                }
            }

            return l;
        }



    }


}
