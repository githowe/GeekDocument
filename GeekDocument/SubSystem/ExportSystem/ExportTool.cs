using GeekDocument.SubSystem.LayoutEngine;
using GeekDocument.SubSystem.WindowSystem;

namespace GeekDocument.SubSystem.ExportSystem
{
    public class ExportTool
    {
        private ExportTool() { }
        public static ExportTool Instance { get; } = new ExportTool();

        public void Init()
        {
            _exporterDict.Add(ExportFormat.Html, new Exporter.HtmlExporter());
            _exporterDict.Add(ExportFormat.Bdoc, new Exporter.BdocExporter());
        }

        public void Export(页面 page, ExportFormat format, string path, string name)
        {
            if (_exporterDict.TryGetValue(format, out var exporter))
            {
                exporter.Export(page, path, name);
            }
            else WM.ShowErrorTip($"不支持的导出格式：{format}");
        }

        private readonly Dictionary<ExportFormat, IExporter> _exporterDict = new Dictionary<ExportFormat, IExporter>();
    }
}