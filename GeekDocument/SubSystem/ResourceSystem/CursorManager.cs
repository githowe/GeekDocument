using System.Windows;
using System.Windows.Input;
using System.Windows.Resources;

namespace GeekDocument.SubSystem.ResourceSystem
{
    public class CursorManager
    {
        #region 单例

        private CursorManager() { }
        public static CursorManager Instance { get; } = new CursorManager();

        #endregion

        #region 光标

        public Cursor Select { get; set; } = null!;

        public Cursor SelectAndMove { get; set; } = null!;

        public Cursor Move { get; set; } = null!;

        public Cursor Drag { get; set; } = null!;

        public Cursor ResizeX { get; set; } = null!;

        public Cursor ResizeY { get; set; } = null!;

        public Cursor SelectRow { get; set; } = null!;

        public Cursor SelectCol { get; set; } = null!;

        #endregion

        public void Init()
        {
            Select = LoadCursor("Assets/Cursor/Select.cur");
            SelectAndMove = LoadCursor("Assets/Cursor/MoveSelected.cur");
            Move = LoadCursor("Assets/Cursor/Move.cur");
            Drag = LoadCursor("Assets/Cursor/Drag.cur");
            ResizeX = LoadCursor("Assets/Cursor/ResizeX.cur");
            ResizeY = LoadCursor("Assets/Cursor/ResizeY.cur");
            SelectRow = LoadCursor("Assets/Cursor/SelectRow.cur");
            SelectCol = LoadCursor("Assets/Cursor/SelectCol.cur");
        }

        private Cursor LoadCursor(string cursorPath)
        {
            StreamResourceInfo resourceInfo = Application.GetResourceStream(new Uri(cursorPath, UriKind.Relative));
            return new Cursor(resourceInfo.Stream);
        }
    }
}