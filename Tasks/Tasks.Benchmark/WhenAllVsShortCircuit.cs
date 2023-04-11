using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Benchmark
{
    [MemoryDiagnoser]
    public class WhenAllVsShortCircuit
    {
        private readonly int iterations = 1_000;
        private readonly int delayMilliseconds = 10;
        private Task[] _tasks;

        [GlobalSetup]
        public void Setup()
        {
            _tasks = new Task[iterations];
            for (int i = 0; i < iterations; i++)
            {
                _tasks[i] = Task.Delay(delayMilliseconds);
            }
        }

        [Benchmark(Baseline = true)]
        public async Task AwaitSerially()
        {
            for (int i = 0; i < _tasks.Length; i++)
            {
                await _tasks[i];
            }
        }

        [Benchmark]
        public async Task WhenAll()
        {
            await Task.WhenAll(_tasks);
        }

        [Benchmark]
        public async Task ShortCircuit()
        {
            for (int i = 0; i < _tasks.Length; i++)
            {
                var task = _tasks[i];
                if (task.IsCompletedSuccessfully)
                    continue;
                await task;
            }
        }
    }
}
