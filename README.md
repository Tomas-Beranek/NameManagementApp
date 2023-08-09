# Klient-Server Aplikace pro Správu Seznamu Jmen

Tato případová studie popisuje vytvoření klient-server aplikace pro správu seznamu jmen pomocí jazyka C#.

## Server

### Funkce Serveru:

- Umožňuje připojení klientů na konfigurovatelné IP adrese a portu.
- Ukládá seznam jmen do souboru ve formátu CSV.
- Podporuje obsluhu několika klientů zároveň.
- Zajišťuje bezpečné ukládání dat i při výpadku.

### Technické Detaily:

- Jazyk: C#
- .NET Framework: 4.8
- Použití TCP/IP pro komunikaci mezi serverem a klienty.

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
- Data nejsou ztracena při výpadku serveru.
- Aplikace je vyvíjena na .NET Framework 4.8.

## Instalace

1. Stáhněte si projektový kód ze zdroje (GitHub repository).
2. Otevřete projekt v Visual Studiu.

## Konfigurace Serveru

1. Otevřete projekt serverové aplikace.
2. Upravte konfigurační soubor s IP adresou a portem serveru.
3. Spusťte serverovou aplikaci.

## Použití Klienta

1. Otevřete projekt klientové aplikace.
2. Spusťte klientovou aplikaci.
3. Zadejte IP adresu a port serveru a připojte se k němu.
4. Vytvářejte, editujte nebo mažte záznamy v seznamu jmen.

## Dodatečné Informace

- Pro instalaci a použití projektu je vyžadován .NET Framework 4.8.
- Tento projekt je součástí případové studie pro tvorbu klient-server aplikace pro správu seznamu jmen v C#.
