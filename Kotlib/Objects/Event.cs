//
//  Event.cs
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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Kotlib.Objects
{
	
	/// <summary>
	/// Méthodes de répétition
	/// </summary>
	public enum RepeatType
	{
		/// <summary>
		/// Répéter tous les jours
		/// </summary>
		[XmlEnum(Name = "Day")]
		Day,
		
		/// <summary>
		/// Répéter toutes les semaines
		/// </summary>
		[XmlEnum(Name = "Week")]
		Week,
		
		/// <summary>
		/// Répéter tous les mois
		/// </summary>
		[XmlEnum(Name = "Month")]
		Month,
		
		/// <summary>
		/// Répéter tous les ans
		/// </summary>
		[XmlEnum(Name = "Year")]
		Year
	}

	/// <summary>
	/// Programmation d'une opération ou d'un transfert
	/// </summary>
	[XmlRoot(ElementName = "Event")]
	public class Event : INotifyPropertyChanged
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
            
			if (PropertyChanged != null)
				PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            
			OnUpdated(this, new EventArgs());
		}

		/// <summary>
		/// Informe que le dossier financier a été modifié
		/// </summary>
		public void OnUpdated(object sender, EventArgs e)
		{
			if (UpdatedEvent != null)
				UpdatedEvent.Invoke(sender, e);
		}
		
		/// <summary>
		/// Informe que le dossier financier a été modifié
		/// </summary>
		public void OnPostRaised(DateTime date, Event postEvent)
		{
			if (PostRaisedEvent != null)
				PostRaisedEvent.Invoke(date, postEvent);
		}
		
		/// <summary>
		/// Calcule la date suivante en fonction des paramatres RepeatType et RepeatStep
		/// </summary>
		/// <param name="date">Date initilale</param>
		/// <returns></returns>
		private DateTime ComputeNextDate(DateTime date)
		{
			switch (RepeatType)
			{
				case RepeatType.Day:
					return date.AddDays(RepeatStep);
				case RepeatType.Week:
					return date.AddDays(RepeatStep * 7);
				case RepeatType.Month:
					return date.AddMonths(RepeatStep);
				case RepeatType.Year:
					return date.AddYears(RepeatStep);
				default:
					return date;
			}
		}
		
		/// <summary>
		/// Calcule et met à jour le nombre de répétitions entre la date de fin 
		/// et celle du début en fonction de la méthode de répétition
		/// </summary>
		private void ComputeCounts()
		{
			int count = 0;
			
			var sd = StartDate;
			while (sd.Date <= EndDate.Date)
			{
				count++;			
				sd = ComputeNextDate(sd);
			}

			_repeatCount = count;
			_counter = _repeatCount;
		}
		
		/// <summary>
		/// Calcule et met à jour la date de fin en fonction de 
		/// celle du début et de la méthode de répétition
		/// </summary>
		private void ComputeEndDate()
		{
			var ed = StartDate;
			for (int i = 1; i < RepeatCount; i++)
				ed = ComputeNextDate(ed);
			
			if (ed.Date > EndDate.Date)
				_endDate = ed;
			
			ComputeCounts();
			
			if (_nextDate.Date > _endDate.Date)
			{
				_nextDate = _endDate;
				_counter = 0;
			}
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
		/// Délégué en charge de transmettre les informations des événements postés
		/// </summary>
		/// <param name="date">Date de l'occurence</param>
		/// <param name="postEvent">Objet représentant l'occurence postée</param>
		public delegate void PostDelegate(DateTime date, Event postEvent);
		/// <summary>
		/// Se produit lorsqu'une occurence programmée est postée
		/// </summary>
		public event PostDelegate PostRaisedEvent;

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
		/// Dénomination de la programmation
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
					throw new ArgumentException("Dénomination de la programmation requise.");

				if (value != _name)
				{
					_name = value;
					OnPropertyChanged();
				}
			}
		}
		/// <summary>
		/// Vérifie si la propriété est correctement définie avant d'être sérialisée
		/// </summary>
		/// <returns><c>true</c></returns>
		private bool ShouldSerializeName()
		{
			if (Name.Trim() == "")
				throw new ArgumentException("Dénomination de la programmation requise.");

			return true;
		}

		private bool _active = false;
		/// <summary>
		/// Retourne ou définit si la programmation est active ou non
		/// </summary>
		/// <value>Etat de la programmation</value>
		[XmlAttribute(AttributeName = "active")]
		public  bool Active
		{
			get { return _active; }
			set
			{
				if (value != _active)
				{
					_active = value;
					OnPropertyChanged();
				}
			}
		}
		
		private Guid _accountId = Guid.Empty;
		/// <summary>
		/// Identifiant unique de l'élément bancaire associé à cette programmation
		/// </summary>
		/// <value>Identifiant unique de l'élément bancaire associé à cette programmation</value>
		public Guid AccountId
		{
			get { return _accountId; }
			set
			{
				if (value.Equals(Guid.Empty))
					throw new ArgumentException("Un élément bancaire doit être associé à cette programmation!");
				
				if (value != _accountId)
				{
					_accountId = value;
					OnPropertyChanged();
				}
			}
		}
		
		private DateTime _startDate = DateTime.Now;
		/// <summary>
		/// Date et heure de début de la programmation
		/// </summary>
		/// <value></value>
		[XmlAttribute(AttributeName = "startdate")]
		public DateTime StartDate
		{
			get { return _startDate; }
			set
			{
				if (value.Date > EndDate.Date)
					EndDate = value;
					
				if (value.Date != _startDate.Date)
				{
					_startDate = value;
					ComputeCounts();
					OnPropertyChanged();
				}
			}
		}
		
		private DateTime _endDate = DateTime.Now;
		/// <summary>
		/// Date et heure de fin de la programmation
		/// </summary>
		/// <value></value>
		[XmlAttribute(AttributeName = "enddate")]
		public DateTime EndDate
		{
			get { return _endDate; }
			set
			{
				if (value.Date < StartDate.Date)
					value = StartDate;
				
				if (value.Date != _endDate.Date)
				{
					_endDate = value;
					ComputeCounts();
					OnPropertyChanged();
				}
			}
		}
		
		private DateTime _nextDate = DateTime.Now;
		/// <summary>
		/// Date et heure de la prochaine occurence
		/// </summary>
		/// <value></value>
		[XmlAttribute(AttributeName = "nextdate")]
		public DateTime NextDate
		{
			get { return _nextDate; }
			set
			{
				if (value.Date < StartDate.Date)
					value = StartDate;
				
				if (value.Date != _nextDate.Date)
				{
					_nextDate = value;
					OnPropertyChanged();
				}
			}
		}
		
		/// <summary>
		/// Informe s'il existe au moins une occurence apres la prochaine connue
		/// </summary>
		public bool HasFuture
		{
			get { return ComputeNextDate(NextDate).Date <= EndDate.Date; }
		}
		
		private RepeatType _repeatType = RepeatType.Month;
		/// <summary>
		/// Méthode de répétition
		/// eg: tous les X jours, tous les X mois
		/// </summary>
		/// <value>Méthode de répétition</value>
		[XmlElement(ElementName = "RepeatType")]
		public RepeatType RepeatType
		{
			get { return _repeatType; }
			set
			{
				if (!value.Equals(_repeatType))
				{
					_repeatType = value;
					ComputeCounts();
					OnPropertyChanged();
				}
			}
		}
		
		private int _repeatStep = 1;
		/// <summary>
		/// Nombre de fois que la méthode est répétée
		/// eg: X jours, X mois
		/// </summary>
		/// <value>Nombre de fois que la méthode est répétée</value>
		[XmlElement(ElementName = "RepeatStep")]
		public int RepeatStep
		{
			get { return _repeatStep; }
			set
			{
				if (value < 1)
					value = 1;
				
				if (value != _repeatStep)
				{
					_repeatStep = value;
					ComputeCounts();
					OnPropertyChanged();
				}
			}
		}
		
		private int _counter = 0;
		/// <summary>
		/// Retoune le nombre d'occurences restantes
		/// </summary>
		public int Counter
		{
			get { return _counter; }
		}
		
		private int _repeatCount = 0;
		/// <summary>
		/// Nombre de répétitions
		/// eg: répéter la méthode X fois
		/// </summary>
		/// <value>Nombre de répétitions</value>
		[XmlElement(ElementName = "RepeatCount")]
		public int RepeatCount
		{
			get { return _repeatCount; }
			set
			{
				if (value < 0)
					value = 0;
				
				if (value != _repeatCount)
				{
					_repeatCount = value;
					ComputeEndDate();
					OnPropertyChanged();
				}
			}
		}
		
		
		
		#endregion

		/// <summary>
		/// Constructeurs
		/// </summary>
		public Event()
		{
			Id = Guid.NewGuid();
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom de la programmation.</param>
		/// <param name="accountId">Identifiant unique de l'élément bancaire concerné</param>
		/// <param name="startDate">Date de début de la programmation</param>
		/// <param name="endDate">Date de fin (optionnel)</param>
		/// <param name="count">Nombre de répétitions (optionnel)</param>
		/// <param name="step"><paramref name="step">step</paramref> fois par <paramref name="type">type</paramref> </param>
		/// <param name="type">Méthode de répétition. (Day | Week | Month | Year)</param>
		public Event(string name, Guid accountId, DateTime startDate, DateTime? endDate = null, int count = 1, int step = 1, RepeatType type = RepeatType.Month)
			: this()
		{
			Name = name;
			AccountId = accountId;
			StartDate = startDate;
			NextDate = startDate;
			RepeatStep = step;
			RepeatType = type;
			if (endDate == null)
				RepeatCount = count;
			else
				EndDate = endDate ?? startDate;
		}
		
		/// <summary>
		/// Remise à zéro de la prochaine occurence à la date de départ
		/// </summary>
		public void Reset()
		{
			NextDate = StartDate;
		}
		
		/// <summary>
		/// Retourne la liste des dates programmées
		/// </summary>
		/// <returns>Liste des dates programmées</returns>
		public List<DateTime> GetCalendar()
		{
			var calendar = new List<DateTime>();
			
			var dt = StartDate;
			for (int i = 0; i < RepeatCount; i++)
			{
				calendar.Add(dt);
				dt = ComputeNextDate(dt);
			}
			
			return calendar;
		}
		
		/// <summary>
		/// Retourne la liste des dates restantes programmées
		/// </summary>
		/// <returns>Liste des dates restantes programmées</returns>
		public List<DateTime> GetNextCalendar()
		{
			var calendar = new List<DateTime>();
			
			var dt = NextDate;
			for (int i = 0; i < RepeatCount; i++)
			{
				calendar.Add(dt);
				dt = ComputeNextDate(dt);
				
				if (dt.Date > EndDate.Date)
					break;
			}
			
			return calendar;
		}
		
		/// <summary>
		/// Poste la prochaine occurence, qu'elle soit passée, présente ou future
		/// </summary>
		/// <returns><c>true</c>, s'il existe une prochaine occurence, sinon, <c>false</c></returns>
		public bool Post()
		{
			if (!Active)
				return false;
			
			if (NextDate.Date > EndDate.Date)
			{
				Active = false;
				return false;
			}
			
			OnPostRaised(NextDate, this);
			
			NextDate = ComputeNextDate(NextDate);
			
			_counter--;
			if (_counter < 0)
				_counter = 0;
			
			Active &= (NextDate.Date <= EndDate.Date && _counter > 0);
			return Active;
		}
		
		/// <summary>
		/// Poste toutes les occurences restantes jusqu'a la date spécifiée, comprise
		/// </summary>
		/// <param name="date"></param>
		public void PostUntil(DateTime date)
		{
			do
			{
				if (!Post())
					break;
			}
			while (NextDate.Date <= date.Date);
		}
		
		/// <summary>
		/// Poste toutes les occurences restantes du calendrier
		/// </summary>
		public void PostAll()
		{
			PostUntil(EndDate);
		}
		
		/// <summary>
		/// Poste toutes les occurences restantes jusqu'a ce jour
		/// </summary>
		public void PostOverdue()
		{
			PostUntil(DateTime.Now);
		}
		
		
		
		

	}
	
}
