using System;
using System.Collections.Generic;
using Niddle.Test.Samples;
using NUnit.Framework;

namespace Niddle.Test
{
    [TestFixture]
    public class InjectionTest
    {
        private IDependencyInjector _injector;
        private IDependencyRegistry _registry;

        [SetUp]
        public void SetUp()
        {
            _registry = new DependencyRegistry();
            _injector = new RegistryInjector(_registry);
        }

        [Test]
        public void RegisterType()
        {
            _registry.Register<Player>();

            var player = _injector.Resolve<Player>();
            var otherPlayer = _injector.Resolve<Player>();

            Assert.IsNotNull(player);
            Assert.AreNotEqual(player, otherPlayer);
        }

        [Test]
        public void RegisterImplementationA()
        {
            _registry.Register<ICharacter, Player>();

            var character = _injector.Resolve<ICharacter>();

            Assert.IsTrue(character is Player);
        }

        [Test]
        public void RegisterImplementationB()
        {
            _registry.Register(typeof(ICharacter), typeof(Player));

            object character = _injector.Resolve(typeof(ICharacter));

            Assert.IsTrue(character is Player);
        }

        [Test]
        public void RegisterInstance()
        {
            var alpha = new Character();
            _registry.RegisterInstance<ICharacter>(alpha);

            var resolvedAlpha = _injector.Resolve<ICharacter>();

            Assert.AreEqual(alpha, resolvedAlpha);
        }

        [Test]
        public void RegisterLazy()
        {
            _registry.RegisterLazy<ICharacter>(() => new Character());
            var lazy = _injector.Resolve<Lazy<ICharacter>>();

            Assert.IsFalse(lazy.IsValueCreated);

            Assert.IsNotNull(lazy.Value);
            Assert.IsTrue(lazy.IsValueCreated);
        }

        [Test]
        public void RegisterSingleton()
        {
            _registry.RegisterSingleton<Game>();

            var game = _injector.Resolve<Game>();
            var otherGame = _injector.Resolve<Game>();

            Assert.AreEqual(game, otherGame);
        }

        [Test]
        public void RegisterAction()
        {
            _registry.Register<Level>();
            _registry.Register<ICharacter, Character>();
            _registry.Register<IPlayer, Player>();
            _registry.RegisterSingleton<Game>();

            var level = _injector.Resolve<Level>();

            Assert.IsNotNull(level.CharacterA);

            _registry.RegisterAction<Level>(x => x.CharacterA = null);
            var action = _injector.Resolve<Action<Level>>();

            action(level);

            Assert.IsNull(level.CharacterA);
        }

        [Test]
        public void RegisterFunc()
        {
            _registry.RegisterFunc<int, int>(x => x * 2);

            var func = _injector.Resolve<Func<int, int>>();
            int result = func(10);

            Assert.AreEqual(result, 20);
        }

        [Test]
        public void RegisterGeneric()
        {
            _registry.RegisterGeneric(typeof(List<>));

            var list = _injector.Resolve<List<int>>();
            var list2 = _injector.Resolve<List<string>>();
            var list3 = _injector.Resolve<List<Player>>();

            Assert.IsNotNull(list);
            Assert.IsNotNull(list2);
            Assert.IsNotNull(list3);
        }

        [Test]
        public void RegisterKeyed()
        {
            _registry.RegisterSingleton<IPlayer, Player>(PlayerId.One);
            _registry.RegisterSingleton<IPlayer, Player>(PlayerId.Two);

            var playerOne = _injector.Resolve<IPlayer>(serviceKey: PlayerId.One);
            var otherPlayerOne = _injector.Resolve<IPlayer>(serviceKey: PlayerId.One);
            var playerTwo = _injector.Resolve<IPlayer>(serviceKey: PlayerId.Two);
            var otherPlayerTwo = _injector.Resolve<IPlayer>(serviceKey: PlayerId.Two);

            Assert.AreEqual(playerOne, otherPlayerOne);
            Assert.AreEqual(playerTwo, otherPlayerTwo);

            Assert.AreNotEqual(playerOne, playerTwo);
            Assert.AreNotEqual(otherPlayerOne, otherPlayerTwo);
            Assert.AreNotEqual(playerOne, otherPlayerTwo);
            Assert.AreNotEqual(playerTwo, otherPlayerOne);
        }

        [Test]
        public void LinkTypes()
        {
            _registry.RegisterSingleton<IPlayer, Player>(PlayerId.One);
            _registry.RegisterSingleton<IPlayer, Player>(PlayerId.Two);
            _registry.Link<IPlayer, IPlayer>(PlayerId.One, "MainPlayer");

            var playerOne = _injector.Resolve<IPlayer>(serviceKey: PlayerId.One);
            var playerTwo = _injector.Resolve<IPlayer>(serviceKey: PlayerId.Two);
            var mainPlayer = _injector.Resolve<IPlayer>(serviceKey: "MainPlayer");

            Assert.AreEqual(playerOne, mainPlayer);
            Assert.AreNotEqual(playerTwo, mainPlayer);
        }

        [Test]
        public void RegisterWithDependencies()
        {
            _registry.Register<ILevel, Level>();
            _registry.Register<IPlayer, Player>();
            _registry.RegisterSingleton<Game>();

            var level = _injector.Resolve<ILevel>();

            Assert.IsNotNull(level);
            Assert.IsNotNull(level.Game);
            Assert.IsNotNull(level.Player);
        }

        [Test]
        public void InjectPropertyAndField()
        {
            _registry.Register<Level>();
            _registry.Register<IPlayer, Player>();
            _registry.RegisterSingleton<Game>();

            var level = _injector.Resolve<Level>();

            Assert.IsNotNull(level);
            Assert.IsNotNull(level.Game);
            Assert.IsNotNull(level.Player);
            Assert.IsNull(level.CharacterA);
            Assert.IsNull(level.CharacterB);

            _registry.Register<ICharacter, Character>();

            var otherLevel = _injector.Resolve<Level>();

            Assert.IsNotNull(otherLevel);
            Assert.IsNotNull(otherLevel.Game);
            Assert.IsNotNull(otherLevel.Player);
            Assert.IsNotNull(otherLevel.CharacterA);
            Assert.IsNotNull(otherLevel.CharacterB);
        }
    }
}