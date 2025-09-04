namespace GeekDocument.SubSystem.CodeAnalyzeSystem.Define
{
    public class 标识符与关键字识别器 : 识别器
    {
        public override Token? 识别(string 语句, int startIndex)
        {
            // 如果第一个字符不是字母也不是“@”，说明不是标识符或关键字
            if (!char.IsLetter(语句[startIndex]) && 语句[startIndex] != '@') return null;

            Token result = new Token();
            int index = startIndex;
            int state = 0;
            bool accepted = false;

            while (true)
            {
                if (accepted || index == 语句.Length) break;
                switch (state)
                {
                    case 0:
                        // 如果是字母或“@”，进入状态“1”
                        if (char.IsLetter(语句[index]) || 语句[index] == '@')
                        {
                            result.Value += 语句[index];
                            index++;
                            state = 1;
                        }
                        break;
                    case 1:
                        // 字母、数字、下划线，继续留在状态“1”
                        if (char.IsLetter(语句[index]) || char.IsDigit(语句[index]) || 语句[index] == '_')
                        {
                            result.Value += 语句[index];
                            index++;
                        }
                        // 否则，进入状态“2”
                        else state = 2;
                        break;
                    case 2:
                        // 接受状态，识别完成
                        accepted = true;
                        break;
                }
            }

            if (关键字.Contains(result.Value)) result.Type = "关键字";
            else if (类型关键字.Contains(result.Value)) result.Type = "类型关键字";
            else if (控制流关键字.Contains(result.Value)) result.Type = "控制流关键字";
            else result.Type = "标识符";
            return result;
        }

        private readonly List<string> 关键字 = "abstract,as,base,checked,class,const,default,delegate,enum,event,explicit,extern,false,fixed,implicit,in,interface,internal,is,lock,namespace,new,null,operator,out,override,params,private,protected,public,readonly,ref,sealed,sizeof,stackalloc,static,struct,this,true,typeof,unchecked,unsafe,using,virtual,void,volatile".Split(',').ToList();
        private readonly List<string> 类型关键字 = "bool,byte,char,decimal,double,float,int,long,object,sbyte,short,string,uint,ulong,ushort".Split(',').ToList();
        private readonly List<string> 控制流关键字 = "if,else,switch,case,for,foreach,while,do,break,continue,goto,return,throw,try,catch,finally".Split(',').ToList();
    }
}