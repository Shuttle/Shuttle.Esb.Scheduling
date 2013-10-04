shuttle-scheduling
==================

A simple scheduling solution built on Shuttle ESB.



Simple Configuration
--------------------
1. Specify a name for your scheduled job, e.g. **ProcessAccounts**
2. Select the endpoint uri to send the **RunScheduleCommand** to when execution is required.
3. Enter the [cron](http://en.wikipedia.org/wiki/Cron) expression to use.
4. Ensure that the **Shuttle.Scheduling.Server** endpoint is running.
 
### cron samples

Format is {minute} {hour} {day-of-month} {month} {day-of-week}

{minutes} : 0-59 , - * /
{hours} : 	0-23 , - * /
{day-of-month} 1-31 , - * ? / L W
{month} : 1-12 or JAN-DEC	, - * /
{day-of-week} : 1-7 or SUN-SAT , - * ? / L #

Examples:
* * * * * - is every minute of every hour of every day of every month
5,10-12,17/5 * * * * - minute 5, 10, 11, 12, and every 5th minute after that


# But how does it fit together?

