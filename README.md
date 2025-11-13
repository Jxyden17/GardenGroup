Project NoSQL - Jaar 2 Periode 1
Inholland Haarlem
Environment Setup

De .env file staat op Trello.
Zorg ervoor dat deze correct wordt ingesteld voordat je het project uitvoert.

Verantwoordelijkheden

Jayden:

The application has a built-in rights management system to differentiate between regular and Service Desk employees.
alle user-acties zijn ook geüpdatet zodat het werkt met het Identity Framework.

Waarom heb ik voor het Microsoft identity framework met mongodb gekozen? 
omdat het geadviseerd werdt om het te gebruiken zodat je makkelijke gebruik kan maken van rights management system en de andere extra functionaliteiten.

Waarom zijn er meerdere viewModels die erg op elkaar lijken? 
Door aparte viewModels te gebruiken maak je de applicatie meer modular waardoor latere aanpassingen in de code makkelijker worden.

Individual Functionality: 
Forget Password-functionaliteit. De gebruiker moet in staat zijn om zijn/haar wachtwoord te resetten via een reset e-mail met een unieke link.
Het is vereist om hiervoor het MVC Framework (C#) of het MVC-patroon (Java, PHP, etc.) te gebruiken bij de implementatie.

Waarom heb ik  gekozen voor de Forget Password-functionaliteit?
Omdat het een veelgebruikte functionaliteit is in webapplicaties en het biedt een goede gelegenheid om te werken met e-mailintegratie.
Ik heb er ook voor gekozen om gebruik te maken van Mailkit omdat het een populaire en betrouwbare bibliotheek is voor het verzenden van e-mails in .NET-toepassingen.

Ook heb ik gebruik gemaakt van het Identity Framework omdat het al ingebouwde methoden heeft voor het beheren van gebruikerswachtwoorden en beveiliging, wat de implementatie van de wachtwoordresetfunctionaliteit vereenvoudigt.

Om de functionaliteit te testen heb ik gebruik gemaakt van smtp4dev, een lokale SMTP-server die e-mails opvangt zonder ze daadwerkelijk te verzenden wat lokaal testen heel gemakkelijk maakt.
👉 smtp4dev - v3.10.3 - https://github.com/rnwood/smtp4dev


Ernest: 
📊 Dashboard & Charts
Het applicatie-dashboard is het centrale punt voor gebruikersinzichten. Het is ontworpen met focus op performance en gebruiksgemak, waarbij elke gebruikersrol een op maat gemaakt overzicht krijgt.

1. Functionaliteit per Rol
Dankzij Role-Based Authorization krijgt elke gebruiker specifieke KPI's (Key Performance Indicators) te zien:

Regular Employee: Ziet direct hoeveel eigen tickets nog open staan en hoeveel er te laat zijn (Over Deadline).
Service Desk: Heeft focus op eigen productiviteit: "Mijn Taken" (huidige werkvoorraad) en "Opgelost door mij".
Administrator: Krijgt een helikopterview van het hele systeem: Totaal aantal open tickets, alle tickets (Over Deadline) en de dagprestatie (Vandaag Gesloten).

*De statistieken worden visueel weergegeven met herbruikbare Partial Views (`_StatusDonut`).*

Mijn Individuele Component: Ticket Transfer
Ik heb de functionaliteit gebouwd waarmee Service Desk medewerkers en Admins een ticket kunnen **overdragen (transfer)** aan een collega.

### Hoe het werkt:
1.  **ViewModel:** `TransferTicketViewModel` toont het ticket én een dropdown met beschikbare Service Desk collega's.
2.  **Service:** Haalt gefilterde lijst van collega's op (sluit de huidige gebruiker uit).
3.  **Repository:** Gebruikt `UpdateOneAsync` met de `$set` operator om alleen de `Solver` van het ticket aan te passen (veilig en efficiënt).

🔮 Reflectie & Toekomstige Verbetering (Backlog)
Tijdens het testen van de Transfer-functionaliteit realiseerde ik me dat de huidige dropdown-lijst minder gebruiksvriendelijk wordt naarmate het Service Desk team groeit (bijvoorbeeld naar 50+ medewerkers). 
Het is voor een gebruiker niet wenselijk om eindeloos te moeten scrollen om de juiste collega te vinden.

Hoewel dit nog niet in de huidige versie is geïmplementeerd, heb ik wel onderzoek gedaan naar een oplossing voor de volgende sprint:
- Oplossing: Implementatie van Bootstrap Select (of een vergelijkbare library).
- Voordeel: Dit voegt een zoekbalk toe binnenin de dropdown (live search). Hierdoor kunnen gebruikers simpelweg de naam van een collega typen om te filteren,
  in plaats van te scrollen. Dit is een standaard UX-patroon dat bij veel grote bedrijven wordt toegepast.


Menno:

Individuele functionaliteit:
Archiving the entire database (For example all tickets older than 2 years): With a simple click
on a buton, several entries before a certain date, are moved to a secondary (archive)
database.



##Gebruikersnamen en wachtwoord.
User: user@test.nl
ServiceDesk: sd@test.nl
Admin: admin@test.nl

Wachtwoord: Welkom1234