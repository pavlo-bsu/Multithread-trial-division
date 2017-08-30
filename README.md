The program is meant for perform multithread integer factorization by trial division algorithm (for general approach to singlethread trial division see https://en.wikipedia.org/wiki/Trial_division).
System.Threading.Tasks.Task is using to perform multithreading.
=================
The program is meant for perform multithread integer factorization by trial division algorithm (for general approach to singlethread trial division see <https://en.wikipedia.org/wiki/Trial_division>).

`System.Threading.Tasks.Task` is using to perform multithreading.

The program algorithm is implemented in a class `TrialDivisionMT` (TrialDivisionMT.cs) and is listed below:

1. Create `tasksCount` `System.Threading.Tasks.Task`. Each `Task` carries out trial division at segment of length `segmentLength`.
2. After the completion of all `Tasks`, output the found divisors to the Console and a file.
3. Repeat the steps 2 and 3 while searching divisor is less than `finishPosition (finishPosition=dividend^0.5)`.
4. Evaluate and output the divisors greater than the `finishPosition`. Each divisor is calculated by division of the dividend by divisor found in steps 1 to 3.

The dividend and the divisors are stored at `System.Numerics.BigInteger` variables. To calculate a square root of a `BigInteger` variable, use the method specified in <https://stackoverflow.com/questions/3432412/calculate-square-root-of-a-biginteger-system-numerics-biginteger>. The method is implemented in a class 'BigIntegerExtension' (BigIntegerExtension.cs) by extension method.
