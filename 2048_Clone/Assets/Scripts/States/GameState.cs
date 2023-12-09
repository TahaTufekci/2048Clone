using System;


[Flags]
public enum GameState
{
    SpawningBlocks,
    WaitingInput,
    Moving,
    Lose,
    Win
}
