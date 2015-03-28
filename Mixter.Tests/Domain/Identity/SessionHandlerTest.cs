﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixter.Domain.Identity;
using Mixter.Domain.Identity.Events;
using NFluent;

namespace Mixter.Tests.Domain.Identity
{
    [TestClass]
    public class SessionHandlerTest
    {
        private SessionHandler _handler;
        private SessionsRepositoryFake _repository;

        [TestInitialize]
        public void Initialize()
        {
            _repository = new SessionsRepositoryFake();
            _handler = new SessionHandler(_repository);
        }

        [TestMethod]
        public void WhenUserConnectedThenStoreSessionProjection()
        {
            var userConnected = new UserConnected(SessionId.Generate(), new UserId("user@mixit.fr"), DateTime.Now);

            _handler.Handle(userConnected);

            Check.That(_repository.Projections)
                 .ContainsExactly(new SessionProjection(userConnected.SessionId, userConnected.UserId, SessionState.Enabled));
        }

        [TestMethod]
        public void WhenUserDiconnectedThenUpdateSessionProjectionAndEnableDisconnectedFlag()
        {
            var userConnected = new UserConnected(SessionId.Generate(), new UserId("user@mixit.fr"), DateTime.Now);
            _handler.Handle(userConnected);

            _handler.Handle(new UserDisconnected(userConnected.SessionId, userConnected.UserId));

            Check.That(_repository.Projections)
                 .ContainsExactly(new SessionProjection(userConnected.SessionId, userConnected.UserId, SessionState.Disabled));
        }

        private class SessionsRepositoryFake : ISessionsRepository
        {
            private readonly Dictionary<SessionId, SessionProjection> _projectionsById = new Dictionary<SessionId, SessionProjection>();

            public IEnumerable<SessionProjection> Projections
            {
                get { return _projectionsById.Values; }
            }

            public void Save(SessionProjection projection)
            {
                _projectionsById[projection.SessionId] = projection;
            }

            public void ReplaceBy(SessionProjection projection)
            {
                Save(projection);
            }
        }
    }
}
