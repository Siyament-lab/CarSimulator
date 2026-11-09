# CarSimulator

Ett IoT-projekt i C# och .NET 10 som simulerar biltelemetri, skickar data till ThingSpeak och hämtar den för analys.

Lösningen består av två konsolprojekt som körs separat:

- **CarSimulator** – simulerar bilkörning och skickar telemetri.
- **CarSimulator.Analysis** – hämtar telemetri och beräknar statistik.

Simuleringslogiken bygger på lärarens exempel:
https://github.com/CKV-SM/IoT_CarSimulator


### Simulator

- Genererar hastighet, motorvarvtal, motortemperatur och bränslenivå.
- Skickar avläsningar till ThingSpeak via HTTPS.
- Väntar 15 sekunder mellan sändningarna.
- Kör i cirka 10 minuter om den inte avbryts.
- Kan avbrytas med Esc.
- Hanterar anslutningsfel, timeout och misslyckade uppdateringar.

### Analys

En konsolmeny erbjuder följande val:

1. Analysera de senaste 24 timmarna.
2. Analysera de senaste 100 avläsningarna.
0. Avsluta.

Analysen visar:

- Antal hämtade avläsningar.
- Genomsnittligt motorvarvtal.
- Genomsnittlig hastighet.
- Antal giltiga värden som använts i respektive beräkning.

Saknade och ogiltiga värden utesluts från medelvärdena.
Felaktiga menyval, anslutningsfel och felaktiga API-svar hanteras utan att programmet kraschar.

## Förutsättningar

- .NET 10 SDK.
- Visual Studio med stöd för .NET 10 och C#.
- Internetanslutning.
- Tillgång till en ThingSpeak-kanal och dess API-nycklar.

## ThingSpeak-kanal

Båda projekten ska använda samma kanal med följande fält:

| Fält    | Innehåll   | Enhet |
|---------|------------|-------|
| Field 1 | Rpm        | varv/min |
| Field 2 | Speed      | km/h |
| Field 3 | EngineTemp | °C |
| Field 4 | FuelLevel  | % |

Vid granskning kan den befintliga kanalen användas.
Kanal-ID och nödvändiga API-nycklar lämnas separat via skolans inlämning för att sökningen enligt alt-2 ska visa några värden.

Analysappen kan läsa redan sparade data utan att simulatorn körs.
Om simulatorn körs med den befintliga kanalens skrivnyckel läggs nya avläsningar till i samma kanal.

Det går även att skapa en egen kanal med fälten ovan.

## Konfiguration med User Secrets

API-nycklar lagras i User Secrets och följer inte med Git-repot.
Inställningarna måste därför läggas in lokalt efter kloning.

### CarSimulator

Högerklicka på projektet **CarSimulator** i Visual Studio och välj
**Manage User Secrets**.

Lägg in följande och ersätt platshållaren med kanalens Write API Key:

```json
{
  "ThingSpeak": {
    "WriteApiKey": "DIN_WRITE_API_KEY"
  }
}
```

### CarSimulator.Analysis

Högerklicka på projektet **CarSimulator.Analysis** och välj
**Manage User Secrets**.

Lägg in följande och ersätt platshållarna med kanalens ID och Read API Key:

```json
{
  "ThingSpeak": {
    "ChannelId": "DITT_CHANNEL_ID",
    "ReadApiKey": "DIN_READ_API_KEY"
  }
}
```

## Bygga och köra

1. Klona repot och öppna solution-filen i Visual Studio.
2. Konfigurera User Secrets för respektive projekt.
3. Välj **Build → Build Solution**.
4. Högerklicka på projektet som ska köras och välj
   **Debug → Start Without Debugging**.

Projekten kan köras samtidigt i varsitt konsolfönster.

### Köra simulatorn

Starta **CarSimulator**.

Telemetrin visas i konsolen. Vid lyckad överföring visas även ett meddelande
om att data sparats i ThingSpeak.

Tryck på **Esc** för att avbryta.
Om ett HTTP-anrop pågår får det avslutas först. Stoppet kan därför dröja
tills anropet är klart eller dess timeout har uppnåtts.

### Köra analysen

Starta **CarSimulator.Analysis** och välj önskad period i menyn.

Om kanalen innehåller färre än 100 avläsningar visas statistiken för de
avläsningar som finns. Om vald period saknar data visas ett meddelande.

## Kodstruktur

- **Models** – telemetrimodell och modell för analysresultat.
- **DTOs** – klasser för att läsa ThingSpeaks JSON-svar.
- **Services** – simulering, kommunikation med ThingSpeak och beräkningar.
- **Menu** – analysappens meny och presentation av statistik.
- **Program.cs** – konfiguration, skapande av tjänster och programstart.

## Felhantering och säkerhet

- Kommunikation med ThingSpeak sker via HTTPS.
- Simulatorn använder skrivnyckeln och analysappen använder läsnyckeln.
- Inställningar kontrolleras vid programstart.
- HTTP-anrop har en tidsgräns.
- Nätverksfel och ogiltiga svar hanteras med felmeddelanden.
- Analysen kontrollerar numeriska värden innan medelvärden beräknas.
- Tidsstämplar skickas i UTC.

Misslyckade sändningar sparas inte lokalt och skickas inte om automatiskt.
Det kan därför uppstå luckor i datan vid anslutningsproblem.

## Avgränsningar

Projektet använder simulerade värden och är inte anslutet till en riktig bil.
Felkoder, GPS-positioner och kartvisning ingår inte i denna version.