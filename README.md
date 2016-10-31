# Elmah.Io.Trace

[![NuGet](https://img.shields.io/nuget/vpre/Elmah.Io.Trace.svg)](https://www.nuget.org/packages/Elmah.Io.Trace)

Log to elmah.io from System.Diagnostics.Trace and friends.

## Installation
Elmah.Io.Trace installs through NuGet:

```
PS> Install-Package Elmah.Io.Trace
```

Add the elmah.io TraceListener to `Trace`:

```csharp
System.Diagnostics.Trace.Listeners.Add(new ElmahIoTraceListener(new Guid("LOG_ID")));
```

(replace `LOG_ID` with your log id)

## Usage
Log messages to elmah.io, just as with every other trace listener:

```csharp
System.Diagnostics.Trace.WriteLine("A trace message");
System.Diagnostics.Trace.TraceError("Hello {0}", "World");
```