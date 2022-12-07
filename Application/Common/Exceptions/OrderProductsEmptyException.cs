using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Exceptions {

    public class OrderProductsEmptyException : Exception {
        public OrderProductsEmptyException() {
        }

        public OrderProductsEmptyException(string? message) : base(message) {
        }

        public OrderProductsEmptyException(string? message, Exception? innerException) : base(message, innerException) {
        }

        protected OrderProductsEmptyException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}
