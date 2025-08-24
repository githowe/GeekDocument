using XLogic.Base;

namespace GeekDocument.SubSystem.EventSystem
{
    public class EM : EM<EventType>
    {
        #region 单例

        private EM() { }
        public static EM Instance { get; } = new EM();

        #endregion

        public void Invoke(EventType eventType)
        {
            InnerInvoke(eventType);
        }

        public void Invoke<T1>(EventType eventType, T1 value1)
        {
            InnerInvoke(eventType, value1);
        }
    }
}