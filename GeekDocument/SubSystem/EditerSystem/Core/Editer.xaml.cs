using GeekDocument.SubSystem.EditerSystem.Core.Component;
using GeekDocument.SubSystem.EditerSystem.Define;
using System.Windows.Controls;
using System.Windows.Input;
using XLogic.Base.UI;

namespace GeekDocument.SubSystem.EditerSystem.Core
{
    public partial class Editer : UserControl
    {
        #region 构造方法

        public Editer() => InitializeComponent();

        #endregion

        #region 公开方法

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            // 添加组件
            _documentComponent = _componentBox.AddComponent<DocumentComponent>(this, "文档组件");
            _ibeamComponent = _componentBox.AddComponent<IBeamComponent>(this, "光标组件");
            _interactionComponent = _componentBox.AddComponent<InteractionComponent>(this, "交互组件");
            _pageComponent = _componentBox.AddComponent<PageComponent>(this, "页面组件");
            _scrollBarComponent = _componentBox.AddComponent<ScrollBarComponent>(this, "滚动条组件");
            _selectComponent = _componentBox.AddComponent<SelectComponent>(this, "选择组件");
            _toolBarComponent = _componentBox.AddComponent<ToolBarComponent>(this, "工具栏组件");
            // 初始化组件
            _componentBox.Init();
        }

        /// <summary>
        /// 处理按键按下
        /// </summary>
        public void HandleKeyDown(KeyEventArgs e)
        {
            _interactionComponent.HandleKeyDown(e);
        }

        /// <summary>
        /// 处理文本输入
        /// </summary>
        public void HandleTextInput(string text)
        {
            _ibeamComponent.StopBlinkIBeam();
            _pageComponent.HandleTextInput(text);
            _ibeamComponent.StartBlinkIBeam();
        }

        #endregion

        #region 核心接口

        /// <summary>
        /// 加载文档
        /// </summary>
        public void LoadDocument(Document document)
        {
            // 加载示例文档
            // _documentComponent.LoadDemoDocument();
            // 加载文档
            _documentComponent.LoadDocument(document);
            // 初始化编辑系统
            InitEditSystem();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 初始化编辑系统
        /// </summary>
        private void InitEditSystem()
        {
            // 初始化光标
            _pageComponent.InitIBeam();
            // 启用交互组件
            _interactionComponent.ReqEnable();
            // 启用滚动条组件
            _scrollBarComponent.ReqEnable();
        }

        #endregion

        #region 字段

        private readonly ComponentBox<Editer> _componentBox = new ComponentBox<Editer>();

        private DocumentComponent _documentComponent;
        private IBeamComponent _ibeamComponent;
        private InteractionComponent _interactionComponent;
        private PageComponent _pageComponent;
        private ScrollBarComponent _scrollBarComponent;
        private SelectComponent _selectComponent;
        private ToolBarComponent _toolBarComponent;

        #endregion
    }
}