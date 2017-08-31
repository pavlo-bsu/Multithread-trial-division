using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Pavlo;

namespace Pavlo.DivisorsSearch
{
    class TrialDivisionMT
    {
        /// <summary>
        /// increment of a testing divisor
        /// =1 (for even)
        /// =2 (for odd)
        /// </summary>
        int increment;

        /// <summary>
        /// Tasks count in each step
        /// </summary>
        int tasksCount;

        /// <summary>
        /// Output file name
        /// </summary>
        public string fileName = "result.txt";

        /// <summary>
        /// The dividend
        /// </summary>
        BigInteger theNumber;

        /// <summary>
        /// Initial number from which the search at each step begins
        /// </summary>
        BigInteger startPosition;

        /// <summary>
        /// The length of the segment at which the divisors are searched (for each task). 
        /// </summary>
        int segmentLength;

        /// <summary>
        /// The search of values is carried out only for numbers less than finishPosition.
        /// </summary>
        BigInteger finishPosition;

        /// <summary>
        /// The divisors which are greater than finishPosition. (Each divisor is evaluated by division of theNumber by searched divisor)
        /// </summary>
        List<BigInteger> lastDivisors = new List<BigInteger>();

        public TrialDivisionMT(BigInteger number, BigInteger startPosition, string fileName)
        {
            this.theNumber = number;
            this.fileName = fileName;
            increment = theNumber.IsEven ? 1 : 2;

            //if increment=2 than startPoint must be odd
            this.startPosition = (startPosition < 1) ? 1 : startPosition;
            if ((increment == 2) && this.startPosition.IsEven)
                this.startPosition += 1;

            finishPosition = theNumber.Sqrt();

            if (Environment.ProcessorCount == 1)
                throw new Exception("Intended for multicore CPU.");
            tasksCount = Environment.ProcessorCount * 2;
            segmentLength = 100000;

            try
            {
                using (FileStream outputFS = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read)) ;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while creating output file!\n" + e.Message);
            }
        }

        /// <summary>
        /// This function performs the calculation.
        /// </summary>
        public void Start()
        {
            TaskFactory<List<BigInteger>> tf = new TaskFactory<List<BigInteger>>(CancellationToken.None, TaskCreationOptions.AttachedToParent, TaskContinuationOptions.AttachedToParent, TaskScheduler.Default);
            Task<List<BigInteger>>[] tasksBunch = new Task<List<BigInteger>>[tasksCount];

            while (startPosition <= finishPosition)
            {
                Console.WriteLine("Processing numbers greater than " + startPosition);
                for (int i = 0; i < tasksCount; i++)
                {
                    tasksBunch[i] = tf.StartNew(o =>
                    {
                        BigInteger val = startPosition + (int)o * segmentLength;
                        return searchInSegment(val);
                    }, i);
                }

                var taskMergeList = tf.ContinueWhenAll(tasksBunch, (tasks =>
                {
                    List<BigInteger> res = new List<BigInteger>();
                    for (int i = 0; i < tasks.Length; i++)
                    {
                        res.AddRange(tasks[i].Result);
                    }
                    res.Sort();
                    return res;
                }));

                taskMergeList.Wait();
                OutputData(taskMergeList.Result);

                startPosition += tasksCount * segmentLength;
            }
            lastDivisors.Sort();
            OutputData(lastDivisors);
        }

        /// <summary>
        /// Trial division at segment of length segmentLength and origin in startPos
        /// </summary>
        /// <param name="startPos">segment origin</param>
        /// <returns>List of searched divisors</returns>
        private List<BigInteger> searchInSegment(BigInteger startPos)
        {
            List<BigInteger> res = new List<BigInteger>();
            if (startPos > finishPosition)
                return res;
            BigInteger finishPos = startPos + segmentLength - 1;
            if (finishPos > finishPosition)
                finishPos = finishPosition;
            for (BigInteger i = startPos; i <= finishPos; i = i + increment)
            {
                if (BigInteger.Remainder(theNumber, i) == 0)
                {
                    res.Add(i);
                    if (i != finishPosition)//Otherwise divisor=finishPosition will appear twice
                        lastDivisors.Add(BigInteger.Divide(theNumber, i));
                }
            }
            return res;
        }

        /// <summary>
        /// output to a file
        /// </summary>
        private void OutputDataToFile(List<BigInteger> data)
        {
            if (data.Count == 0)
                return;
            try
            {
                using (FileStream outputFS = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    using (StreamWriter outputSW = new StreamWriter(outputFS, Encoding.ASCII))
                    {
                        foreach (var number in data)
                            outputSW.WriteLine(number);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while writing output file!\n" + e.Message);
            }
        }
        /// <summary>
        /// output to the Console
        /// </summary>
        private void OutputDataToConsole(List<BigInteger> data)
        {
            foreach (var number in data)
                Console.WriteLine("Found divisor " + number.ToString());
        }
        /// <summary>
        /// output searched divisors
        /// </summary>
        /// <param name="data">searched divisors</param>
        private void OutputData(List<BigInteger> data)
        {
            OutputDataToConsole(data);
            OutputDataToFile(data);
        }
    }

}
