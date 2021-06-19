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
            /*var fi = new Financial("Mon dossier financier",
                new Identity("Christophe LEMOINE")
                {
                    Lastname = "LEMOINE",
                    Forname = "Christophe"
                });
            fi.UpdatedEvent += (sender, e) => Console.WriteLine("fi1 updated " + sender.GetType().UnderlyingSystemType);

            var bc = new BankCard("CIC Mastercard Tof")
            {
                Number = "5136 4830 1141 7908",
                CVV = "123",
                Date = (2022, 8)
            };
            fi.Paytypes.Add(bc);
            bc.Name = "CIC Mastercard";

            var ba = new BankAccount("CIC Compte personnel", fi.Owner)
            {
                BankName = "CIC",
                Iban = "FR76 3004 7143 1000 0207 6660 120",
                Bic = "CMCIFRPP",
                Contact = new Identity("CIC COUERON")
                {
                    Address = "CIC COUERON\n2 RUE JEAN JAURES\n44220 COUERON\n",
                    Phone = "0228030868"
                },
                Paytypes = new PaytypeList() { bc }
            };
            fi.Accounts.Add(ba);*/

            var me = new Identity("Christophe LEMOINE")
            {
                Lastname = "LEMOINE",
                Forname = "Christophe"
            };

            var bc = new BankCard("CIC Mastercard Tof")
            {
                Number = "5136 4830 1141 7908",
                CVV = "123",
                Date = (2022, 8)
            };
            bc.Name = "CIC Mastercard";

            var ba = new BankAccount("CIC Compte personnel", me)
            {
                BankName = "CIC",
                Iban = "FR76 3004 7143 1000 0207 6660 120",
                Bic = "CMCIFRPP",
                Contact = new Identity("CIC COUERON")
                {
                    Address = "CIC COUERON\n2 RUE JEAN JAURES\n44220 COUERON\n",
                    Phone = "0228030868"
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

            /*byte[] datas = fi.Serialize();
            Console.WriteLine(Encoding.UTF8.GetString(datas));
            Console.WriteLine();

            var fi2 = Serializable.Deserialize<Financial>(datas);
            fi2.UpdatedEvent += (sender, e) => Console.WriteLine("fi2 updated " + sender.GetType().UnderlyingSystemType);
            fi2.Paytypes[0].Name = "Bob le mousquetaire";
            byte[] datas2 = fi2.Serialize();
            Console.WriteLine(Encoding.UTF8.GetString(datas2));*/


        }

    }

}
