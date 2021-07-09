**v0.1-alpha.3**
- Financial.ToOfx : *Exporte les mouvements bancaires d'un élément au format OFX*
- Financial.ToQif : *Exporte les données du portefeuille au format OFX*

**v0.1-alpha.2**
- Ajout de la compatibilité Mono 6.12 : *Désormais, Kotlib_452.sln est compatible .Net 4.5.2 et Mono 6.12. Peut aussi être édité par Sharpdevelop 5, Monodevelop 7.8, VSCode, et Visual Studio. Kotlib.sln est compatible .Net 5, éditable avec Visual Studio 2019 à minima.*
- Financial.GetEventsInfosAt : *Permet de connaitre le nombre d'événements à une date précise ou entre 2 dates. Renvoie aussi le total des revenus et le total des dépenses provoqués par les événements trouvés.*
- Financial.Export2Html : *Exporte les mouvements bancaires d'un élément au format HTML*
- Financial.Export2CSV : *Exporte les mouvements bancaires d'un élément au format CSV*
- Financial.Export2List : *Exporte les mouvements bancaires sous forme d'un relevé de compte*
- Automatisation des opérations et transferts
- Ajout d'événements pour remonter les ajouts, modifications, et suppressions *d'éléments dans les différentes liste (dérivées d'ObjectList).*
- Modification des entètes de fichier (correction du texte de licence)
- Kotlib.Objects.Event, Kotlib.Objects.EventList, AutoPost, AutoPostOverdue, AutoPostAll, AutoPostUntil : *Gestion des opérations et transferts programmables.*
- Modification du fichier README.md
- Kotlib.Tools.Currency : *Gestion et formattage monétaire*

**v0.1-alpha.1**
- Première publication