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
            _registry.Add(Dependency.OnType<Player>());

            var player = _injector.Resolve<Player>();
            var otherPlayer = _injector.Resolve<Player>();

            Assert.IsNotNull(player);
            Assert.AreNotEqual(player, otherPlayer);
        }

        [Test]
        public void RegisterImplementationA()
        {
            _registry.Add(Dependency.OnType<ICharacter>().Creating<Player>());

            var character = _injector.Resolve<ICharacter>();

            Assert.IsTrue(character is Player);
        }

        [Test]
        public void RegisterImplementationB()
        {
            _registry.Add(Dependency.OnType<ICharacter>().Creating<Player>());

            object character = _injector.Resolve(typeof(ICharacter));

            Assert.IsTrue(character is Player);
        }

        [Test]
        public void RegisterInstance()
        {
            var alpha = new Character();
            _registry.Add(Dependency.OnType<ICharacter>().Using(alpha));

            var resolvedAlpha = _injector.Resolve<ICharacter>();

            Assert.AreEqual(alpha, resolvedAlpha);
        }

        [Test]
        public void RegisterSingleton()
        {
            _registry.Add(Dependency.OnType<Game>().AsSingleton());

            var game = _injector.Resolve<Game>();
            var otherGame = _injector.Resolve<Game>();

            Assert.AreEqual(game, otherGame);
        }

        [Test]
        public void RegisterAction()
        {
            _registry.Add(Dependency.OnType<Level>());
            _registry.Add(Dependency.OnType<ICharacter>().Creating<Character>());
            _registry.Add(Dependency.OnType<IPlayer>().Creating<Player>());
            _registry.Add(Dependency.OnType<Game>());

            var level = _injector.Resolve<Level>();

            Assert.IsNotNull(level.CharacterA);

            _registry.Add(Dependency.OnType<Action<Level>>().Using(x => x.CharacterA = null));
            var action = _injector.Resolve<Action<Level>>();

            action(level);

            Assert.IsNull(level.CharacterA);
        }

        [Test]
        public void RegisterFunc()
        {
            _registry.Add(Dependency.OnType<Func<int, int>>().Using(x => x * 2));

            var func = _injector.Resolve<Func<int, int>>();
            int result = func(10);

            Assert.AreEqual(result, 20);
        }

        [Test]
        public void RegisterGeneric()
        {
            _registry.Add(Dependency.OnGeneric(typeof(List<>)));

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
            _registry.Add(Dependency.OnType<IPlayer>().Creating<Player>().AsSingleton().Keyed(PlayerId.One));
            _registry.Add(Dependency.OnType<IPlayer>().Creating<Player>().AsSingleton().Keyed(PlayerId.Two));

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
            _registry.Add(Dependency.OnType<IPlayer>().Creating<Player>().AsSingleton().Keyed(PlayerId.One));
            _registry.Add(Dependency.OnType<IPlayer>().Creating<Player>().AsSingleton().Keyed(PlayerId.Two));
            _registry.Add(Dependency.OnType<IPlayer>().Keyed("MainPlayer").LinkedTo<IPlayer>().Keyed(PlayerId.One));

            var playerOne = _injector.Resolve<IPlayer>(serviceKey: PlayerId.One);
            var playerTwo = _injector.Resolve<IPlayer>(serviceKey: PlayerId.Two);
            var mainPlayer = _injector.Resolve<IPlayer>(serviceKey: "MainPlayer");

            Assert.AreEqual(playerOne, mainPlayer);
            Assert.AreNotEqual(playerTwo, mainPlayer);
        }

        [Test]
        public void RegisterWithDependencies()
        {
            _registry.Add(Dependency.OnType<ILevel>().Creating<Level>());
            _registry.Add(Dependency.OnType<IPlayer>().Creating<Player>());
            _registry.Add(Dependency.OnType<Game>().AsSingleton());

            var level = _injector.Resolve<ILevel>();

            Assert.IsNotNull(level);
            Assert.IsNotNull(level.Game);
            Assert.IsNotNull(level.Player);
        }

        [Test]
        public void InjectPropertyAndField()
        {
            _registry.Add(Dependency.OnType<Level>());
            _registry.Add(Dependency.OnType<IPlayer>().Creating<Player>());
            _registry.Add(Dependency.OnType<Game>().AsSingleton());

            var level = _injector.Resolve<Level>();

            Assert.IsNotNull(level);
            Assert.IsNotNull(level.Game);
            Assert.IsNotNull(level.Player);
            Assert.IsNull(level.CharacterA);
            Assert.IsNull(level.CharacterB);
            
            _registry.Add(Dependency.OnType<ICharacter>().Creating<Character>());

            var otherLevel = _injector.Resolve<Level>();

            Assert.IsNotNull(otherLevel);
            Assert.IsNotNull(otherLevel.Game);
            Assert.IsNotNull(otherLevel.Player);
            Assert.IsNotNull(otherLevel.CharacterA);
            Assert.IsNotNull(otherLevel.CharacterB);
        }
    }
}