using GeekDocument.SubSystem.OptionSystem;
using GeekDocument.SubSystem.WindowSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLogic.Base.UI;

namespace GeekDocument.SubSystem.EditerSystem.Core.Component
{
    /// <summary>
    /// 编辑器组件
    /// </summary>
    public class EditerComponent : Component<Editer>
    {
        protected override void Init()
        {
            GetComponent<ToolBarComponent>().ToolClick += ToolBar_ToolClick;
        }

        private void ToolBar_ToolClick(string name)
        {
            switch (name)
            {
                case "Tool_Save":
                    break;
                case "Tool_SaveAs":
                    break;
                case "Tool_Export":
                    break;
                case "Tool_Undo":
                    break;
                case "Tool_Redo":
                    break;
                case "Tool_Option":
                    OpenOptionDialog();
                    break;
            }
        }

        /// <summary>
        /// 打开选项对话框
        /// </summary>
        private void OpenOptionDialog()
        {
            DocumentOptionDialog dialog = new DocumentOptionDialog { Owner = WM.Main };
            dialog.Init(GetComponent<DocumentComponent>().Document);
            if (dialog.ShowDialog() == true)
            {
                // 更新文档选项
                // 更新为未保存状态
            }
        }
    }
}