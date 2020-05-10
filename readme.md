# Workshell Tempus

[![License](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/Workshell/tempus/blob/master/license.txt)
[![NuGet](https://img.shields.io/nuget/v/Workshell.Tempus.svg)](https://www.nuget.org/packages/Workshell.Tempus/)
[![Build Status](https://dev.azure.com/Workshell-DevOps/tempus/_apis/build/status/Build%20Master?branchName=master)](https://dev.azure.com/Workshell-DevOps/tempus/_build/latest?definitionId=2&branchName=master)

This is a class library for scheduling and executing jobs in a similar vain to Hangfire or Quartz, although with a simpler API surface and feature set.

## Installation

Stable builds are available as NuGet packages. You can install it via the Package Manager or via the Package Manager Console:

```
Install-Package Workshell.Tempus
Install-Package Workshell.Tempus.AspNetCore // If you're using ASP.NET Core
```

## Scheduler Setup

The scheduler is a singleton instance that manages all jobs. To create it it's as simple as:

```
var scheduler = JobScheduler.Create();
```

After that it's necessary to start the scheduler, again another one-liner:

```
scheduler.Start();
```

The scheduler has a timing resolution of 1 second so you can't create schedules smaller than this granularity.

The scheduler supports the following scheduling:

* Immediately
* Once
* Interval (using Cron patterns with second resolution)

You also have a choice of how to implement a job, you can use `Action`, `Func` (which can be used with async calls) or create a class that implements the `IJob` interface.

## Immediate Jobs

An immediate job is placed in the schedule and executed as soon as possible.

You can use a class and register it with the scheduler like below:

```
class SomeJob : IJob
{
    public Task ExecuteAsync(JobExecutionContext context)
    {
        // Do some work

        return Task.CompletedTask;  
    }
}

scheduler.Schedule<SomeJob>();
```

Using an `Action`:

```
scheduler.Schedule((context) => 
{
    // Do some work
});
```

Using a `Func`:

```
scheduler.Schedule((context) => 
{
    // Do some work

    return Task.CompletedTask;
});
```

Or...

```
scheduler.Schedule(async (context) => 
{
    // Do some work
});
```

## One-time Jobs

A one-time job has a static execution point in the future, say 00:00 Christmas Day.

Using a class:

```
[Once("2020-12-25T00:00:00Z")] // Note the attribute
class SomeJob : IJob
{
    public Task ExecuteAsync(JobExecutionContext context)
    {
        // Do some work

        return Task.CompletedTask;  
    }
}

scheduler.Schedule<SomeJob>();
```

Using an `Action`:

```
var when = DateTime.Parse("2020-12-25T00:00:00Z");

scheduler.Schedule(when, (context) => 
{
    // Do some work
});
```

Using a `Func`:

```
var when = DateTime.Parse("2020-12-25T00:00:00Z");

scheduler.Schedule(when, (context) => 
{
    // Do some work

    return Task.CompletedTask;
});
```

Or...

```
var when = DateTime.Parse("2020-12-25T00:00:00Z");

scheduler.Schedule(when, async (context) => 
{
    // Do some work
});
```

## Interval Jobs

Interval jobs run on a schedule for example, every 30 seconds.

Using a class:

```
[Cron("*/30 * * * * *")] // Note the attribute
class SomeJob : IJob
{
    public Task ExecuteAsync(JobExecutionContext context)
    {
        // Do some work

        return Task.CompletedTask;  
    }
}

scheduler.Schedule<SomeJob>();
```

Using an `Action`:

```
scheduler.Schedule("*/30 * * * * *", (context) => 
{
    // Do some work
});
```

Using a `Func`:

```
scheduler.Schedule("*/30 * * * * *", (context) => 
{
    // Do some work

    return Task.CompletedTask;
});
```

Or...

```
scheduler.Schedule("*/30 * * * * *", async (context) => 
{
    // Do some work
});
```

## Overlapping

With smaller intervals and/or longer executions there's a chance that executions can overlap each other, for example scheduling something to run every 10 seconds but each execution taking 15-20 seconds.

There are three ways to deal with the overlap:

1. Allow - Allow overlapping executions
2. Wait - Wait until a previous execution finishes before executing itself
3. Skip - Don't execute if overlapped, roll over if on a schedule

By default the scheduler will allow overlapped executions, however you can override this behaviour.

With classes you can just add the `[Overlap]` attribute to the class that implements the IJob you want to change the overlap behaviour, and specify a value from the `OverlapHandling` enum:

```
[Overlap(OverlapHandling.Skip)]
class SomeJob : IJob
{
    ...
}
```

For `Action` and `Func` methods that's a `overlapHandling` parameter you can set.

## ASP.NET Core

We have an integration package for ASP.NET Core which makes using Tempus very easy. Instead of manually configuring the scheduler you can use the built in dependency injection to do it all for you.

To add in the scheduler and everything needed, in `ConfigureServices` you can do:

```
services.AddTempus();
```

This will register the `IJobScheduler` as a singleton along with support infrastructure such as an `IHostedService` which will hook into starting and stopping the scheduler as needed.

Then to register jobs, for a class:

```
services.AddTempusJob<SomeJob>();
```

For an `Action` or `Func`:

```
services.AddTempusJob("*/10 * * * * *", () =>
{
    // Do some work
});
```

You can also then pick up the core interfaces in other classes through depedency injection, for example:

```
class SomeOtherClass
{
    public SomeOtherClass(IJobScheduler scheduler)
    {
        ...
    }
}
```

## Cron Format

Tempus currently uses the excellent [HangfireIO/Cronos](https://github.com/HangfireIO/Cronos) library, by the people who wrote Hangfire, for parsing Cron scheduling expressions.

We've shamelessy copied their cron format details below.

Cron expression is a mask to define fixed times, dates and intervals. The mask consists of second (optional), minute, hour, day-of-month, month and day-of-week fields. All of the fields allow you to specify multiple values, and any given date/time will satisfy the specified Cron expression, if all the fields contain a matching value.

                                           Allowed values    Allowed special characters   Comment

    ┌───────────── second (optional)       0-59              * , - /                      
    │ ┌───────────── minute                0-59              * , - /                      
    │ │ ┌───────────── hour                0-23              * , - /                      
    │ │ │ ┌───────────── day of month      1-31              * , - / L W ?                
    │ │ │ │ ┌───────────── month           1-12 or JAN-DEC   * , - /                      
    │ │ │ │ │ ┌───────────── day of week   0-6  or SUN-SAT   * , - / # L ?                Both 0 and 7 means SUN
    │ │ │ │ │ │
    * * * * * *

### Base characters

In all fields you can use number, `*` to mark field as *any value*, `-` to specify ranges of values. Reversed ranges like `22-1`(equivalent to `22,23,0,1,2`) are also supported.

It's possible to define **step** combining `/` with `*`, numbers and ranges. For example, `*/5` in minute field describes *every 5 minute* and `1-15/3` in day-of-month field – *every 3 days from the 1st to the 15th*. Pay attention that `*/24` is just equivalent to `0,24,48` and `*/24` in minute field doesn't literally mean *every 24 minutes* it means *every 0,24,48 minute*.

Concatinate values and ranges by `,`. Comma works like `OR` operator. So `3,5-11/3,12` is equivalent to `3,5,8,11,12`.

In month and day-of-week fields, you can use names of months or days of weeks abbreviated to first three letters (`Jan-Dec` or `Mon-Sun`) instead of their numeric values. Full names like `JANUARY` or `MONDAY` **aren't supported**.

For day of week field, both `0` and `7` stays for Sunday, 1 for Monday.

| Expression           | Description                                                                           |
|----------------------|---------------------------------------------------------------------------------------|
| `* * * * *`          | Every minute                                                                          |
| `0  0 1 * *`         | At midnight, on day 1 of every month                                                  |
| `*/5 * * * *`        | Every 5 minutes                                                                       |
| `30,45-15/2 1 * * *` | Every 2 minute from 1:00 AM to 01:15 AM and from 1:45 AM to 1:59 AM and at 1:30 AM    |
| `0 0 * * MON-FRI`    | At 00:00, Monday through Friday                                                       |

### Special characters

Most expressions you can describe using base characters. If you want to deal with more complex cases like *the last day of month* or *the 2nd Saturday* use special characters:

**`L`** stands for "last". When used in the day-of-week field, it allows you to specify constructs such as *the last Friday* (`5L`or `FRIL`). In the day-of-month field, it specifies the last day of the month.

**`W`** in day-of-month field is the nearest weekday. Use `W`  with single value (not ranges, steps or `*`) to define *the nearest weekday* to the given day. In this case there are two base rules to determine occurrence: we should shift to **the nearest weekday** and **can't shift to different month**. Thus if given day is Saturday we shift to Friday, if it is Sunday we shift to Monday. **But** if given day is **the 1st day of month** (e.g. `0 0 1W * *`) and it is Saturday we shift to the 3rd Monday, if given day is **last day of month** (`0 0 31W 0 0`) and it is Sunday we shift to that Friday. Mix `L` (optionaly with offset) and `W` characters to specify *last weekday of month* `LW` or more complex like `L-5W`.

**`#`** in day-of-week field allows to specify constructs such as *second Saturday* (`6#2` or `SAT#2`).

**`?`** is synonym of `*`. It's supported but not obligatory, so `0 0 5 * ?` is the same as `0 0 5 * *`.

| Expression        | Description                                              |
|-------------------|----------------------------------------------------------|
| `0 0 L   * *`     | At 00:00 AM on the last day of the month                 |
| `0 0 L-1 * *`     | At 00:00 AM the day before the last day of the month     |
| `0 0 3W  * *`     | At 00:00 AM, on the 3rd weekday of every month           |
| `0 0 LW  * *`     | At 00:00 AM, on the last weekday of the month            |
| `0 0 *   * 2L`    | At 00:00 AM on the last tuesday of the month             |
| `0 0 *   * 6#3`   | At 00:00 AM on the third Saturday of the month           |
| `0 0 ?   1 MON#1` | At 00:00 AM on the first Monday of the January           |

### Specify Day of month and Day of week

You can set both **day-of-month** and **day-of-week**, it allows you to specify constructs such as **Friday the thirteenth**. Thus `0 0 13 * 5` means at 00:00, Friday the thirteenth.

It differs from Unix crontab and Quartz cron implementations. Crontab handles it like `OR` operator: occurrence can happen in given day of month or given day of week. So `0 0 13 * 5` means *at 00:00 AM, every friday or every the 13th of a month*. Quartz doesn't allow specify both day-of-month and day-of-week.

### Macro

A macro is a string starting with `@` and representing a shortcut for simple cases like *every day* or *every minute*.

 Macro          | Equivalent    | Comment
----------------|---------------| -------
`@every_second` | `* * * * * *` | Run once a second
`@every_minute` | `* * * * *`   | Run once a minute at the beginning of the minute
`@hourly`       | `0 * * * *`   | Run once an hour at the beginning of the hour
`@daily`        | `0 0 * * *`   | Run once a day at midnight
`@midnight`     | `0 0 * * *`   | Run once a day at midnight
`@weekly`       | `0 0 * * 0`   | Run once a week at midnight on Sunday morning
`@monthly`      | `0 0 1 * *`   | Run once a month at midnight of the first day of the month
`@yearly`       | `0 0 1 1 *`   | Run once a year at midnight of 1 January
`@annually`     | `0 0 1 1 *`   | Run once a year at midnight of 1 January

## MIT License

Copyright (c) Workshell Ltd

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.