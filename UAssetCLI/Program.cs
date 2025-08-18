using UAssetAPI;
using UAssetAPI.UnrealTypes;
using UAssetAPI.Unversioned;

namespace UAssetCLI
{
    static class Program
    {
        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length >= 2)
            {
                Usmap? selectedMappings = null;

                switch (args[1].ToLowerInvariant())
                {
                    // tojson <source> <destination> <engine version> [mappings name]
                    // UAssetCLI tojson A.umap B.json 23 Outriders
                    case "tojson":
                        UAGConfig.LoadMappings();

                        if (args.Length < 5) break;
                        if (args.Length >= 6) UAGConfig.TryGetMappings(args[5], out selectedMappings);

                        EngineVersion selectedVer;
                        if (int.TryParse(args[4], out int selectedVerRaw))
                            selectedVer = EngineVersion.VER_UE4_0 + selectedVerRaw;
                        else Enum.TryParse(args[4], out selectedVer);

                        string jsonSerializedAsset =
                            new UAsset(args[2], selectedVer, selectedMappings).SerializeJson(Newtonsoft.Json.Formatting
                                .Indented);
                        File.WriteAllText(args[3], jsonSerializedAsset);
                        return;
                    // fromjson <source> <destination> [mappings name]
                    // UAssetCLI fromjson B.json A.umap Outriders
                    case "fromjson":
                        UAGConfig.LoadMappings();

                        if (args.Length < 4) break;
                        if (args.Length >= 5) UAGConfig.TryGetMappings(args[4], out selectedMappings);

                        UAsset? jsonDeserializedAsset;
                        using (var sr = new FileStream(args[2], FileMode.Open))
                        {
                            jsonDeserializedAsset = UAsset.DeserializeJson(sr);
                        }

                        if (jsonDeserializedAsset != null)
                        {
                            jsonDeserializedAsset.Mappings = selectedMappings;
                            jsonDeserializedAsset.Write(args[3]);
                        }

                        return;
                }
            }

            Console.WriteLine(@"Usage: UAssetCLI [ fromjson <source> <destination> [mappings name]");
            Console.WriteLine(@"                 | tojson <source> <destination> <engine version> [mappings name] ]");
        }
    }
}