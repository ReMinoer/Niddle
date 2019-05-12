using System;
using System.Collections.Generic;
using Niddle.Test.Samples;
using NUnit.Framework;

namespace Niddle.Test
{
    [TestFixture]
    public class InjectionTest
    {
        private IDependencyResolver _resolver;
        private IDependencyRegistry _registry;

        [SetUp]
        public void SetUp()
        {
            _registry = new DependencyRegistry();
            _resolver = new RegistryResolver(_registry);
        }

        [Test]
        public void RegisterType()
        {
            _registry.Add(Dependency.OnType<Player>());

            var player = _resolver.Resolve<Player>();
            var otherPlayer = _resolver.Resolve<Player>();

            Assert.IsNotNull(player);
            Assert.AreNotEqual(player, otherPlayer);
        }

        [Test]
        public void RegisterImplementationA()
        {
            _registry.Add(Dependency.OnType<ICharacter>().Creating<Player>());

            var character = _resolver.Resolve<ICharacter>();

            Assert.IsTrue(character is Player);
        }

        [Test]
        public void RegisterImplementationB()
        {
            _registry.Add(Dependency.OnType<ICharacter>().Creating<Player>());

            object character = _resolver.Resolve(typeof(ICharacter));

            Assert.IsTrue(character is Player);
        }

        [Test]
        public void RegisterInstance()
        {
            var alpha = new Character();
            _registry.Add(Dependency.OnType<ICharacter>().Using(alpha));

            var resolvedAlpha = _resolver.Resolve<ICharacter>();

            Assert.AreEqual(alpha, resolvedAlpha);
        }

        [Test]
        public void RegisterSingleton()
        {
            _registry.Add(Dependency.OnType<Game>().AsSingleton());

            var game = _resolver.Resolve<Game>();
            var otherGame = _resolver.Resolve<Game>();

            Assert.AreEqual(game, otherGame);
        }

        [Test]
        public void RegisterAction()
        {
            IResolvableMembersProvider<object> resolvableMembersProvider = new ResolvableMembersProvider();

            _registry.Add(Dependency.OnType<Level>().ResolvingMembersFrom(resolvableMembersProvider));
            _registry.Add(Dependency.OnType<ICharacter>().Creating<Character>());
            _registry.Add(Dependency.OnType<IPlayer>().Creating<Player>());
            _registry.Add(Dependency.OnType<Game>());

            var level = _resolver.Resolve<Level>();

            Assert.IsNotNull(level.CharacterA);

            _registry.Add(Dependency.OnType<Action<Level>>().Using(x => x.CharacterA = null));
            var action = _resolver.Resolve<Action<Level>>();

            action(level);

            Assert.IsNull(level.CharacterA);
        }

        [Test]
        public void RegisterFunc()
        {
            _registry.Add(Dependency.OnType<Func<int, int>>().Using(x => x * 2));

            var func = _resolver.Resolve<Func<int, int>>();
            int result = func(10);

            Assert.AreEqual(result, 20);
        }

        [Test]
        public void RegisterGeneric()
        {
            _registry.Add(Dependency.OnGeneric(typeof(List<>)));

            var list = _resolver.Resolve<List<int>>();
            var list2 = _resolver.Resolve<List<string>>();
            var list3 = _resolver.Resolve<List<Player>>();

            Assert.IsNotNull(list);
            Assert.IsNotNull(list2);
            Assert.IsNotNull(list3);
        }

        [Test]
        public void RegisterKeyed()
        {
            _registry.Add(Dependency.OnType<IPlayer>().Creating<Player>().AsSingleton().Keyed(PlayerId.One));
            _registry.Add(Dependency.OnType<IPlayer>().Creating<Player>().AsSingleton().Keyed(PlayerId.Two));

            var playerOne = _resolver.Resolve<IPlayer>(key: PlayerId.One);
            var otherPlayerOne = _resolver.Resolve<IPlayer>(key: PlayerId.One);
            var playerTwo = _resolver.Resolve<IPlayer>(key: PlayerId.Two);
            var otherPlayerTwo = _resolver.Resolve<IPlayer>(key: PlayerId.Two);

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

            var playerOne = _resolver.Resolve<IPlayer>(key: PlayerId.One);
            var playerTwo = _resolver.Resolve<IPlayer>(key: PlayerId.Two);
            var mainPlayer = _resolver.Resolve<IPlayer>(key: "MainPlayer");

            Assert.AreEqual(playerOne, mainPlayer);
            Assert.AreNotEqual(playerTwo, mainPlayer);
        }

        [Test]
        public void RegisterWithDependencies()
        {
            _registry.Add(Dependency.OnType<ILevel>().Creating<Level>());
            _registry.Add(Dependency.OnType<IPlayer>().Creating<Player>());
            _registry.Add(Dependency.OnType<Game>().AsSingleton());

            var level = _resolver.Resolve<ILevel>();

            Assert.IsNotNull(level);
            Assert.IsNotNull(level.Game);
            Assert.IsNotNull(level.Player);
        }

        [Test]
        public void InjectPropertyAndField()
        {
            IResolvableMembersProvider<object> resolvableMembersProvider = new ResolvableMembersProvider();

            _registry.Add(Dependency.OnType<Level>().ResolvingMembersFrom(resolvableMembersProvider));
            _registry.Add(Dependency.OnType<IPlayer>().Creating<Player>());
            _registry.Add(Dependency.OnType<Game>().AsSingleton());

            var level = _resolver.Resolve<Level>();

            Assert.IsNotNull(level);
            Assert.IsNotNull(level.Game);
            Assert.IsNotNull(level.Player);
            Assert.IsNull(level.CharacterA);
            Assert.IsNull(level.CharacterB);
            
            _registry.Add(Dependency.OnType<ICharacter>().Creating<Character>());

            var otherLevel = _resolver.Resolve<Level>();

            Assert.IsNotNull(otherLevel);
            Assert.IsNotNull(otherLevel.Game);
            Assert.IsNotNull(otherLevel.Player);
            Assert.IsNotNull(otherLevel.CharacterA);
            Assert.IsNotNull(otherLevel.CharacterB);
        }
    }
}