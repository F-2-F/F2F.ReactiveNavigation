using Microsoft.Reactive.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.Testing;

namespace F2F.ReactiveNavigation.UnitTests
{
    public static class TestSchedulerExtensions
    {
        public static void Advance(this TestScheduler scheduler)
        {
            scheduler.AdvanceByMs(1);
        }

        public static Task Schedule(this Task task, TestScheduler scheduler)
        {
            scheduler.Advance();
            return task;
        }
    }
}
