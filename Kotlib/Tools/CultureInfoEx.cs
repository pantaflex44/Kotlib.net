//
//  CultureInfoEx.cs
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
using System.Globalization;

namespace Kotlib.Tools
{

	/// <summary>
	/// Extension de la classe CultureInfo
	/// </summary>
	public static class CultureInfoEx
	{
	
		/// <summary>
		/// Retourne le premier jour de la semaine
		/// </summary>
		/// <param name="ci">CultureInfo à étendre</param>
		/// <param name="date">Date du jour de la semaine souhaitée</param>
		/// <returns>Date du premier jour de la semaine</returns>
		public static DateTime GetFirstDateOfWeek(this CultureInfo ci, DateTime date)
		{
			var fd = date.Date;
			while (fd.DayOfWeek != ci.DateTimeFormat.FirstDayOfWeek)
				fd = fd.AddDays(-1);

			return fd;
		}
	
		/// <summary>
		/// Retourne le dernier jour de la semaine
		/// </summary>
		/// <param name="ci">CultureInfo à étendre</param>
		/// <param name="date">Date du jour de la semaine souhaitée</param>
		/// <returns>Date du dernier jour de la semaine</returns>
		public static DateTime GetLastDateOfWeek(this CultureInfo ci, DateTime date)
		{
			var fd = date.Date;
			while (fd.DayOfWeek != (ci.DateTimeFormat.FirstDayOfWeek - 1))
				fd = fd.AddDays(1);

			return fd;
		}
		
		/// <summary>
		/// Retourne le premier jour du mois
		/// </summary>
		/// <param name="ci">CultureInfo à étendre</param>
		/// <param name="date">Date du jour du mois souhaitée</param>
		/// <returns>Date du premier jour du mois</returns>
		public static DateTime GetFirstDateOfMonth(this CultureInfo ci, DateTime date)
		{
			return new DateTime(date.Year, date.Month, 1);
		}
		
		/// <summary>
		/// Retourne le premier jour du mois
		/// </summary>
		/// <param name="ci">CultureInfo à étendre</param>
		/// <param name="date">Date du jour du mois souhaitée</param>
		/// <returns>Date du premier jour du mois</returns>
		public static DateTime GetLastDateOfMonth(this CultureInfo ci, DateTime date)
		{
			return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
		}
		
		/// <summary>
		/// Retourne le premier jour de l'année
		/// </summary>
		/// <param name="ci">CultureInfo à étendre</param>
		/// <param name="date">Date du jour de l'année souhaitée</param>
		/// <returns>Date du premier jour de l'année</returns>
		public static DateTime GetFirstDateOfYear(this CultureInfo ci, DateTime date)
		{
			return new DateTime(date.Year, 1, 1);
		}
		
		/// <summary>
		/// Retourne le premier jour de l'année
		/// </summary>
		/// <param name="ci">CultureInfo à étendre</param>
		/// <param name="date">Date du jour de l'année souhaitée</param>
		/// <returns>Date du premier jour de l'année</returns>
		public static DateTime GetLastDateOfYear(this CultureInfo ci, DateTime date)
		{
			return new DateTime(date.Year, 1, 1).AddYears(1).AddDays(-1);
		}
	
	}

}