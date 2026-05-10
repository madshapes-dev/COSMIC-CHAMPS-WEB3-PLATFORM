using CosmicChamps.Api.Configs;
using CosmicChamps.Api.Model.Validation;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace CosmicChamps.Api.Services;

public class WalletRequest
{
    public class Payload
    {
        public string auth { set; get; }
        public string userId { set; get; }
    }

    public string operation { set; get; }
    public Payload payload { set; get; }
}

public class ValidationService
{
    private readonly ValidationServiceConfig _config;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Configs.WalletRequest _walletRequestConfig;

    public ValidationService (
        IOptions<ValidationServiceConfig> config,
        IHttpClientFactory httpClientFactory,
        Configs.WalletRequest walletRequestConfig)
    {
        _config = config.Value;
        _httpClientFactory = httpClientFactory;
        _walletRequestConfig = walletRequestConfig;
    }

    public async Task<string> GetWalletId (string playerId)
    {
        using var client = _httpClientFactory.CreateClient ();
        var response = await client.PostAsJsonAsync (
            _config.WalletEndpoint,
            new WalletRequest
            {
                payload = new WalletRequest.Payload
                {
                    auth = _walletRequestConfig.Auth,
                    userId = playerId
                },
                operation = "getWallet"
            });

        var responseJson = JObject.Parse (await response.Content.ReadAsStringAsync ());
        var error = responseJson["error"]?.Value<string> ();
        if (!string.IsNullOrEmpty (error))
            throw new InvalidOperationException (error);

        var walletId = responseJson["success"]?.Value<string> ();
        if (string.IsNullOrEmpty (walletId))
            throw new InvalidOperationException ("No wallet provided");

        return walletId;
    }

    public async Task<bool> PerformKillswitchCheck (string? killswitch)
    {
        if (string.IsNullOrEmpty (killswitch))
            return false;

        var url = $"{_config.KillswitchEndpoint}/{killswitch}";
        using var client = _httpClientFactory.CreateClient ();
        var validationResponse = await client.GetFromJsonAsync<KillswitchResponse> (url);
        
        
        
            
            
                
        
        return validationResponse is { valid: true };
    }
    
    public async Task<Inventory> GetInventory (string walletId)
    {
        var url = $"{_config.InventoryEndpoint}/{walletId}?timestamp={DateTimeOffset.Now.ToUnixTimeMilliseconds ()}";
        using var client = _httpClientFactory.CreateClient ();
        // var inventoryItems = await client.GetFromJsonAsync<ValidationInventoryItem[]> (url);
        var responseJsonStr = await client.GetStringAsync (url);
        var responseJson = JArray.Parse (responseJsonStr);
        var boostsJson = responseJson.Children ().FirstOrDefault (x => x["_id"]?.Value<string> () == "boost");
        var inventoryJson = responseJson.Children ().Where (x => x["_id"]?.Value<string> () != "boost");
        var items = inventoryJson
            .Select (x => x.ToObject<InventoryItem> ()!)
            .ToArray ();

        if (walletId == "DEVWALLET1" ||
            walletId == "DEVWALLET2")
        {
            items = items.Concat (
                    new[]
                    {
                        /*new InventoryItem { _id = "helio_798415616", quantity = 10 },
                        new InventoryItem { _id = "helio_798415766", quantity = 10 },
                        new InventoryItem { _id = "helio_798415432", quantity = 10 },
                        new InventoryItem { _id = "helio_798415284", quantity = 10 },
                        new InventoryItem { _id = "helio_798415137", quantity = 10 },
                        new InventoryItem { _id = "helio_798414953", quantity = 10 },
                        new InventoryItem { _id = "helio_798414797", quantity = 10 },
                        new InventoryItem { _id = "helio_798414597", quantity = 10 },
                        new InventoryItem { _id = "helio_798414425", quantity = 10 },
                        new InventoryItem { _id = "helio_798414243", quantity = 10 },
                        new InventoryItem { _id = "helio_798414063", quantity = 10 },
                        new InventoryItem { _id = "helio_798413845", quantity = 10 },*/

                        new InventoryItem { _id = "cybi_2395662892", quantity = 10 },
                        
                        new InventoryItem { _id = "invin_815317076", quantity = 10 },
                        new InventoryItem { _id = "invin_815317022", quantity = 10 },
                        new InventoryItem { _id = "invin_815316935", quantity = 10 },
                        new InventoryItem { _id = "invin_815316865", quantity = 10 },
                        new InventoryItem { _id = "invin_815316767", quantity = 10 },
                        new InventoryItem { _id = "invin_815316700", quantity = 10 },
                        new InventoryItem { _id = "invin_815316629", quantity = 10 },
                        new InventoryItem { _id = "invin_815316558", quantity = 10 },
                        new InventoryItem { _id = "invin_815316472", quantity = 10 },
                        new InventoryItem { _id = "invin_815316384", quantity = 10 },
                        new InventoryItem { _id = "invin_815316306", quantity = 10 },
                        new InventoryItem { _id = "invin_798485160", quantity = 10 },

                        new InventoryItem { _id = "trig_798473335", quantity = 10 },
                        new InventoryItem { _id = "trig_798473467", quantity = 10 },
                        new InventoryItem { _id = "trig_798473623", quantity = 10 },
                        new InventoryItem { _id = "trig_798473794", quantity = 10 },
                        new InventoryItem { _id = "trig_798473949", quantity = 10 },
                        new InventoryItem { _id = "trig_798474101", quantity = 10 },
                        new InventoryItem { _id = "trig_798474292", quantity = 10 },
                        new InventoryItem { _id = "trig_798474468", quantity = 10 },
                        new InventoryItem { _id = "trig_815278091", quantity = 10 },
                        new InventoryItem { _id = "trig_815278156", quantity = 10 },
                        new InventoryItem { _id = "trig_815281236", quantity = 10 },
                        new InventoryItem { _id = "trig_815281301", quantity = 10 },

                        new InventoryItem { _id = "ram_1280977839", quantity = 10 },
                        new InventoryItem { _id = "ram_1280977773", quantity = 10 },
                        new InventoryItem { _id = "ram_1108380528", quantity = 10 },
                        new InventoryItem { _id = "ram_1108378539", quantity = 10 },
                        new InventoryItem { _id = "ram_798413704", quantity = 10 },
                        new InventoryItem { _id = "ram_798413518", quantity = 10 },
                        new InventoryItem { _id = "ram_798413364", quantity = 10 },
                        new InventoryItem { _id = "ram_798413212", quantity = 10 },
                        new InventoryItem { _id = "ram_798413075", quantity = 10 },
                        new InventoryItem { _id = "ram_798412919", quantity = 10 },
                        new InventoryItem { _id = "ram_798412788", quantity = 10 },
                        new InventoryItem { _id = "ram_798412603", quantity = 10 },
                        new InventoryItem { _id = "ram_798412449", quantity = 10 },
                        new InventoryItem { _id = "ram_798412284", quantity = 10 },
                        new InventoryItem { _id = "ram_798412077", quantity = 10 },
                        new InventoryItem { _id = "ram_798411946", quantity = 10 },

                        new InventoryItem { _id = "tertius_798408091", quantity = 10 },
                        new InventoryItem { _id = "tertius_798407875", quantity = 10 },
                        new InventoryItem { _id = "tertius_798407610", quantity = 10 },
                        new InventoryItem { _id = "tertius_798407410", quantity = 10 },
                        new InventoryItem { _id = "tertius_798407177", quantity = 10 },
                        new InventoryItem { _id = "tertius_798407031", quantity = 10 },
                        new InventoryItem { _id = "tertius_786131874", quantity = 10 },
                        new InventoryItem { _id = "tertius_786131696", quantity = 10 },
                        new InventoryItem { _id = "tertius_786131555", quantity = 10 },
                        new InventoryItem { _id = "tertius_786131403", quantity = 10 },
                        new InventoryItem { _id = "tertius_786131264", quantity = 10 },
                        new InventoryItem { _id = "tertius_786131096", quantity = 10 },

                        new InventoryItem { _id = "bff_1013353488", quantity = 10 },
                        new InventoryItem { _id = "bff_1013353395", quantity = 10 },
                        new InventoryItem { _id = "bff_1013353336", quantity = 10 },
                        new InventoryItem { _id = "bff_1013353281", quantity = 10 },
                        new InventoryItem { _id = "bff_1013353222", quantity = 10 },
                        new InventoryItem { _id = "bff_1013353154", quantity = 10 },
                        new InventoryItem { _id = "bff_1013353055", quantity = 10 },
                        new InventoryItem { _id = "bff_1013352971", quantity = 10 },
                        new InventoryItem { _id = "bff_1013352851", quantity = 10 },
                        new InventoryItem { _id = "bff_1013352778", quantity = 10 },
                        new InventoryItem { _id = "bff_1013352720", quantity = 10 },
                        new InventoryItem { _id = "bff_1013352643", quantity = 10 },
                        new InventoryItem { _id = "bff_1013352565", quantity = 10 },
                        new InventoryItem { _id = "bff_1013352483", quantity = 10 },
                        new InventoryItem { _id = "bff_1013352421", quantity = 10 },
                        new InventoryItem { _id = "bff_1013352360", quantity = 10 },
                        
                        new InventoryItem { _id = "boom_100000000", quantity = 10 },
                        new InventoryItem { _id = "boom_100100000", quantity = 10 },
                        new InventoryItem { _id = "boom_200100000", quantity = 10 },
                        new InventoryItem { _id = "boom_200200000", quantity = 10 },
                        new InventoryItem { _id = "boom_300100000", quantity = 10 },
                        new InventoryItem { _id = "boom_300200000", quantity = 10 },
                        new InventoryItem { _id = "boom_400100000", quantity = 10 },
                        new InventoryItem { _id = "boom_400200000", quantity = 10 },
                        new InventoryItem { _id = "boom_500100000", quantity = 10 },
                        new InventoryItem { _id = "boom_500200000", quantity = 10 },
                        new InventoryItem { _id = "boom_600100000", quantity = 10 },
                        new InventoryItem { _id = "boom_600200000", quantity = 10 },
                        new InventoryItem { _id = "boom_700100000", quantity = 10 },
                        new InventoryItem { _id = "boom_700200000", quantity = 10 },
                        new InventoryItem { _id = "boom_800100000", quantity = 10 },
                        new InventoryItem { _id = "boom_800200000", quantity = 10 },
                        
                        new InventoryItem { _id = "fireball_100000000", quantity = 10 },
                        new InventoryItem { _id = "fireball_200000000", quantity = 10 },
                        new InventoryItem { _id = "fireball_300000000", quantity = 10 },
                        new InventoryItem { _id = "fireball_400000000", quantity = 10 },
                        new InventoryItem { _id = "fireball_500000000", quantity = 10 },
                        new InventoryItem { _id = "fireball_600000000", quantity = 10 },
                        new InventoryItem { _id = "fireball_700000000", quantity = 10 }
                        
                        /*new InventoryItem { _id = "striker_2230768437", quantity = 10 },
                        new InventoryItem { _id = "striker_2230768537", quantity = 10 },*/
                        
                        /*new InventoryItem { _id = "digg_777777777", quantity = 10 },
                        new InventoryItem { _id = "digg_888888888", quantity = 10 }*/
                    })
                .ToArray ();
        }

        return new Inventory
        {
            Items = items /*inventoryJson
                .Select (x => x.ToObject<InventoryItem> ()!)
                .Append (
                    new InventoryItem
                    {
                        _id = "digg_777777777",
                        quantity = 10
                    })
                .Append (
                    new InventoryItem
                    {
                        _id = "digg_888888888",
                        quantity = 10
                    })
                .ToArray ()*/,
            Boosts = boostsJson
                         ?.Children ()
                         .Where (x => !x.Path.EndsWith ("_id"))
                         .Select (
                             x =>
                             {
                                 var boost = x.First?.ToObject<Boost> ()!;
                                 boost.id = x.Path.Split (".").Last ();
                                 return boost;
                             })
                         .ToArray () ??
                     Array.Empty<Boost> ()
        };
    }
}