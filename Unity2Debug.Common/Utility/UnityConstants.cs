namespace Unity2Debug.Common.Utility
{
    public static class UnityConstants
    {
        public const string UNITY_DEFAULT_BASE = @"C:\Program Files\Unity\Hub\Editor";
        public const string UNITY_EXE_NAME = @"Unity.exe";
        public const string UNITY_EXE_PATH = @"Editor\" + UNITY_EXE_NAME;
        public const string DEV64_MONO_PATH = @"Data\PlaybackEngines\windowsstandalonesupport\Variations\win64_development_mono\";
        public const string DEV64_MONO_PATH2 = @"Data\PlaybackEngines\windowsstandalonesupport\Variations\win64_player_development_mono\";
        public const string DEV_PLAYER_FILE = @"WindowsPlayer.exe";
        public const string DEV_LIBRARY_FILE = @"UnityPlayer.dll";
        public const string DEV_WINPIX_FILE = @"WinPixEventRuntime.dll";
        public const string DEV_MONO_FILE = @"MonoBleedingEdge\EmbedRuntime\mono-2.0-bdwgc.dll";
        public const string BOOTCONFIG_WAIT_DEBUG = "wait-for-managed-debugger=";
        public const string BOOTCONFIG_CONNECT_DEBUG = "player-connection-debug=";
        public const string BOOTCONFIG_PORT_DEBUG = "managed-debugger-fixed-port=";
        public const string BOOTCONFIG_FILENAME = "boot.config";
    }
}
