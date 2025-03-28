namespace CoCoOnline.Uitles
{
    public class ReturnData<T>
    {
        public int IntResult { get; set; }
        public string strMessage { get; set; }

        public T ResultData { get; set; }

        public ReturnData(int intResult, T result = default)
        {
            IntResult = intResult;
            ResultData = result;
        }
    }
}
