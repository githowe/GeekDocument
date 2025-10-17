using GeekDocument.SubSystem.LayoutEngine;

namespace GeekDocument.SubSystem.ExportSystem
{
    /// <summary>
    /// 导出器接口
    /// </summary>
    public interface IExporter
    {
        void Export(页面 page, string path, string name);
    }
}