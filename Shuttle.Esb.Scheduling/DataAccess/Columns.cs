using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.Esb.Scheduling.DataAccess
{
    public class Columns
    {
        public static readonly MappedColumn<Guid> Id = new MappedColumn<Guid>("Id", DbType.Guid);
        public static readonly MappedColumn<string> Name = new MappedColumn<string>("Name", DbType.AnsiString);
        public static readonly MappedColumn<string> CronExpression = new MappedColumn<string>("CronExpression", DbType.AnsiString);
        public static readonly MappedColumn<DateTime> NextNotification = new MappedColumn<DateTime>("NextNotification", DbType.DateTime);
        public static readonly MappedColumn<string> Match = new MappedColumn<string>("Match", DbType.AnsiString);
    }
}