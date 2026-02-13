using Dapper;
using System.Data;
using System.Globalization;

namespace LearnMVC.Handlers
{
    public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
    {
        private static readonly string[] DateFormats = new[]
        {
            "dd-MM-yyyy",
            "MM-dd-yyyy",
            "yyyy-MM-dd",
            "dd/MM/yyyy",
            "MM/dd/yyyy",
            "yyyy/MM/dd",
            "dd.MM.yyyy",
            "yyyy.MM.dd"
        };

        public override DateOnly Parse(object value)
        {
            if (value == null || value is DBNull)
            {
                throw new InvalidCastException("Cannot convert null to DateOnly");
            }

            if (value is DateTime dateTime)
            {
                return DateOnly.FromDateTime(dateTime);
            }

            if (value is DateOnly dateOnly)
            {
                return dateOnly;
            }

            if (value is string dateString)
            {
                dateString = dateString.Trim();

                // Try parsing with multiple formats
                foreach (var format in DateFormats)
                {
                    if (DateOnly.TryParseExact(dateString, format,
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                    {
                        return result;
                    }
                }

                // Try general parsing as last resort
                if (DateOnly.TryParse(dateString, CultureInfo.InvariantCulture, out var generalResult))
                {
                    return generalResult;
                }

                throw new InvalidCastException($"Unable to parse '{dateString}' to DateOnly. Tried formats: {string.Join(", ", DateFormats)}");
            }

            throw new InvalidCastException($"Unable to convert {value.GetType()} (value: {value}) to DateOnly");
        }

        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            parameter.DbType = DbType.Date;
            parameter.Value = value.ToDateTime(TimeOnly.MinValue);
        }
    }

    public class NullableDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly?>
    {
        private static readonly string[] DateFormats = new[]
        {
            "dd-MM-yyyy",
            "MM-dd-yyyy",
            "yyyy-MM-dd",
            "dd/MM/yyyy",
            "MM/dd/yyyy",
            "yyyy/MM/dd",
            "dd.MM.yyyy",
            "yyyy.MM.dd"
        };

        public override DateOnly? Parse(object value)
        {
            if (value == null || value is DBNull)
            {
                return null;
            }

            if (value is DateTime dateTime)
            {
                return DateOnly.FromDateTime(dateTime);
            }

            if (value is DateOnly dateOnly)
            {
                return dateOnly;
            }

            if (value is string dateString)
            {
                dateString = dateString.Trim();

                // Try parsing with multiple formats
                foreach (var format in DateFormats)
                {
                    if (DateOnly.TryParseExact(dateString, format,
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                    {
                        return result;
                    }
                }

                // Try general parsing as last resort
                if (DateOnly.TryParse(dateString, CultureInfo.InvariantCulture, out var generalResult))
                {
                    return generalResult;
                }

                // Return null instead of throwing for nullable
                return null;
            }

            return null;
        }

        public override void SetValue(IDbDataParameter parameter, DateOnly? value)
        {
            if (value.HasValue)
            {
                parameter.DbType = DbType.Date;
                parameter.Value = value.Value.ToDateTime(TimeOnly.MinValue);
            }
            else
            {
                parameter.Value = DBNull.Value;
            }
        }
    }
}
