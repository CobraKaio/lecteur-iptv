# Modèles M3U

Ce dossier contient les classes utilisées pour représenter et manipuler les données des fichiers M3U.

## Format M3U

Le format M3U (MPEG URL) est un format de fichier de playlist utilisé pour les flux multimédia. Dans le contexte IPTV, il est utilisé pour définir des listes de chaînes de télévision.

Un fichier M3U typique pour IPTV ressemble à ceci :

```
#EXTM3U
#EXTINF:-1 tvg-id="france2.fr" tvg-name="France 2" tvg-logo="https://example.com/logos/france2.png" group-title="France TV",France 2
https://example.com/france2.m3u8
#EXTINF:-1 tvg-id="france3.fr" tvg-name="France 3" tvg-logo="https://example.com/logos/france3.png" group-title="France TV",France 3
https://example.com/france3.m3u8
```

## Classes

### M3UPlaylist

La classe `M3UPlaylist` représente une playlist M3U complète. Elle contient :

- `Header` : L'en-tête de la playlist (généralement "#EXTM3U")
- `Name` : Le nom de la playlist
- `Channels` : La liste des chaînes dans la playlist
- `Attributes` : Les attributs supplémentaires de la playlist
- `SourceUrl` : L'URL source de la playlist
- `LastUpdated` : La date de dernière mise à jour de la playlist

### M3UChannelItem

La classe `M3UChannelItem` représente une chaîne dans une playlist M3U. Elle contient :

- `ExtInf` : La ligne #EXTINF brute
- `Name` : Le nom de la chaîne
- `Url` : L'URL du flux
- `Duration` : La durée (en secondes) si spécifiée dans #EXTINF
- `TvgId` : L'ID TVG (Television Guide)
- `TvgName` : Le nom TVG
- `LogoUrl` : L'URL du logo
- `Group` : Le groupe de la chaîne
- `Language` : La langue de la chaîne
- `Attributes` : Les attributs supplémentaires de la chaîne

## Utilisation

Ces classes sont utilisées par le service `M3UParser` pour parser les fichiers M3U et les convertir en objets C# manipulables.

Exemple d'utilisation :

```csharp
// Créer une playlist
var playlist = new M3UPlaylist
{
    Name = "Ma Playlist IPTV",
    SourceUrl = "https://example.com/playlist.m3u"
};

// Ajouter une chaîne
var channel = new M3UChannelItem
{
    Name = "France 2",
    Url = "https://example.com/france2.m3u8",
    TvgId = "france2.fr",
    Group = "France TV",
    LogoUrl = "https://example.com/logos/france2.png"
};

// Ajouter des attributs supplémentaires
channel.Attributes["tvg-shift"] = "0";
channel.Attributes["tvg-country"] = "FR";

// Ajouter la chaîne à la playlist
playlist.AddChannel(channel);

// Obtenir toutes les chaînes d'un groupe spécifique
var franceTV = playlist.GetChannelsByGroup("France TV");

// Rechercher des chaînes
var sportsChannels = playlist.SearchChannels("sport");
```

## Remarques

- Ces classes sont des modèles de parsing et non des modèles de base de données. Elles sont utilisées pour représenter les données lues directement d'un fichier M3U.
- Le service d'importation fera le travail de "mapping" et de validation entre ces structures et les modèles de base de données.
- L'utilisation d'un `Dictionary<string, string>` pour les attributs permet une grande flexibilité, car les attributs dans un fichier M3U peuvent varier.
