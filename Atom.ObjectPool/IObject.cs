namespace Atom
{
    public interface IObject
    {
        /// <summary>
        /// 获取时调用
        /// </summary>
        void OnSpawn();

        /// <summary>
        /// 回收时调用
        /// </summary>
        void OnRecycle();
    }
}