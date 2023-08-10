# Klient-Server Aplikace pro Správu Seznamu Jmen

Jedná se o klient-server aplikaci pro správu seznamu jmen pomocí jazyka C#.

## Server

### Funkce Serveru:

- Umožňuje připojení klientů na konfigurovatelné IP adrese a portu.
- Ukládá seznam jmen do souboru ve formátu CSV.
- Podporuje obsluhu několika klientů zároveň.

### Technické Detaily:

- Jazyk: C#
- .NET Framework: 4.8
- Použití TCP/IP pro komunikaci mezi serverem a klienty.
- Uživatelské rozhraní vytvořené pomocí WPF.


## Klient

### Funkce Klienta:

- Připojení k serveru na zadané IP adrese a portu.
- Vytváření nových záznamů s automaticky generovaným unikátním ID.
- Editace existujících záznamů (Jméno a Příjmení).
- Mazání záznamů.

### Technické Detaily:

- Jazyk: C#
- .NET Framework: 4.8
- Uživatelské rozhraní vytvořené pomocí WPF.

## Další Požadavky

- Server zvládá obsluhu více klientů současně.
- Aplikace je vyvíjena na .NET Framework 4.8.

## Instalace

1. Stáhněte si projektový kód ze zdroje (GitHub repository).
2. Otevřete projekt v Visual Studiu.
3. Nainstalujet si NuGet balík SimpleTcp

## Konfigurace Serveru

1. Otevřete projekt serverové aplikace.
2. Upravte konfigurační soubor s IP adresou a portem serveru.
3. Spusťte serverovou aplikaci.

## Použití Klienta

1. Otevřete projekt klientové aplikace.
2. Spusťte klientovou aplikaci.
3. Zadejte IP adresu a port serveru a připojte se k němu.
4. Vytvářejte, editujte nebo mažte záznamy v seznamu jmen.
5. Mazání záznamu se provede přes kontextové "delete" menu(pravý klik myši)
