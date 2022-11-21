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
            Unknown = -1,
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
            Unknown = -1,
            Console = 0,
            Handheld = 1,
            Arcade = 2,
            Pinball = 3,
            Chess = 4,
            Pong = 5,
            VirtualReality = 6
        }
    }
}