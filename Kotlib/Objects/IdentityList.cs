//
//  IdentityList.cs
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

namespace Kotlib.Objects
{

    /// <summary>
    /// Liste de carte d'identité
    /// </summary>
    public class IdentityList : ObjectList<Identity>
    {

        #region Propriétés publiques

        /// <summary>
        /// Retourne une liste vide
        /// </summary>
        /// <value>Liste vide.</value>
        public static IdentityList Empty
        {
            get
            {
                return new IdentityList();
            }
        }

        #endregion

        /// <summary>
        /// Constructeur
        /// </summary>
        public IdentityList() : base() { }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="items">Liste à charger</param>
        public IdentityList(IEnumerable<Identity> items) : base(items) { }

    }

}
