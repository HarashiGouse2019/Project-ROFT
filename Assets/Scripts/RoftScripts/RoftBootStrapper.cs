using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cakewalk.IoC.Core;

public class RoftBootStrapper : BaseBootStrapper
{
    public override void Configure(Container _container)
    {
        _container.Register<MapReader>();
        _container.Register<RoftPlayer>();
        _container.Register<Key_Layout>();
    }
}
