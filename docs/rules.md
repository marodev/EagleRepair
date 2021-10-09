# Rules

EagleRepair currently aims to fix static analysis violations reported by SonarQube and ReSharper.

## SonarQube

|  ID       |  Squid   | SonarQube Description |
| :---------: |:--------- |:-------------|
R1 | [3881](https://rules.sonarsource.com/csharp/RSPEC-3881) | "IDisposable" should be implemented correctly | 
R3 | [4201](https://rules.sonarsource.com/csharp/RSPEC-4201) | Null checks should not be used with "is" |
R4 | [2971](https://rules.sonarsource.com/csharp/RSPEC-2971) | "IEnumerable" LINQs should be simplified |
R5 | [3247](https://rules.sonarsource.com/csharp/RSPEC-3247) | Duplicate casts should not be made |
R6 | [1155](https://rules.sonarsource.com/csharp/RSPEC-1155) | "Any()" should be used to test for emptiness |
R10 | [3256](https://rules.sonarsource.com/csharp/RSPEC-3256) | "string.IsNullOrEmpty" should be used |

## ReSharper

| ID    | TypeId  | ReSharper Description |
| :---------------: |:--------- |:-------------|
R2 | [MergeSequentialChecks](https://www.jetbrains.com/help/resharper/MergeSequentialChecks.html) | Merge sequential checks in && or \|\| expressions |
R3 | [MergeSequentialChecks](https://www.jetbrains.com/help/resharper/MergeSequentialChecks.html) | Merge sequential checks in && or \|\| expressions |
R4 | ReplaceWithSingleCallTo |         Any, Count, FirstOrDefault, SingleOrDefault, First, Single, Last |
R5 | [MergeCastWithTypeCheck](https://www.jetbrains.com/help/resharper/MergeCastWithTypeCheck.html) | Type check and casts can be merged |
R6 | UseMethodAny | Use ".Any()" to test whether this IEnumerable is empty or not |
R7 | [UseNullPropagation](https://www.jetbrains.com/help/resharper/UseNullPropagation.html) | Replace if statement with null-propagating code |
R8 | [UsePatternMatching](https://www.jetbrains.com/help/resharper/UsePatternMatching.html) | Convert "as" expression type check and the following null check into pattern matching |
R9 | [UseStringInterpolation](https://www.jetbrains.com/help/resharper/UseStringInterpolation.html) | Use string interpolation expression |
R10 | [ReplaceWithStringIsNullOrEmpty](https://www.jetbrains.com/help/resharper/ReplaceWithStringIsNullOrEmpty.html) | Use "String.IsNullOrEmpty" |
R11 | [UseCountProperty](https://www.jetbrains.com/help/resharper/UseCollectionCountProperty.html) - *Coming soon* | Use collection’s count property | 