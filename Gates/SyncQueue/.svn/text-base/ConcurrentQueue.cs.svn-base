using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace Gates.SyncQueue
{
    // See: http://msdn.microsoft.com/en-us/library/yy12yx1f(VS.80).aspx

    /// <summary>
    /// Thread-safe producer/consumer queue.  This is a blocking queue
    /// for the consumer.  Think java.util.concurrent.BlockingQueue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class ConcurrentQueue<T> 
    {
        private Queue<T> _queue;
        private SyncEvents _syncEvents;
        private bool isWaiting = false;

        public ConcurrentQueue()
        {
            _syncEvents = new SyncEvents();
            _queue = new Queue<T>();
        }

        /// <summary>
        /// Remove all items from the queue
        /// </summary>
        public void Clear()
        {
            lock (((ICollection)_queue).SyncRoot)
            {
                _queue.Clear();
            }
        }

        /// <summary>
        /// Add an item to the end of the queue
        /// </summary>
        /// <param name="item">Item to add</param>
        public void Enqueue(T item) {
            lock (((ICollection)_queue).SyncRoot)
            {
                _queue.Enqueue(item);
                _syncEvents.NewItemEvent.Set();        
            }   
        }

        /// <summary>
        /// Retrieve the first item on the queue.
        /// If there is no item on the queue, this method will wait until an item
        /// has been placed on the queue.
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            
            T item;
            while (!HasItems())
            {
                lock (((ICollection)_queue).SyncRoot)
                {
                    isWaiting = true;
                }
                WaitHandle.WaitAny(_syncEvents.EventArray);
                lock (((ICollection)_queue).SyncRoot)
                {
                    isWaiting = false;
                }
            }
            
            lock (((ICollection)_queue).SyncRoot)
            {
                item = _queue.Dequeue();
            }
            return item;
            
            
            
            
        }

        /// <summary>
        /// Indicates if a thread is waiting on the
        /// dequeue method.
        /// </summary>
        /// <returns></returns>
        public bool IsWaiting()
        {
            bool res;
            lock (((ICollection)_queue).SyncRoot)
            {
                res = isWaiting;
            }
            return res;
        }
        
        /// <summary>
        /// Determine if the queue has any items.
        /// </summary>
        /// <returns></returns>
        public bool HasItems()
        {
            bool items = false;
            lock (((ICollection)_queue).SyncRoot)
            {
                items = _queue.Count > 0;
            }
            return items;
        }
        
        
        

    }
}
