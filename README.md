![Kotlib.net](https://pantaflex44.me/wp-content/uploads/2021/06/kotlib_back.png)
## Finances personnelles assistées par ordinateur
***Librairie .Net pour la gestion assistée par ordinateur des finances personnelles.***

- Compatibilité .Net 5 : `Kotlib.dll`
- Compatibilité .Net 4.5.2 : `Kotlib_452.dll`

[pantaflex44/Kotlib.net: Librairie .Net pour la gestion assistée par ordinateur des finances personnelles. (github.com)](https://github.com/pantaflex44/Kotlib.net)



>  **Auteur**:
>
>  - Christophe LEMOINE <[pantaflex@tuta.io](mailto:%20pantaflex@tuta.io?subject=Kotlib)>
>
>  **Participations**:
>
>  **Aide**:
>
>  - Serveur Discord [NaN | Not a Name  (323076998576603137)](https://discord.com/invite/notaname), salon <u>csharp</u>
>    - Remerciements pour leurs aides et conseils:
>      - *Sehnsucht©*
>      - *Hipster de microservices*
>
>  **Divers:**
>
>  - [PF44 - Code lover (pantaflex44.me)](https://pantaflex44.me/)
>  - [Repositories (github.com)](https://github.com/pantaflex44?tab=repositories)
>
>  **License**
>
>  - [GNU General Public License v3](https://raw.githubusercontent.com/pantaflex44/kotlib/main/LICENSE)





### Exemple d'utilisation:

```c#
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
	InitialAmount = 300.0d
};

//###############################################################################################################

// Création du dossier financier et ajout des informations liées
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
	Console.WriteLine("fi1 - L'occurence '{0}' programmée pour le {1} sur le compte {2} vient dêtre postée.",
		postEvent.Name,
		date.ToLongDateString(),
		fi.Accounts.GetById(postEvent.AccountId).Name);
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
	Console.WriteLine("fi2 - L'occurence '{0}' programmée pour le {1} sur le compte {2} vient dêtre postée.",
		postEvent.Name,
		date.ToLongDateString(),
		fi2.Accounts.GetById(postEvent.AccountId).Name);
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



Console.ReadLine();
```





<small>Copyright (C) 2020-2021 Christophe LEMOINE</small>
