﻿using System;

namespace Rebus.Testing
{
    /// <summary>
    /// Allows you to establish a fake message context for the duration of a test.
    /// It should only be necessary to use <see cref="FakeMessageContext"/> in places
    /// where you cannot set up your DI container to automatically inject the current
    /// <see cref="IMessageContext"/>.
    /// In tests where this one is used, you should take care to always call
    /// <see cref="Reset"/> as part of the test teardown logic.
    /// </summary>
    public class FakeMessageContext
    {
        /// <summary>
        /// Attaches the specified (most likely mocked) message context to the current thread,
        /// which will cause <see cref="MessageContext.GetCurrent"/> to return it.
        /// </summary>
        public static IDisposable Establish(IMessageContext messageContext)
        {
            if (messageContext == null) throw new ArgumentNullException("messageContext");

            if (messageContext.Headers == null)
            {
                throw new ArgumentException(string.Format(
                    "Passed-in message context {0} has null as the Headers property! Please make sure"
                    + " that your faked message context has a proper Headers dictionary in place,"
                    + " otherwise stuff would not work.", messageContext));
            }
            
            if (messageContext.Items == null)
            {
                throw new ArgumentException(string.Format(
                    "Passed-in message context {0} has null as the Items property! Please make sure"
                    + " that your faked message context has a proper Items dictionary in place,"
                    + " otherwise stuff would not work.", messageContext));
            }

            MessageContext.current = messageContext;
            
            return new MessageContextResetter(messageContext);
        }

        /// <summary>
        /// Removes any attached message contexts from the current thread.
        /// </summary>
        public static void Reset()
        {
            MessageContext.current = null;
        }

        class MessageContextResetter : IDisposable
        {
            readonly IMessageContext messageContext;
            bool disposed;

            public MessageContextResetter(IMessageContext messageContext)
            {
                this.messageContext = messageContext;
            }

            public void Dispose()
            {
                if (disposed) return;

                if (!ReferenceEquals(messageContext, MessageContext.current))
                {
                    throw new InvalidOperationException("Message context was disposed from another thread than the thread that established it!");
                }

                Reset();

                disposed = true;
            }
        }
    }
}