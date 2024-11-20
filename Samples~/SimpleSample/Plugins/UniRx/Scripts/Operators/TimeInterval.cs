﻿using System;

namespace SampleUnirx.Operators
{
    internal class TimeIntervalObservable<T> : OperatorObservableBase<SampleUnirx.TimeInterval<T>>
    {
        readonly IObservable<T> source;
        readonly IScheduler scheduler;

        public TimeIntervalObservable(IObservable<T> source, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<SampleUnirx.TimeInterval<T>> observer, IDisposable cancel)
        {
            return source.Subscribe(new TimeInterval(this, observer, cancel));
        }

        class TimeInterval : OperatorObserverBase<T, SampleUnirx.TimeInterval<T>>
        {
            readonly TimeIntervalObservable<T> parent;
            DateTimeOffset lastTime;

            public TimeInterval(TimeIntervalObservable<T> parent, IObserver<SampleUnirx.TimeInterval<T>> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
                this.lastTime = parent.scheduler.Now;
            }

            public override void OnNext(T value)
            {
                var now = parent.scheduler.Now;
                var span = now.Subtract(lastTime);
                lastTime = now;

                base.observer.OnNext(new SampleUnirx.TimeInterval<T>(value, span));
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); }
                finally { Dispose(); }
            }
        }
    }
}