namespace CheekyStork.Logging
{
    public enum LogTag
    {
        Misc = 0, // Miscellaneous
        Editor = 1, // Editor-only things
        Tools = 2, // Things related to development (but not editor-specific)
        Sandbox = 3, // Development sandbox/testing
        Core = 4, // Core systems (e.g. saving, input, etc.)
        Flow = 5, // Things related to loading/unloading scenes and game flow (e.g. scene management, bootstrap systems, etc.)
        Player = 6, // Things related to the playable character
        World = 7, // Things related to in-game levels
        GameMaster = 8, // Related to game master systems, enemy spawners, wave management, etc.
        UI = 9, // User Interface
        AI = 10, // NPC's logic and behavior
        Audio = 11, // Sound and music
        Animation = 12, // Character and object animations
        Skills = 13, // Abilities, powers, unlocks, etc.
        Items = 14, // Inventory, pickups, equipment, etc.
        Stats = 15, // Stat systems, stats, stat modifications, etc.
    }
}