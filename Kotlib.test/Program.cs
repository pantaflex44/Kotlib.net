//
//  Program.cs
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
using System.Text;
using System.Linq;
using Kotlib.Objects;

namespace Kotlib.test
{

	class MainClass
	{

		public static void Main(string[] args)
		{
			var me = new Identity("Christophe LEMOINE")
			{
				Lastname = "LEMOINE",
				Forname = "Christophe"
			};

			var bc = new BankCard("CIC Mastercard Tof")
			{
				Number = "5135 1800 0000 0001",
				CVV = "123",
				Date = new CardDate(2025, 12)
			};
			bc.Name = "CIC Mastercard";

			var ba = new BankAccount("CIC Compte courant", me)
			{
				BankName = "CIC",
				Iban = "FR76 1180 8009 1012 3456 7890 147",
				Bic = "CMCIFRPP",
				Contact = new Identity("CIC COUERON")
				{
					Address = "CIC\n1 RUE DE CHEZ TOI\n12345 CHEZ TOI\n",
					Phone = "0102030405"
				},
				Paytypes = new PaytypeList() { bc }
			};
			

			var fi = Financial.Create(
				         name: "Mon dossier financier",
				         owner: me,
				         accounts: new AccountList() { ba },
				         loadDefaults: true
			         );
			fi.Paytypes.Add(bc);
			fi.UpdatedEvent += (sender, e) => Console.WriteLine("fi1 updated " + sender.GetType().UnderlyingSystemType);
			string filepath = fi.SaveToFile(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), password: "bob");
			
			Console.WriteLine("Solde total: {0}", fi.AmountAt(DateTime.Now, addInitialAmount: true).ToString("C"));

			var fi2 = Financial.LoadFromFile(filepath, password: "bob");
			byte[] datas2 = fi2.Serialize();
			Console.WriteLine(Encoding.UTF8.GetString(datas2));

            
            
			Console.ReadLine();
		}

	}

}
