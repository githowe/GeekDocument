using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekDocument.SubSystem.EditerSystem.Define.BlockDerive
{
    /// <summary>
    /// 图片块
    /// </summary>
    public class BlockImage : Block
    {
        public BlockImage()
        {
            Type = BlockType.Image;
        }

        public override void LoadJson(string json)
        {
            
        }

        public override string ToJson()
        {
            return "";
        }

        public override void UpdateViewData(int blockWidth)
        {
            
        }
    }
}