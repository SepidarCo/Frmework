using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Framework
{
    public class ExceptionTranslation : IEquatable<ExceptionTranslation>
    {
        public MessageType Type { get; set; }
        public string ServerMessage { get; set; }
        public string ClientMessage { get; set; }

        public bool Equals(ExceptionTranslation msg)
        {

            //Check whether the compared object is null.
            if (Object.ReferenceEquals(msg, null)) return false;

            //Check whether the compared object references the same data.
            if (Object.ReferenceEquals(this, msg)) return true;

            //Check whether the products' properties are equal.
            return ServerMessage.ToLower().Equals(msg.ServerMessage.ToLower());
        }

        public override int GetHashCode()
        {
            //Get hash code for the Code field.
            int hashServerMessage = ServerMessage.GetHashCode();

            //Calculate the hash code for the product.
            return hashServerMessage;
        }
    }
}
