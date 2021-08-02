using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Framework
{
    public class FilterOperatorNormalizer
    {
        public static string NormalizeFilterOperator(FilterOperator enumValue)
        {
            string operatorString;
            switch (enumValue)
            {
                case FilterOperator.Equal:
                    operatorString = "=";
                    break;
                case FilterOperator.GreaterThan:
                    operatorString = ">";
                    break;
                case FilterOperator.LessThan:
                    operatorString = "<";
                    break;
                case FilterOperator.GreaterThanOrEqual:
                    operatorString = ">=";
                    break;
                case FilterOperator.LessThanOrEqual:
                    operatorString = "<=";
                    break;
                case FilterOperator.NotEqual:
                    operatorString = "!=";
                    break;
                default:
                    operatorString = null;
                    break;
            }
            return operatorString;
        }
    }
}
