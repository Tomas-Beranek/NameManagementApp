<!DOCTYPE html>
<html>
<head>
    <title>Klient-Server Aplikace pro Správu Seznamu Jmen</title>
</head>
<body>
    <h1>Případová studie: Klient-Server Aplikace pro Správu Seznamu Jmen v C#</h1>

    <p>Tato případová studie popisuje tvorbu klient-server aplikace v jazyce C#, která umožní správu seznamu jmen. Aplikace bude sestávat z dvou částí: serveru a klienta. Server bude schopen přijímat připojení od klientů, ukládat seznam jmen do souboru ve formátu CSV a zajišťovat simultánní obsluhu několika klientů. Klient bude mít jednoduché uživatelské rozhraní, které umožní vytvořit nový záznam, smazat existující záznam nebo upravit hodnoty jmen a příjmení.</p>

    <h2>Server</h2>
    <h3>Funkce Serveru:</h3>
    <ul>
        <li>Připojování klientů na konfigurovatelné IP adrese a portu.</li>
        <li>Ukládání seznamu jmen do souboru ve formátu CSV.</li>
        <li>Podpora pro simultánní obsluhu více klientů.</li>
        <li>Zajištění bezpečného ukládání dat při výpadku.</li>
    </ul>

    <h3>Technické Detaily:</h3>
    <ul>
        <li>Jazyk: C#</li>
        <li>.NET Framework: 4.8</li>
        <li>Použití TCP/IP pro komunikaci mezi serverem a klienty.</li>
    </ul>

    <h2>Klient</h2>
    <h3>Funkce Klienta:</h3>
    <ul>
        <li>Připojení k serveru na zadané IP adrese a portu.</li>
        <li>Vytváření nových záznamů s automaticky generovaným unikátním ID.</li>
        <li>Editace existujících záznamů (Jméno a Příjmení).</li>
        <li>Mazání záznamů.</li>
    </ul>

    <h3>Technické Detaily:</h3>
    <ul>
        <li>Jazyk: C#</li>
        <li>.NET Framework: 4.8</li>
        <li>Jednoduché uživatelské rozhraní pomocí WPF.</li>
    </ul>

    <h2>Další Požadavky</h2>
    <ul>
        <li>Server musí zvládat obsluhu více klientů současně.</li>
        <li>Data nesmí být ztracena při výpadku serveru.</li>
        <li>Aplikace bude vyvíjena na .NET Framework 4.8.</li>
    </ul>

    <h2>GitHub README</h2>
    <h3>Instalace</h3>
    <ol>
        <li>Stáhněte si projektový kód ze zdroje (GitHub repository).</li>
        <li>Otevřete projekt v Visual Studiu.</li>
    </ol>

    <h3>Konfigurace Serveru</h3>
    <ol>
        <li>Otevřete projekt serveru.</li>
        <li>Upravte konfigurační soubor s IP adresou a portem serveru.</li>
        <li>Spusťte serverovou aplikaci.</li>
    </ol>

    <h3>Použití Klienta</h3>
    <ol>
        <li>Otevřete projekt klienta.</li>
        <li>Spusťte klientovou aplikaci.</li>
        <li>Zadejte IP adresu a port serveru a připojte se k němu.</li>
        <li>Vytvářejte, editujte nebo mažte záznamy v seznamu jmen.</li>
    </ol>

    <h3>Dodatečné Informace</h3>
    <ul>
        <li>Pro instalaci a použití projektu je nutný .NET Framework 4.8.</li>
        <li>Tento projekt je součástí případové studie pro tvorbu klient-server aplikace pro správu seznamu jmen v C#.</li>
    </ul>
</body>
</html>
