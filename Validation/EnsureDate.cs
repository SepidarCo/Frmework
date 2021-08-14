using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System;

namespace Sepidar.Validation
{
    public class EnsureDate
    {
        DateTime? date;

        public EnsureDate(DateTime? date)
        {
            this.date = date;
        }

        public EnsureDate IsAfterThan(DateTime date)
        {
            if (this.date <= date)
            {
                throw new FrameworkException("Ensure validation: '{0}' should be after '{1}'.".Fill(this.date.ToString(), date.ToString()));
            }
            return this;
        }

        public EnsureDate And()
        {
            return this;
        }
    }
}