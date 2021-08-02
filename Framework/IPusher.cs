using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Framework
{
    public interface IPusher
    {
        void Subscribe(string who, List<string> toWhat);

        void Unsubscribe(string who, List<string> fromWhat);

        void Push/*Deliver*/(string what, List<string> who);
    }
}
