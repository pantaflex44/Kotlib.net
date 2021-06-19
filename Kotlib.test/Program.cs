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

using Kotlib.Objects;
using Kotlib.Core;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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

            // Informations de la CB  a modifier car non valide
            var bc = new BankCard("CIC Mastercard Tof")
            {
                Number = "1234 5678 9012 3456",
                CVV = "123",
                Date = (2022, 8)
            };
            bc.Name = "CIC Mastercard";

            // Informations du compte bancaire à modifier car non valide
            var ba = new BankAccount("CIC Compte personnel", me)
            {
                BankName = "CIC",
                Iban = "FR76 3004 1234 4567 8901 2345 678",
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
            string filepath = fi.SaveToFile(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), password: "bob").Result;

            var fi2 = Financial.LoadFromFile(filepath, password: "bob").Result;
            byte[] datas2 = fi2.Serialize().Result;
            Console.WriteLine(Encoding.UTF8.GetString(datas2));

        }

    }

}
