# Istoricul modificărilor

## Modificări nepublicate

### Credit auto și închiriere

- Butoane separate „Credit auto” și „Închiriere auto” în meniul principal.
- Credit: avans, dobândă anuală introdusă manual, perioadă 1–120 luni, rate lunare fixe, ultima rată ajustată la cent și cost total.
- La înregistrarea creditului, automobilul devine Sold. Ratele achitate se înregistrează manual, cu sold rămas și următoarea scadență în detalii.
- Închiriere: date de preluare/returnare, tarif zilnic introdus manual și cost total; data returnării nu se taxează.
- Automobilul închiriat rămâne Reserved până la confirmarea returnării în Reports, apoi revine la Available.
- Reports → „Credite și închirieri”: listă contracte, detalii, înregistrarea ratelor sau returnării.
- Migrația `010_credit_and_rental.sql`; validarea datelor și salvarea tranzacțională previn contractele concurente pentru aceeași mașină.
- Modulele înregistrează contracte și încasări manuale; nu efectuează plăți sau aprobări bancare.

### Corectarea deschiderii formularului de editare

- Kilometrajul acceptă întregul interval pozitiv al câmpului INTEGER din baza de date, inclusiv 1 245 523 km.
- Limita este aceeași la înregistrare, editare și Trade-in.
- Editarea acceptă și anul următor și prețurile sub 1 000 €, posibile pentru automobilele primite prin Trade-in.
- Fișiere: Pages/EditCarPage.cs, Pages/RegisterCarPage.cs și Pages/OrderPage.cs.

### Editarea prețului unei comenzi Trade-in

- În Reports, selectezi comanda și deschizi „Detalii trade-in”.
- Poți modifica prețul negociat al mașinii dorite și salva modificarea.
- Diferența dintre prețul negociat și evaluarea mașinii oferite se recalculează automat.
- Prețul negociat se aplică doar comenzii respective.
- Comenzile finalizate nu mai permit modificarea prețului.
- Salvarea verifică dacă între timp comanda a fost modificată sau finalizată.

Fișiere:

- `Pages/TradeInDetailsPage.cs`: formularul pentru vizualizarea și editarea prețului.
- `Pages/OrderReportsPage.cs`: deschiderea detaliilor și actualizarea listei.
- `Database/Repositories/OrderRepository.cs`: salvarea prețului negociat.

## Funcționalități publicate anterior

### Comenzi și Reports

- Formular cu datele clientului, automobil, cantitate și metoda de plată Cash/Card.
- Validarea câmpurilor obligatorii, a telefonului și a emailului.
- Salvarea comenzilor prin migrațiile bazei de date.
- Rezervarea automată a automobilului la crearea comenzii.
- Afișarea tuturor comenzilor în Reports și finalizarea lor prin bifă.
- Trecerea automobilului în Sold la finalizare.
- Afișarea Reports maximizat, cu coloane adaptate lățimii ferestrei.

### Trade-in

- Formular separat pentru datele clientului și mașina oferită la schimb.
- Evaluare manuală și calculul diferenței pentru automobilul dorit.
- Afișarea sumei de restituit dacă evaluarea depășește prețul automobilului dorit.
- Adăugarea mașinii primite în inventar la finalizare, cu statut Available și prețul evaluat.
- Migrația `009_trade_in_orders.sql` pentru datele schimbului auto.

### Interfață și autentificare

- Revenire la logare după crearea unui cont, fără închiderea aplicației.
- Corectarea erorilor la apăsarea celulelor din inventar și la desenarea gradientelor.
- Aspect uniform al butoanelor laterale și evidențiere temporară la apăsare.
- Pictogramă pentru comanda nouă.
- Redesign pentru „Prețuri după culoare”, cu preț minim și maxim.

### Executabil

- Publicare Windows x64 cu componentele .NET incluse.
- Resursele și migrațiile sunt distribuite împreună cu aplicația.
- Folderul `Aplicatie`, `bin` și `obj` sunt excluse din Git.
