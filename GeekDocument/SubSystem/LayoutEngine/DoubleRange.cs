namespace GeekDocument.SubSystem.LayoutEngine
{
    public class DoubleRange
    {
        public double Start { get; set; } = double.NaN;

        public double End { get; set; } = double.NaN;

        public bool Contains(double value)
        {
            if (double.IsNaN(Start) || double.IsNaN(End)) return false;
            return value >= Start && value < End;
        }
    }
}