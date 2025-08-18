using GeekDocument.SubSystem.EditerSystem.Control.Layer;

namespace GeekDocument.SubSystem.EditerSystem.Define
{
    /// <summary>
    /// 页面接口
    /// </summary>
    public interface IPage
    {
        void 移动光标(int x, int y, int height);

        void 更新光标横坐标();

        int 获取光标横坐标();

        void 移动光标至前块末尾(BlockLayer block);

        void 移动光标至后块开头(BlockLayer block);

        void 移动光标至前块最后一行(BlockLayer block);

        void 移动光标至后块第一行(BlockLayer block);

        void 设置当前块(BlockLayer block);

        int 获取块索引(BlockLayer block);

        bool 有上一个块(BlockLayer block);

        bool 有下一个块(BlockLayer block);

        public BlockLayer? 获取上一个块(BlockLayer block);

        public BlockLayer? 获取下一个块(BlockLayer block);

        void 插入块(Block block, int index);

        void 移除块(BlockLayer block);
    }
}