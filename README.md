[![.NET](https://github.com/code-dispenser/Flow-Validated/actions/workflows/buildandtest.yml/badge.svg?branch=main)](https://github.com/code-dispenser/Flow-Validated/actions/workflows/buildandtest.yml) [![Coverage Status](https://coveralls.io/repos/github/code-dispenser/Flow-Validated/badge.svg?branch=main)](https://coveralls.io/github/code-dispenser/Flow?branch=main) 


<h1>
<img src="https://raw.github.com/code-dispenser/Flow-Validated/main/assets/icon-64.png" align="center" alt="flow icon" /> Flow.Validated
</h1>
<!--
# ![icon](https://raw.githubusercontent.com/code-dispenser/Flow/main/Assets/icon-64.png) Flow.Validated 
-->
<!-- H1 for git hub, but for nuget the markdown is fine as it centers the image, uncomment as appropriate and do the same at the bottom of this file for the icon author -->

## Overview
This library provides simple extension methods for converting Validated<T> results into Flow<T> results, making it easier to integrate validation outcomes with flow returns.

## Getting started

Add the Flow.Validated nuget package to your project using Nuget Package Manager or the dotnet CLI:

```csharp
dotnet add package Flow.Validated
```
Add the using statement `using Validated.Core.Extensions.` to access the `ToFlow<T>` extension.


## Usage
Currently there is a single `ToFlow<T>` extension method with an async overload that attaches to a ``Validated<T>`` returning a `Flow<T>`

If the `Validated<T>` is invalid it creates a failed `Flow<T>` using the `Failure.InvalidEntryFailure` type, copying the list of `InvalidEntry` into the failure.
**Note:** If any of the invalid entries have a Cause that is not set to *Validation*, such as *SystemError* or *RuleConfigError* then the CanRetry property is set to false otherwise it is set to true 