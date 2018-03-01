using System;

public class CalendarEvent
{
    /// <summary> Partition key or column name for table </summary>
    public string PartitionKey { get; set; }
    /// <summary> Rowkey for table </summary>
    public string RowKey { get; set; }
    /// <summary> Start datetime for the event </summary>
    public string Start { get; set; }
    /// <summary> End datetime for the event </summary>
    public string End { get; set; }
    /// <summary> Name of the event </summary>
    public string Name { get; set; }
    /// <summary> Details for the event (parking, times, fees) </summary>
    public string Details { get; set; }
    /// <summary> Which organisation is holding the event </summary>
    public string Org { get; set; }

    /// Constructors
    public CalendarEvent() { }
    public CalendarEvent(DateTime start, DateTime end, string name, string details, string org)
    {
        PartitionKey = DateToPartitionMonth(start);
        RowKey = NameToRow(name);
        Start = start.ToString();
        End = end.ToString();
        Name = name;
        Details = details;
        Org = org;
    }

    /// <summary> Converts a datetime to a partition for the azure table </summary>
    public static string DateToPartition(DateTime date)
    {
        return date.Year.ToString().ToLower() + date.Month.ToString().ToLower() + date.Day.ToString().ToLower();
    }

    /// <summary> Converts a datetime to a partition for the azure table using only month and year </summary>
    public static string DateToPartitionMonth(DateTime date)
    {
        return date.Year.ToString().ToLower() + date.Month.ToString("00").ToLower();
    }

    /// <summary> Converts a datetime to a partition for the azure table using only year  </summary>
    public static string DateToPartitionYear(DateTime date)
    {
        return date.Year.ToString().ToLower();
    }

    /// <summary> Converts string to a valid row key for azure table </summary>
    public static string NameToRow(string name)
    {
        string newName = name.ToLower();
        string[] split = newName.Split(null);
        newName = "";
        for(int i = 0; i < split.Length; ++i)
        {
            newName += split[i];
        }
        return newName;
    }

    /// <summary> Returns the start date as a date time </summary>
    public DateTime GetStart()
    {
        return DateTime.Parse(Start);
    }

    /// <summary> Returns the end date as a date time </summary>
    public DateTime GetEnd()
    {
        return DateTime.Parse(End);
    }

    /// <summary> Converts the event to a string for listing purposes </summary>
    public override string ToString()
    {
        return String.Format("{0, -32} | {1, -12} | {2, -12}\t", new string[] { Name, GetStart().Date.ToString("MM,dd,yyyy"), GetEnd().Date.ToString("MM,dd,yyyy") });
    }

    /// <summary> Overloads the evuivilency operator </summary>
    public static bool operator ==(CalendarEvent left, CalendarEvent right)
    {
        if (object.ReferenceEquals(right, null) || object.ReferenceEquals(left, null))
        {
            if(object.ReferenceEquals(right, null) && object.ReferenceEquals(left, null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        bool equal = (left.Name == right.Name) ?(true):(false);
        equal = (left.Start == right.Start) ? (true) : (false);
        equal = (left.End == right.End) ? (true) : (false);
        equal = (left.Details == right.Details) ? (true) : (false);
        equal = (left.Org == right.Org) ? (true) : (false);

        return equal;
    }

    /// <summary> overloads the not equivilent opperator </summary>
    public static bool operator !=(CalendarEvent left, CalendarEvent right)
    {
        if (object.ReferenceEquals(right, null) || object.ReferenceEquals(left, null))
        {
            if (object.ReferenceEquals(right, null) && object.ReferenceEquals(left, null))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        bool equal = (left.Name != right.Name) ? (true) : (false);
        equal = (left.Start != right.Start) ? (true) : (false);
        equal = (left.End != right.End) ? (true) : (false);
        equal = (left.Details != right.Details) ? (true) : (false);
        equal = (left.Org != right.Org) ? (true) : (false);

        return equal;
    }
}