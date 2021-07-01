## Sommaire

* [Les catégories](https://github.com/pantaflex44/Kotlib.net/wiki/#les-catégories)
* [Liste des catégories](https://github.com/pantaflex44/Kotlib.net/wiki/#liste-des-cat%C3%A9gories)



## Les catégories

```c#
Kotlib.Objects.Category.cs
```

`Category` représente un conteneur spécifiant d'une part, le nom de la catégorie, mais aussi la liste des catégories enfants.

Les catégories peuvent être hiérarchisées à l'infini. Deux catégories peuvent porter le même nom si leur parent est différent.

```c#
Category(string name, CategoryList childs = null)
```



#### Propriétés

- `Childs` <small>*CategoryList*</small> : Liste des catégories enfants.
- `Id` <small>*Guid*</small> : Identifiant unique.
- `Name` <small>*string*</small> : Nom donné à la catégorie.



#### Utilisation

```c#
var abonnements = new Category(name: "Abonnements",
                               childs: new CategoryList(new List<Category>
                                                        {
                                                        	new Category("TV"),
                                                        	new Category("Musique"),
                                                        	new Category("Téléphonie"),
                                                        	new Category("Internet"),
                                                        	new Category("Sport et Fitness"),
                                                        	new Category("Divers")
                                                        })
                              );
```



## Liste des catégories

```c#
Kotlib.Objects.CategoryList.cs
```

`CategoryList` représente une liste de `Category`. Elle permet la gestion complète de cette liste, et possède des fonctionnalités qui lui sont propres.

`CategoryList` est dérivée de `ObjectList`.

```c#
CategoryList(IEnumerable<Category> items)
```



#### Propriétés

- `Count` <small>*int*</small> : Nombre de catégories dans la liste.
- `IsReadOnly` <small>*bool*</small> : Liste en lecture seule.
- `Items` <small>*List&lt;Category&gt;*</small> : Liste générique représentant la liste des catégories.
- ` Empty` <small>*CategoryList*</small> : Nouvelle liste vide.
- `Defaults` <small>*CategoryList*</small> : Liste des catégories par défaut incluses dans la librairie.
  - <u>Abonnements</u> : TV, Musique, Téléphonie, Internet, Sport et Fitness, Divers
  - <u>Véhicules</u> : Carburant, Assurance, Entretien, Réparation, Parking, Divers
  - <u>Maison</u> : Loyer, Assurance, Electricité, Gaz, Eau, Internet, Courses, Participation, Divers
  - <u>Revenus</u> : Salaire, Acompte, Don, Remboursement, Congés payés, Divers
  - <u>Taxes</u> : Amende, Impôts
  - <u>Divers</u> : Crédit, Remboursement, Don, Frais, Frais bancaire, Emprunt, Participation, Divers
  - <u>Santé et bien être</u> : Médecin, Hôpital, Pharmacie, Médecine alternative, Coiffeur, Epilation, Massage et SPA, Sport et Fitness, Divers



#### Méthodes

- `void Add(Category item)` : Ajoute une nouvelle catégorie.
- `void Clear()` : Vide la liste des catégories.
- `bool Contains(T item)` : Retourne `true` si la catégorie existe, sinon, `false`.
- `void CopyTo(Category[] array, int arrayIndex)` : Copie une portion d'une liste de catégories à partir de la position `arrayIndex`.
- `IEnumerator<Category> GetEnumerator()` : Retourne un énumérateur.
- `IEnumerator IEnumerable.GetEnumerator()` : Retourne un énumérateur.
- `int IndexOf(Category item)` : Retourne la position d'une catégorie dans la liste.
- `void Insert(int index, Category item)` : Insère une catégorie dans la liste à la position `index`.
- `bool Remove(Category item)` : Supprime une catégorie.
- `int RemoveAll(Predicate<Category> predicate)` : Supprime une ou plusieurs catégories en fonction des conditions `Linq` représentées par `predicate`. Retourne le nombre de catégories supprimées.
- `void RemoveAt(int index)` : Supprime une catégorie à la position `index`.
- `Category GetById(Guid id)` : Retourne une catégorie associée à l'identifiant unique `id` passé en paramètre.



#### Evénements

- `event CategoryEventHandler CategoryAddedEvent` : Se produit lorsqu'une catégorie a été ajoutée à la liste.
- `event CategoryEventHandler CategoryUpdatedEvent` : Se produit lorsqu'une catégorie a été modifiée.
- `event CategoryEventHandler CategoryRemovedEvent` : Se produit lorsqu'une catégorie a été supprimée



