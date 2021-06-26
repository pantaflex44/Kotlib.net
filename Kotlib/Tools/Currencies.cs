//
//  Currencies.cs
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
using System.Globalization;
using System.Linq;
using System.Resources;

namespace Kotlib.Tools
{
	
	/// <summary>
	/// Contient les informations culturelles nécessaires
	/// </summary>
	public class Currency
	{
		
		private CultureInfo _culture = null;
		private ResourceManager _resources = null;
		
		/// <summary>
		/// Nom de la culture
		/// eg: fr_FR , en_US, etc.
		/// </summary>
		public string CultureName { get; private set; }
		
		/// <summary>
		/// Nom complet
		/// eg: Français (France)
		/// </summary>
		public string CultureFullname { get; private set; }
		
		/// <summary>
		/// Nom de la région du monde
		/// eg: Portugal, France
		/// </summary>
		public string RegionName { get; private set; }
		
		/// <summary>
		/// Symbole monétaire
		/// eg: $, $, £
		/// </summary>
		public string Symbol { get; private set; }
		
		/// <summary>
		/// Dénomination ISO de la monnaie
		/// eg: EUR
		/// </summary>
		public string Name { get; private set; }
		
		/// <summary>
		/// Indique si la culture correspond à la culture du système
		/// </summary>
		public bool IsCurrentCulture
		{
			get { return _culture.Equals(CultureInfo.CurrentCulture); }
		}
		
		/// <summary>
		/// Contructeur
		/// </summary>
		/// <param name="ci">Informations culturelles</param>
		public Currency(CultureInfo ci)
		{
			_culture = ci;
			_resources = new ResourceManager("Kotlib.flags", GetType().Assembly);
			var ri = new RegionInfo(_culture.LCID);
			CultureName = _culture.Name.Replace("-", "_");
			CultureFullname = _culture.DisplayName;
			Symbol = _culture.NumberFormat.CurrencySymbol;
			Name = ri.ISOCurrencySymbol;
			RegionName = ri.DisplayName;
		}
		
		/// <summary>
		/// Format une valeur monétaire
		/// </summary>
		/// <param name="value">Valeur monétaire</param>
		/// <param name="iso">Formattage ISO</param>
		/// <returns>Valeur formattée</returns>
		public string Format(double value, bool iso = false)
		{
			if (_culture == null)
				_culture = new CultureInfo(CultureName);
			
			var ret = value.ToString("C", _culture);
			if (iso)
				ret = string.Format("{0} {1}", Name, ret);
			
			return ret;
		}
		
		/// <summary>
		/// Format une valeur monétaire
		/// </summary>
		/// <param name="value">Valeur monétaire</param>
		/// <param name="iso">Formattage ISO</param>
		/// <returns>Valeur formattée</returns>
		public string Format(decimal value, bool iso = false)
		{
			return Format(Convert.ToDouble(value), iso);
		}
		
		/// <summary>
		/// Format une valeur monétaire
		/// </summary>
		/// <param name="value">Valeur monétaire</param>
		/// <param name="iso">Formattage ISO</param>
		/// <returns>Valeur formattée</returns>
		public string Format(float value, bool iso = false)
		{
			return Format(Convert.ToDouble(value), iso);
		}
		
		/// <summary>
		/// Format une valeur monétaire
		/// </summary>
		/// <param name="value">Valeur monétaire</param>
		/// <param name="iso">Formattage ISO</param>
		/// <returns>Valeur formattée</returns>
		public string Format(int value, bool iso = false)
		{
			return Format(Convert.ToDouble(value), iso);
		}
		
	}
	
	/// <summary>
	/// Outils pour la gestion de la culture.
	/// </summary>
	public static class Currencies
	{
		
		/// <summary>
		/// Retourne la liste des cultures disponibles et leurs informations.
		/// </summary>
		/// <returns>Liste des cultures</returns>
		public static List<Currency> Availlable
		{
			get
			{
				return CultureInfo.GetCultures(CultureTypes.AllCultures)
				         .Where(a => !a.IsNeutralCulture && a.LCID != CultureInfo.InvariantCulture.LCID)
					.Select(a => new Currency(a)).ToList();
			}
		}
		
	}
	
}
