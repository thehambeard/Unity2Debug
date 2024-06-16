using Unity2Debug.Common.Utility;

namespace Unity2Debug.Common.SettingsService
{
    public class Settings
    {
        private const string _fileName = "settings.json";
        public Dictionary<string, SettingProfile> Profiles { get; private set; }

        public Settings()
        {
            Profiles = [];
        }

        public bool TryAddProfile(string profileName)
        {
            bool result;
            DebugSettings debug = new();

            if (Directory.Exists(UnityConstants.UNITY_DEFAULT_BASE))
                debug.UnityInstallPath = UnityConstants.UNITY_DEFAULT_BASE;

            if (result = !Profiles.ContainsKey(profileName))
                Profiles.Add(profileName, new(profileName, new(), debug));

            return result;
        }

        public bool TryAddProfile(string profileName, SettingProfile profile)
        {
            bool result;

            if (result = TryAddProfile(profileName))
                Profiles[profileName] = profile;

            return result;
        }

        public bool TryAddProfile(string profileName, DecompileSettings decompileSettings, DebugSettings debugSettings)
        {
            bool result;

            if (result = TryAddProfile(profileName))
                Profiles[profileName] = new(profileName, decompileSettings, debugSettings);

            return result;
        }

        public bool TryRemoveProfile(string profileName)
        {
            bool result;
            if (result = Profiles.ContainsKey(profileName))
                Profiles.Remove(profileName);

            return result;
        }

        public void Save() => StaticSave(Profiles);

        public static void StaticSave(Dictionary<string, SettingProfile> profiles)
        {
            try
            {
                var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _fileName);
                File.WriteAllText(file, Json.ToJSON(profiles));
            }
            catch
            {
                throw;
            }
        }

        public static Settings Load()
        {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _fileName);

            var settings = new Settings();

            try
            {
                if (File.Exists(file))
                {
                    var json = File.ReadAllText(file);
                    settings.Profiles = Json.FromJSON<Dictionary<string, SettingProfile>>(json)
                        ?? throw new NullReferenceException();
                }
                else
                {
                    settings.GenerateDefaultProfiles();
                    settings.Save();
                }
            }
            catch
            {
                throw;
            }

            return settings;
        }

        private void GenerateDefaultProfiles()
        {
            string defaultUnityPath = Directory.Exists(UnityConstants.UNITY_DEFAULT_BASE) ? UnityConstants.UNITY_DEFAULT_BASE : string.Empty;

            List<string> wrathSymlinks =
            [
                "Bundles\\",
                "Wrath_Data\\StreamingAssets\\",
                "blueprints.zip",
                "Wrath_Data\\*.resS",
                "Wrath_Data\\*.assets"
            ];

            List<string> kmSymlinks =
            [
                "Kingmaker_Data\\StreamingAssets\\",
                "Kingmaker_Data\\level*",
                "Kingmaker_Data\\*.assets",
                "Kingmaker_Data\\*.resS"
            ];

            List<string> rtSymLinks =
            [
                "Steam Workshop tool\\",
                "Modding\\",
                "Bundles\\",
                "WH40KRT_Data\\StreamingAssets\\",
                "WH40KRT_Data\\*.assets",
                "WH40KRT_Data\\*.resS"
            ];

            Profiles.Clear();
            Profiles.Add(
                "WOTR: Basic",
                    new SettingProfile(
                        "WOTR: Basic",
                        new()
                        {
                            AssemblyPaths = ["C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\Assembly-CSharp.dll"],
                        },
                        new()
                        {
                            RetailGameExe = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath.exe",
                            SteamAppId = "1184370",
                            CreateDebugCopy = true,
                            UnityInstallPath = defaultUnityPath,
                            UseSymlinks = true,
                            Symlinks = wrathSymlinks,
                        }));
            Profiles.Add(
                "WOTR: Full",
                    new SettingProfile(
                        "WOTR: Full",
                        new()
                        {
                            AssemblyPaths =
                            [
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\Assembly-CSharp.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\Assembly-CSharp-firstpass.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\Core.Async.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\Core.Cheats.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\Core.Console.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\Core.Overlays.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\Core.Reflection.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\DungeonArchitect.Builders.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\DungeonArchitect.Core.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\DungeonArchitect.Modules.Common.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\DungeonArchitect.Modules.Flow.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\DungeonArchitect.Modules.Flow.Implementations.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\DungeonArchitect.Modules.Grammar.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\DungeonArchitect.Modules.Graph.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\DungeonArchitect.Modules.Meshing.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\DungeonArchitect.Modules.Navigation2D.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\DungeonArchitect.Modules.SxEngine.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\DungeonArchitect.Modules.UI.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\DungeonArchitect.Modules.VisibilityGraph.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\DungeonArchitect.ThirdParty.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\Owlcat.Runtime.Core.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\Owlcat.Runtime.Hardware.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\Owlcat.Runtime.UI.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\Owlcat.Runtime.UniRx.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\Owlcat.Runtime.Validation.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\Owlcat.Runtime.Visual.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath_Data\\Managed\\Owlcat.SharedTypes.dll"
                            ],
                        },
                        new()
                        {
                            RetailGameExe = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Second Adventure\\Wrath.exe",
                            SteamAppId = "1184370",
                            CreateDebugCopy = true,
                            UnityInstallPath = defaultUnityPath,
                            UseSymlinks = true,
                            Symlinks = wrathSymlinks
                        }));
            Profiles.Add(
                "WH40KRT",
                    new SettingProfile(
                        "WH40KRT",
                        new()
                        {
                            AssemblyPaths =
                            [
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Assembly-CSharp-firstpass.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Autodesk.Fbx.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\BuildMode.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\BundlesBaseTypes.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\CircularBuffer.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Code.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Core.Async.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Core.Cheats.Attribute.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Core.Cheats.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Core.Console.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Core.Overlays.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Core.Reflection.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Core.RestServer.Client.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Core.RestServer.Common.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Core.RestServer.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Core.StateCrawler.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\CountingGuard.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\GeometryExtensions.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\GuidUtility.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.AreaLogic.TimeOfDay.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Blueprints.Attributes.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Blueprints.Base.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Blueprints.Hack.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Blueprints.JsonSystem.EditorDatabase.FileDatabaseClient.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Blueprints.JsonSystem.Hepers.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Blueprints.JsonSystem.PropertyUtility.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Blueprints.JsonSystem.PropertyUtility.Helper.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Blueprints.OverridesManager.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Controllers.Enums.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Controllers.Interfaces.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.ElementsSystem.Interfaces.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.EntitySystem.Stats.Base.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Enums.Damage.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Enums.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.GameInfo.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Localization.Enums.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.PubSubSystem.Core.Interfaces.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.QA.Arbiter.Profiling.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.ResourceLinks.BaseInterfaces.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.ResourceReplacementProvider.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.RuleSystem.Enum.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.RuleSystem.Rules.Interfaces.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Settings.ConstructionHelpers.KeyPrefix.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Settings.Interfaces.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Sound.Base.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Stores.DlcInterfaces.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Stores.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.TextTools.Base.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.TextTools.Core.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.UI.InputSystems.Enums.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.UI.Models.Log.ContextFlag.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.UI.Models.Log.Enums.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.UI.Models.Tooltip.Base.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.UnitLogic.Enums.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.UnitLogic.Mechanics.Facts.Interfaces.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Utility.Enums.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Utility.FlagCountable.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Utility.Fsm.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Utility.Random.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Visual.Animation.GraphVisualizerClient.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Visual.Base.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Visual.HitSystem.Base.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Kingmaker.Visual.Particles.GameObjectsPooling.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\LocalizationShared.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Owlcat.Runtime.Core.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Owlcat.Runtime.UI.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Owlcat.Runtime.UniRx.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Owlcat.Runtime.Validation.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Owlcat.Runtime.Visual.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Owlcat.ShaderLibrary.Visual.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\Owlcat.Shaders.Visual.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\PFlog.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\ReadOnlyState.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\ReplayLog.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\RogueTrader.Code.ShaderConsts.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\RogueTrader.Editor.ElementsDescription.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\RogueTrader.GameCore.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\RogueTrader.ModInitializer.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\RogueTrader.NetPlayer.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\RogueTrader.QA.QAModeExceptionEvents.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\RogueTrader.SharedTypes.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\StatefulRandom.dll",
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT_Data\\Managed\\UnitLogic.Alignments.Enums.dll"
                            ],
                        },
                        new()
                        {
                            RetailGameExe = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warhammer 40,000 Rogue Trader\\WH40KRT.exe",
                            SteamAppId = "2186680",
                            CreateDebugCopy = true,
                            UnityInstallPath = defaultUnityPath,
                            UseSymlinks = true,
                            Symlinks = rtSymLinks
                        }));
            Profiles.Add(
                "Kingmaker",
                    new SettingProfile(
                        "Kingmaker",
                        new()
                        {
                            AssemblyPaths =
                            [
                                "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Kingmaker\\Kingmaker_Data\\Managed\\Assembly-CSharp.dll",
                            ],
                        },
                        new()
                        {
                            RetailGameExe = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pathfinder Kingmaker\\Kingmaker.exe",
                            SteamAppId = "640820",
                            CreateDebugCopy = true,
                            UnityInstallPath = defaultUnityPath,
                            UseSymlinks = true,
                            Symlinks = kmSymlinks
                        }));
        }
    }
}
