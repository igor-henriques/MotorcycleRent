namespace MotorcycleRent.Domain.Models;

/// <summary>
/// https://raw.githubusercontent.com/TobiStr/DateTimeRange/master/src/DateTimeRange/DateTimeRange.cs
/// </summary>
public readonly struct DateTimeRange : IEquatable<DateTimeRange>, IComparable<DateTimeRange>
{
    [Newtonsoft.Json.JsonIgnore]
    [JsonIgnore]
    /// <inheritdoc cref="DateTime.Date"/>
    /// <remarks>
    /// Had to add <see cref="JsonIgnoreAttribute"/> to avoid circular reference
    /// error but not sure why
    /// </remarks>
    public readonly DateTimeRange Date
    {
        get
        {
            return new DateTimeRange(Start.Date, End.Date);
        }
    }

    /// <summary>
    /// The <see cref="DateTime"/>, the range starts with
    /// </summary>
    public DateTime Start { get; init; }

    /// <summary>
    /// The <see cref="DateTime"/>, the range ends with
    /// </summary>
    public DateTime End { get; init; }

    /// <summary>
    /// Initializes a new instance of <see cref="DateTimeRange"/>
    /// </summary>
    /// <param name="startDate"><see cref="DateTime"/> the range starts with</param>
    /// <param name="endDate"><see cref="DateTime"/> the range ends with</param>
    public DateTimeRange(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate) throw new DateTimeInvalidRangeException("Start date is later than end date");
        Start = startDate;
        End = endDate;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="DateTimeRange"/>
    /// </summary>
    /// <param name="startDate"><see cref="DateTime"/> the range starts with</param>
    /// <param name="duration">Duration from StartDate to EndDate</param>
    public DateTimeRange(DateTime startDate, TimeSpan duration)
    {
        if (duration.TotalMilliseconds < 0) throw new DateTimeInvalidRangeException("Duration is negative");
        Start = startDate;
        End = startDate.Add(duration);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="DateTimeRange"/>
    /// </summary>
    /// <param name="duration">Duration from StartDate to EndDate</param>
    /// <param name="endDate"><see cref="DateTime"/> the range ends with</param>
    public DateTimeRange(TimeSpan duration, DateTime endDate)
    {
        if (duration.TotalMilliseconds < 0) throw new DateTimeInvalidRangeException("Duration is negative");
        Start = endDate.Add(-duration);
        End = endDate;
    }

    public readonly double NumberOfDays()
    {
        return (End - Start).TotalDays;
    }


    /// <summary>
    /// Validate if <see cref="other"/> Start and End date falls within DateTimeRange
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public readonly bool Contains(DateTimeRange other)
    {
        if (Start > other.Start)
        {
            // This starts after the other
            return false;
        }

        if (End < other.End)
        {
            // This ends before the other
            return false;
        }

        return true;
    }

    public readonly bool Equals(DateTimeRange other)
    {
        return Start == other.Start && End == other.End;
    }

    /// <summary>
    /// Validate if <paramref name="other"/> overlaps at least one day.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public readonly bool Overlaps(DateTimeRange other, bool inclusive = true)
    {
        if (inclusive)
        {
            return ((Start <= other.End) && (other.Start <= End));
        }
        else
        {
            return ((Start < other.End) && (other.Start < End));
        }
    }

    public readonly int CompareTo(DateTimeRange other)
    {
        int startDateComparison = Start.CompareTo(other.Start);
        if (startDateComparison != 0)
        {
            return startDateComparison;
        }

        return End.CompareTo(other.End);
    }
    /// <summary>
    /// Converts the start and end dates to the format MM/DD/YYYY - MM/DD/YYYY
    /// </summary>
    /// <returns></returns>
    public readonly string ToShortDateString()
    {
        return $"{Start:MM/dd/yyyy} - {End:MM/dd/yyyy}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;        
        if (obj.GetType() != GetType()) return false;
        return Equals((DateTimeRange)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Start, End);
    }

    public static bool operator ==(DateTimeRange left, DateTimeRange right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DateTimeRange left, DateTimeRange right)
    {
        return !(left == right);
    }

    public static bool operator <(DateTimeRange left, DateTimeRange right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(DateTimeRange left, DateTimeRange right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(DateTimeRange left, DateTimeRange right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(DateTimeRange left, DateTimeRange right)
    {
        return left.CompareTo(right) >= 0;
    }
}
