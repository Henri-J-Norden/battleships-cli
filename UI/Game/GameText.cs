using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Game;

namespace UI {
    public static class GameText {
        static string Header() {
            var player = (Players.CurrentPlayer.ShowEnemyBoard ? Players.GetOtherPlayer() : Players.CurrentPlayer);
            return $"  Turn {State.Turn} : {Players.CurrentPlayer} =[{(Players.CurrentPlayer.ShowEnemyBoard ? "ENEMY" : "FRIENDLY")} VIEW]=\n"+
                   $"\t{player}'s board: {player.HP} / {Players.MaxHP} HP";
        }
        static string ToggleBoard() => $"Press [SPACE] to show {(Players.CurrentPlayer.ShowEnemyBoard ? "your" : "enemy")} board.";

        public static string RenderBoard() {
            if (Players.CurrentPlayer.IsAI) Players.CurrentPlayer.ShowEnemyBoard = true;
            return Boards.CurrentBoard.Render(Boards.CurrentBoard.Player != Players.CurrentPlayer);
        }

        static string RenderMessages() => String.Join("\n", Players.CurrentPlayer.Messages);

        public static string Generate(string renderedBoard = null) =>
$@"{Header()}
{renderedBoard ?? RenderBoard()}
{(State.Turn != 0 && !State.TurnEnded ? "\n" + ToggleBoard() : "")}
{RenderMessages()}
";
        
    }
}
