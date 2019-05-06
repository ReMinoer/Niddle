using Niddle.Attributes;
using Niddle.Injectables;

namespace Niddle.Test.Samples
{
    public interface ILevel
    {
        Game Game { get; }
        IPlayer Player { get; }
    }

    public class Level : ILevel
    {
        [Resolvable]
        public ICharacter CharacterB;

        public Game Game { get; private set; }
        public IPlayer Player { get; private set; }

        [Resolvable]
        public ICharacter CharacterA { get; set; }

        public Level(Game game, IPlayer player)
        {
            Game = game;
            Player = player;
        }
    }
}