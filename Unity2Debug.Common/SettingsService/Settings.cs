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

            var wrathPath = OwlCatUtil.FindWrathPath();

            if (!string.IsNullOrEmpty(wrathPath))
            {
                Profiles.Add(
                    "WOTR: Basic",
                        new SettingProfile(
                            "WOTR: Basic",
                            new()
                            {
                                AssemblyPaths = [Path.Combine(wrathPath, "Wrath_Data\\Managed\\Assembly-CSharp.dll")]
                            },
                            new()
                            {
                                RetailGameExe = Path.Combine(wrathPath, "Wrath.exe"),
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
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\Assembly-CSharp.dll",
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\Assembly-CSharp-firstpass.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\Core.Async.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\Core.Cheats.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\Core.Console.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\Core.Overlays.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\Core.Reflection.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\DungeonArchitect.Builders.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\DungeonArchitect.Core.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\DungeonArchitect.Modules.Common.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\DungeonArchitect.Modules.Flow.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\DungeonArchitect.Modules.Flow.Implementations.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\DungeonArchitect.Modules.Grammar.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\DungeonArchitect.Modules.Graph.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\DungeonArchitect.Modules.Meshing.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\DungeonArchitect.Modules.Navigation2D.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\DungeonArchitect.Modules.SxEngine.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\DungeonArchitect.Modules.UI.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\DungeonArchitect.Modules.VisibilityGraph.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\DungeonArchitect.ThirdParty.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\Owlcat.Runtime.Core.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\Owlcat.Runtime.Hardware.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\Owlcat.Runtime.UI.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\Owlcat.Runtime.UniRx.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\Owlcat.Runtime.Validation.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\Owlcat.Runtime.Visual.dll"),
                                    Path.Combine(wrathPath, "Wrath_Data\\Managed\\Owlcat.SharedTypes.dll")
)                                ],
                            },
                            new()
                            {
                                RetailGameExe = Path.Combine(wrathPath, "Wrath.exe"),
                                SteamAppId = "1184370",
                                CreateDebugCopy = true,
                                UnityInstallPath = defaultUnityPath,
                                UseSymlinks = true,
                                Symlinks = wrathSymlinks
                            }));
            }

            var rtPath = OwlCatUtil.FindRTPath();

            if (!string.IsNullOrEmpty(rtPath))
            {
                Profiles.Add(
                    "WH40KRT",
                        new SettingProfile(
                            "WH40KRT",
                            new()
                            {
                                AssemblyPaths =
                                [
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Assembly-CSharp-firstpass.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Autodesk.Fbx.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\BuildMode.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\BundlesBaseTypes.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\CircularBuffer.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Code.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Core.Async.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Core.Cheats.Attribute.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Core.Cheats.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Core.Console.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Core.Overlays.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Core.Reflection.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Core.RestServer.Client.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Core.RestServer.Common.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Core.RestServer.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Core.StateCrawler.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\CountingGuard.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\GeometryExtensions.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\GuidUtility.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.AreaLogic.TimeOfDay.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Blueprints.Attributes.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Blueprints.Base.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Blueprints.Hack.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Blueprints.JsonSystem.EditorDatabase.FileDatabaseClient.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Blueprints.JsonSystem.Hepers.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Blueprints.JsonSystem.PropertyUtility.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Blueprints.JsonSystem.PropertyUtility.Helper.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Blueprints.OverridesManager.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Controllers.Enums.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Controllers.Interfaces.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.ElementsSystem.Interfaces.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.EntitySystem.Stats.Base.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Enums.Damage.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Enums.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.GameInfo.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Localization.Enums.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.PubSubSystem.Core.Interfaces.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.QA.Arbiter.Profiling.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.ResourceLinks.BaseInterfaces.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.ResourceReplacementProvider.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.RuleSystem.Enum.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.RuleSystem.Rules.Interfaces.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Settings.ConstructionHelpers.KeyPrefix.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Settings.Interfaces.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Sound.Base.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Stores.DlcInterfaces.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Stores.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.TextTools.Base.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.TextTools.Core.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.UI.InputSystems.Enums.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.UI.Models.Log.ContextFlag.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.UI.Models.Log.Enums.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.UI.Models.Tooltip.Base.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.UnitLogic.Enums.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.UnitLogic.Mechanics.Facts.Interfaces.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Utility.Enums.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Utility.FlagCountable.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Utility.Fsm.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Utility.Random.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Visual.Animation.GraphVisualizerClient.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Visual.Base.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Visual.HitSystem.Base.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Kingmaker.Visual.Particles.GameObjectsPooling.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\LocalizationShared.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Owlcat.Runtime.Core.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Owlcat.Runtime.UI.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Owlcat.Runtime.UniRx.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Owlcat.Runtime.Validation.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Owlcat.Runtime.Visual.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Owlcat.ShaderLibrary.Visual.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\Owlcat.Shaders.Visual.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\PFlog.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\ReadOnlyState.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\ReplayLog.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\RogueTrader.Code.ShaderConsts.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\RogueTrader.Editor.ElementsDescription.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\RogueTrader.GameCore.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\RogueTrader.ModInitializer.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\RogueTrader.NetPlayer.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\RogueTrader.QA.QAModeExceptionEvents.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\RogueTrader.SharedTypes.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\StatefulRandom.dll"),
                                    Path.Combine(rtPath, "WH40KRT_Data\\Managed\\UnitLogic.Alignments.Enums.dll"
)                            ],
                            },
                            new()
                            {
                                RetailGameExe = Path.Combine(rtPath, "WH40KRT.exe"),
                                SteamAppId = "2186680",
                                CreateDebugCopy = true,
                                UnityInstallPath = defaultUnityPath,
                                UseSymlinks = true,
                                Symlinks = rtSymLinks
                            }));
            }

            var kmPath = OwlCatUtil.FindKMPath();

            if (!string.IsNullOrEmpty(kmPath))
            {

                Profiles.Add(
                    "Kingmaker",
                        new SettingProfile(
                            "Kingmaker",
                            new()
                            {
                                AssemblyPaths =
                                [
                                    Path.Combine(kmPath, "Kingmaker_Data\\Managed\\Assembly-CSharp.dll"),
                                ],
                            },
                            new()
                            {
                                RetailGameExe = Path.Combine(kmPath, "Kingmaker.exe"),
                                SteamAppId = "640820",
                                CreateDebugCopy = true,
                                UnityInstallPath = defaultUnityPath,
                                UseSymlinks = true,
                                Symlinks = kmSymlinks
                            }));
            }
        }
    }
}
