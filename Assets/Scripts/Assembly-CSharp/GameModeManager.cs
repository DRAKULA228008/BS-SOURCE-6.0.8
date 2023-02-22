using System;

public class GameModeManager
{
    public static GameMode[] gameMode
    {
        get
        {
            return new GameMode[20]
            {
                GameMode.TeamDeathmatch,
                GameMode.Classic,
                GameMode.Bomb,
                GameMode.Bomb2,
                GameMode.ZombieSurvival,
                GameMode.KnifeMode,
                GameMode.AWPMode,
                GameMode.GunGame,
                GameMode.Escort,
                GameMode.DeathRun,
                GameMode.Murder,
                GameMode.Hunter,
                GameMode.RandomMode,
                GameMode.Deathmatch,
                GameMode.BunnyHop,
                GameMode.HungerGames,
                GameMode.Only,
                GameMode.Juggernaut,
                GameMode.Surf,
                GameMode.MiniGames
            };
        }
    }

    public static GameMode[] customGameMode
    {
        get
        {
            return new GameMode[7]
            {
                GameMode.TeamDeathmatch,
                GameMode.Classic,
                GameMode.KnifeMode,
                GameMode.AWPMode,
                GameMode.GunGame,
                GameMode.RandomMode,
                GameMode.BunnyHop
            };
        }
    }

    public static bool ContainsCustomMode(GameMode mode)
    {
        GameMode[] array = new GameMode[7]
        {
            GameMode.TeamDeathmatch,
            GameMode.Classic,
            GameMode.KnifeMode,
            GameMode.AWPMode,
            GameMode.GunGame,
            GameMode.RandomMode,
            GameMode.BunnyHop
        };
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == mode)
            {
                return true;
            }
        }
        return false;
    }

    public static GameMode Get(string value)
    {
        return (GameMode)(int)Enum.Parse(typeof(GameMode), value);
    }
}
