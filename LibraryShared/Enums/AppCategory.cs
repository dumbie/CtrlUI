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
            Game = 0,
            App = 1,
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
            Arcade = 3,
            Pinball = 4,
            Pong = 5,
            Chess = 6,
            VirtualReality = 7
        }
    }
}