using Cakewalk.IoC.Core;

public class BootStrapper : BaseBootStrapper
{
    public override void Configure(Container container)
    {
        container.Register<MusicManager>();
        container.Register<RoftCreator>();
        container.Register<MapReader>();
        container.Register<AudioManager>();
        container.Register<Key_Layout>();
        container.Register<RoftPlayer>();
    }
}
