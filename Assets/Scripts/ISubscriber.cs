using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IParam
{

}

struct TypeParam<T> : IParam
{
    readonly T param;

    TypeParam(T p)
    {
        param = p;
    }
};

public interface ISubscriber
{
    void Notify(EventName eventName, IParam parameters);
}
