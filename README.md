# Shuttle.Esb.Scheduling

A simple scheduling solution built on Shuttle.Esb.

Download the latest release source code and then build from the command prompt:

> .\Shuttle.Esb.Scheduling\.build\msbuild package.msbuild

You will see the `deployment` folder under `Shuttle.Esb.Scheduling\.build` that will contain the binaries for the various configuration/framework versions.

## Simple Configuration

1. Specify a name for your scheduled job, e.g. **ProcessAccountsCommand**
2. Select the endpoint uri to send the **RunScheduleCommand** to when execution is required.
3. Enter the [cron](http://en.wikipedia.org/wiki/Cron) expression to use.
4. Ensure that the **Shuttle.Esb.Scheduling.Server** endpoint is running.
 
### cron samples

Format is {minute} {hour} {day-of-month} {month} {day-of-week}

<pre>
{minutes} : 0-59 , - * /
{hours} : 	0-23 , - * /
{day-of-month} 1-31 , - * ? / L W
{month} : 1-12 or JAN-DEC	, - * /
{day-of-week} : 1-7 or SUN-SAT , - * ? / L #

Examples:
* * * * * - is every minute of every hour of every day of every month
5,10-12,17/5 * * * * - minute 5, 10, 11, 12, and every 5th minute after that
</pre>

## But how does it fit together?

The basic idea behind the scheduling is that you will have endpoints that already contain the required behaviour.  For instance, you may have a **PrintInvoiceCommand** that you can send to an endpoint to perform the processing.  You periodically click on a button and this sends off the command.  However, you do not want to necessarily send the **PrintInvoicesCommand** from the scheduler as it should not be that tightly coupled to any message.

Instead you can set up how often the **RunScheduleCommand** is sent to a particular endpoint.  In this case it may be the same endpoint that processes the **PrintInvoicesCommand**.  It is, therefore, quite conceivable that an endpoint may receive a number of **RunScheduleCommand** requests, each with a different *Name*.  The handler for this command on the endpoint could use a ```switch``` statement to then do a ```bus.SendLocal``` to send the relevant ***actual*** command to be processed.

So by adding this layer of indirection we can have enterprise-grade scheduling that is not at all intrusive and leverages existing functionality.
