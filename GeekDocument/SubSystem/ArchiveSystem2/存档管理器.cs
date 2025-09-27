using GeekDocument.SubSystem.EditerSystem3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekDocument.SubSystem.ArchiveSystem2
{
    public class 存档管理器
    {
        #region 单例

        private 存档管理器() { }
        public static 存档管理器 Instance { get; } = new 存档管理器();

        #endregion

        #region 公开方法

        public byte[] 生成存档数据(文档 文档)
        {
            return Array.Empty<byte>();
        }

        #endregion
    }
}