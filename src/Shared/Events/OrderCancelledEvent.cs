using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Events
{
    public record OrderCancelledEvent(Guid OrderId, DateTime CancelledAt);

}
