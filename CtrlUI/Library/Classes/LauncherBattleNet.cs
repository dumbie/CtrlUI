using ProtoBuf;
using System.Collections.Generic;

namespace LibraryShared
{
    [ProtoContract()]
    public partial class BattleNetProductDatabase
    {
        [ProtoMember(1)]
        public List<ProductInstall> productInstall { get; set; }
    }

    [ProtoContract()]
    public partial class ProductInstall
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
    public partial class ProductSettings
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
    public partial class Languages
    {
        [ProtoMember(1)]
        public string language { get; set; }

        [ProtoMember(2)]
        public LanguageOptionEnum option { get; set; }
    }

    [ProtoContract()]
    public partial class CachedProductState
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
    public partial class BaseProductState
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
    public partial class BackfillProgress
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
    public partial class RepairProgress
    {
        [ProtoMember(1)]
        public double progress { get; set; }
    }

    [ProtoContract()]
    public partial class UpdateProgress
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
    public partial class ProductOperations
    {
        [ProtoMember(1)]
        public OperationEnum activeOperation { get; set; }

        [ProtoMember(2)]
        public ulong priority { get; set; }
    }

    [ProtoContract()]
    public partial class BuildConfig
    {
        [ProtoMember(1)]
        public string region { get; set; }

        [ProtoMember(2)]
        public string buildConfig { get; set; }
    }

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