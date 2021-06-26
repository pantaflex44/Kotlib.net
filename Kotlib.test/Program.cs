//
//  Program.cs
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
using System.Text;
using System.Linq;
using Kotlib.Objects;
using Kotlib.Tools;

namespace Kotlib.test
{

	class MainClass
	{

		public static void Main(string[] args)
		{
			Console.OutputEncoding = Encoding.UTF8;
			
			var me = new Identity(name: "Christophe LEMOINE")
			{
				Lastname = "LEMOINE",
				Forname = "Christophe"
			};

			var bc = new BankCard(name: "CIC Mastercard Tof")
			{
				Number = "5135 1800 0000 0001",
				CVV = "123",
				Date = new CardDate(2025, 12)
			};
			bc.Name = "CIC Mastercard";

			var ba = new BankAccount(name: "CIC Compte courant", 
				         owner: me)
			{
				BankName = "CIC",
				Iban = "FR76 1180 8009 1012 3456 7890 147",
				Bic = "CMCIFRPP",
				Contact = new Identity(name: "CIC COUERON")
				{
					Address = "CIC\n1 RUE DE CHEZ TOI\n12345 CHEZ TOI\n",
					Phone = "0102030405"
				},
				Paytypes = new PaytypeList() { bc },
				InitialAmount = 300.0d
			};
			
			var fi = Financial.Create(
				         name: "Mon dossier financier",
				         owner: me,
				         accounts: new AccountList() { ba },
				         cultureName: "en_US",
				         loadDefaults: true,
				         paytypes: new PaytypeList() { bc }
			         );
			Console.WriteLine();
			Console.WriteLine("Creation d'un dossier financier nommé {0}", fi.Name);
			fi.UpdatedEvent += (sender, e) => Console.WriteLine("fi1 updated " + sender.GetType().UnderlyingSystemType);
			fi.SavedEvent += (sender, e) => Console.WriteLine("fi1 saved.");
			
			Console.WriteLine();
			var p = new Event(name: "essai",
				        accountId: fi.Accounts[0].Id,
				        startDate: new DateTime(2021, 5, 12),
				        count: 4,
				        step: 1, 
				        type: RepeatType.Month);
			Console.WriteLine("Programmation '{0}', début le {1}",
				p.Name,
				p.StartDate.ToLongDateString());
			Console.WriteLine("prochaine date le {0} jusqu'au {1} ({2} occurences)", 
				p.NextDate.ToLongDateString(), 
				p.EndDate.ToLongDateString(),
				p.RepeatCount);
			Console.WriteLine("dates restantes: {0}", 
				string.Join(", ", p.GetNextCalendar().Select(d => d.ToLongDateString())));
			
			Console.WriteLine();
			Console.WriteLine("Activation de la programmation");
			p.Active = true;
			fi.Events.Add(p);
	
			Console.WriteLine();
			Console.WriteLine("Enregistrement du dossier financier...");
			string filepath = fi.SaveToFile(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), password: "bob");
			
			Console.WriteLine();
			Console.WriteLine("Rechargement du dossier financier...");
			var fi2 = Financial.LoadFromFile(filepath, password: "bob");
			fi2.UpdatedEvent += (sender, e) => Console.WriteLine("fi2 updated " + sender.GetType().UnderlyingSystemType);
			byte[] datas2 = fi2.Serialize();
			
			Console.WriteLine();
			Console.WriteLine("Affichage du contenu brut:");
			Console.WriteLine(Encoding.UTF8.GetString(datas2));
			
			Console.WriteLine();
			Console.WriteLine("Solde total: {0}", fi2.Currency.Format(fi2.AmountAt(DateTime.Now, addInitialAmount: true)));
			
			Console.WriteLine();
			foreach (var c in Currencies.Availlable.OrderBy(a => a.CultureFullname))
			{
				if (c.IsCurrentCulture)
					Console.WriteLine("Culture système: {0} | Monnaie: {1} {2}", c.CultureFullname, c.Symbol, c.Name);
			}
			
			if (!fi2.Accounts.GetById(fi2.Events[0].AccountId).Equals(default(Account)))
			{
				Console.WriteLine();
				Console.WriteLine("Postage des occurences passées pour le compte {0} depuis le {1}",
					fi2.Accounts.GetById(fi2.Events[0].AccountId).Name,
					fi2.Events[0].StartDate.ToLongDateString());
				fi2.Events[0].PostOverdue();
				Console.WriteLine("dates restantes: {0}", 
					string.Join(", ", fi2.Events[0].GetNextCalendar().Select(d => d.ToLongDateString())));
				Console.WriteLine("prochaine date: {0} ({1}/{2} occurences restantes)",
					fi2.Events[0].NextDate.ToLongDateString(), 
					fi2.Events[0].Counter,
					fi2.Events[0].RepeatCount);
			}
			else
				fi2.Events.Remove(fi2.Events[0]);
			
			Console.ReadLine();
		}

	}

}
