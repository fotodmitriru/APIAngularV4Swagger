using System.ComponentModel.DataAnnotations;

namespace APIAngular.Database
{
    /// <summary>
    /// Атрибут для проверки минимального и максимального значений. Значения можно задавать по отдельности, или вместе.
    /// </summary>
    public class RangeValueAttribute : ValidationAttribute
    {
        private readonly int _minValue;
        private readonly int _maxValue;
        private readonly int _defaultValue;

        public RangeValueAttribute(int minValue = 0, int maxValue = 0, int defaultValue = -1)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _defaultValue = defaultValue;
        }
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                if ((int) value == _defaultValue)
                    return true;
                if ((_minValue > 0) & (_maxValue == 0))
                    if ((int) value >= _minValue)
                        return true;
                if ((_minValue == 0) & (_maxValue > 0))
                    if ((int)value <= _maxValue)
                        return true;
                if ((_minValue > 0) & (_maxValue > 0))
                    if (((int)value >= _minValue) & ((int)value <= _maxValue))
                        return true;
            }
            return false;
        }
    }
}