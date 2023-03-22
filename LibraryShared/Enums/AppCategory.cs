namespace LibraryShared
{
    public partial class Enums
    {
        public enum AppCategory
        {
            Unknown = -1,
            App = 0,
            Game = 1,
            Emulator = 2,
            Shortcut = 3,
            Process = 4,
            Launcher = 5
        }

        public enum ListCategory
        {
            App = 0,
            Game = 1,
            Emulator = 2,
            Launcher = 3,
            Shortcut = 4,
            Process = 5,
            Search = 6
        }

        public enum EmulatorCategory
        {
            Other = 0,
            Console = 1,
            Handheld = 2,
            Computer = 3,
            Arcade = 4,
            Pinball = 5,
            Pong = 6,
            Chess = 7,
            VirtualReality = 8,
            OperatingSystem = 9
        }
    }
}