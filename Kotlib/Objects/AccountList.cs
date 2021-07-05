//
//  AccountList.cs
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Kotlib.Objects
{

	/// <summary>
	/// Liste d'éléments bancaires
	/// </summary>
	public class AccountList : ObjectList<Account>
	{

		#region Evénements
		
		/// <summary>
		/// Délégué en charge des événements d'ajout, modification et supppression
		/// </summary>
		/// <param name="account">Elément concené</param>
		public delegate void AccountEventHandler(Account account);
		/// <summary>
		/// Un élément a été ajouté
		/// </summary>
		public event AccountEventHandler AccountAddedEvent;
		/// <summary>
		/// Un élément a été modifié
		/// </summary>
		public event AccountEventHandler AccountUpdatedEvent;
		/// <summary>
		/// Un élément a été supprimé
		/// </summary>
		public event AccountEventHandler AccountRemovedEvent;
		
		#endregion
		
		#region Fonctions privées
		
		/// <summary>
		/// Informe qu'un élément a été ajouté
		/// </summary>
		/// <param name="account">Elément concerné</param>
		public void OnAccountAdded(Account account)
		{
			if (AccountAddedEvent != null)
				AccountAddedEvent(account);
		}
		
		/// <summary>
		/// Informe qu'un élément a été modifié
		/// </summary>
		/// <param name="account">Elément concerné</param>
		public void OnAccountUpdated(Account account)
		{
			if (AccountUpdatedEvent != null)
				AccountUpdatedEvent(account);
		}
		
		/// <summary>
		/// Informe qu'un élément a été supprimé
		/// </summary>
		/// <param name="account">Elément concerné</param>
		public void OnAccountRemoved(Account account)
		{
			if (AccountRemovedEvent != null)
				AccountRemovedEvent(account);
		}
		
		#endregion
		
		#region Propriétés publiques

		/// <summary>
		/// Retourne une liste vide
		/// </summary>
		/// <value>Liste vide.</value>
		[XmlIgnore]
		public static AccountList Empty
		{
			get
			{
				return new AccountList();
			}
		}
		
		private TransferList _transfers = null;
		/// <summary>
		/// Liste des transferts
		/// </summary>
		/// <value>Liste des transferts.</value>
		[XmlArray(ElementName = "Transfers")]
		[XmlArrayItem(ElementName = "Transfer")]
		public TransferList Transfers
		{
			get { return _transfers; }
			set
			{
				if (value != null && value != _transfers)
				{
					if (_transfers != null)
						_transfers.UpdatedEvent -= OnUpdated;

					_transfers = value;
					_transfers.UpdatedEvent += OnUpdated;
				}
			}
		}

		#endregion

		/// <summary>
		/// Constructeur
		/// </summary>
		public AccountList()
			: base()
		{
			Transfers = TransferList.Empty;
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="items">Liste à charger</param>
		public AccountList(IEnumerable<Account> items)
			: base(items)
		{
			Transfers = TransferList.Empty;
		}

		/// <summary>
		/// Retourne le premier élément bancaire trouvé possédant l'identifiant unique passé en paramètre
		/// </summary>
		/// <returns>Elément bancaire trouvé.</returns>
		/// <param name="id">Identifiant unique.</param>
		public Account GetById(Guid id)
		{
			return this.ToList().FirstOrDefault(a => a.Id.Equals(id));
		}
		
		/// <summary>
		/// Retournel'élément à la position <c>index</c>
		/// </summary>
		/// <param name="index">Position de l'élément.</param>
		public new Account this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				Transfers.RemoveAll(a => a.FromAccountId.Equals(base[index].Id) || a.ToAccountId.Equals(base[index].Id));
				
				OnAccountRemoved(base[index]);
					
				base[index] = value;
				
				base[index].UpdatedEvent += (sender, e) => OnAccountUpdated(base[index]);
				OnAccountAdded(base[index]);
			}
		}
		
		/// <summary>
		/// Supprime l'élément de la liste à la position spécifié
		/// </summary>
		/// <param name="index">Position de l'élément.</param>
		public new void RemoveAt(int index)
		{
			if (index >= 0 && index < base.Count)
			{
				Transfers.RemoveAll(a => a.FromAccountId.Equals(base[index].Id) || a.ToAccountId.Equals(base[index].Id));

				OnAccountRemoved(base[index]);
				base.RemoveAt(index);
			}
		}

		/// <summary>
		/// Vide la liste de ses éléments
		/// </summary>
		public new void Clear()
		{
			foreach (var e in Items)
			{
				Transfers.RemoveAll(a => a.FromAccountId.Equals(e.Id) || a.ToAccountId.Equals(e.Id));
				OnAccountRemoved(e);
			}

			base.Clear();
		}
		
		/// <summary>
		/// Supprime un élément de la liste
		/// </summary>
		/// <returns>true, l'élément est supprimé, sinon, false.</returns>
		/// <param name="item">Elément à supprimer.</param>
		public new bool Remove(Account item)
		{
			if (base.IndexOf(item) > -1)
			{
				Transfers.RemoveAll(a => a.FromAccountId.Equals(item.Id) || a.ToAccountId.Equals(item.Id));
				OnAccountRemoved(item);
			}
			return base.Remove(item);
		}
		
		/// <summary>
		/// Ajoute un élément à la liste
		/// </summary>
		/// <param name="item">Item.</param>
		public new void Add(Account item)
		{
			OnAccountAdded(item);
			item.UpdatedEvent += (sender, e) => OnAccountUpdated(item);
			base.Add(item);
		}
		
		/// <summary>
		/// Insert un élément à la position spécifiée
		/// </summary>
		/// <param name="index">Position d'insertion.</param>
		/// <param name="item">Elément à insérer.</param>
		public new void Insert(int index, Account item)
		{
			OnAccountAdded(item);
			item.UpdatedEvent += (sender, e) => OnAccountUpdated(item);
			base.Insert(index, item);
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

		/// <summary>
		/// Nettoie la liste des transferts.
		/// Supprime tous les transferts ayant un compte émetteur ou destinataure inconnu.
		/// </summary>
		public void CleanTransfers()
		{
			var actIdList = Items.Select(a => a.Id).ToList();
			Transfers.RemoveAll(a => !actIdList.Contains(a.FromAccountId) || !actIdList.Contains(a.ToAccountId));
		}

        /// <summary>
        /// Exporte les mouvements d'un élément bancaire au format CSV
        /// </summary>
        /// <returns>
        /// Liste des fichiers nouvellement créés.
        /// Structure comprenant:
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
        public List<Tuple<Guid, string>> Export2CSV(string directory, List<Guid> accountsId, DateTime startDate, DateTime endDate, string delimiter = ";", string decimalSeparator = ",", string dateFormat = "dd/MM/yyyy")
        {
            var d = directory.Trim().Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            if (!Directory.Exists(d))
                throw new DirectoryNotFoundException();

            var filenames = new List<Tuple<Guid, string>>();

            var columns = new string[] { "Date", "Débit", "Crédit", "Libellé" };
            foreach (var uid in accountsId)
            {
                var a = GetById(uid);
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

                var transfersOut = Transfers.Items
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

                var transfersIn = Transfers.Items
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
        /// Exporte les mouvements bancaires des éléments souhaité
        /// </summary>
        /// <returns>
        /// Liste des mouvements par éléments bancaires:
        /// - Key: Identifiant unique de l'élément bancaire
        /// - Value: Dictionnaire classé et trié par date des mouvements
        ///     - Key: Date des mouvements
        ///     - Value: Liste des mouvements de type <c>Operation</c> ou <c>Transfer</c>
        /// </returns>
        /// <param name="directory">Chemin du répertoire recevant les fichiers CSV.</param>
        /// <param name="accountsId">Liste d'identifiants uniques correspondant aux éléments bancaires à traiter.</param>
        /// <param name="startDate">Date de début des mouvements pris en compte.</param>
        /// <param name="endDate">Date de fin des mouvements pris en compte.</param>
        public Dictionary<Guid, Dictionary<DateTime, List<IEventAction>>> Export2List(string directory, List<Guid> accountsId, DateTime startDate, DateTime endDate)
        {
            var result = new Dictionary<Guid, Dictionary<DateTime, List<IEventAction>>>();

            foreach (var uid in accountsId)
            {
                var a = GetById(uid);
                if (a == null || a.Equals(default(Account)))
                    continue;

                var datas = new Dictionary<DateTime, List<IEventAction>>();

                a.Operations.Items
                .Where(o => o.Active && o.Date.Date >= startDate.Date && o.Date.Date <= endDate.Date)
                .ToList()
                .ForEach(o => 
                    {
                        if (!datas.Keys.Contains(o.Date.Date))
                            datas[o.Date.Date] = new List<IEventAction>();

                        datas[o.Date.Date].Add(o);
                    });

                Transfers.Items
                .Where(t => t.Active && (t.FromAccountId.Equals(a.Id) || t.ToAccountId.Equals(a.Id)) && t.Date.Date >= startDate.Date && t.Date.Date <= endDate.Date)
                .ToList()
                .ForEach(t =>
                {
                    if (!datas.Keys.Contains(t.Date.Date))
                        datas[t.Date.Date] = new List<IEventAction>();

                    datas[t.Date.Date].Add(t);
                });

                datas.ToList().Sort((mvt1, mvt2) =>
                    {
                        if (mvt2.Key > mvt1.Key)
                            return -1;

                        if (mvt2.Key < mvt1.Key)
                            return 1;

                        return 0;
                    }
                );

                result.Add(a.Id, datas);
            }

            return result;
        }











    }
}
