using Dalamud.Game.ClientState.Objects.SubKinds;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace Collections;

public class LodestoneClient
{
    public Dictionary<string, CollectionData> characterNameToCollectionData = new();

    private readonly HttpClient httpClient =
    new(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All })
    {
        Timeout = TimeSpan.FromMilliseconds(20000)
    };

    public void Dispose()
    {
        httpClient.Dispose();
    }

    public (string, string)? SchedulePopulateCollectionData()
    {
        var targetDetails = GetCurrentTargetNameAndWorld();
        if (targetDetails == null) return null;
        var charName = targetDetails.Value.Item1;
        var server = targetDetails.Value.Item2;

        if (characterNameToCollectionData.ContainsKey(charName))
        {
            return (charName, server);
        }

        characterNameToCollectionData[charName] = null;

        Task.Run(async () =>
        {
            var collectionData = await GetCollectionData(targetDetails.Value.Item1, targetDetails.Value.Item2);
            characterNameToCollectionData[targetDetails.Value.Item1] = collectionData;
        });

        return (charName, server);
    }

    public (string, string)? GetCurrentTargetNameAndWorld()
    {
        var target = Services.TargetManager.SoftTarget ?? Services.TargetManager.Target;
        if (target is not IPlayerCharacter)
        {
            return null;
        }

        var playerTarget = target as IPlayerCharacter;

        return (playerTarget.Name.ToString(), playerTarget.HomeWorld.Value.Name.ToString());
    }

    private async Task<CollectionData> GetCollectionData(string charName, string server)
    {
        var lodestoneId = await GetLodestoneId(charName, server);
        if (lodestoneId == null) return null;

        var collectionData = await GetCollectionDataFromId((int)lodestoneId);
        return collectionData;
    }

    private async Task<int?> GetLodestoneId(string charName, string server)
    {
        try
        {
            var formattedCharName = charName.Replace(" ", "%20");
            Dev.Log($"Calling lodestone 1");
            using var result = await httpClient.GetAsync($"https://xivapi.com/character/search?name={formattedCharName}&targetHomeWorld={server}");
            Dev.Log($"Calling lodestone 1 - done");
            if (result.StatusCode != HttpStatusCode.OK) return null;

            await using var responseStream = await result.Content.ReadAsStreamAsync();
            var lodestoneCharacterSearch = await JsonSerializer.DeserializeAsync<LodestoneCharacterSearch>(responseStream);
            if (lodestoneCharacterSearch == null) return null;

            if (lodestoneCharacterSearch.Results == null)
            {
                Dev.Log("Lodestone Id Result is null");
            }

            var firstResult = lodestoneCharacterSearch.Results.FirstOrDefault();
            if (firstResult == null) return null;

            return firstResult.ID;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task<CollectionData> GetCollectionDataFromId(int Id)
    {
        try
        {
            Dev.Log($"Calling lodestone 2");
            using var result = await httpClient.GetAsync($"https://xivapi.com/character/{Id}?data=AC,MIMO");
            Dev.Log($"Calling lodestone 2 - done");
            if (result.StatusCode != HttpStatusCode.OK) return null;

            var responseStream = await result.Content.ReadAsStringAsync();

            var minionCount = CountOccurrencesBetweenSubstrings(responseStream, "\"Name\":", ",\"Minions\":[", "],\"Mounts\":[");
            var mountCount = CountOccurrencesBetweenSubstrings(responseStream, "\"Name\":", "],\"Mounts\":[", "],\"PvPTeam\"");
            var achievementCount = CountOccurrencesBetweenSubstrings(responseStream, "\"ID\":", "\"Achievements\":{", ",\"Character\":");
            Dev.Log($"Calling lodestone - parsing done");
            return new CollectionData() { MinionCount = minionCount, MountCount = mountCount, AchievementCount = achievementCount };
        }
        catch (Exception e)
        {
            Dev.Log(e.ToString());
            return null;
        }
    }

    static int CountOccurrencesBetweenSubstrings(string input, string targetString, string startSubstring, string endSubstring)
    {
        int startIndex = input.IndexOf(startSubstring);
        int endIndex = input.IndexOf(endSubstring, startIndex + startSubstring.Length);

        if (startIndex == -1 || endIndex == -1)
            return 0;

        string substringBetween = input.Substring(startIndex + startSubstring.Length, endIndex - startIndex - startSubstring.Length);
        return CountOccurrences(substringBetween, targetString);
    }

    static int CountOccurrences(string input, string substring)
    {
        int count = 0;
        int index = 0;
        while ((index = input.IndexOf(substring, index)) != -1)
        {
            count++;
            index += substring.Length;
        }
        return count;
    }
}

public class CollectionData
{
    public int MountCount;
    public int MinionCount;
    public int AchievementCount;
}

public class Pagination
{
    public int Page { get; set; }
    public object PageNext { get; set; }
    public object PagePrev { get; set; }
    public int PageTotal { get; set; }
    public int Results { get; set; }
    public int ResultsPerPage { get; set; }
    public int ResultsTotal { get; set; }
}

public class Result
{
    public string Avatar { get; set; }
    public int FeastMatches { get; set; }
    public int ID { get; set; }
    public string Lang { get; set; }
    public string Name { get; set; }
    public object Rank { get; set; }
    public object RankIcon { get; set; }
    public string Server { get; set; }
}

public class LodestoneCharacterSearch
{
    public Pagination Pagination { get; set; }
    public List<Result> Results { get; set; }
}
