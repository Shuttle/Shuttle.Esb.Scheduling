# Shuttle.Esb.Scheduling

A simple scheduling solution built on Shuttle.Esb.

See [Shuttle.Esb.Scheduling.Server](https://github.com/Shuttle/Shuttle.Esb.Scheduling/tree/master/Shuttle.Esb.Scheduling.Server) for an example implementation of a scheduler.

## Simple Configuration

A schedule has a name for your scheduled job, e.g. **ProcessAccounts**.  You then use a [cron](https://www.pendel.co.za/shuttle-core/infrastructure/shuttle-core-cron.html) expression to indicate when a `ScheduleNotification` message is published.  Any endpoint that is interested in the notification can then subscribe to the `ScheduleNotification` message and perform the relevant processing.
 
### cron samples

Format is {minute} {hour} {day-of-month} {month} {day-of-week}

```
{minutes} : 0-59 , - * /
{hours} : 	0-23 , - * /
{day-of-month} 1-31 , - * ? / L W
{month} : 1-12 or JAN-DEC	, - * /
{day-of-week} : 1-7 or SUN-SAT , - * ? / L #

Examples:
* * * * * - is every minute of every hour of every day of every month
5,10-12,17/5 * * * * - minute 5, 10, 11, 12, and every 5th minute after that
```

## How does it fit together?

The basic idea behind the scheduling is that you will have endpoints that already contain the required behaviour.  For instance, you may have a `PrintInvoices` that you can send to an endpoint to perform the processing.  You periodically click on a button and this sends off the command.  However, you do not want to necessarily send the **PrintInvoices** from the scheduler as it should not be that tightly coupled to any message.

Instead you can set up how often a named `ScheduleNotification` is published.  In this case it may be the same endpoint that processes the `PrintInvoices` can subscribe to the `ScheduleNotification`.  It is, therefore, quite conceivable that an endpoint may receive a number of published `ScheduleNotification` messages, each with a different `Name`.  The handler for this command on the endpoint could use a `switch` statement to then do a `bus.Send` to send the relevant ***actual*** command to be processed, typically by configuring the message to be sent locally.

By adding this layer of indirection we can have enterprise-grade scheduling that is not at all intrusive and leverages existing functionality.
