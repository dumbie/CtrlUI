using ProtoBuf;
using System.Collections.Generic;

namespace CtrlUI
{
    public partial class Classes
    {
        //Arrays
        public static string[] vBattleNetUidBlacklist = { "agent", "agent_beta", "bna", "battle.net" };
        public static string[] vBattleNetBranchReplace = { "retail" };
        public static BattleNetLaunchIdConvert[] vBattleNetLaunchIdentifiers =
        {
            //Test LaunchId with battlenet://LaunchId
            new BattleNetLaunchIdConvert { UID = "wlby", LaunchID = "WLBY" }, //Crash Bandicoot 4: It's About Time
            new BattleNetLaunchIdConvert { UID = "rtro", LaunchID = "RTRO" }, //Blizzard Arcade Collection
            new BattleNetLaunchIdConvert { UID = "heroes", LaunchID = "Hero" }, //Heroes of the Storm
            new BattleNetLaunchIdConvert { UID = "prometheus", LaunchID = "Pro" }, //Overwatch
            new BattleNetLaunchIdConvert { UID = "s1", LaunchID = "S1" }, //StarCraft: Remastered
            new BattleNetLaunchIdConvert { UID = "s2", LaunchID = "S2" }, //StarCraft II
            new BattleNetLaunchIdConvert { UID = "hs_beta", LaunchID = "WTCG" }, //Hearthstone
            new BattleNetLaunchIdConvert { UID = "w3", LaunchID = "W3" }, //Warcraft III: Reforged
            new BattleNetLaunchIdConvert { UID = "wow", LaunchID = "WoW" }, //World of Warcraft
            //new LaunchIdConvert { UID = "wow_classic", LaunchID = "WoWC" }, //World of Warcraft Classic Expansion
            //new LaunchIdConvert { UID = "wow_classic_era", LaunchID = "WoWC" }, //World of Warcraft Classic Basegame
            new BattleNetLaunchIdConvert { UID = "anbs", LaunchID = "ANBS" }, //Diablo: Immortal
            new BattleNetLaunchIdConvert { UID = "osi", LaunchID = "OSI" }, //Diablo II: Resurrected
            new BattleNetLaunchIdConvert { UID = "d3cn", LaunchID = "D3CN" }, //Diablo III China
            new BattleNetLaunchIdConvert { UID = "diablo3", LaunchID = "D3" }, //Diablo III
            new BattleNetLaunchIdConvert { UID = "fenris", LaunchID = "Fen" }, //Diablo IV
            new BattleNetLaunchIdConvert { UID = "fore", LaunchID = "FORE" }, //Call of Duty: Vanguard
            new BattleNetLaunchIdConvert { UID = "auks", LaunchID = "AUKS" }, //Call of Duty: Modern Warfare II
            new BattleNetLaunchIdConvert { UID = "lazarus", LaunchID = "LAZR" }, //Call of Duty: Modern Warfare II Campaign
            new BattleNetLaunchIdConvert { UID = "odin", LaunchID = "ODIN" }, //Call of Duty: Modern Warfare
            new BattleNetLaunchIdConvert { UID = "zeus", LaunchID = "ZEUS" }, //Call of Duty: Black Ops Cold War
            new BattleNetLaunchIdConvert { UID = "viper", LaunchID = "VIPR" }, //Call of Duty: Black Ops 4
        };

        //Classes
        public class BattleNetLaunchIdConvert
        {
            public string UID { get; set; }
            public string LaunchID { get; set; }
        }

        //Classes ProtoBuf
        [ProtoContract()]
        public class BattleNetProductDatabase
        {
            [ProtoMember(1)]
            public List<ProductInstall> productInstall { get; set; }
        }

        [ProtoContract()]
        public class ProductInstall
        {
            [ProtoMember(1)]
            public string uid { get; set; }

            [ProtoMember(2)]
            public string productCode { get; set; }

            [ProtoMember(3)]
            public ProductSettings settings { get; set; }

            [ProtoMember(4)]
            public CachedProductState cachedProductState { get; set; }

            [ProtoMember(5)]
            public ProductOperations productOperations { get; set; }
        }

        [ProtoContract()]
        public class ProductSettings
        {
            [ProtoMember(1)]
            public string installPath { get; set; }

            [ProtoMember(2)]
            public string playRegion { get; set; }

            [ProtoMember(3)]
            public ShortcutEnum desktopShortcut { get; set; }

            [ProtoMember(4)]
            public ShortcutEnum startmenuShortcut { get; set; }

            [ProtoMember(5)]
            public LanguageSettingsEnum languageSettings { get; set; }

            [ProtoMember(6)]
            public string selectedTextLanguage { get; set; }

            [ProtoMember(7)]
            public string selectedSpeechLanguage { get; set; }

            [ProtoMember(8)]
            public List<Languages> languages { get; set; }

            [ProtoMember(13)]
            public string branch { get; set; }
        }

        [ProtoContract()]
        public class Languages
        {
            [ProtoMember(1)]
            public string language { get; set; }

            [ProtoMember(2)]
            public LanguageOptionEnum option { get; set; }
        }

        [ProtoContract()]
        public class CachedProductState
        {
            [ProtoMember(1)]
            public BaseProductState baseProductState { get; set; }

            [ProtoMember(2)]
            public BackfillProgress backfillProgress { get; set; }

            [ProtoMember(3)]
            public RepairProgress repairProgress { get; set; }

            [ProtoMember(4)]
            public UpdateProgress updateProgress { get; set; }
        }

        [ProtoContract()]
        public class BaseProductState
        {
            [ProtoMember(1)]
            public bool installed { get; set; }

            [ProtoMember(2)]
            public bool playable { get; set; }

            [ProtoMember(3)]
            public bool updateComplete { get; set; }

            [ProtoMember(4)]
            public bool backgroundDownloadAvailable { get; set; }

            [ProtoMember(5)]
            public bool backgroundDownloadComplete { get; set; }

            [ProtoMember(6)]
            public string currentVersion { get; set; }

            [ProtoMember(7)]
            public string currentVersionStr { get; set; }

            [ProtoMember(8)]
            public List<BuildConfig> installedBuildConfig { get; set; }

            [ProtoMember(9)]
            public List<BuildConfig> backgroundDownloadBuildConfig { get; set; }

            [ProtoMember(10)]
            public string decryptionKey { get; set; }

            [ProtoMember(11)]
            public List<string> completedInstallActions { get; set; }
        }

        [ProtoContract()]
        public class BackfillProgress
        {
            [ProtoMember(1)]
            public double progress { get; set; }

            [ProtoMember(2)]
            public bool backgrounddownload { get; set; }

            [ProtoMember(3)]
            public bool paused { get; set; }

            [ProtoMember(4)]
            public ulong downloadLimit { get; set; }
        }

        [ProtoContract()]
        public class RepairProgress
        {
            [ProtoMember(1)]
            public double progress { get; set; }
        }

        [ProtoContract()]
        public class UpdateProgress
        {
            [ProtoMember(1)]
            public string lastDiscSetUsed { get; set; }

            [ProtoMember(2)]
            public double progress { get; set; }

            [ProtoMember(3)]
            public bool discIgnored { get; set; }

            [ProtoMember(4)]
            public ulong totalToDownload { get; set; }

            [ProtoMember(5)]
            public ulong downloadRemaining { get; set; }
        }

        [ProtoContract()]
        public class ProductOperations
        {
            [ProtoMember(1)]
            public OperationEnum activeOperation { get; set; }

            [ProtoMember(2)]
            public ulong priority { get; set; }
        }

        [ProtoContract()]
        public class BuildConfig
        {
            [ProtoMember(1)]
            public string region { get; set; }

            [ProtoMember(2)]
            public string buildConfig { get; set; }
        }

        //Enumerators ProtoBuf
        [ProtoContract()]
        public enum LanguageOptionEnum
        {
            LANGOPTION_NONE = 0,
            LANGOPTION_TEXT = 1,
            LANGOPTION_SPEECH = 2,
            LANGOPTION_TEXT_AND_SPEECH = 3,
        }

        [ProtoContract()]
        public enum LanguageSettingsEnum
        {
            LANGSETTING_NONE = 0,
            LANGSETTING_SINGLE = 1,
            LANGSETTING_SIMPLE = 2,
            LANGSETTING_ADVANCED = 3,
        }

        [ProtoContract()]
        public enum ShortcutEnum
        {
            SHORTCUT_NONE = 0,
            SHORTCUT_USER = 1,
            SHORTCUT_ALL_USERS = 2,
        }

        [ProtoContract()]
        public enum OperationEnum
        {
            OP_NONE = -1,
            OP_UPDATE = 0,
            OP_BACKFILL = 1,
            OP_REPAIR = 2,
        }
    }
}