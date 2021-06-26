![Kotlib.net](https://pantaflex44.me/wp-content/uploads/2021/06/kotlib_back.png)
## Finances personnelles assistées par ordinateur
***Librairie .Net pour la gestion assistée par ordinateur des finances personnelles.***

- Compatibilité .Net 5 : `Kotlib.dll`
- Compatibilité .Net 4.5.2 : `Kotlib_452.dll`

[pantaflex44/Kotlib.net: Librairie .Net 5 pour la gestion assistée par ordinateur des finances personnelles. (github.com)](https://github.com/pantaflex44/Kotlib.net)



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
			
// Création d'une identité
var me = new Identity(name: "Christophe LEMOINE")
{
	Lastname = "LEMOINE",
	Forname = "Christophe"
};

// Création d'un élément bancaire de type 'Carte bancaire'
var bc = new BankCard(name: "CIC Mastercard Tof")
{
	Number = "5135 1800 0000 0001",
	CVV = "123",
	Date = new CardDate(2025, 12)
};
bc.Name = "CIC Mastercard";

// Création d'un élément bancaire de type 'Compte bancaire'
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

// Création d'un nouveau dossier financier
// puis ajout des informations créées précédement
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
// abonnement aux différents événements
fi.UpdatedEvent += (sender, e) => Console.WriteLine("fi1 updated " + sender.GetType().UnderlyingSystemType);
fi.SavedEvent += (sender, e) => Console.WriteLine("fi1 saved.");

// Création d'un événement programmable
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

// Activation de l'événement prrogrammable
Console.WriteLine();
Console.WriteLine("Activation de la programmation");
p.Active = true;
fi.Events.Add(p);

// Enregistrement du dossier financier
Console.WriteLine();
Console.WriteLine("Enregistrement du dossier financier...");
string filepath = fi.SaveToFile(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), password: "bob");

// Ouverture et rechargement du dossier financier
Console.WriteLine();
Console.WriteLine("Rechargement du dossier financier...");
var fi2 = Financial.LoadFromFile(filepath, password: "bob");
// Réabonnement aux événements
fi2.UpdatedEvent += (sender, e) => Console.WriteLine("fi2 updated " + sender.GetType().UnderlyingSystemType);
byte[] datas2 = fi2.Serialize();

// Affichage du contenu brute
Console.WriteLine();
Console.WriteLine("Affichage du contenu brut:");
Console.WriteLine(Encoding.UTF8.GetString(datas2));

// Exemple d'affichage du solde total de tous les éléments bancaires reunis
Console.WriteLine();
Console.WriteLine("Solde total: {0}", fi2.Currency.Format(fi2.AmountAt(DateTime.Now, addInitialAmount: true)));

// Liste toutes les monnaies disponibles et détecte celle qui correspond au systeme
Console.WriteLine();
foreach (var c in Currencies.Availlable.OrderBy(a => a.CultureFullname))
{
	if (c.IsCurrentCulture)
		Console.WriteLine("Culture système: {0} | Monnaie: {1} {2}", c.CultureFullname, c.Symbol, c.Name);
}

// Affiche le premier événement programmable
if (!fi2.Accounts.GetById(fi2.Events[0].AccountId).Equals(default(Account)))
{
	Console.WriteLine();
	Console.WriteLine("Postage des occurences passées pour le compte {0} depuis le {1}",
		fi2.Accounts.GetById(fi2.Events[0].AccountId).Name,
		fi2.Events[0].StartDate.ToLongDateString());
	
    // Poste toutes les occurences restantes en retard de cet événement
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

Console.ReadLine();`
```





<small>Copyright (C) 2020-2021 Christophe LEMOINE</small>
