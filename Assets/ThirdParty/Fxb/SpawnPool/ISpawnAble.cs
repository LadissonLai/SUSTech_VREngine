namespace Fxb.SpawnPool
{
    public interface ISpawnAble
    {
        string Key { get;set;}

        void OnSpawn();

        void OnDespawn();
    }
}
