# Backend PR: Uređivanje profila (autor/turista)

## Naslov
Dodavanje prikaza i izmene korisničkog profila (autor/turista) – backend

## Kratak opis
- Dodati REST endpointi za prikaz i izmenu profila turista i autora.
- Kreirani DTO modeli, validacija i AutoMapper profil za mapiranje domena na API odgovore.
- Servisna logika sada automatski kreira prazan profil ako ne postoji i ažurira postojeći.
- Repo sloj i EF konfiguracija omogućavaju jedinstveni profil po osobi; dodati integracioni testovi i SQL seed podaci sa negativnim ID-jevima.

## Detaljnije (šta i zašto)
- **Novi/izmenjeni API endpointi**: `GET /api/tourist/profile`, `PUT /api/tourist/profile`, `GET /api/author/profile`, `PUT /api/author/profile` u kontrolerima `Explorer.API.Controllers.Tourist.ProfileController` i `Explorer.API.Controllers.Author.ProfileController`. Endpointi uzimaju `personId` iz JWT-a i vraćaju/menjaju profil u okviru odgovarajuće politike autorizacije (turist/autor). 
- **DTO i validacija**: `ProfileDto` uvodi obavezna polja `Name` i `Surname` (StringLength 100) i ograničenja od 250 karaktera za biografiju/moto, čime se sprečava predugačak unos. 
- **Servis i mapiranje**: `ProfileService` koristi `IProfileRepository` i `IPersonRepository` da vrati postojeći profil ili ga kreira iz entiteta `Person` ako ne postoji, uz validaciju domenskog modela; mapiranje DTO ↔ domen je u `StakeholderProfile` AutoMapper profilu. 
- **Validacija domena**: Entitet `Profile` proverava obaveznost imena/prezimena, pozitivan `PersonId` i maksimalnu dužinu teksta (250) za biografiju i moto, uz izuzetke tipa `EntityValidationException`. 
- **Repo sloj**: `ProfileDbRepository` iz Stakeholders modula dodaje CRUD za profil (dohvatanje po `PersonId`, kreiranje, ažuriranje) oslanjajući se na EF `StakeholdersContext` koji osigurava jedinstveni indeks i max dužinu kolona. 
- **Testovi i podaci**: Integracioni test `RegistrationTests` proverava uspešno registrovanje turiste i vezivanje `Person` → `User` → `Profile`; SQL skripte `c-people.sql`, `d-profiles.sql`, `a-delete.sql` koriste negativne ID-eve kako bi test podaci bili izolovani od realnih i lako brisani pre/posle testiranja.

## Lista najbitnijih fajlova
- **Kontroleri**
  - `src/Explorer.API/Controllers/Tourist/ProfileController.cs` – GET/PUT rute za turistički profil sa čitanjem `personId` iz tokena. 
  - `src/Explorer.API/Controllers/Author/ProfileController.cs` – GET/PUT rute za autorski profil.
- **DTO / modeli / servisi**
  - `src/Modules/Stakeholders/Explorer.Stakeholders.API/Dtos/ProfileDto.cs` – definicija DTO-a sa DataAnnotations validacijom. 
  - `src/Modules/Stakeholders/Explorer.Stakeholders.Core/Domain/Profile.cs` – domenski entitet i metoda `Update` sa internom validacijom. 
  - `src/Modules/Stakeholders/Explorer.Stakeholders.Core/UseCases/ProfileService.cs` – poslovna logika: kreiranje profila ako ne postoji, ažuriranje i mapiranje. 
  - `src/Modules/Stakeholders/Explorer.Stakeholders.Core/Mappers/StakeholderProfile.cs` – AutoMapper konfiguracija za Profile ↔ ProfileDto. 
  - `src/Modules/Stakeholders/Explorer.Stakeholders.API/Public/IProfileService.cs` – javni interfejs servisa za kontrolere. 
  - `src/Modules/Stakeholders/Explorer.Stakeholders.Infrastructure/Database/Repositories/ProfileDbRepository.cs` – implementacija repozitorijuma za EF Core. 
  - `src/Modules/Stakeholders/Explorer.Stakeholders.Infrastructure/Database/StakeholdersContext.cs` – konfiguracija šeme, jedinstveni indeks i max length za Profile. 
- **Testovi**
  - `src/Modules/Stakeholders/Explorer.Stakeholders.Tests/Integration/Authentication/RegistrationTests.cs` – integracioni test registracije turiste i provera tokena/DB zapisa. 
- **SQL skripte (TestData)**
  - `src/Modules/Stakeholders/Explorer.Stakeholders.Tests/TestData/c-people.sql` – seed test osoba (negativni ID-evi). 
  - `src/Modules/Stakeholders/Explorer.Stakeholders.Tests/TestData/d-profiles.sql` – seed profili (negativni ID-evi). 
  - `src/Modules/Stakeholders/Explorer.Stakeholders.Tests/TestData/a-delete.sql` – čišćenje negativnih ID-eva pre/posle testova.

## Kako pokrenuti backend za testiranje
1. Pokrenuti migracije za StakeholdersContext (primer):
   ```bash
   dotnet ef database update --startup-project src/Explorer.API/Explorer.API.csproj --project src/Modules/Stakeholders/Explorer.Stakeholders.Infrastructure/Explorer.Stakeholders.Infrastructure.csproj --context StakeholdersContext
   ```
2. Startovati API: 
   ```bash
   dotnet run --project src/Explorer.API/Explorer.API.csproj
   ```
3. Swagger je dostupan na `http://localhost:5173/swagger` (ili port naveden u launchSettings).

## Manualno testiranje (Swagger/Postman)
- **Prikaz profila turiste**: prijaviti se kao turista (JWT u Authorize), pozvati `GET /api/tourist/profile`; očekuje se profil sa imenom/prezimenom i opcionim bio/moto/picture. 
- **Izmena profila turiste**: `PUT /api/tourist/profile` sa primerom tela: 
  ```json
  {"name":"Pera","surname":"Perić","biography":"Planinar.","motto":"Carpe diem","profilePictureUrl":"data:image/png;base64,..."}
  ```
  očekuje 200 i vraća ažuriran profil; proveriti ponovnim GET. 
- **Autor profil**: isto ponoviti za `GET/PUT /api/author/profile` sa tokenom autora. 
- **Negativni scenariji**: 
  - prazno `name` ili `surname` → 400 zbog DTO/domenske validacije; 
  - `biography` ili `motto` > 250 karaktera → 400; 
  - neautorizovan poziv bez tokena ili pogrešne uloge → 401/403; 
  - nepostojeći `personId` u tokenu → 404 (KeyNotFound u servisu).

## Automatski testovi
- Izmenjena/dodata klasa `RegistrationTests` (metoda `Successfully_registers_tourist`) pokriva pozitivan scenario registracije turiste, proveru JWT claim-a i da su User/Person upisani. 
- Komanda za pokretanje testova Stakeholders modula: 
  ```bash
  dotnet test src/Modules/Stakeholders/Explorer.Stakeholders.Tests/Explorer.Stakeholders.Tests.csproj
  ```
  Pozitivni scenario proverava kompletan tok registracije; negativni (implicitno) pokriven validacijom domena/DTO preko koda.

## TestData / SQL
- Seed skripte `c-people.sql` i `d-profiles.sql` koriste **negativne ID-eve** da ne kolidiraju sa realnim podacima; `a-delete.sql` ih briše pre/posle testa kako bi se baza vratila u početno stanje. Pri pokretanju integracionih testova SQL fajlovi se izvršavaju redom delete → insert.

---

# Frontend PR: Uređivanje profila (autor/turista)

## Naslov
Korisnički profil turista i autora – Angular UI i povezivanje sa API-jem

## Kratak opis
- Dodate stranice za pregled i izmenu profila turista i autora sa validacijom forme i preview-em slike.
- `ProfileClientService` povezuje UI sa backend endpointima za GET/PUT profila.
- UI prikazuje poruke o uspehu/greškama, loader i blokira slanje nevalidnih formi.

## Detaljniji opis
- **Komponente**: `tourist-profile` i `author-profile` prikazuju postojeće podatke (slika, biografija, moto) sa dugmetom „Edit”. `tourist-profile-edit` i `author-profile-edit` sadrže reaktivne forme sa obaveznim imenom/prezimenom, ograničenjem dužine bio/moto (250), preview-em uploadovane slike (JPG/PNG do 2 MB) i jasnim porukama o grešci. 
- **Servis**: `profile-client.service.ts` dodaje metode `getTouristProfile`/`updateTouristProfile` i `getAuthorProfile`/`updateAuthorProfile` koje gađaju backend rute `tourist/profile` i `author/profile` na `environment.apiHost`. 
- **Validacija i UX**: forme koriste Angular Validators; onSubmit markira sva polja ako forma nije validna; prikazani loader/poruke sprečavaju nejasno stanje. Nakon uspešnog čuvanja, korisnik se preusmerava na pregled profila uz state flag koji prikazuje „Profile updated successfully.”. 
- **Navigacija**: dugmad „Edit” vode na `/tourist/profile/edit` ili `/author/profile/edit`, dok Cancel vraća na pregled. 

## Lista najbitnijih fajlova
- `Explorer/src/app/feature-modules/stakeholders/profile-client.service.ts` – HTTP servis za profil (GET/PUT turista/autor). 
- `Explorer/src/app/feature-modules/stakeholders/model/profile.model.ts` – tipizacija profila/payload-a. 
- **Turista**: 
  - `tourist-profile/tourist-profile.component.ts|html|css` – prikaz profila sa loaderom/porukama i navigacijom ka edit formi. 
  - `tourist-profile-edit/tourist-profile-edit.component.ts|html|css` – reaktivna forma, validacija, upload i preview slike, poruke o greškama, redirect posle uspeha. 
- **Autor**: 
  - `author-profile/author-profile.component.ts|html|css` – prikaz i navigacija ka edit-u. 
  - `author-profile-edit/author-profile-edit.component.ts|html|css` – forma i validacija identična turističkoj. 
- Dodatne izmene u modulu/routingu (npr. `stakeholders-module.ts`, `app.module.ts`) povezuju rute i komponente.

## Kako pokrenuti frontend
```bash
cd Explorer
npm install
npm run start   # ili ng serve
```
Aplikacija je dostupna na `http://localhost:4200`.

## Manualno testiranje u browseru
- **Turista profil**: ulogovati se kao turista (seed nalog npr. turista1@gmail.com / lozinka iz backend seeda), otvoriti `/tourist/profile`; proveriti da su učitani podaci. Klik na „Edit”, promeniti ime/biografiju/moto, uploadovati JPG/PNG < 2MB, klik „Save” → očekuje se poruka o uspehu i vidljive izmene posle refresh-a. 
- **Validacija**: pokušati slanje bez imena/prezimena ili sa >250 karaktera u bio/moto – forma treba da prikaže poruke i ne šalje zahtev. Upload pogrešnog tipa/veličine slike treba da prikaže grešku i očisti input. 
- **Autor profil**: isto ponoviti na rutama `/author/profile` i `/author/profile/edit`. 
- **Edge slučajevi**: pokušaj pristupa bez logina treba da dovede do redirect-a/403; simulirati backend grešku (isključiti API) i proveriti da UI prikaže „Unable to load/save your profile.” umesto praznog ekrana.

## Šta još može da se očisti
- Proći kroz komponente i ukloniti eventualne `console.log` i zakomentarisane delove ako ostanu iz debugovanja; dodati loader za upload slike ukoliko bude potrebno.
