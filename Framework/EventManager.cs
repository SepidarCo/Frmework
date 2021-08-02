using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepidar.Framework
{
    public class EventManager
    {
        static List<Handler> handlers = new List<Handler>();

        public static void On(string @event, Action<dynamic> handler)
        {
            if (handlers.Where(i => i.Event == @event).Count() > 0)
            {
                // This event has already some handlers. Let's add the new handler to those handlers.
                handlers.Single(i => i.Event == @event).Handlers.Add(handler);
            }
            else
            {
                handlers.Add(new Handler
                {
                    Event = @event,
                    Handlers = new List<Action<dynamic>>
                    {
                        handler
                    }
                });
            }
        }

        public static void Raise(string @event, dynamic parameters)
        {
            Handler handler = handlers.SingleOrDefault(h => h.Event == @event);
            if (handler.IsNull())
            {
                return;
            }
            handler.Handlers.ForEach(h =>
            {
                h(parameters);
            });
        }

        private class Handler
        {
            public string Event { get; set; }

            public List<Action<dynamic>> Handlers { get; set; }
        }
    }
}
