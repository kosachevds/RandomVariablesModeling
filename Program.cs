﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandomVariablesModeling
{
    class Program
    {
        const string OutPath = "./values.txt";
        const int ValuesCount = 1_000_000;
        static void Main(string[] args)
        {
            SetInvariantCulture();

            // AnalyzeGenerator(new Generator());
            // AnalyzeGenerator(new MaskGenerator(0xFF));

            SumRandomVariables();

            Console.WriteLine("Done!");
        }

        static void SumRandomVariables() {
            const int SequenceSize = 1000;
            const int SequencesCount = 12;
            List<double> sequencesSum = null;
            var generator = new RandomSequenceGenerator();
            Console.WriteLine("Generating...");
            for (int i = 0; i < SequencesCount; ++i) {
                var newSequence = generator.GenerateSequence(SequenceSize);
                if (sequencesSum == null) {
                    sequencesSum = newSequence.ToList();
                } else {
                    sequencesSum = newSequence
                        .Select((item, index) => item + sequencesSum[index])
                        .ToList();
                }
            }
            WriteSequenceAsync(sequencesSum).Wait();
        }

        static void SetInvariantCulture() {
            System.Threading.Thread.CurrentThread.CurrentCulture =
                System.Globalization.CultureInfo.InvariantCulture;
        }

        static void AnalyzeGenerator(Generator generator) {
            Console.WriteLine("Generating...");
            var values = Enumerable.Range(0, ValuesCount)
                .Select(_ => generator.GenerateValue())
                .ToList();
            var writingTask = WriteSequenceAsync(values);
            Console.WriteLine("Period = ");
            Console.WriteLine("{0}", GetPeriodSize(generator));
            writingTask.Wait();
        }

        static async Task WriteSequenceAsync<TValue>(IEnumerable<TValue> values) {
            Console.WriteLine("Writing...");
            if (values.Count() > 1e6) {
                await System.IO.File.WriteAllLinesAsync(OutPath, values.Select(x => x.ToString() + ";"));
                // Console.WriteLine(stringValues);
            } else {
                var stringValues = String.Join(";\n", values);
                await System.IO.File.WriteAllTextAsync(OutPath, stringValues);
            }
        }

        static ulong GetPeriodSize(Generator generator) {
            var firstValue = generator.GenerateValue();
            ulong periodSize = 1;
            while (generator.GenerateValue() != firstValue) {
                try {
                    periodSize = checked(periodSize + 1);
                } catch (OverflowException) {
                    return 0;
                }
            }
            return periodSize;
        }
    }
}
