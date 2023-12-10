﻿namespace Injact;

public interface IFactory { }

public interface IFactory<out TValue> : IFactory
{
    public TValue Create();
    
    public TValue Create(params object[] args);
}