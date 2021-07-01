## Sommaire

* [L'élément bancaire](https://github.com/pantaflex44/Kotlib.net/wiki/Les-%C3%A9l%C3%A9ments-bancaires/#l%C3%A9l%C3%A9ment-bancaire)
    * [L'élément bancaire par défaut](https://github.com/pantaflex44/Kotlib.net/wiki/Les-%C3%A9l%C3%A9ments-bancaires#l%C3%A9l%C3%A9ment-bancaire-par-d%C3%A9faut)
    * [Le compte bancaire](https://github.com/pantaflex44/Kotlib.net/wiki/Les-%C3%A9l%C3%A9ments-bancaires#le-compte-bancaire)
    * [La carte de paiements](https://github.com/pantaflex44/Kotlib.net/wiki/Les-%C3%A9l%C3%A9ments-bancaires#la-carte-de-paiements)
    * [Le portefeuille d'espèces](https://github.com/pantaflex44/Kotlib.net/wiki/Les-%C3%A9l%C3%A9ments-bancaires#le-portefeuille-desp%C3%A8ces)
* [Liste des éléments bancaires](https://github.com/pantaflex44/Kotlib.net/wiki/Les-%C3%A9l%C3%A9ments-bancaires#liste-des-%C3%A9l%C3%A9ments-bancaires)



## L'élément bancaire

```c#
Kotlib.Objects.Account.cs
```



### L'élément bancaire par défaut

`Account` représente un élément bancaire. C'est l'objet de base dont hérite les suivants. Il permet aussi à l'utilisateur de créer son propre type d'élément bancaire.

```c#
Account(string name, Identity owner)
```



#### Propriétés

- `AllowedCredit` <small>*decimal*</small> : Montant du découvert autorisé. Montant positif.
- `Id` <small>*Guid*</small> : Identifiant unique.
- `InitialAmount` <small>*decimal*</small> : Solde initial à la création de cet élément bancaire.
- `Name` <small>*string*</small> : Dénomination de l'élément bancaire.
- `Note` <small>*string*</small> : Informations complémentaires libres.
- `Operations` <small>*OperationList*</small> : Liste des opérations associées.
- `Owner` <small>*Identity*</small> : Identité du propriétaire.
- `PartialAmount` <small>*decimal*</small> : Retourne le solde total des opérations associées.
- `Paytypes` <small>*PaytypeList*</small> : Liste des moyens de paiements associés.



#### Méthodes

- `decimal PartialAmountAt(DateTime date, bool addInitialAmount = true)` : Calcule le solde des opérations liées à la date souhaitée, incluant ou non le solde initial.



#### Utilisation

```c#
var me = new Identity(name: "Ma carte d'identité")
{
    Address = "1 rue des mimosas, 12345 Borne",
    Forname = "Christophe",
    Lastname = "Lemoine",
    Mail = "pantaflex@tuta.io",
    Note = "Born to be code",
    Phone = "0102030405",
    Url = "https://pantaflex44.me"
};

var account = new Account(name: "Mon élément bancaire",
                          owner: me)
{
	AllowedCredit = 200m,
	InitialAmount = 1300m,
	Note = "Compte principal",
	Owner = me,
	Operations = OperationList.Empty,
	Paytypes = PaytypeList.Defaults				
};
```



### Le compte bancaire

`BankAccount` est dérivé de `Account` et représente un compte bancaire.

```c#
BankAccount(string name, Identity owner)
```



#### Propriétés

- `BankName` <small>*string*</small> : Nom de l'organisme bancaire.
- `Bic` <small>*string*</small> : Code BIC.
- `Contact` <small>*Identity*</small> : Identité du contact ou conseiller financier.
- `Iban` <small>*string*</small> : Numéro IBAN.



#### Utilisation

```c#
var me = new Identity(name: "Ma carte d'identité")
{
    Address = "1 rue des mimosas, 12345 Borne",
    Forname = "Christophe",
    Lastname = "Lemoine",
    Mail = "pantaflex@tuta.io",
    Note = "Born to be code",
    Phone = "0102030405",
    Url = "https://pantaflex44.me"
};

var bankaccount = new BankAccount(name: "Mon compte en banque",
                                  owner: me)
{
	AllowedCredit = 200m,
	InitialAmount = 1300m,
	Note = "Compte chèque",
	Owner = me,
	Operations = OperationList.Empty,
	Paytypes = new PaytypeList(new List<Paytype>()
	                           {
	                           		new BankCard(name: "Ma carte bancaire")
	                           		{
	                           			Number = "5135 1800 0000 0001",
	                           			CVV = "123",
	                           			Date = new CardDate(2025, 12)
	                           		},
	                           		new Check(name: "Mon chéquier")
	                           }),
	BankName = "CIC",
	Iban = "FR76 5896 1234 5678 9012 3456 789",
	Bic = "CMCIFRPP",
	Contact = new Identity(name: "Mon conseiller")
	{
	    Address = "586 route de la soif, 98745 Rennes",
	    Forname = "Jean",
	    Lastname = "Martin",
	    Mail = "j.martin@cic.org",
	    Phone = "0102030405"
	}
};
```



### La carte de paiements

`Paycard` est dérivée de `Account` et représente une carte de paiement autonome (PCS, etc.)

```c#
Paycard(string name, Identity owner, BankCard card)
```



#### Propriétés

- `Card` <small>*BankCard*</small> : Informations de la carte de paiement.



#### Utilisation

```c#
var me = new Identity(name: "Ma carte d'identité")
{
    Address = "1 rue des mimosas, 12345 Borne",
    Forname = "Christophe",
    Lastname = "Lemoine",
    Mail = "pantaflex@tuta.io",
    Note = "Born to be code",
    Phone = "0102030405",
    Url = "https://pantaflex44.me"
};

var paycard = new Paycard(name: "Ma carte de paiement",
                          owner: me,
                          card: new BankCard(name: "N26")
                          {
                          	Number = "5135 1800 0000 0001",
	                        CVV = "123",
	                        Date = new CardDate(2025, 12)
                          })
{
	AllowedCredit = 0m,
	InitialAmount = 2500m,
	Note = "Compte N26",
	Owner = me,
	Operations = OperationList.Empty,
	Paytypes = PaytypeList.Empty
};
```



### Le portefeuille d'espèces

`Wallet` est dérivé de `Account` et représente un porte monnaie, le fond d'une poche ou les économies sous le matelas de mamie.

```c#
Wallet(string name, Identity owner)
```



#### Propriétés

- `Electronic` <small>*bool*</small> : Portefeuille électronique.
- ~~`AllowedCredit`~~ <small>*decimal*</small> : Non autorisé.



#### Utilisation

```c#
var me = new Identity(name: "Ma carte d'identité")
{
    Address = "1 rue des mimosas, 12345 Borne",
    Forname = "Christophe",
    Lastname = "Lemoine",
    Mail = "pantaflex@tuta.io",
    Note = "Born to be code",
    Phone = "0102030405",
    Url = "https://pantaflex44.me"
};

var wallet = new Wallet(name: "Mon portefeuille d'espèces",
                        owner: me)
{
	InitialAmount = 10m,
	Note = "Le fond de ma poche",
	Owner = me,
	Operations = OperationList.Empty,
	Paytypes = PaytypeList.Empty,
	Electronic = false
};
```



## Liste des éléments bancaires

```c#
Kotlib.Objects.AccountList.cs
```

`AccountList` représente une liste de type base `Account`. Elle permet la gestion complète de cette liste, et possède des fonctionnalités qui lui sont propres.

`AccountList` est dérivée de `ObjectList`.

```c#
AccountList(IEnumerable<Account> items)
```



#### Propriétés

- `Count` <small>*int*</small> : Nombre d'éléments dans la liste.
- `IsReadOnly` <small>*bool*</small> : Liste en lecture seule.
- `Items` <small>*List&lt;Account&gt;*</small> : Liste générique représentant la liste des éléments.
- ` Empty` <small>*AccountList*</small> : Nouvelle liste vide.
- `Transfers` <small>*TransferList*</small> : Liste des transferts bancaires associés à cet élément.



#### Méthodes

- `void Add(Account item)` : Ajoute un nouvel élément bancaire.
- `void Clear()` : Vide la liste des éléments bancaires.
- `bool Contains(T item)` : Retourne `true` si élément bancaire existe, sinon, `false`.
- `void CopyTo(Account[] array, int arrayIndex)` : Copie une portion d'une liste d'éléments à partir de la position `arrayIndex`.
- `IEnumerator<Account> GetEnumerator()` : Retourne un énumérateur.
- `IEnumerator IEnumerable.GetEnumerator()` : Retourne un énumérateur.
- `int IndexOf(T item)` : Retourne la position d'un élément bancaire dans la liste.
- `void Insert(int index, Account item)` : Insère un élément dans la liste à la position `index`.
- `bool Remove(Account item)` : Supprime un élément bancaire.
- `int RemoveAll(Predicate<Account> predicate)` : Supprime un ou plusieurs éléments bancaires en fonction des conditions `Linq` représentées par `predicate`. Retourne le nombre d'éléments supprimés.
- `void RemoveAt(int index)` : Supprime un élément bancaire à la position `index`.
- `void CleanTransfers()` : Vide la liste des transferts bancaires.
- `static List<Tuple<string, string, Type>> GetAvaillableAccounts()` : Retourne une liste des types d'éléments bancaires inclus dans la librairie. Chaque élément de la liste est formé comme suit: `Tuple<string nom, string description, Type type>`, où `nom` représente la dénomination de l'élément bancaire, `description`, la description de l'élément nommé, et `type`, le type interne de l'élément bancaire.
- `Account GetById(Guid id)` : Retourne un l'élément bancaire associé à l'identifiant unique `id` passé en paramètre.



#### Evénements

- `event AccountEventHandler AccountAddedEvent` : Se produit lorsqu'un élément a été ajouté à la liste.
- `event AccountEventHandler AccountUpdatedEvent` : Se produit lorsqu'un élément a été modifié.
- `event AccountEventHandler AccountRemovedEvent` : Se produit lorsqu'un élément a été supprimé.



