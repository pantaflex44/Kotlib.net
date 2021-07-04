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
using System.Collections.Generic;
using System.Globalization;
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
			
			// Création d'une nouvelle identité
			var me = new Identity(name: "Christophe LEMOINE")
			{
				Lastname = "LEMOINE",
				Forname = "Christophe"
			};

			// Création d'un moyen de paiement de type 'Carte bancaire'
			var bc = new BankCard(name: "CIC Mastercard Tof")
			{
				Number = "5135 1800 0000 0001",
				CVV = "123",
				Date = new CardDate(2025, 12)
			};
			
			// Création d'un élément bancaire de type 'Portefeuille d'espèces'
			var wl = new Wallet(name: "Portefeuille de Tof",
				         owner: me)
			{
				Electronic = false
			};
			
			// Création d'un élément bancaire de type 'Compte bancaire'
			var ba = new BankAccount(name: "CIC Compte courant", 
				         owner: me)
			{
				BankName = "CIC",
				Iban = "FR76 5896 1234 7852 1456 9856 147",
				Bic = "CMCIFRPP",
				Contact = new Identity(name: "CIC COUERON")
				{
					Address = "CIC\n1 RUE DE CHEZ TOI\n12345 CHEZ TOI\n",
					Phone = "0102030405"
				},
				Paytypes = new PaytypeList() { bc },
				InitialAmount = 300.0m
			};
			
			//###############################################################################################################
			
			// Création du dossier financier et ajout des informations liées
			var fi = Financial.Create(
				         name: "Mon dossier financier",
				         owner: me,
				         accounts: new AccountList() { ba },
				         cultureName: "fr_FR",
				         loadDefaults: true,
				         paytypes: new PaytypeList() { bc }
			         );
			Console.WriteLine();
			Console.WriteLine("Creation d'un dossier financier nommé {0}", fi.Name);
			
			// Abonnements aux événements
			// dossier financier
			fi.UpdatedEvent += (sender, e) => Console.WriteLine("fi1 - Modification d'une propriété de type: " + sender.GetType().Name);
			fi.SavedEvent += (sender, e) => Console.WriteLine("fi1 - Sauvegardé.");
			// événements programmés
			fi.Events.PostEventAddedEvent += (postEvent) => Console.WriteLine("fi1 - Ajout de la programmation id {2}, '{0}' de type {1}", postEvent.Name, postEvent.GetType().Name, postEvent.Id);
			fi.Events.PostEventUpdatedEvent += (postEvent) => Console.WriteLine("fi1 - Modification de la programmation id {1}, '{0}'", postEvent.Name, postEvent.Id);
			fi.Events.PostEventRemovedEvent += (postEvent) => Console.WriteLine("fi1 - Suppression de la programmation id {1}, '{0}'", postEvent.Name, postEvent.Id);
			fi.PostRaisedEvent += (date, postEvent) =>
			{
				Console.WriteLine("fi1 - L'occurence '{0}' ({2}, {3}) programmée pour le {1} vient dêtre postée.",
					postEvent.Name,
					date.ToLongDateString(),
					postEvent.EventAction.Name,
					fi.Currency.Format(postEvent.EventAction.Amount));
				Console.WriteLine("dates restantes: {0}", 
					string.Join(", ", postEvent.GetNextCalendar().Select(d => d.ToLongDateString())));
				Console.WriteLine("prochaine date: {0} ({1}/{2} occurences restantes)\r\n",
					postEvent.NextDate.ToLongDateString(), 
					postEvent.Counter,
					postEvent.RepeatCount);
			};
			// éléments bancaires
			fi.Accounts.AccountAddedEvent += (account) => Console.WriteLine("fi1 - Ajout de l'élément bancaire id {2}, '{0}' de type {1}", account.Name, account.GetType().Name, account.Id);
			fi.Accounts.AccountUpdatedEvent += (account) => Console.WriteLine("fi1 - Modification de l'élément bancaire id {1}, '{0}'", account.Name, account.Id);
			fi.Accounts.AccountRemovedEvent += (account) => Console.WriteLine("fi1 - Suppression de l'élément bancaire id {1}, '{0}'", account.Name, account.Id);
			// catégories
			fi.Categories.CategoryAddedEvent += (category) => Console.WriteLine("fi1 - Ajout de la catégorie id {2}, '{0}' de type {1}", category.Name, category.GetType().Name, category.Id);
			fi.Categories.CategoryUpdatedEvent += (category) => Console.WriteLine("fi1 - Modification de la catégorie id {1}, '{0}'", category.Name, category.Id);
			fi.Categories.CategoryRemovedEvent += (category) => Console.WriteLine("fi1 - Suppression de la catégorie id {1}, '{0}'", category.Name, category.Id);
			// moyens de paiements
			fi.Paytypes.PaytypeAddedEvent += (paytype) => Console.WriteLine("fi1 - Ajout d'un moyen de paiements id {2}, '{0}' de type {1}", paytype.Name, paytype.GetType().Name, paytype.Id);
			fi.Paytypes.PaytypeUpdatedEvent += (paytype) => Console.WriteLine("fi1 - Modification d'un moyen de paiements id {1}, '{0}'", paytype.Name, paytype.Id);
			fi.Paytypes.PaytypeRemovedEvent += (paytype) => Console.WriteLine("fi1 - Suppression d'un moyen de paiements id {1}, '{0}'", paytype.Name, paytype.Id);
			// tiers
			fi.Thirdparties.ThirdpartyAddedEvent += (thirdparty) => Console.WriteLine("fi1 - Ajout d'un tiers id {2}, '{0}' de type {1}", thirdparty.Name, thirdparty.GetType().Name, thirdparty.Id);
			fi.Thirdparties.ThirdpartyUpdatedEvent += (thirdparty) => Console.WriteLine("fi1 - Modification d'un tiers id {1}, '{0}'", thirdparty.Name, thirdparty.Id);
			fi.Thirdparties.ThirdpartyRemovedEvent += (thirdparty) => Console.WriteLine("fi1 - Suppression d'un tiers id {1}, '{0}'", thirdparty.Name, thirdparty.Id);
			// transferts
			fi.Accounts.Transfers.TransferAddedEvent += (transfer) => Console.WriteLine("fi1 - Ajout d'un transfert id {2}, '{0}' de type {1}", transfer.Name, transfer.GetType().Name, transfer.Id);
			fi.Accounts.Transfers.TransferUpdatedEvent += (transfer) => Console.WriteLine("fi1 - Modification d'un transfert id {1}, '{0}'", transfer.Name, transfer.Id);
			fi.Accounts.Transfers.TransferRemovedEvent += (transfer) => Console.WriteLine("fi1 - Suppression d'un transfert id {1}, '{0}'", transfer.Name, transfer.Id);
			// operations
			fi.Accounts.Items.ForEach(a =>
				{
					a.Operations.OperationAddedEvent += (operation) => Console.WriteLine("fi1 - {0} - Ajout d'une opération id {3}, '{1}' de type {2}", a.Name, operation.Name, operation.GetType().Name, operation.Id);
					a.Operations.OperationUpdatedEvent += (operation) => Console.WriteLine("fi1 - {0} - Modification d'une opération id {2}, '{1}'", a.Name, operation.Name, operation.Id);
					a.Operations.OperationRemovedEvent += (operation) => Console.WriteLine("fi1 - {0} - Suppression d'une opération id {2}, '{1}'", a.Name, operation.Name, operation.Id);
				});
			
			// Création d'une nouvelle programmation
			Console.WriteLine();
			var p = new Event(name: "essai",
				        eventAction: new Operation(name: "Abonnement YouTube",
					        date: new DateTime(2021, 7, 5),
					        amount: -14m,
					        toId: ba.Id,
					        typeId: bc.Id,
					        categoryId: fi.Categories[0].Id,
					        active: true),
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
			Console.WriteLine("Activation de la programmation");
			p.Active = true;
			fi.Events.Add(p);
			
			// Ajout de l'élément bancaire 'Portefeuille d'espèces' au dossie financier
			Console.WriteLine();
			Console.WriteLine("Ajout d'un portefeuille d'èspèces...");
			fi.Accounts.Add(wl);
			Console.WriteLine("renommage du portefeuille d'espèces en 'Mon porte monnaie'...");
			wl.Name = "Mon porte monnaie";
			
			// Sauvegarde du dossier financier
			Console.WriteLine();
			Console.WriteLine("Enregistrement du dossier financier...");
			string filepath = fi.SaveToFile(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), password: "bob");
			
			
			//###############################################################################################################
			
			// Chargement du dossier financier précédement enregistré
			Console.WriteLine();
			Console.WriteLine("Rechargement du dossier financier...");
			var fi2 = Financial.LoadFromFile(filepath, password: "bob");
			
			// Abonnements aux événements
			// dossier financier
			fi2.UpdatedEvent += (sender, e) => Console.WriteLine("fi2 - Modification d'une propriété de type: " + sender.GetType().Name);
			fi.SavedEvent += (sender, e) => Console.WriteLine("fi2 - Sauvegardé.");
			// événements programmés
			fi2.Events.PostEventAddedEvent += (postEvent) => Console.WriteLine("fi2 - Ajout de la programmation id {2}, '{0}' de type {1}", postEvent.Name, postEvent.GetType().Name, postEvent.Id);
			fi2.Events.PostEventUpdatedEvent += (postEvent) => Console.WriteLine("fi2 - Modification de la programmation id {1}, '{0}'", postEvent.Name, postEvent.Id);
			fi2.Events.PostEventRemovedEvent += (postEvent) => Console.WriteLine("fi2 - Suppression de la programmation id {1}, '{0}'", postEvent.Name, postEvent.Id);
			fi2.PostRaisedEvent += (date, postEvent) =>
			{
				Console.WriteLine("fi2 - L'occurence '{0}' ({2}, {3}) programmée pour le {1} vient dêtre postée.",
					postEvent.Name,
					date.ToLongDateString(),
					postEvent.EventAction.Name,
					fi2.Currency.Format(postEvent.EventAction.Amount));
				Console.WriteLine("dates restantes: {0}", 
					string.Join(", ", postEvent.GetNextCalendar().Select(d => d.ToLongDateString())));
				Console.WriteLine("prochaine date: {0} ({1}/{2} occurences restantes)\r\n",
					postEvent.NextDate.ToLongDateString(), 
					postEvent.Counter,
					postEvent.RepeatCount);
			};
			// éléments bancaires
			fi2.Accounts.AccountAddedEvent += (account) => Console.WriteLine("fi2 - Ajout de l'élément bancaire id {2}, '{0}' de type {1}", account.Name, account.GetType().Name, account.Id);
			fi2.Accounts.AccountUpdatedEvent += (account) => Console.WriteLine("fi2 - Modification de l'élément bancaire id {1}, '{0}'", account.Name, account.Id);
			fi2.Accounts.AccountRemovedEvent += (account) => Console.WriteLine("fi2 - Suppression de l'élément bancaire id {1}, '{0}'", account.Name, account.Id);
			// catégories
			fi2.Categories.CategoryAddedEvent += (category) => Console.WriteLine("fi2 - Ajout de la catégorie id {2}, '{0}' de type {1}", category.Name, category.GetType().Name, category.Id);
			fi2.Categories.CategoryUpdatedEvent += (category) => Console.WriteLine("fi2 - Modification de la catégorie id {1}, '{0}'", category.Name, category.Id);
			fi2.Categories.CategoryRemovedEvent += (category) => Console.WriteLine("fi2 - Suppression de la catégorie id {1}, '{0}'", category.Name, category.Id);
			// moyens de paiements
			fi2.Paytypes.PaytypeAddedEvent += (paytype) => Console.WriteLine("fi2 - Ajout d'un moyen de paiements id {2}, '{0}' de type {1}", paytype.Name, paytype.GetType().Name, paytype.Id);
			fi2.Paytypes.PaytypeUpdatedEvent += (paytype) => Console.WriteLine("fi2 - Modification d'un moyen de paiements id {1}, '{0}'", paytype.Name, paytype.Id);
			fi2.Paytypes.PaytypeRemovedEvent += (paytype) => Console.WriteLine("fi2 - Suppression d'un moyen de paiements id {1}, '{0}'", paytype.Name, paytype.Id);
			// tiers
			fi2.Thirdparties.ThirdpartyAddedEvent += (thirdparty) => Console.WriteLine("fi2 - Ajout d'un tiers id {2}, '{0}' de type {1}", thirdparty.Name, thirdparty.GetType().Name, thirdparty.Id);
			fi2.Thirdparties.ThirdpartyUpdatedEvent += (thirdparty) => Console.WriteLine("fi2 - Modification d'un tiers id {1}, '{0}'", thirdparty.Name, thirdparty.Id);
			fi2.Thirdparties.ThirdpartyRemovedEvent += (thirdparty) => Console.WriteLine("fi2 - Suppression d'un tiers id {1}, '{0}'", thirdparty.Name, thirdparty.Id);
			// transferts
			fi2.Accounts.Transfers.TransferAddedEvent += (transfer) => Console.WriteLine("fi2 - Ajout d'un transfert id {2}, '{0}' de type {1}", transfer.Name, transfer.GetType().Name, transfer.Id);
			fi2.Accounts.Transfers.TransferUpdatedEvent += (transfer) => Console.WriteLine("fi2 - Modification d'un transfert id {1}, '{0}'", transfer.Name, transfer.Id);
			fi2.Accounts.Transfers.TransferRemovedEvent += (transfer) => Console.WriteLine("fi2 - Suppression d'un transfert id {1}, '{0}'", transfer.Name, transfer.Id);
			// operations
			fi2.Accounts.Items.ForEach(a =>
				{
					a.Operations.OperationAddedEvent += (operation) => Console.WriteLine("fi2 - {0} - Ajout d'une opération id {3}, '{1}' de type {2}", a.Name, operation.Name, operation.GetType().Name, operation.Id);
					a.Operations.OperationUpdatedEvent += (operation) => Console.WriteLine("fi2 - {0} - Modification d'une opération id {2}, '{1}'", a.Name, operation.Name, operation.Id);
					a.Operations.OperationRemovedEvent += (operation) => Console.WriteLine("fi2 - {0} - Suppression d'une opération id {2}, '{1}'", a.Name, operation.Name, operation.Id);
				});
		
			// Affichage pour l'exemple des données brutes du dossier financier
			Console.WriteLine("Affichage du contenu brut:");
			Console.WriteLine("------------------------------------------------------------------------------------------------------");
			Console.WriteLine(Encoding.UTF8.GetString(fi2.Serialize()));
			Console.WriteLine("------------------------------------------------------------------------------------------------------");
			
			// Poste automatiquement toutes les occurences programmées en retard (jusqu'a ce jour)
			Console.WriteLine();
			Console.WriteLine("Postage automatique de toutes les occurences restantes en retard...\r\n");
			fi2.AutoPostOverdue();
			
			
			//###############################################################################################################
			
			// Pour l'exemple, calcule et affichage formatté du solde total de tous les éléments bancaires
			Console.WriteLine();
			Console.WriteLine("Solde total: {0}", fi2.Currency.Format(fi2.AmountAt(DateTime.Now, addInitialAmount: true)));
			
			// Pour l'exemple, liste toutes les monnaies disponibles et indique celle du systeme
			Console.WriteLine();
			foreach (var c in Currencies.Availlable.OrderBy(a => a.CultureFullname))
			{
				if (c.IsCurrentCulture)
					Console.WriteLine("Culture système: {0} | Monnaie: {1} {2}", c.CultureFullname, c.Symbol, c.Name);
			}
			
			Console.WriteLine();
			var now = DateTime.Now;
			Console.WriteLine("Aujourd'hui nous sommes le {0}", now.ToLongDateString());
			Console.WriteLine("le premier jour de la semaine est le {0}", CultureInfo.GetCultureInfo(fi.CultureName).GetFirstDateOfWeek(now).ToLongDateString());
			Console.WriteLine("le dernier jour de la semaine est le {0}", CultureInfo.GetCultureInfo(fi.CultureName).GetLastDateOfWeek(now).ToLongDateString());
			Console.WriteLine("le premier jour du mois est le {0}", CultureInfo.GetCultureInfo(fi.CultureName).GetFirstDateOfMonth(now).ToLongDateString());
			Console.WriteLine("le dernier jour du mois est le {0}", CultureInfo.GetCultureInfo(fi.CultureName).GetLastDateOfMonth(now).ToLongDateString());
			Console.WriteLine("le premier jour de l'année est le {0}", CultureInfo.GetCultureInfo(fi.CultureName).GetFirstDateOfYear(now).ToLongDateString());
			Console.WriteLine("le dernier jour de l'année est le {0}", CultureInfo.GetCultureInfo(fi.CultureName).GetLastDateOfYear(now).ToLongDateString());
			
			var l = fi2.GetEventsInfosAt(fi2.Accounts[0].Id, new DateTime(2021, 7, 1), new DateTime(2021, 8, 31));
			

			
			Console.ReadLine();
		}

	}

}
