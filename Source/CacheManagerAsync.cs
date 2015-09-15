using System;
using log4net;
using System.Threading;

namespace Tyresales.Web.Sitefinity.Service.Caching
{
    public class CacheManagerAsync
    {
        private const int READ_LOCK_WAIT_SECONDS = 120;

        private const int WRITE_LOCK_WAIT_SECONDS = 120;

        private static ReaderWriterLock _lock = new ReaderWriterLock();

        public static T Get<T>(string key, Func<T> callback)
        {
            T _value = default(T);

            try
            {
                var log = LogManager.GetLogger(typeof(CacheManagerAsync));

                // Have to try catch as a timeout on the reader lock throws an unhandled application error.
                try
                {
                    _lock.AcquireReaderLock(new TimeSpan(0, 0, READ_LOCK_WAIT_SECONDS));
                }
                catch (ApplicationException e)
                {
                    log.Debug(string.Format("CACHE ACQUIRE READ LOCK TIMEOUT EXCEPTION: key({0}), read lock wait seconds {1}.", key, READ_LOCK_WAIT_SECONDS));
                }

                if (_lock.IsReaderLockHeld)
                {
                    _value = CacheFactory.GetInstance.Get<T>(key);

                    if (_value == null)
                    {
                        //log.Debug("CACHE PRE INSERT LOCK: " + key);

                        // Have to try catch as a timeout on the reader lock throws an unhandled application error.
                        try
                        {
                            _lock.UpgradeToWriterLock(new TimeSpan(0, 0, WRITE_LOCK_WAIT_SECONDS));
                        }
                        catch (ApplicationException e)
                        {
                            log.Debug(string.Format("CACHE ACQUIRE WRITE LOCK TIMEOUT EXCEPTION: key({0}), read lock wait seconds {1}.", key, WRITE_LOCK_WAIT_SECONDS));
                        }

                        if (_lock.IsWriterLockHeld)
                        {
                            // check if another thread populated the value while this thread waited for the write lock
                            _value = CacheFactory.GetInstance.Get<T>(key);
                            if (_value == null)
                            {
                                //log.Debug("CACHE INSERT : " + key);

                                _value = callback();
                                CacheFactory.GetInstance.Insert(key, _value);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (_lock.IsReaderLockHeld)
                {
                    _lock.ReleaseReaderLock();
                }

                if (_lock.IsWriterLockHeld)
                {
                    _lock.ReleaseWriterLock();
                }
            }

            return _value;
        }
    }
}
