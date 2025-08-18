using GeekDocument.SubSystem.ArchiveSystem;
using GeekDocument.SubSystem.ArchiveSystem.Define;
using GeekDocument.SubSystem.CacheSystem;
using GeekDocument.SubSystem.CacheSystem.Define;
using GeekDocument.SubSystem.DocumentSystem;
using GeekDocument.SubSystem.EditerSystem.Core;
using GeekDocument.SubSystem.EditerSystem.Define;
using GeekDocument.SubSystem.FileSystem;
using GeekDocument.SubSystem.OptionSystem;
using GeekDocument.SubSystem.ResourceSystem;
using GeekDocument.SubSystem.WindowSystem;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using XLogic.Wpf.Window;
using XLogic.WpfControl;

using TabContrlItem = System.Windows.Controls.TabItem;

namespace GeekDocument
{
    public partial class MainWindow : XMainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        #region 生命周期

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WM.Main = this;

            // 恢复窗口状态并监听窗口缩放
            RecoverWindowState();
            ListenWindowResize();
            // 恢复面板状态并监听面板缩放
            RecoverPanelState();
            ListenPanelResize();

            InitToolBar();
            InitTabBar();

            Panel_DocLib.Init();
            Panel_DocLib.LoadDocumentLib();

            KeyDown += MainWindow_KeyDown;
            TextInput += MainWindow_TextInput;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // 处理系统按键
            HandleSystemKey(e);
            if (e.Handled) return;
            // 处理编辑器按键
            HandleEditerKey(e);
        }

        private void MainWindow_TextInput(object sender, TextCompositionEventArgs e)
        {
            // 无编辑器，忽略
            if (TabControl_Doc.Items.Count == 0) return;
            // 获取当前选中的选项卡
            if (TabControl_Doc.SelectedItem is TabContrlItem selectedItem)
                if (selectedItem.Content is Editer editer)
                    editer.HandleTextInput(e.Text);
        }

        #endregion

        #region 公开方法

        public void OpenDocument(string filePath)
        {
            // 读取文件内容
            byte[] fileData = File.ReadAllBytes(filePath);
            // 创建存档文件并加载文件内容
            ArchiveFile archiveFile = new ArchiveFile();
            try
            {
                archiveFile.LoadArchive(fileData);
            }
            catch (Exception)
            {
                WM.ShowErrorTip("加载文件内容失败");
                return;
            }
            // 创建文档实例并加载存档
            Document document = new Document();
            try
            {
                document.LoadArchive(archiveFile);
            }
            catch (Exception)
            {
                WM.ShowErrorTip("加载存档失败");
                return;
            }
            // 打开文档
            OpenDocument(document, Path.GetFileNameWithoutExtension(filePath));
            // 添加打开记录
            CacheManager.Instance.Cache.DocumentManager.AddRecentDocument(filePath);
            CacheManager.Instance.SaveCache();
            // 添加已打开的文档
            DocManager.Instance.AddOpenedDocument(document, filePath);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 恢复窗口状态
        /// </summary>
        private void RecoverWindowState()
        {
            WindowState = CacheManager.Instance.Cache.MainWindow.State;
            Width = CacheManager.Instance.Cache.MainWindow.Width;
            Height = CacheManager.Instance.Cache.MainWindow.Height;
            // 居中窗口
            Left = (SystemParameters.WorkArea.Width - Width) / 2;
            Top = (SystemParameters.WorkArea.Height - Height) / 2;
        }

        /// <summary>
        /// 监听窗口缩放
        /// </summary>
        private void ListenWindowResize()
        {
            StateChanged += (s, e) =>
            {
                if (WindowState is WindowState.Normal or WindowState.Maximized)
                {
                    CacheManager.Instance.Cache.MainWindow.State = WindowState;
                    CacheManager.Instance.SaveCache();
                }
            };
            SizeChanged += (s, e) =>
            {
                if (WindowState == WindowState.Maximized) return;
                CacheManager.Instance.Cache.MainWindow.Width = (int)Width;
                CacheManager.Instance.Cache.MainWindow.Height = (int)Height;
                CacheManager.Instance.SaveCache();
            };
        }

        /// <summary>
        /// 恢复面板状态
        /// </summary>
        private void RecoverPanelState()
        {
            // 获取缓存
            MainWindowCache cache = CacheManager.Instance.Cache.MainWindow;
            // 显隐面板
            LeftPanel.Visibility = cache.LeftPanelHided ? Visibility.Collapsed : Visibility.Visible;
            RightPanel.Visibility = cache.RightPanelHided ? Visibility.Collapsed : Visibility.Visible;
            // 显隐区域
            LeftArea.MinWidth = cache.LeftPanelHided ? 0 : 300;
            LeftArea.Width = cache.LeftPanelHided ? _zeroLength : new GridLength(cache.LeftPanelWidth);
            RightArea.MinWidth = cache.RightPanelHided ? 0 : 300;
            RightArea.Width = cache.RightPanelHided ? _zeroLength : new GridLength(cache.RightPanelWidth);
            // 显隐分割条
            LeftSplit.Width = cache.LeftPanelHided ? _zeroLength : _splitLength;
            RightSplit.Width = cache.RightPanelHided ? _zeroLength : _splitLength;
            // 立即更新布局
            UpdateLayout();
        }

        /// <summary>
        /// 监听面板缩放
        /// </summary>
        private void ListenPanelResize()
        {
            Splitter_Left.LostMouseCapture += (s, e) =>
            {
                CacheManager.Instance.Cache.MainWindow.LeftPanelWidth = (int)LeftPanel.ActualWidth;
                CacheManager.Instance.SaveCache();
            };
        }

        private void FoldRight_Click(object sender, RoutedEventArgs e)
        {
            MainWindowCache cache = CacheManager.Instance.Cache.MainWindow;

            // 切换面板显隐
            RightPanel.Visibility = RightPanel.IsVisible ? Visibility.Collapsed : Visibility.Visible;
            bool panelVisible = RightPanel.IsVisible;
            // 切换区域显隐
            RightArea.MinWidth = panelVisible ? 300 : 0;
            RightArea.Width = panelVisible ? new GridLength(cache.RightPanelWidth) : _zeroLength;
            // 切换分割线显隐
            RightSplit.Width = panelVisible ? _splitLength : _zeroLength;

            cache.RightPanelHided = !panelVisible;
            CacheManager.Instance.SaveCache();
        }

        /// <summary>
        /// 初始化工具栏
        /// </summary>
        private void InitToolBar()
        {
            if (GetTemplateChild("TopToolBar") is ToolBar bar)
            {
                bar.ToolStyle = (Style)FindResource("ToolBarButton");
                // bar.AddSplit(new Thickness(0, 5, 5, 5));
                bar.AddTool(GetToolIcon("Lib"), "DocLib", "文档库");
                bar.AddSplit();
                bar.AddTool(GetToolIcon("NewFile"), "NewFile", "新建文档");
                bar.AddTool(GetToolIcon("OpenFile"), "OpenFile", "打开文档");
                bar.AddTool(GetToolIcon("SaveAll"), "SaveAll", "保存全部");
                bar.AddSplit();
                // 监听工具栏
                bar.ToolClick += ToolBar_ToolClick;
            }
        }

        /// <summary>
        /// 初始化标签栏
        /// </summary>
        private void InitTabBar()
        {
            if (GetTemplateChild("TopTabBar") is TabBar tabBar)
            {
                _tabBar = tabBar;
                _tabBar.SelectChanged = TabBar_SelectChanged;
            }
        }

        private ImageSource? GetToolIcon(string name) => ImageResManager.Instance.GetIcon16($"{name}.png");

        /// <summary>
        /// 工具栏.单击工具
        /// </summary>
        private void ToolBar_ToolClick(string name)
        {
            switch (name)
            {
                // 文档库
                case "DocLib":
                    SwitchDocumentLib();
                    break;
                // 新建文档
                case "NewFile":
                    NewDocument();
                    break;
                // 打开文档
                case "OpenFile":
                    OpenDocument();
                    break;
                case "SaveAll":
                    break;
            }
        }

        /// <summary>
        /// 切换文档库显隐
        /// </summary>
        private void SwitchDocumentLib()
        {
            MainWindowCache cache = CacheManager.Instance.Cache.MainWindow;

            // 切换面板显隐
            LeftPanel.Visibility = LeftPanel.IsVisible ? Visibility.Collapsed : Visibility.Visible;
            bool panelVisible = LeftPanel.IsVisible;
            // 切换区域显隐
            LeftArea.MinWidth = panelVisible ? 300 : 0;
            LeftArea.Width = panelVisible ? new GridLength(cache.LeftPanelWidth) : _zeroLength;
            // 切换分割线显隐
            LeftSplit.Width = panelVisible ? _splitLength : _zeroLength;

            cache.LeftPanelHided = !panelVisible;
            CacheManager.Instance.SaveCache();
        }

        /// <summary>
        /// 处理系统按键：新建、打开、保存全部
        /// </summary>
        private void HandleSystemKey(KeyEventArgs e)
        {
            // 按下了“Ctrl”
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.N)
                {
                    e.Handled = true;
                    NewDocument();
                }
                else if (e.Key == Key.O)
                {
                    e.Handled = true;
                    OpenDocument();
                }
            }
            // 按下了“Ctrl + Shift”
            else if (Keyboard.Modifiers == (ModifierKeys.Shift | ModifierKeys.Control))
            {
                if (e.Key == Key.S)
                {
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// 处理编辑器按键：删除、回车、复制、粘贴、剪切、撤销、重做等
        /// </summary>
        private void HandleEditerKey(KeyEventArgs e)
        {
            // 无编辑器，忽略按键
            if (TabControl_Doc.Items.Count == 0) return;
            // 获取当前选中的选项卡
            if (TabControl_Doc.SelectedItem is TabContrlItem selectedItem)
                if (selectedItem.Content is Editer editer)
                    editer.HandleKeyDown(e);
        }

        /// <summary>
        /// 新建文档
        /// </summary>
        private void NewDocument()
        {
            // 打开新建文档对话框
            NewDocumentDialog dialog = new NewDocumentDialog { Owner = this };
            if (dialog.ShowDialog() == true)
            {
                // 新建文档实例
                Document document = new Document
                {
                    PageWidth = Options.Instance.Page.PageWidth,
                    Padding = Options.Instance.Page.PageMargin,
                };
                // 添加标题块
                BlockText title = new BlockText
                {
                    Content = dialog.DocumentName,
                    FontSize = 32,
                };
                title.UpdateViewData(document.PageWidth);
                document.BlockList.Add(title);
                // 在磁盘中新建文件
                FileStream fileStream = File.Create($"{dialog.DocumentPath}\\{dialog.DocumentName}.gdoc");
                // 刷新文档库
                Panel_DocLib.RefreshDocumentLib();
                // 保存文档数据至文件
                byte[] archiveData = ArchiveManager.Instance.GenerateArchiveData(document);
                fileStream.Write(archiveData, 0, archiveData.Length);
                fileStream.Close();
                // 打开新建的文档
                OpenDocument(document, dialog.DocumentName);
                // 添加打开记录
                CacheManager.Instance.Cache.DocumentManager.AddRecentDocument($"{dialog.DocumentPath}\\{dialog.DocumentName}.gdoc");
                CacheManager.Instance.SaveCache();
                // 添加已打开的文档
                DocManager.Instance.AddOpenedDocument(document, $"{dialog.DocumentPath}\\{dialog.DocumentName}.gdoc");
            }
        }

        /// <summary>
        /// 打开文档
        /// </summary>
        private void OpenDocument()
        {
            List<string> pathList = FM.Instance.OpenReadDocumentDialog();
            if (pathList.Count == 0) return;

            foreach (var path in pathList)
            {
                if (DocManager.Instance.DocumentOpened(path))
                {
                    WM.ShowErrorTip($"文档“{Path.GetFileName(path)}”已打开");
                    continue;
                }
                OpenDocument(path);
            }
        }

        /// <summary>
        /// 打开文档
        /// </summary>
        private void OpenDocument(Document document, string docName)
        {
            // 关闭主页并打开编辑器页
            if (Control_Home.Visibility == Visibility.Visible)
            {
                Control_Home.Visibility = Visibility.Collapsed;
                Grid_Editer.Visibility = Visibility.Visible;
            }
            // 新建一个选项卡用于承载编辑器
            TabContrlItem editerItem = new TabContrlItem();
            TabControl_Doc.Items.Add(editerItem);
            // 新建一个编辑器并添加至选项卡
            Editer editer = new Editer();
            editer.Init();
            editerItem.Content = editer;
            // 新建选项卡标签并关联至选项卡
            TabItem tabItem = _tabBar.AddTabItem(docName);
            tabItem.ItemInstance = editerItem;
            // 选择新建的选项卡标签
            _tabBar.UpdateSelect(tabItem);
            // 选择选项卡
            editerItem.IsSelected = true;
            // 加载文档
            editer.LoadDocument(document);
        }

        #endregion

        #region 控件事件

        private void TabBar_SelectChanged(TabItem? tabItem)
        {
            if (tabItem == null) return;

            if (tabItem.ItemInstance != null)
                tabItem.ItemInstance.IsSelected = true;
        }

        #endregion

        #region 字段

        private TabBar _tabBar;

        private GridLength _zeroLength = new GridLength(0);
        private GridLength _splitLength = new GridLength(2);

        #endregion
    }
}