﻿namespace Mixter.Domain.Identity
{
    public interface ISessionsRepository
    {
        void Save(SessionProjection projection);

        void ReplaceBy(SessionProjection projection);
    }
}